using System.DirectoryServices.AccountManagement;

namespace WebRegistry.Services
{
    public class UserHelper
    {
        //получить ФИО юзера по логину
        public static string GetFio(string fullLogin)
        {
            return GetParam(fullLogin, "fio");
        }

        public static string GetEmail(string fullLogin)
        {
            return GetParam(fullLogin, "email");
        }

        public static string GetParam(string fullLogin, string param)
        {
            string res = string.Empty;
            try
            {
                string login = fullLogin.Split('\\')[1];

                PrincipalContext userActiveDirectory = new PrincipalContext(ContextType.Domain, "sial-group.ru");
                UserPrincipal userName = new UserPrincipal(userActiveDirectory);
                PrincipalSearcher searchObj = new PrincipalSearcher(userName);

                foreach (UserPrincipal result in searchObj.FindAll())
                {
                    if (result.DisplayName != null && result.UserPrincipalName != null)
                    {
                        if (result.UserPrincipalName.ToLower().Contains(login.ToLower()))
                        {
                            if (param == "email")
                                res = result.EmailAddress;
                            if (param == "fio")
                                res = result.DisplayName;
                        }
                    }
                }
                searchObj.Dispose();
            }

            catch { }
            return res;
        }
    }
}
