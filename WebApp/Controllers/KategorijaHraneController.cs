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


        public async Task<IActionResult> Edit(int id)
        {
            var kategorija = await _context.KategorijaHranes.FindAsync(id);
            if (kategorija == null)
            {
                return NotFound();
            }

            return View(kategorija);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, KategorijaHrane kategorijaHrane)
        {
            if (id != kategorijaHrane.IdkategorijaHrane)
            {
                return BadRequest("Invalid ID.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Entry(kategorijaHrane).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!KategorijaHraneExists(kategorijaHrane.IdkategorijaHrane))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return RedirectToAction(nameof(Index));
            }

            return View(kategorijaHrane);
        }

        private bool KategorijaHraneExists(int id)
        {
            return _context.KategorijaHranes.Any(e => e.IdkategorijaHrane == id);
        }


        public async Task<IActionResult> Delete(int id)
        {
            var kategorija = await _context.KategorijaHranes.FirstOrDefaultAsync(k => k.IdkategorijaHrane == id);

            if (kategorija == null)
            {
                return NotFound();
            }

            return View(kategorija);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
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
