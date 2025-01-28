using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApp.Models;
using System.Linq;
using System.Threading.Tasks;
using WebApp.ViewModels;

namespace WebApp.Controllers
{
    public class KategorijaHraneController : Controller
    {
        private readonly RwagrillContext _context;

        public KategorijaHraneController(RwagrillContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var kategorije = await _context.KategorijaHranes.ToListAsync();
            return View(kategorije);
        }

        [HttpGet("Details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            var kategorijaHrane = await _context.KategorijaHranes
                .Include(k => k.Hranas) 
                .FirstOrDefaultAsync(k => k.IdkategorijaHrane == id);

            if (kategorijaHrane == null)
            {
                return NotFound();
            }

            var kategorijaHraneVM = new KategorijaHraneVM
            {
                Naziv = kategorijaHrane.Naziv,
                Opis = kategorijaHrane.Opis
            };

            foreach (var hrana in kategorijaHrane.Hranas)
            {
                kategorijaHraneVM.Hranas.Add(hrana);
            }

            return View(kategorijaHraneVM);
        }



        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(KategorijaHrane kategorijaHrane)
        {
            if (ModelState.IsValid)
            {
                var lastId = await _context.KategorijaHranes.MaxAsync(k => (int?)k.IdkategorijaHrane) ?? 0;
                kategorijaHrane.IdkategorijaHrane = lastId + 1;

                _context.KategorijaHranes.Add(kategorijaHrane);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(kategorijaHrane);
        }

        [HttpGet("Edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            var kategorija = await _context.KategorijaHranes.FindAsync(id);
            if (kategorija == null)
            {
                return NotFound();
            }

            var kategorijaVM = new KategorijaHraneVM
            {
                IdkategorijaHrane = kategorija.IdkategorijaHrane,
                Naziv = kategorija.Naziv,
                Opis = kategorija.Opis
            };

            return View(kategorijaVM);
        }


        [HttpPost("Edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, KategorijaHraneVM kategorijaVM)
        {
            kategorijaVM.IdkategorijaHrane = id;
            if (id != kategorijaVM.IdkategorijaHrane)
            {
                return BadRequest("Invalid ID.");
            }

            if (ModelState.IsValid)
            {
                var kategorija = await _context.KategorijaHranes.FindAsync(id);
                if (kategorija == null)
                {
                    return NotFound();
                }

                kategorija.Naziv = kategorijaVM.Naziv;
                kategorija.Opis = kategorijaVM.Opis;

                _context.Entry(kategorija).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View("Edit",kategorijaVM);
        }



        private bool KategorijaHraneExists(int id)
        {
            return _context.KategorijaHranes.Any(e => e.IdkategorijaHrane == id);
        }


        [HttpGet("Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var kategorija = await _context.KategorijaHranes
                .Include(k => k.Hranas)
                .FirstOrDefaultAsync(k => k.IdkategorijaHrane == id);

            if (kategorija == null)
            {
                return NotFound();
            }

            var kategorijaVM = new KategorijaHraneVM
            {
                IdkategorijaHrane = kategorija.IdkategorijaHrane,
                Naziv = kategorija.Naziv,
                Opis = kategorija.Opis,
                Hranas = kategorija.Hranas.ToList()
            };

            return View(kategorijaVM);
        }


        [HttpPost("Delete/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var relatedHranas = _context.Hranas.Where(h => h.KategorijaHraneId == id);
            if (relatedHranas.Any())
            {
                _context.Hranas.RemoveRange(relatedHranas);
            }

            var kategorija = await _context.KategorijaHranes.FindAsync(id);
            if (kategorija == null)
            {
                return NotFound();
            }

            _context.KategorijaHranes.Remove(kategorija);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }



    }
}
