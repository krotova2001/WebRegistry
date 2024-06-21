using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebRegistry.Models;
using WebRegistry.Services.DraftConverter;
using WebRegistry.Services.DwgConverter;
using WebRegistry.Services.ShifrRezerver;

namespace WebRegistry.Controllers.AddingDraft
{
    [Authorize(Roles = "Admin, Constructor")]
    public class AddDraftController : Controller
    {
        private Microsoft.AspNetCore.Hosting.IHostingEnvironment Environment;
        private readonly IShifrService _shifrService;
        private readonly IDwgConverterService dwgConverter;

        public AddDraftController(Microsoft.AspNetCore.Hosting.IHostingEnvironment environment, IShifrService shifrService,
            IDwgConverterService dwgConverterService)
        {
            Environment = environment;
            _shifrService = shifrService;
            dwgConverter = dwgConverterService;
        }   

        [HttpGet]
        public IActionResult Index()
        {
            return View("AddDraft");
        }

        [HttpGet]
        public IActionResult RedBook()
        {
            return View("RedBook");
        }

        [HttpGet]
        public IResult ShifrExist(string shifr)
        {
            if (_shifrService.IsShifrExist(shifr))
            {
                return Results.Ok("Шифр занят");
            }
            return Results.Ok("Шифр свободен");
            
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(IFormFile file)
        {
            //добавить проверку на разрешение dwg
            if (file != null && file.Length>0)
            {
                if (Path.GetExtension(file.FileName).ToLower() != ".dwg")
                {
                    TempData["Error"] = "Неверное расширение файала";
                    return RedirectToAction("Index");
                }

                var temDir = Path.Combine(Environment.WebRootPath, "temp");
                var tempFile = Path.Combine(temDir, file.FileName);

                using (var stream = System.IO.File.Create(tempFile))
                {
                    file.CopyTo(stream);
                }

                DraftToIzdelie toIzdelie = new DraftToIzdelie(tempFile, _shifrService);
                Izdelie newIzdelie = toIzdelie.GetNewIzdelie;
                newIzdelie.IsReserved = 1;

                //показать картинку
                var fileSource = Path.GetFullPath(tempFile);
                string filedestination = Path.Combine(temDir, $"temp.png");
                dwgConverter.ExportDWGAToJpg(fileSource, filedestination);

                if (Environment.IsProduction())
                {
                    //System.IO.File.SetAttributes(tempFile, FileAttributes.Normal);
                    //System.IO.File.Delete(tempFile);
                }

                return View("../Izdelies/Create", newIzdelie);
            }
            else
                return RedirectToAction("Index");
        }
    }
}
