using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebRegistry.Models;

namespace WebRegistry.Controllers
{
    [Authorize]
    public class SystemasController : Controller
    {
        private readonly NomenclatureContext _context;

        public SystemasController(NomenclatureContext context)
        {
            _context = context;
        }
      
        public async Task<IActionResult> Index()
        {
            var nomenclatureContext = _context.Systemas.Include(e => e.IzdelieNavigaion).ThenInclude(i => i.IdIzdelieNavigation);
            return View(await nomenclatureContext.ToListAsync());
        }

        // GET: SystemasControllercs/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: SystemasControllercs/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: SystemasControllercs/Create
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

        // GET: SystemasControllercs/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: SystemasControllercs/Edit/5
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

        // GET: SystemasControllercs/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: SystemasControllercs/Delete/5
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
