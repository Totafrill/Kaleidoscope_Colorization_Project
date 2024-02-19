
namespace Kolorowanie.Services
{
    public class Paths
    {
        public static string Models_path_fusion { get; set; }
        public static string Models_path_plain { get; set; }
        public static string Model_path_inception { get; set; }
        public static string Uploads_folder { get; set; }
        public static List<string> Paths_of_images_short { get; set; }
        public static List<string> Paths_of_images { get; set; }

        public static void SetPaths(IWebHostEnvironment _environment)
        {
            Models_path_fusion = System.IO.Path.Combine(_environment.WebRootPath, "models\\models_onnx\\with_fusion"); // Ścieżka do pliku modelu ONNX
            Models_path_plain = System.IO.Path.Combine(_environment.WebRootPath, "models\\models_onnx\\without_fusion"); // Ścieżka do pliku modelu ONNX
            Model_path_inception = System.IO.Path.Combine(_environment.WebRootPath, "models\\models_for_fusion_model_use\\inception_model.onnx"); // Ścieżka do pliku modelu ONNX
            Uploads_folder = System.IO.Path.Combine(_environment.WebRootPath, "images\\work_images");
            Paths_of_images_short = new List<string>();
            Paths_of_images = new List<string>();
        }

        public static void ClearImagesPaths()
        {
            if (Paths_of_images_short != null)
            {
                Paths_of_images_short.Clear();
                Paths_of_images.Clear();
            }
        }

        public static void AddNewImagePath(IWebHostEnvironment _environment)
        {
            Paths_of_images_short.Add(System.IO.Path.Combine("images/work_images", System.IO.Path.GetRandomFileName() + ".png"));
            Paths_of_images.Add(System.IO.Path.Combine(_environment.WebRootPath, Paths_of_images_short.LastOrDefault()));
        }
    }
}
