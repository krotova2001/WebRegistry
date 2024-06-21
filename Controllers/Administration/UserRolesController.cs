using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebRegistry.Models;
using WebRegistry.Services.AuthorizationProvider;

namespace WebRegistry.Controllers.Administration
{

    [Authorize(Roles = SialSimpleRoleProvider.ADMIN)]
    public class UserRolesController : Controller
    {
        private readonly NomenclatureContext _context;

        public UserRolesController(NomenclatureContext context)
        {
            _context = context;
        }

        // GET: UserRoles
        public async Task<IActionResult> Index()
        {
            var nomenclatureContext = _context.User_roles.Include(u => u.UserRoleNavigation);
            return View("~/Views/Administration/Userroles/Index.cshtml", await nomenclatureContext.ToListAsync());
        }

        // GET: UserRoles/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user_roles = await _context.User_roles
                .Include(u => u.UserRoleNavigation)
                .FirstOrDefaultAsync(m => m.idUserRole == id);
            if (user_roles == null)
            {
                return NotFound();
            }

            return View("~/Views/Administration/Userroles/Details.cshtml", user_roles);
        }

        // GET: UserRoles/Create
        public IActionResult Create()
        {
            ViewData["roleId"] = new SelectList(_context.Roles, "iduser_roles", "role");
            return View("~/Views/Administration/Userroles/Create.cshtml");
        }

        // POST: UserRoles/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("idUserRole,Login,roleId")] User_roles user_roles)
        {
            if (ModelState.IsValid)
            {
                _context.Add(user_roles);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["roleId"] = new SelectList(_context.Roles, "iduser_roles", "role", user_roles.roleId);
            return View("~/Views/Administration/Userroles/Create.cshtml", user_roles);
        }

        // GET: UserRoles/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user_roles = await _context.User_roles.FindAsync(id);
            if (user_roles == null)
            {
                return NotFound();
            }

            //себя нельзя менять, только через другого админа
            if (user_roles.Login.ToLower() == User.Identity.Name.ToLower())
            {
                TempData["ErrorSelfUser"] = true;
                return RedirectToAction("Index");
            }

            ViewData["roleId"] = new SelectList(_context.Roles, "iduser_roles", "role", user_roles.roleId);
            return View("~/Views/Administration/Userroles/Edit.cshtml", user_roles);
        }

        // POST: UserRoles/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("idUserRole,Login,roleId")] User_roles user_roles)
        {
            if (id != user_roles.idUserRole)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    //баг - пользователь не может менять сам свои роли почему-то. Получается двойное отслеживание в EFCore
                    _context.Update(user_roles);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!User_rolesExists(user_roles.idUserRole))
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
            ViewData["roleId"] = new SelectList(_context.Roles, "iduser_roles", "role", user_roles.roleId);
            return View("~/Views/Administration/Userroles/Edit.cshtml", user_roles);
        }

        // GET: UserRoles/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user_roles = await _context.User_roles
                .Include(u => u.UserRoleNavigation)
                .FirstOrDefaultAsync(m => m.idUserRole == id);
            if (user_roles == null)
            {
                return NotFound();
            }

            return View("~/Views/Administration/Userroles/Delete.cshtml", user_roles);
        }

        // POST: UserRoles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user_roles = await _context.User_roles.FindAsync(id);
            if (user_roles != null)
            {
                _context.User_roles.Remove(user_roles);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool User_rolesExists(int id)
        {
            return _context.User_roles.Any(e => e.idUserRole == id);
        }
    }
}
