using Microsoft.EntityFrameworkCore;
using WebRegistry.Models;

namespace WebRegistry.Services.AuthorizationProvider
{
    public interface ISimpleRoleProvider
    {
        Task<ICollection<string>> GetUserRolesAsync(string userName);
    }
    //поиск и присвоение роли юзера в базе данных.
    //дефолтная роль - BasicUser
    public class SialSimpleRoleProvider : ISimpleRoleProvider
    {
        public const string ADMIN = "Admin";
        public const string BASIC_USER = "BasicUser";
        public const string CONSTRUCTOR = "Constructor";
        private readonly NomenclatureContext _context;

        public SialSimpleRoleProvider(NomenclatureContext context)
        {
            _context = context;
        }

        public Task<ICollection<string>> GetUserRolesAsync(string userName)
        {
            ICollection<string> result = new string[0];

            if (!string.IsNullOrEmpty(userName))
            {
                List<string> roles = new List<string>();
                var userroles = _context.User_roles.Where(r => r.Login.ToLower() == userName.ToLower()).Include(t => t.UserRoleNavigation).ToList();

                //если юзер есть в базе - берем его роли
                if (userroles.Any())
                {
                    foreach (var r in userroles)
                    {
                        roles.Add(r.UserRoleNavigation.role);
                    }
                    result = roles.ToArray();
                }

                //если нет - добавляем его в базу с базовой ролью
                else
                {
                    var basicRole = _context.Roles.Where(r => r.role == BASIC_USER).FirstOrDefault();
                    if (basicRole != null)
                    {
                        _context.User_roles.Add(
                            new User_roles { Login = userName, UserRoleNavigation = basicRole, Fio = UserHelper.GetFio(userName) });
                    }
                    _context.SaveChanges();
                    result = new[] { BASIC_USER };
                }
            }
            return Task.FromResult(result);
        }
    }
}
