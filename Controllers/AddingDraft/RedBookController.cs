using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebRegistry.Controllers.AddingDraft
{
    public class RedBookController : Controller
    {
        // GET: RedBookController
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ShowAllBook()
        {
            return View("RedBookArchive");
        }

        // GET: RedBookController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: RedBookController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: RedBookController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
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

        // GET: RedBookController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: RedBookController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
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

        // GET: RedBookController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: RedBookController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
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
