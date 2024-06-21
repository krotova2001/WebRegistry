using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebRegistry.Models;
using WebRegistry.Services.ShifrRezerver;
using WebRegistry.Services;
using WebRegistry.Services.UserActionLog;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebRegistry.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RedBookApiController(NomenclatureContext context, IShifrService service, ILoggerFactory loggerFactory, IUserActionLogger ulog) : ControllerBase
    {
        private readonly NomenclatureContext _context = context;
        private readonly IShifrService _shifrService = service;
        private readonly ILoggerFactory _loggerFactory = loggerFactory;
        private readonly IUserActionLogger _ulogger = ulog;

        [HttpGet]
        public IActionResult Get()
        {
            var redBook = _context.RedBookArchive.OrderByDescending(t => t.userPublishingDate)
                .ThenByDescending(n => n.Inventory)
                .ToList();
            return new JsonResult(redBook);
        }

        // GET api/<RedBookApi>/5
        [HttpGet("{id}")]
        public IActionResult GetDetail(int id) 
        {
            var redBook = _context.RedBookArchive.Where(r => r.IdRedBook == id);
            return new JsonResult(redBook);
        }

        // POST api/<RedBookApi>
        //создание
        [HttpPost]
        [Authorize(Roles = "Admin, Constructor")]
        public async Task<IActionResult> Post([FromBody] RedBook[] newrows)
        {
            foreach (var rb in newrows)
            {
                if (ModelState.IsValid)
                {
                    if (!_shifrService.IsShifrExist(rb.ShifrRedBook))
                    {
                        _context.RedBookArchive.Add(rb);
                        var logger = _loggerFactory.CreateLogger<RedBook>();
                        logger.LogWarning("Создание записи в Красной книге {0} - {1}", rb.ShifrRedBook, UserHelper.GetFio(User.Identity.Name));
                        rb.AuthorLogin = User.Identity.Name;
                        await _context.SaveChangesAsync();
                        _ulogger.Log(User.Identity.Name, $"Создание записи в Красной книге - {rb.ShifrRedBook}");
                    }
                    else
                    {
                        return BadRequest($"Шифр {rb.ShifrRedBook} занят");
                    }
                }
            }
            return Ok();
        }


        // PUT api/<RedBookApi>/5
        //изменение
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin, Constructor")]
        public async Task<IActionResult> Put([FromBody] RedBook newRow)
        {
            if (ModelState.IsValid)
            {
                _context.Update(newRow);
                var logger = _loggerFactory.CreateLogger<RedBook>();
                logger.LogWarning("Изменение записи в Красной книге {0} - {1}", newRow.ShifrRedBook, UserHelper.GetFio(User.Identity.Name));
                await _context.SaveChangesAsync();
                return Ok();
            }
            return BadRequest();
        }

        // DELETE api/<RedBookApi>/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin, Constructor")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var rbToDelete = await _context.RedBookArchive.FindAsync(id);
            if (rbToDelete != null)
            {
                var deleted = _context.RedBookArchive.Remove(rbToDelete);
                var logger = _loggerFactory.CreateLogger<RedBook>();
                logger.LogWarning("Удаление записи в Красной книге {0} - {1}", deleted.Entity.ShifrRedBook, UserHelper.GetFio(User.Identity.Name));
                await _context.SaveChangesAsync();
                return Ok();
            }
            return NotFound();
        }

        //существует ли такой шифр
        [HttpGet("ShifrExist/{shifrRedBook}")]
        public Task<bool> ShifrExist(string shifrRedBook)
        {
           return Task.FromResult(_shifrService.IsShifrExist(shifrRedBook));
        }

        //существует ли такой инв номер
        [HttpGet("IsInvNumberExist/{invNumber}")]
        public Task<bool> IsInvNumberExistst(string invNumber)
        {
            return Task.FromResult(_shifrService.IsInvNumberExist(invNumber));
        }
    }
}
