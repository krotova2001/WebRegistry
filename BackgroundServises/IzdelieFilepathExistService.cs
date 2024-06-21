using System.Text;
using WebRegistry.Models;

namespace WebRegistry.BackgroundServises
{
    //фоновая служба, проверяющая изделия на наличие битых чертежей
    public class IzdelieFilepathExistService : BackgroundService
    {
        private readonly IServiceProvider _provider;
        private string[] _filepathes;
        private StringBuilder erorrFiles;

        public IzdelieFilepathExistService(IServiceProvider provider)
        {
            _provider = provider;
            erorrFiles = new StringBuilder();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            //чтоб при запуске программы не сразу грузила ресурсы
            await Task.Delay(12000000, stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                using (IServiceScope scope = _provider.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<NomenclatureContext>();
                    _filepathes = context.Izdelies.Select(p => p.FilePath).ToArray();
                }

                foreach (var path in _filepathes)
                {
                    if (!File.Exists(path))
                        erorrFiles.AppendLine(path);
                }

                if (erorrFiles.Length > 0)
                {
                    WriteFileErr(erorrFiles.ToString());
                    erorrFiles.Clear();
                }

                //вынести настройки тайминга
                await Task.Delay(TimeSpan.FromDays(5), stoppingToken);
            }
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            await base.StopAsync(stoppingToken);
        }

        private void WriteFileErr(string content)
        {
            //вынести настройки путей
            string outputFile = @"C:\web\WebregistryLog\НетЧертежей.txt";
            File.WriteAllText(outputFile, content);
        }
    }
}
