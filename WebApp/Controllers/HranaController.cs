using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApp.Models;
using WebApp.ViewModels;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using X.PagedList.Mvc.Core;
using X.PagedList.Extensions;

namespace WebApp.Controllers
{
    [Route("[controller]")]
    public class HranaController : Controller
    {
        private readonly RwagrillContext _context;

        public HranaController(RwagrillContext context)
        {
            _context = context;
        }

        [HttpGet("")]
        public async Task<IActionResult> Index(string search, int? filter, int page = 1, int pageSize = 10)
        {
            var query = _context.Hranas
                .Include(h => h.KategorijaHrane)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(h => h.Naslov.Contains(search) || h.Opis.Contains(search));
            }

            if (filter.HasValue)
            {
                query = query.Where(h => h.KategorijaHraneId == filter);
            }

            var hranaList = query
                .OrderBy(h => h.Naslov)
                .Select(h => new HranaVM
                {
                    Idhrana = h.Idhrana,
                    Naslov = h.Naslov,
                    Opis = h.Opis,
                    Cijena = h.Cijena,
                    KategorijaHrane = h.KategorijaHrane,
                    HranaAlergens = h.HranaAlergens.ToList(),
                    NarudzbaHranas = h.NarudzbaHranas.ToList()
                })
                .ToPagedList(page, pageSize);

            ViewBag.Search = search;
            ViewBag.Filter = filter;
            ViewBag.PageSize = pageSize;
            ViewBag.Categories = await _context.KategorijaHranes.ToListAsync();

            return View(hranaList);
        }



        [HttpGet("Details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            var hrana = await _context.Hranas
                .Include(h => h.KategorijaHrane)
                .Include(h => h.HranaAlergens)
                .Include(h => h.NarudzbaHranas)
                .FirstOrDefaultAsync(h => h.Idhrana == id);

            if (hrana == null)
            {
                return NotFound();
            }

            var hranaVM = new HranaVM
            {
                Idhrana = hrana.Idhrana,
                Naslov = hrana.Naslov,
                Opis = hrana.Opis,
                Cijena = hrana.Cijena,
                KategorijaHrane = hrana.KategorijaHrane,
                HranaAlergens = hrana.HranaAlergens.ToList(),
                NarudzbaHranas = hrana.NarudzbaHranas.ToList()
            };

            return View(hranaVM);
        }


        [HttpGet("Create")]
        public IActionResult Create()
        {
            ViewBag.Categories = _context.KategorijaHranes.ToList();
            return View();
        }


        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(HranaVM hranaVM)
        {
                var hrana = new Hrana
                {
                    Naslov = hranaVM.Naslov,
                    Opis = hranaVM.Opis,
                    Cijena = hranaVM.Cijena,
                    KategorijaHraneId = hranaVM.KategorijaHrane?.IdkategorijaHrane
                };

                _context.Hranas.Add(hrana);

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));

            ViewBag.Categories = _context.KategorijaHranes.ToList();
            return View(hranaVM);
        }

        [HttpGet("Update/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            var hrana = await _context.Hranas.Include(h => h.KategorijaHrane).FirstOrDefaultAsync(h => h.Idhrana == id);
            if (hrana == null)
            {
                return NotFound();
            }

            ViewBag.Categories = await _context.KategorijaHranes.ToListAsync();

            var hranaVM = new HranaVM
            {
                Idhrana = hrana.Idhrana,
                Naslov = hrana.Naslov,
                Opis = hrana.Opis,
                Cijena = hrana.Cijena,
                KategorijaHrane = hrana.KategorijaHrane
            };

            return View(hranaVM);
        }

        [HttpPost("Update/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, HranaVM hranaVM)
        {
            
            var hrana = await _context.Hranas.FindAsync(id);
            if (hrana == null)
            {
                return NotFound();
            }

            hrana.Naslov = hranaVM.Naslov;
            hrana.Opis = hranaVM.Opis;
            hrana.Cijena = hranaVM.Cijena;
            hrana.KategorijaHraneId = hranaVM.KategorijaHrane?.IdkategorijaHrane;

            _context.Entry(hrana).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }



        [HttpGet("Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var hrana = await _context.Hranas
                .Include(h => h.KategorijaHrane)
                .FirstOrDefaultAsync(h => h.Idhrana == id);

            if (hrana == null)
            {
                return NotFound();
            }

            var hranaVM = new HranaVM
            {
                Idhrana = hrana.Idhrana,
                Naslov = hrana.Naslov,
                Opis = hrana.Opis,
                Cijena = hrana.Cijena,
                KategorijaHrane = hrana.KategorijaHrane
            };

            return View(hranaVM);
        }


        [HttpPost("Delete/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var relatedHranaAlergens = _context.HranaAlergens.Where(ha => ha.HranaId == id);
            _context.HranaAlergens.RemoveRange(relatedHranaAlergens);

            var hrana = await _context.Hranas.FindAsync(id);
            if (hrana == null)
            {
                return NotFound();
            }

            _context.Hranas.Remove(hrana);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

    }
}
