using WebRegistry.Models;

namespace WebRegistry.Services.UserActionLog;

public class UserActionToDB:IUserActionLogger
{
    private readonly NomenclatureContext _context;
    public UserActionToDB(NomenclatureContext context)
    {
        _context = context;
    }

    public async void Log(string userlogin, string message)
    {
        var log = new UserAction
        {
            date = DateTime.Now,
            userLogin = userlogin,
            actionText = message
        };
        _context.UserActions.Add(log);
        await _context.SaveChangesAsync();
    }
}