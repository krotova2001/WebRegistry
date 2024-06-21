namespace WebRegistry.Services.ShifrRezerver
{
    public interface IShifrService
    {
        bool IsShifrExist(string shifr);
        string GetNewShifr();
        bool RezerveShifr(string shifr);
        int GetNewInvNumber();
        bool IsInvNumberExist(string number);
    }
}
