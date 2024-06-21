using System.Text;
using WebRegistry.Models;
using WebRegistry.Services;

namespace WebRegistry.BackgroundServises
{
    //фоновая служба, актуализирующая ФИО пользователей системы
    public class UserFioUpdateService : BackgroundService
    {
        private readonly IServiceProvider _provider;
        private User_roles[] _Users;

        public UserFioUpdateService(IServiceProvider provider)
        {
            _provider = provider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            //чтоб при запуске программы не сразу грузила ресурсы
            await Task.Delay(10000000, stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                using (IServiceScope scope = _provider.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<NomenclatureContext>();
                    _Users = context.User_roles.ToArray();
               

                foreach (var user in _Users)
                {
                    string fio = UserHelper.GetFio(user.Login);
                    if (fio != user.Fio)
                        user.Fio = fio;
                }
                await context.SaveChangesAsync(stoppingToken);
                }

                //вынести настройки тайминга
                await Task.Delay(TimeSpan.FromDays(30), stoppingToken);
            }
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            await base.StopAsync(stoppingToken);
        }
    }
}
