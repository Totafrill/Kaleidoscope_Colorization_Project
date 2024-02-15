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
    public class FusionModel : PageModel
    {
        public readonly IWebHostEnvironment _environment;

        public List<SelectListItem> ListModelsFusion { get; private set; }

        public string ResultPath1 { get; private set; }

        public string ErrorMessage { get; private set; }

        public string InputPath { get; private set; }

        [BindProperty]
        public IFormFile ImageFile { get; set; }

        [BindProperty]
        public string SelectedModel1 { get; set; }



        public void OnGet()
        {


            Paths.SetPaths(_environment);

            SetFileList();

            Files.RemoveOldFiles(Paths.Uploads_folder, TimeSpan.FromMinutes(5));

        }

        public FusionModel(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        [HttpPost]
        public IActionResult OnPost()
        {
            OnGet();

            if (SelectedModel1 == null || SelectedModel1.Length == 0)
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
                Model_Run_Preparations.RunModelAndSaveNewResult(SelectedModel1, Paths.Paths_of_images.First(), Paths.Paths_of_images.Last(), true); // plain 1

                InputPath = Paths.Paths_of_images_short.FirstOrDefault();
                ResultPath1 = Paths.Paths_of_images_short[1];
            }
            catch (Exception ex)
            {
                ErrorMessage = "Error during running model" + ex.Message;
            }

            return Page();
        }

        public void SetFileList()
        {
            // Pobierz listę plików z folderu

            var files = Directory.GetFiles(Paths.Models_path_fusion);

            // Zainicjuj listę opcji
            ListModelsFusion = new List<SelectListItem>();
            foreach (var file in files)
            {
                ListModelsFusion.Add(new SelectListItem(System.IO.Path.GetFileNameWithoutExtension(file), file));
            }

            ViewData["ListModelsFusion"] = ListModelsFusion;

        }
    }
}
