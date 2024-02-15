using Kolorowanie.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.ML;
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using Color = System.Drawing.Color;

namespace Kolorowanie.Pages
{
    public class DoublePlainModel : PageModel
    {
        public readonly IWebHostEnvironment _environment;

        public List<SelectListItem> ListModelsPlain { get; private set; }

        public string ResultPath1 { get; private set; }

        public string ResultPath2 { get; private set; }
        public string ErrorMessage { get; private set; }

        public string InputPath { get; private set; }

        [BindProperty]
        public IFormFile ImageFile { get; set; }

        [BindProperty]
        public string SelectedModel1 { get; set; }

        [BindProperty]
        public string SelectedModel2 { get; set; }


        public void OnGet()
        {
            Paths.SetPaths(_environment);

            SetFileList();

            Files.RemoveOldFiles(Paths.Uploads_folder, TimeSpan.FromMinutes(5));
        }

        public DoublePlainModel(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        [HttpPost]
        public IActionResult OnPost()
        {
            OnGet();

            if (SelectedModel1 == null || SelectedModel1.Length == 0 || SelectedModel2 == null || SelectedModel2.Length == 0)
            {
                ErrorMessage = "The model has not been selected";
                return Page();
            }

            if (ImageFile == null || ImageFile.Length == 0)
            {
                ErrorMessage = "No image file selected or the image does not have the appropriate extension (\\\".jpg\\\", \\\".jpeg\\\", \\\".png\\\", \\.gif\\\")";
                return Page();
            }

            try
            {
                Paths.AddNewImagePath(_environment);

                using (var fileStream = new FileStream(Paths.Paths_of_images.FirstOrDefault(), FileMode.Create))
                    ImageFile.CopyTo(fileStream);

                Paths.AddNewImagePath(_environment);
                Model_Run_Preparations.RunModelAndSaveNewResult(SelectedModel1, Paths.Paths_of_images.First(), Paths.Paths_of_images.Last(), false); // plain 1
                Paths.AddNewImagePath(_environment);
                Model_Run_Preparations.RunModelAndSaveNewResult(SelectedModel2, Paths.Paths_of_images.First(), Paths.Paths_of_images.Last(), false); // plain 2

                InputPath = Paths.Paths_of_images_short.FirstOrDefault();
                ResultPath1 = Paths.Paths_of_images_short[1];
                ResultPath2 = Paths.Paths_of_images_short[2];
            }
            catch (Exception ex)
            {
                ErrorMessage = "Error during running model" + ex.Message;
            }

            return Page();
        }

        public void SetFileList()
        {
            // Pobierz list� plik�w z folderu

            var files = Directory.GetFiles(Paths.Models_path_plain);

            // Zainicjuj list� opcji
            ListModelsPlain = new List<SelectListItem>();
            foreach (var file in files)
            {
                ListModelsPlain.Add(new SelectListItem(System.IO.Path.GetFileNameWithoutExtension(file), file));
            }

            ViewData["ListModelsPlain"] = ListModelsPlain;

        }
    }
}
