using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using WebApi.Models;
using WebApi.Dto;
using Microsoft.EntityFrameworkCore;


namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HranaController : ControllerBase
    {
        private readonly RwagrillContext _context;

        public HranaController(RwagrillContext context)
        {
            _context = context;
        }

        private void LogAction(string level, string message)
        {
            var log = new Log
            {
                Timestamp = DateTime.UtcNow,
                Level = level,
                Message = message
            };

            _context.Logs.Add(log);
            _context.SaveChanges();
        }

        [HttpPost]
        public async Task<IActionResult> CreateHrana([FromBody] HranaDto hranaDto)
        {
            if (hranaDto == null)
                return BadRequest("Invalid data.");

            
            int newId = _context.Hranas.Any()
                ? _context.Hranas.Max(h => h.Idhrana) + 1
                : 1; 

            while (_context.Hranas.Any(h => h.Idhrana == newId))
            {
                newId++; 
            }

            var hrana = new Hrana
            {
                Idhrana = newId, 
                Naslov = hranaDto.Naslov,
                Opis = hranaDto.Opis,
                Cijena = hranaDto.Cijena,
                KategorijaHraneId = hranaDto.KategorijaHrane?.IdkategorijaHrane
            };

            _context.Hranas.Add(hrana);
            await _context.SaveChangesAsync();

            LogAction("CREATE", $"Created a new hrana with id={hrana.Idhrana}.");

            return CreatedAtAction(nameof(GetHranaById), new { id = hrana.Idhrana }, hrana);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<HranaDto>>> GetAllHrana()
        {
            var hranaList = await _context.Hranas
            .Select(h => new HranaDto
            {
                Naslov = h.Naslov,
                Opis = h.Opis,
                Cijena = h.Cijena,
                KategorijaHrane = h.KategorijaHrane
            })
            .ToListAsync();

            foreach (var hranaDto in hranaList)
            {
                var hranaEntity = _context.Hranas.FirstOrDefault(h => h.Naslov == hranaDto.Naslov);
                if (hranaEntity != null)
                {
                    foreach (var alergen in hranaEntity.HranaAlergens)
                    {
                        hranaDto.HranaAlergens.Add(alergen); 
                    }
                    foreach (var narudzba in hranaEntity.NarudzbaHranas)
                    {
                        hranaDto.NarudzbaHranas.Add(narudzba);
                    }
                }
            }


            LogAction("READ", $"Fetched all hrana.");

            return Ok(hranaList);
        }
    

        [HttpGet("{id}")]
        public async Task<ActionResult<HranaDto>> GetHranaById(int id)
        {
            var hrana = await _context.Hranas.FindAsync(id);

            if (hrana == null)
                return NotFound();

            var hranaDto = new HranaDto
            {
                Naslov = hrana.Naslov,
                Opis = hrana.Opis,
                Cijena = hrana.Cijena,
                KategorijaHrane = hrana.KategorijaHrane
            };

            foreach (var alergen in hrana.HranaAlergens)
            {
                hranaDto.HranaAlergens.Add(alergen);
            }

            foreach (var narudzbaHrana in hrana.NarudzbaHranas)
            {
                hranaDto.NarudzbaHranas.Add(narudzbaHrana);
            }


            LogAction("CREATE", $"Fetched hrana with id={hrana.Idhrana}.");

            return Ok(hranaDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateHrana(int id, [FromBody] HranaDto hranaDto)
        {
            var hrana = await _context.Hranas.FindAsync(id);

            if (hrana == null)
                return NotFound();

            hrana.Naslov = hranaDto.Naslov;
            hrana.Opis = hranaDto.Opis;
            hrana.Cijena = hranaDto.Cijena;
            hrana.KategorijaHraneId = hranaDto.KategorijaHrane?.IdkategorijaHrane; 

            await _context.SaveChangesAsync();


            LogAction("UPDATE", $"Updated a hrana with id={hrana.Idhrana}.");

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteHrana(int id)
        {
            var hrana = await _context.Hranas.FindAsync(id);

            if (hrana == null)
                return NotFound();

            _context.Hranas.Remove(hrana);
            await _context.SaveChangesAsync();


            LogAction("DELETE", $"Deleted a hrana with id={hrana.Idhrana}.");

            return NoContent();
        }
    }
}
