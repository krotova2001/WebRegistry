using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebRegistry.Models;
using WebRegistry.Services.AuthorizationProvider;

namespace WebRegistry.Controllers
{
    [Authorize]
    public class ExtComponentsIzdelieController : Controller
    {
        private readonly NomenclatureContext _context;

        public ExtComponentsIzdelieController(NomenclatureContext context)
        {
            _context = context;
        }

        // GET: Extcomponents
        public async Task<IActionResult> Index()
        {
            var nomenclatureContext = _context.Extcomponents.AsNoTracking().Include(e => e.IzdelieIdNavigation);
            return View(await nomenclatureContext.ToListAsync());
        }

        // GET: Extcomponents/Details/5
        [Authorize(Roles = SialSimpleRoleProvider.ADMIN)]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var extcomponent = await _context.Extcomponents
                .Include(e => e.IzdelieIdNavigation)
                .FirstOrDefaultAsync(m => m.IdExtComponent == id);
            if (extcomponent == null)
            {
                return NotFound();
            }

            return View(extcomponent);
        }

        [Authorize(Roles = SialSimpleRoleProvider.ADMIN)]
        public IActionResult Create()
        {
            ViewData["IzdelieId"] = new SelectList(_context.Izdelies, "Id", "Id");
            return View();
        }

        // POST: Extcomponents/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
       
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = SialSimpleRoleProvider.ADMIN)]
        public async Task<IActionResult> Create([Bind("IdExtComponent,ParentShifr,Name,Shifr,Count,Weight,IzdelieId,Note")] ExtCompInIzdelie extcomponent)
        {
            if (ModelState.IsValid)
            {
                _context.Add(extcomponent);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IzdelieId"] = new SelectList(_context.Izdelies, "Id", "Id", extcomponent.IzdelieId);
            return View(extcomponent);
        }

        // GET: Extcomponents/Edit/5
        [Authorize(Roles = SialSimpleRoleProvider.ADMIN)]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var extcomponent = await _context.Extcomponents.FindAsync(id);
            if (extcomponent == null)
            {
                return NotFound();
            }
            ViewData["IzdelieId"] = new SelectList(_context.Izdelies, "Id", "Id", extcomponent.IzdelieId);
            return View(extcomponent);
        }

        // POST: Extcomponents/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = SialSimpleRoleProvider.ADMIN)]
        public async Task<IActionResult> Edit(int id, [Bind("IdExtComponent,ParentShifr,Name,Shifr,Count,Weight,IzdelieId,Note")] ExtCompInIzdelie extcomponent)
        {
            if (id != extcomponent.IdExtComponent)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(extcomponent);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ExtcomponentExists(extcomponent.IdExtComponent))
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
            ViewData["IzdelieId"] = new SelectList(_context.Izdelies, "Id", "Id", extcomponent.IzdelieId);
            return View(extcomponent);
        }

        // GET: Extcomponents/Delete/5
        [Authorize(Roles = SialSimpleRoleProvider.ADMIN)]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var extcomponent = await _context.Extcomponents
                .Include(e => e.IzdelieIdNavigation)
                .FirstOrDefaultAsync(m => m.IdExtComponent == id);
            if (extcomponent == null)
            {
                return NotFound();
            }

            return View(extcomponent);
        }

        // POST: Extcomponents/Delete/5
        [Authorize(Roles = SialSimpleRoleProvider.ADMIN)]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var extcomponent = await _context.Extcomponents.FindAsync(id);
            if (extcomponent != null)
            {
                _context.Extcomponents.Remove(extcomponent);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ExtcomponentExists(int id)
        {
            return _context.Extcomponents.Any(e => e.IdExtComponent == id);
        }
    }
}
