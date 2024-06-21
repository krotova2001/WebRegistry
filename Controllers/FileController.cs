using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IO;

namespace WebRegistry.Controllers
{
    [Authorize]
    public class FileController : Controller
    {
        [HttpGet]
        public IActionResult GetDwgFile(string filename)
        {
            if (System.IO.File.Exists(filename))
            {
                string name = Path.GetFileName(filename);
                var bytes = System.IO.File.ReadAllBytes(filename);
                return new FileContentResult(bytes, "application/acad")
                {
                    FileDownloadName = name
                };
            }
            else
            {
                return BadRequest("Файла не существует");
            }
        }
    }
}
