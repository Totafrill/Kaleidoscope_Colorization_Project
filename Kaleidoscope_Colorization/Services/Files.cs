namespace Kolorowanie.Services
{
    public class Files
    {
        public static void RemoveOldFiles(string folderPath, TimeSpan threshold)
        {
            var files = Directory.GetFiles(folderPath);

            foreach (var filePath in files)
            {
                try
                {
                    var fileInfo = new FileInfo(filePath);

                    if (DateTime.Now - fileInfo.LastWriteTime > threshold)
                    {
                        System.IO.File.Delete(filePath);
                        Console.WriteLine($"Remove file: {filePath}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error during file removal: {ex.Message}");
                }
            }
        }
    }
}
