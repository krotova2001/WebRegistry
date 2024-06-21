using System.Linq;
using System.Text;
using WebRegistry.Models;
using WebRegistry.Services;

namespace WebRegistry.BackgroundServises
{
    //фоновая служба, актуализирующая ФИО пользователей системы
    public class _1CSynchronizeService : BackgroundService
    {
        private readonly IServiceProvider _provider;
        private readonly _1CService _service;

        public _1CSynchronizeService(IServiceProvider provider, _1CService service)
        {
            _provider = provider;
            _service = service;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
           // чтоб при запуске программы не сразу грузила ресурсы - 20 минут
            await Task.Delay(1200000, stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                using (IServiceScope scope = _provider.CreateScope()) 
                {
                    var context = scope.ServiceProvider.GetRequiredService<NomenclatureContext>();
                    var massAll = await _service.GetAll();
                    if (massAll != null)
                    {
                        var archivedShifres = massAll.Where(x => x.IsArc == true).Select(i=>i.Name);
                        archivedShifres = archivedShifres.Where(s => !s.EndsWith('_')).Select(x=>x = x.Replace(" ",""));
                        var redBookMassive = context.RedBookArchive.ToList();
                        var izdelies = context.Izdelies.ToList();

                        if (!archivedShifres.Any())
                            return;

                        foreach (var redBook in redBookMassive)
                        {
                            string Shifr = redBook.ShifrRedBook.Replace(" ", "");
                            if (archivedShifres.Contains(Shifr))
                                redBook.IsArchived = true;
                        }

                        foreach (var izdelie in izdelies)
                        {
                            string Shifr = izdelie.Shifr.Replace(" ", "");
                            if (archivedShifres.Contains(Shifr))
                                izdelie.IsArchived = 1;
                        }

                        await context.SaveChangesAsync(stoppingToken);
                    }
                }

                //вынести настройки тайминга
                await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
            }
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            await base.StopAsync(stoppingToken);
        }
    }
}
