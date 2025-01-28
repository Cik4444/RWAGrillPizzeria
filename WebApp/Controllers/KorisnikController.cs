using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApp.Models;
using WebApp.ViewModels;
using WebApp.Security;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;

namespace WebApp.Controllers
{
    [Route("korisnik")]
    public class KorisnikController : Controller
    {
        private readonly RwagrillContext _context;
        private const string JwtSecretKey = "your-secure-key-here";
        private const int JwtExpirationMinutes = 60;

        public KorisnikController(RwagrillContext context)
        {
            _context = context;
        }

        [HttpGet("list")]
        public async Task<IActionResult> Index()
        {
            var korisnici = await _context.Korisniks
                .Select(k => new KorisnikVM
                {
                    Idkorisnik = k.Idkorisnik,
                    Ime = k.Ime,
                    Prezime = k.Prezime,
                    Email = k.Email
                }).ToListAsync();

            return View(korisnici);
        }

        [HttpGet("details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            var korisnik = await _context.Korisniks.FindAsync(id);
            if (korisnik == null)
            {
                return NotFound();
            }

            var korisnikVM = new KorisnikVM
            {
                Idkorisnik = korisnik.Idkorisnik,
                Ime = korisnik.Ime,
                Prezime = korisnik.Prezime,
                Email = korisnik.Email
            };

            return View(korisnikVM);
        }

        [HttpGet("create")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost("create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(KorisnikVM korisnikVM)
        {
            if (ModelState.IsValid)
            {
                var nextId = (_context.Korisniks.Max(k => (int?)k.Idkorisnik) ?? 0) + 1;
                var salt = PasswordHashProvider.GetSalt();
                var hashedPassword = PasswordHashProvider.GetHash(korisnikVM.Lozinka, salt);

                var korisnik = new Korisnik
                {
                    Idkorisnik = nextId,
                    Ime = korisnikVM.Ime,
                    Prezime = korisnikVM.Prezime,
                    Email = korisnikVM.Email,
                    Lozinka = hashedPassword,
                    Salt = salt
                };

                _context.Korisniks.Add(korisnik);
                await _context.SaveChangesAsync();
                return RedirectToAction("Login");
            }

            return View(korisnikVM);
        }

        [HttpGet("edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            var korisnik = await _context.Korisniks.FindAsync(id);
            if (korisnik == null)
            {
                return NotFound();
            }

            var korisnikVM = new KorisnikVM
            {
                Idkorisnik = korisnik.Idkorisnik,
                Ime = korisnik.Ime,
                Prezime = korisnik.Prezime,
                Email = korisnik.Email
            };

            return View("Update", korisnikVM);
        }

        [HttpPost("edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, KorisnikVM korisnikVM, string repeatPassword)
        {
            if (id != korisnikVM.Idkorisnik)
            {
                return BadRequest("Invalid user ID.");
            }

            if (!string.IsNullOrEmpty(korisnikVM.Lozinka) && korisnikVM.Lozinka != repeatPassword)
            {
                ModelState.AddModelError("Lozinka", "Passwords do not match.");
                return View("Update", korisnikVM);
            }

            var korisnik = await _context.Korisniks.FindAsync(id);
            if (korisnik == null)
            {
                return NotFound();
            }

            korisnik.Ime = korisnikVM.Ime;
            korisnik.Prezime = korisnikVM.Prezime;

            if (!string.IsNullOrEmpty(korisnikVM.Lozinka))
            {
                var salt = PasswordHashProvider.GetSalt();
                var hashedPassword = PasswordHashProvider.GetHash(korisnikVM.Lozinka, salt);
                korisnik.Lozinka = hashedPassword;
                korisnik.Salt = salt;
            }

            try
            {
                _context.Entry(korisnik).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Korisniks.Any(k => k.Idkorisnik == id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToAction("Index");
        }


        [HttpGet("delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var korisnik = await _context.Korisniks.FindAsync(id);
            if (korisnik == null)
            {
                return NotFound();
            }

            var korisnikVM = new KorisnikVM
            {
                Idkorisnik = korisnik.Idkorisnik,
                Ime = korisnik.Ime,
                Prezime = korisnik.Prezime,
                Email = korisnik.Email
            };

            return View(korisnikVM);
        }

        [HttpPost("delete/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var korisnik = await _context.Korisniks.FindAsync(id);
            if (korisnik == null)
            {
                return NotFound();
            }

            _context.Korisniks.Remove(korisnik);
            await _context.SaveChangesAsync();
            return RedirectToAction("list");
        }

        [HttpGet("login")]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(string email, string lozinka)
        {
            var korisnik = await _context.Korisniks.FirstOrDefaultAsync(k => k.Email == email);
            if (korisnik == null)
            {
                ModelState.AddModelError("", "Invalid email or password.");
                return View();
            }

            var hashedPassword = PasswordHashProvider.GetHash(lozinka, korisnik.Salt);
            if (korisnik.Lozinka != hashedPassword)
            {
                ModelState.AddModelError("", "Invalid email or password.");
                return View();
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, korisnik.Email),
                new Claim(ClaimTypes.Role, "User") 
            };

            var identity = new ClaimsIdentity(claims, "CookieAuth");
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync("CookieAuth", principal);

            return RedirectToAction("Index", "Home");
        }


        [HttpGet("register")]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost("register")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(KorisnikVM korisnikVM)
        {
            if (ModelState.IsValid)
            {
                var nextId = (_context.Korisniks.Max(k => (int?)k.Idkorisnik) ?? 0) + 1;
                var salt = PasswordHashProvider.GetSalt();
                var hashedPassword = PasswordHashProvider.GetHash(korisnikVM.Lozinka, salt);

                var korisnik = new Korisnik
                {
                    Idkorisnik = nextId,
                    Ime = korisnikVM.Ime,
                    Prezime = korisnikVM.Prezime,
                    Email = korisnikVM.Email,
                    Lozinka = hashedPassword,
                    Salt = salt
                };

                _context.Korisniks.Add(korisnik);
                await _context.SaveChangesAsync();
                return RedirectToAction("Login");
            }

            return View(korisnikVM);
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("CookieAuth");
            return RedirectToAction("Login", "Korisnik");
        }



        [HttpGet("profile")]
        public async Task<IActionResult> Profile()
        {
            var email = User.Identity?.Name;
            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction("Login");
            }

            var korisnik = await _context.Korisniks.FirstOrDefaultAsync(k => k.Email == email);
            if (korisnik == null)
            {
                return NotFound();
            }

            var korisnikVM = new KorisnikVM
            {
                Idkorisnik = korisnik.Idkorisnik,
                Ime = korisnik.Ime,
                Prezime = korisnik.Prezime,
                Email = korisnik.Email
            };

            return View(korisnikVM);
        }



        [HttpPost("profile")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(KorisnikVM korisnikVM, string repeatPassword)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { Message = "Invalid input." });
            }

            if (!string.IsNullOrEmpty(korisnikVM.Lozinka) && korisnikVM.Lozinka != repeatPassword)
            {
                return BadRequest(new { Message = "Passwords do not match." });
            }

            var korisnik = await _context.Korisniks.FindAsync(korisnikVM.Idkorisnik);
            if (korisnik == null)
            {
                return NotFound(new { Message = "User not found." });
            }

            korisnik.Ime = korisnikVM.Ime;
            korisnik.Prezime = korisnikVM.Prezime;

            if (!string.IsNullOrEmpty(korisnikVM.Lozinka))
            {
                var salt = PasswordHashProvider.GetSalt();
                var hashedPassword = PasswordHashProvider.GetHash(korisnikVM.Lozinka, salt);
                korisnik.Lozinka = hashedPassword;
                korisnik.Salt = salt;
            }

            try
            {
                _context.Entry(korisnik).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return Ok(new { Message = "Profile updated successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = $"An error occurred: {ex.Message}" });
            }
        }






    }


}
