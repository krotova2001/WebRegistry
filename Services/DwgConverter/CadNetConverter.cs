using CADImport;

namespace WebRegistry.Services.DwgConverter
{
    public class CadNetConverter : IDwgConverterService
    {
        //версия с российиской библиотекой, требует winforms
        void IDwgConverterService.ExportDWGAToJpg(string sourceFilePath, string output)
        {

            CADImage vDrawing = CADImage.CreateImageByExtension(sourceFilePath);
            vDrawing.LoadFromFile(sourceFilePath);
            Bitmap vBitmap = new Bitmap(2000, (int)(2000 * vDrawing.AbsHeight / vDrawing.AbsWidth));
            Graphics vGr = Graphics.FromImage(vBitmap);
            RectangleF vRect = new RectangleF(0, 0, 2000, (float)(vBitmap.Width * vDrawing.AbsHeight / vDrawing.AbsWidth));
            vDrawing.Draw(vGr, vRect);
            vBitmap.Save(output, System.Drawing.Imaging.ImageFormat.Png);

        }
        /*
        Task IDwgConverterService.ExportDWGAToJpgAsync(string sourceFilePath, string output)
        {

            CADImage vDrawing = CADImage.CreateImageByExtension(sourceFilePath);
            vDrawing.LoadFromFile(sourceFilePath);
            Bitmap vBitmap = new Bitmap(1000, (int)(1000 * vDrawing.AbsHeight / vDrawing.AbsWidth));
            Graphics vGr = Graphics.FromImage(vBitmap);
            RectangleF vRect = new RectangleF(0, 0, (float)1000, (float)(vBitmap.Width * vDrawing.AbsHeight / vDrawing.AbsWidth));
            vDrawing.Draw(vGr, vRect);
            return new Task(() => vBitmap.Save(output, System.Drawing.Imaging.ImageFormat.Jpeg));
        }
        */
    }
}

