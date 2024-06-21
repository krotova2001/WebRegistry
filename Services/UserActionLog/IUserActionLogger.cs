namespace WebRegistry.Services.UserActionLog
{
    public interface IUserActionLogger
    {
        public void Log(string userlogin, string message);
    }
}
