using Aspose.CAD.ImageOptions;

namespace WebRegistry.Services.DwgConverter
{
    //класс экспорта DWG в разные форматы
    public class AsposeConverter : IDwgConverterService
    {
        public int resX { get; set; } = 1200;
        public int resY { get; set; } = 1200;

        void IDwgConverterService.ExportDWGAToJpg(string sourceFilePath, string output)
        {
            var file = File.OpenRead(sourceFilePath);
            using (Aspose.CAD.Image image = Aspose.CAD.Image.Load(file))
            {
                CadRasterizationOptions rasterizationOptions = new CadRasterizationOptions();
                rasterizationOptions.PageWidth = resX;
                rasterizationOptions.PageHeight = resY;
                ImageOptionsBase options = new PngOptions();
                options.VectorRasterizationOptions = rasterizationOptions;
                image.Save(output, options);
            }
        }

        async Task ExportDWGAToJpgAsync(string sourceFilePath, string output)
        {
            var file = File.OpenRead(sourceFilePath);
            using (Aspose.CAD.Image image = Aspose.CAD.Image.Load(file))
            {
                CadRasterizationOptions rasterizationOptions = new CadRasterizationOptions();

                rasterizationOptions.PageWidth = resX;
                rasterizationOptions.PageHeight = resY;
                ImageOptionsBase options = new PngOptions();
                options.VectorRasterizationOptions = rasterizationOptions;
                await image.SaveAsync(output, options);
            }
        }
    }
}
