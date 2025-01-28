using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApp.Models;
using WebApp.ViewModels;
using System.Linq;
using System.Threading.Tasks;

namespace WebApp.Controllers
{
    [Route("narudzba")]
    public class NarudzbaController : Controller
    {
        private readonly RwagrillContext _context;

        public NarudzbaController(RwagrillContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var narudzbe = await _context.Narudzbas
                .Include(n => n.Korisnik)
                .Select(n => new NarudzbaVM
                {
                    Idnarudzba = n.Idnarudzba,
                    Datum = n.Datum,
                    Korisnik = n.Korisnik,
                    NarudzbaHranas = n.NarudzbaHranas
                })
                .ToListAsync();

            return View(narudzbe);
        }


        [HttpGet("details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid ID.");
            }

            var narudzba = await _context.Narudzbas
                .Include(n => n.Korisnik)
                .Include(n => n.NarudzbaHranas)
                    .ThenInclude(nh => nh.Hrana)
                .FirstOrDefaultAsync(n => n.Idnarudzba == id);

            if (narudzba == null)
            {
                return NotFound("Narudžba not found.");
            }

            var narudzbaVM = new NarudzbaVM
            {
                Datum = narudzba.Datum,
                Korisnik = narudzba.Korisnik,
                NarudzbaHranas = narudzba.NarudzbaHranas
            };

            return View(narudzbaVM);
        }


        [HttpGet("create")]
        public IActionResult Create()
        {
            ViewBag.Korisnici = _context.Korisniks.ToList();
            ViewBag.Hrana = _context.Hranas.ToList();
            return View();
        }

        [HttpPost("create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(NarudzbaVM narudzbaVM)
        {
                var nextId = (_context.Narudzbas.Max(n => (int?)n.Idnarudzba) ?? 0) + 1;

                var narudzba = new Narudzba
                {
                    Idnarudzba = nextId,
                    Datum = narudzbaVM.Datum,
                    KorisnikId = narudzbaVM.Korisnik?.Idkorisnik
                };

                _context.Narudzbas.Add(narudzba);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index");
        }


        [HttpGet("edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            var narudzba = await _context.Narudzbas
                .Include(n => n.NarudzbaHranas)
                .FirstOrDefaultAsync(n => n.Idnarudzba == id);

            if (narudzba == null)
            {
                return NotFound();
            }

            ViewBag.Korisnici = _context.Korisniks.ToList();
            ViewBag.Hrana = _context.Hranas.ToList();

            var narudzbaVM = new NarudzbaVM
            {
                Datum = narudzba.Datum,
                Korisnik = narudzba.Korisnik,
                NarudzbaHranas = narudzba.NarudzbaHranas.ToList()
            };

            return View(narudzbaVM);
        }

        [HttpPost("edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, NarudzbaVM narudzbaVM, int[] hranaIds, int[] kolicine)
        {
            var narudzba = await _context.Narudzbas
                .Include(n => n.Korisnik)
                .Include(n => n.NarudzbaHranas)
                .FirstOrDefaultAsync(n => n.Idnarudzba == id);

            if (narudzba == null)
            {
                return NotFound();
            }

                narudzba.Datum = narudzbaVM.Datum;
                narudzba.KorisnikId = narudzbaVM.Korisnik?.Idkorisnik;

                _context.NarudzbaHranas.RemoveRange(narudzba.NarudzbaHranas);

                foreach (var (hranaId, kolicina) in hranaIds.Zip(kolicine))
                {
                    narudzba.NarudzbaHranas.Add(new NarudzbaHrana
                    {
                        HranaId = hranaId,
                        Kolicina = kolicina
                    });
                }

                _context.Entry(narudzba).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
        
            ViewBag.Korisnici = _context.Korisniks.ToList();
            ViewBag.Hrana = _context.Hranas.ToList();
            return View(narudzbaVM);
        }

        [HttpGet("delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid ID.");
            }

            var narudzba = await _context.Narudzbas
                .Include(n => n.Korisnik)
                .FirstOrDefaultAsync(n => n.Idnarudzba == id);

            if (narudzba == null)
            {
                return NotFound("Narudžba not found.");
            }

            var narudzbaVM = new NarudzbaVM
            {
                Datum = narudzba.Datum,
                Korisnik = narudzba.Korisnik,
                NarudzbaHranas = narudzba.NarudzbaHranas
            };

            return View(narudzbaVM);
        }


        [HttpPost("delete/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var narudzba = await _context.Narudzbas.FindAsync(id);
            if (narudzba == null)
            {
                return NotFound("Narudžba not found.");
            }

            _context.Narudzbas.Remove(narudzba);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

    }
}
