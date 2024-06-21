using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using WebRegistry.Models;
using WebRegistry.Services.AuthorizationProvider;

namespace WebRegistry.Controllers
{
    public class ExtComponentsController : Controller
    {
        private readonly NomenclatureContext _context;
        private IMemoryCache _cache; //на всякий случай, если будет тормозить или кого-то не устраивать скорость
        public ExtComponentsController(NomenclatureContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        // GET: ExtComponentsController
        public async Task<IActionResult> Index()
        {
            var nomenclatureContext = _context.ExternalComponents.AsNoTracking().
                Include(i => i.IzdelieNavigation).ThenInclude(iz => iz.IzdelieIdNavigation);
            return View(await nomenclatureContext.ToListAsync());
        }


        // GET: ExtComponentsController/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var extcomponent = await _context.ExternalComponents
                .Include(i => i.IzdelieNavigation).ThenInclude(iz => iz.IzdelieIdNavigation)
                .FirstOrDefaultAsync(m => m.idExternalComponents == id);
            if (extcomponent == null)
            {
                return NotFound();
            }

            return View(extcomponent);
        }

        // GET: ExtComponentsController/Create
        [Authorize(Roles = "Admin, Constructor")]
        public ActionResult Create(int? id)
        {
            return View();
        }

        // POST: ExtComponentsController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Constructor")]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: ExtComponentsController/Edit/5
        [Authorize(Roles = "Admin, Constructor")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var e = await _context.ExternalComponents.FindAsync(id);
            if (e == null)
            {
                return NotFound();
            }
            return View(e);
        }

        // POST: ExtComponentsController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Constructor")]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: ExtComponentsController/Delete/5
        [Authorize(Roles = "Admin, Constructor")]
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: ExtComponentsController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Constructor")]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
