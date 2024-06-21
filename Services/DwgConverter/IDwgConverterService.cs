namespace WebRegistry.Services.DwgConverter
{
    public interface IDwgConverterService
    {
        public void ExportDWGAToJpg(string sourceFilePath, string output);
        //public Task ExportDWGAToJpgAsync(string sourceFilePath, string output);
    }
}
