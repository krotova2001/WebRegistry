using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebRegistry.Models;
using WebRegistry.Services.DwgConverter;
using WebRegistry.Services.ShifrRezerver;

namespace WebRegistry.Controllers;

[Authorize]
public class IzdeliesController : Controller
{
    private readonly NomenclatureContext _context;
    private readonly Microsoft.AspNetCore.Hosting.IHostingEnvironment _environment;
    private readonly IDwgConverterService _dwgConverter;
    private readonly IShifrService _shifrService;

    public IzdeliesController(NomenclatureContext context, Microsoft.AspNetCore.Hosting.IHostingEnvironment environment, IDwgConverterService dwgConverter,
        IShifrService shifrService
    )
    {
        _context = context;
        _environment = environment;
        _dwgConverter = dwgConverter;
        _shifrService = shifrService;
    }

    // Спиок всех изделий
    public async Task<IActionResult> IndexFull()
    {
        var nomenclatureContext = _context.Izdelies
            .Include(e => e.DetailIdParentNavigations).ThenInclude(par => par.IdPartNavigation)
            .Include(t => t.DetailIdParentNavigations).ThenInclude(r => r.IdParentNavigation)
            .Include(i => i.IdCategoryNavigation);
        return View(await nomenclatureContext.ToListAsync());
    }

    //вариант с пагинацией
    public async Task<IActionResult> Index(string? shifr, int startIndex=0, int pageSize=50)
    {
        var nomenclatureContext = _context.Izdelies
            .Skip(startIndex).Take(pageSize)
            .Include(i => i.IdCategoryNavigation)
            .Include(e => e.DetailIdParentNavigations).ThenInclude(par => par.IdPartNavigation)
            .Include(t => t.DetailIdParentNavigations).ThenInclude(r => r.IdParentNavigation)
            .Include(s => s.SystemasNavigation).ThenInclude(ss => ss.IdSystemaNavigation)
            .Include(ex => ex.ExtcomponentsNavigations).ThenInclude(ee => ee.ExternalComponentIdNavigation);
            
        //поиск по шифру
        if (shifr != null)
            nomenclatureContext = _context.Izdelies.Where(p => p.Shifr.ToLower().Contains(shifr.ToLower()))
                .Include(i => i.IdCategoryNavigation)
                .Include(e => e.DetailIdParentNavigations).ThenInclude(par => par.IdPartNavigation)
                .Include(t => t.DetailIdPartNavigations).ThenInclude(r => r.IdParentNavigation)
                .Include(s => s.SystemasNavigation).ThenInclude(ss => ss.IdSystemaNavigation)
                .Include(ex => ex.ExtcomponentsNavigations).ThenInclude(ee => ee.ExternalComponentIdNavigation);
        ;

        ViewData["startIndex"] = startIndex;
        ViewData["pageSize"] = pageSize;
        ViewData["IzdelieCount"] = _context.Izdelies.Count();
        ViewData["Added"] = TempData["Added"];
        return View(await nomenclatureContext.ToListAsync());
    }

    // GET: Izdelies/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var izdelie = await _context.Izdelies
            .Include(e => e.DetailIdParentNavigations).ThenInclude(par => par.IdPartNavigation)
            .Include(t => t.DetailIdPartNavigations).ThenInclude(r => r.IdParentNavigation)
            .Include(s => s.SystemasNavigation).ThenInclude(ss => ss.IdSystemaNavigation)
            .Include(i => i.IdCategoryNavigation)
            .Include(ex => ex.ExtcomponentsNavigations).ThenInclude(ee => ee.ExternalComponentIdNavigation)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (izdelie == null)
        {
            return NotFound();
        }

        //создать или найти картинку чертежа
        try
        {
            var fileSource = Path.GetFullPath(izdelie.FilePath);
            //сделать универсальный путь
            
            //редко, но может быть, что чертеж в pdf
            if (Path.GetExtension(fileSource) == ".dwg")
            {
                string filedestination = Path.Combine(_environment.WebRootPath, $"drafts\\{izdelie.Shifr}_{izdelie.Id}.png");
                if (!System.IO.File.Exists(filedestination))
                {
                    _dwgConverter.ExportDWGAToJpg(fileSource, filedestination);
                }
            }
            if (Path.GetExtension(fileSource) == ".pdf")
            {
                string filedestination = Path.Combine(_environment.WebRootPath, $"drafts\\{izdelie.Shifr}_{izdelie.Id}.pdf");
                if (!System.IO.File.Exists(filedestination))
                {
                    System.IO.File.Copy(fileSource, filedestination);
                }
            }
        }
        catch (Exception ex)
        {
            ViewBag.ErrorMessage = ex.Message;
        }
        return View(izdelie);
    }

    // GET: Izdelies/Create
       
    public IActionResult Create(Izdelie izdelie)
    {
        ViewData["IdCategory"] = new SelectList(_context.Categories, "IdCategory", "IdCategory");
        return View();
    }

    // POST: Izdelies/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Save([Bind("Id,Shifr,FilePath,JX,WX,JY,WY,Square,Weight,Perimeter,CircleDiametr,DifficultyGroup,Gost,Razrab,Prov,Tkontr,Nkontr,Master,Utverd,CustomerShifr,IdCategory,WeightAl,Articul,ContainExtComponent,Note,IsArchived,ContainDetails,CustomerName")] Izdelie izdelie)
    {
        if (ModelState.IsValid)
        {
            izdelie.IsReserved = 1;

            //дополнительная проверка на уникальность шфира, если вдруг кто-то успел зарезервировать этот шифр
                
            if (!_shifrService.IsShifrExist(izdelie.Shifr))
            {
                _context.Add(izdelie);
                await _context.SaveChangesAsync();
                TempData["Added"] = "Изделие успешно добавлено";
                return RedirectToAction(nameof(Index), new {shifr = izdelie.Shifr});
            }

            else
            {
                //обновить на единый сервис
                ModelState.Clear();
                izdelie.Shifr = _shifrService.GetNewShifr();
                TempData["ShifrError"] = "Кто-то успел занять ваш шифр, мы его обновили";
                return View(izdelie);
            }
        }

        ViewData["IdCategory"] = new SelectList(_context.Categories, "IdCategory", "Name", izdelie.IdCategory);

        return View(izdelie);
    }

    // GET: Izdelies/Edit/5
    [Authorize(Roles = "Admin, Constructor")]
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var izdelie = await _context.Izdelies.FindAsync(id);
        if (izdelie == null)
        {
            return NotFound();
        }
        ViewData["IdCategory"] = new SelectList(_context.Categories, "IdCategory", "Name", izdelie.IdCategory);
        return View(izdelie);
    }

    // POST: Izdelies/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        
    [HttpPost]
    [Authorize(Roles = "Admin, Constructor")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Shifr,FilePath,JX,WX,JY,WY,Square,Weight,Perimeter,CircleDiametr,DifficultyGroup,Gost,Razrab,Prov,Tkontr,Nkontr,Master,Utverd,CustomerShifr,IdCategory,WeightAl,Articul,ContainExtComponent,Note,IsArchived")] Izdelie izdelie)
    {
        if (id != izdelie.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(izdelie);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!IzdelieExists(izdelie.Id))
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
        ViewData["IdCategory"] = new SelectList(_context.Categories, "IdCategory", "Name", izdelie.IdCategory);
        return View(izdelie);
    }

    // GET: Izdelies/Delete/5
    [Authorize(Roles = "Admin, Constructor")]
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var izdelie = await _context.Izdelies
            .Include(i => i.IdCategoryNavigation)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (izdelie == null)
        {
            return NotFound();
        }

        return View(izdelie);
    }

    // POST: Izdelies/Delete/5
        
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin, Constructor")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var izdelie = await _context.Izdelies.FindAsync(id);
        if (izdelie != null)
        {
            _context.Izdelies.Remove(izdelie);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool IzdelieExists(int id)
    {
        return _context.Izdelies.Any(e => e.Id == id);
    }
}