namespace Kolorowanie.Services
{
    public class Files
    {
        public static void RemoveOldFiles(string folderPath, TimeSpan threshold)
        {
            // Pobierz list� plik�w w folderze
            var files = Directory.GetFiles(folderPath);

            foreach (var filePath in files)
            {
                try
                {
                    // Pobierz informacje o pliku
                    var fileInfo = new FileInfo(filePath);

                    // Sprawd�, czy plik jest starszy ni� threshold
                    if (DateTime.Now - fileInfo.LastWriteTime > threshold)
                    {
                        // Usu� plik
                        System.IO.File.Delete(filePath);
                        Console.WriteLine($"Remove file: {filePath}");
                    }
                }
                catch (Exception ex)
                {
                    // Obs�u� b��dy, je�li istniej�
                    Console.WriteLine($"Error during file removal: {ex.Message}");
                }
            }
        }
    }
}
