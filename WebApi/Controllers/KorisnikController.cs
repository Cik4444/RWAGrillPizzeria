using Microsoft.AspNetCore.Mvc;
using GrillPizzeriaBL.Models;
using WebApi.Dto;
using Microsoft.EntityFrameworkCore;
using WebApi.Security;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KorisnikController : ControllerBase
    {
        private readonly RwagrillContext _context;
        private readonly JwtTokenProvider _jwtTokenProvider;
        private readonly string _jwtSecretKey = "YourSuperSecretKey";
        private readonly int _jwtExpirationMinutes = 60;



        public KorisnikController(RwagrillContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult<Korisnik>> CreateKorisnik([FromBody] KorisnikDto korisnikDto)
        {
            if (korisnikDto == null)
                return BadRequest("Invalid data.");

            // Generate new user ID
            int newId = _context.Korisniks.Any()
                ? _context.Korisniks.Max(k => k.Idkorisnik) + 1
                : 1;

            while (_context.Korisniks.Any(k => k.Idkorisnik == newId))
            {
                newId++;
            }

            string salt = PasswordHashProvider.GetSalt();
            string hashedPassword = PasswordHashProvider.GetHash(korisnikDto.Lozinka, salt);

            bool isFirstUser = !_context.Korisniks.Any();

            var assignedRole = isFirstUser ? "Admin" : "User";

            var role = await _context.Roles.FirstOrDefaultAsync(r => r.RoleName == assignedRole);
            if (role == null)
            {
                role = new Role { RoleName = assignedRole };
                _context.Roles.Add(role);
                await _context.SaveChangesAsync();
            }

            var korisnik = new Korisnik
            {
                Idkorisnik = newId,
                Ime = korisnikDto.Ime,
                Prezime = korisnikDto.Prezime,
                Email = korisnikDto.Email,
                Lozinka = hashedPassword,
                Salt = salt,
                RoleId = role.RoleId
            };

            _context.Korisniks.Add(korisnik);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetKorisnikById), new { id = korisnik.Idkorisnik }, korisnik);
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<KorisnikDto>>> GetAllKorisnik()
        {
            var korisnikList = await _context.Korisniks
                .Select(k => new KorisnikDto
                {
                    Ime = k.Ime,
                    Prezime = k.Prezime,
                    Email = k.Email,
                    Lozinka = k.Lozinka
                })
                .ToListAsync();

            foreach (var korisnikDto in korisnikList)
            {
                var korisnik = _context.Korisniks
                    .Include(k => k.Narudzbas)
                    .FirstOrDefault(k => k.Email == korisnikDto.Email);

                if (korisnik != null)
                {
                    var narudzbaDtos = korisnik.Narudzbas.Select(n => new NarudzbaDto
                    {
                        Datum = n.Datum,
                        Korisnik = n.Korisnik,
                        NarudzbaHranas = n.NarudzbaHranas.ToList()
                    }).ToList();

                    foreach (var narudzbaDto in narudzbaDtos)
                    {
                        korisnikDto.Narudzbas.Add(narudzbaDto);
                    }
                }
            }

            return Ok(korisnikList);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<KorisnikDto>> GetKorisnikById(int id)
        {
            var korisnik = await _context.Korisniks
                 .Where(k => k.Idkorisnik == id)
                 .Include(k => k.Narudzbas)
                 .FirstOrDefaultAsync();

            if (korisnik == null)
                return NotFound();

            var korisnikDto = new KorisnikDto
            {
                Ime = korisnik.Ime,
                Prezime = korisnik.Prezime,
                Email = korisnik.Email,
                Lozinka = korisnik.Lozinka
            };

            var narudzbaDtos = korisnik.Narudzbas.Select(n => new NarudzbaDto
            {
                Datum = n.Datum,
                Korisnik = n.Korisnik,
                NarudzbaHranas = n.NarudzbaHranas.ToList()
            }).ToList();

            foreach (var narudzbaDto in narudzbaDtos)
            {
                korisnikDto.Narudzbas.Add(narudzbaDto);
            }

            return Ok(korisnikDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateKorisnik(int id, [FromBody] KorisnikDto korisnikDto)
        {
            var korisnik = await _context.Korisniks.FindAsync(id);

            if (korisnik == null)
                return NotFound();

            korisnik.Ime = korisnikDto.Ime;
            korisnik.Prezime = korisnikDto.Prezime;
            korisnik.Email = korisnikDto.Email;
            korisnik.Lozinka = korisnikDto.Lozinka;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteKorisnik(int id)
        {
            var korisnik = await _context.Korisniks.FindAsync(id);

            if (korisnik == null)
                return NotFound();

            _context.Korisniks.Remove(korisnik);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            if (loginDto == null || string.IsNullOrEmpty(loginDto.Email) || string.IsNullOrEmpty(loginDto.Lozinka))
                return BadRequest("Invalid login data.");

            var korisnik = _context.Korisniks.FirstOrDefault(k => k.Email == loginDto.Email);

            if (korisnik == null)
                return Unauthorized("Invalid email or password.");

            string hashedPassword = PasswordHashProvider.GetHash(loginDto.Lozinka, korisnik.Salt);
            

            if (hashedPassword != korisnik.Lozinka)
                return Unauthorized("Invalid email or password.");

            var token = JwtTokenProvider.CreateToken(
                secureKey: _jwtSecretKey,
                expiration: _jwtExpirationMinutes,
                subject: korisnik.Email,
                role: "User"
            );

            return Ok(new { Token = token, Message = "Login successful." });
        }


        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] KorisnikDto registerDto)
        {
            if (registerDto == null || string.IsNullOrEmpty(registerDto.Email) || string.IsNullOrEmpty(registerDto.Lozinka))
                return BadRequest("Invalid registration data.");

            if (_context.Korisniks.Any(k => k.Email == registerDto.Email))
                return Conflict("Email already in use.");

            int newId = _context.Korisniks.Any()
                ? _context.Korisniks.Max(k => k.Idkorisnik) + 1
                : 1;

            while (_context.Korisniks.Any(k => k.Idkorisnik == newId))
            {
                newId++;
            }

            string salt = PasswordHashProvider.GetSalt();
            string hashedPassword = PasswordHashProvider.GetHash(registerDto.Lozinka, salt);

            bool isFirstUser = !_context.Korisniks.Any();
            string assignedRoleName = isFirstUser ? "Admin" : "User";

            var role = await _context.Roles.FirstOrDefaultAsync(r => r.RoleName == assignedRoleName);
            if (role == null)
            {
                role = new Role { RoleName = assignedRoleName };
                _context.Roles.Add(role);
                await _context.SaveChangesAsync();
            }

            var korisnik = new Korisnik
            {
                Idkorisnik = newId,
                Ime = registerDto.Ime,
                Prezime = registerDto.Prezime,
                Email = registerDto.Email,
                Lozinka = hashedPassword,
                Salt = salt,
                RoleId = role.RoleId 
            };

            _context.Korisniks.Add(korisnik);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Registration successful.", RoleAssigned = assignedRoleName });
        }



        [HttpGet("getSalt")]
        public IActionResult GetSalt()
        {
            string salt = PasswordHashProvider.GetSalt();
            return Ok(salt);
        }
    }
}
