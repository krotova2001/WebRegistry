using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebRegistry.Models;
using WebRegistry.Services.AuthorizationProvider;

namespace WebRegistry.Controllers
{
    [Authorize]
    public class DetailsController : Controller
    {
        private readonly NomenclatureContext _context;

        public DetailsController(NomenclatureContext context)
        {
            _context = context;
        }

        // GET: Details
        public async Task<IActionResult> Index()
        {
            var nomenclatureContext = _context.Details.Include(d => d.IdParentNavigation).Include(d => d.IdPartNavigation);
            return View(await nomenclatureContext.ToListAsync());
        }

        // GET: Details/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var detail = await _context.Details
                .Include(d => d.IdParentNavigation)
                .Include(d => d.IdPartNavigation)
                .FirstOrDefaultAsync(m => m.IdLink == id);
            if (detail == null)
            {
                return NotFound();
            }

            return View(detail);
        }

        // GET: Details/Create
        [Authorize(Roles = "Admin, Constructor")]
        public IActionResult Create()
        {
            ViewData["IdParent"] = new SelectList(_context.Izdelies, "Id", "Id");
            ViewData["IdPart"] = new SelectList(_context.Izdelies, "Id", "Id");
            return View();
        }

        // POST: Details/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Constructor")]
        public async Task<IActionResult> Create([Bind("IdLink,IdParent,IdPart,Count,Note")] Detail detail)
        {
            if (ModelState.IsValid)
            {
                _context.Add(detail);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdParent"] = new SelectList(_context.Izdelies, "Id", "Id", detail.IdParent);
            ViewData["IdPart"] = new SelectList(_context.Izdelies, "Id", "Id", detail.IdPart);
            return View(detail);
        }

        // GET: Details/Edit/5
        [Authorize(Roles = "Admin, Constructor")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var detail = await _context.Details.FindAsync(id);
            if (detail == null)
            {
                return NotFound();
            }
            ViewData["IdParent"] = new SelectList(_context.Izdelies, "Id", "Id", detail.IdParent);
            ViewData["IdPart"] = new SelectList(_context.Izdelies, "Id", "Id", detail.IdPart);
            return View(detail);
        }

        // POST: Details/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Constructor")]
        public async Task<IActionResult> Edit(int id, [Bind("IdLink,IdParent,IdPart,Count,Note")] Detail detail)
        {
            if (id != detail.IdLink)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(detail);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DetailExists(detail.IdLink))
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
            ViewData["IdParent"] = new SelectList(_context.Izdelies, "Id", "Id", detail.IdParent);
            ViewData["IdPart"] = new SelectList(_context.Izdelies, "Id", "Id", detail.IdPart);
            return View(detail);
        }

        // GET: Details/Delete/5
        [Authorize(Roles = "Admin, Constructor")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var detail = await _context.Details
                .Include(d => d.IdParentNavigation)
                .Include(d => d.IdPartNavigation)
                .FirstOrDefaultAsync(m => m.IdLink == id);
            if (detail == null)
            {
                return NotFound();
            }

            return View(detail);
        }

        // POST: Details/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Constructor")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var detail = await _context.Details.FindAsync(id);
            if (detail != null)
            {
                _context.Details.Remove(detail);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DetailExists(int id)
        {
            return _context.Details.Any(e => e.IdLink == id);
        }
    }
}
