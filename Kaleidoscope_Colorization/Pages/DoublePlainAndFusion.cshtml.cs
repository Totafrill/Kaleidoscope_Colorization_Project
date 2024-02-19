using Kolorowanie.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Kolorowanie.Pages
{
    public class DoublePlainAndFusionModel : PageModel
    {
        public readonly IWebHostEnvironment _environment;

        public List<SelectListItem> ListModelsPlain { get; private set; }
        public List<SelectListItem> ListModelsFusion { get; private set; }

        public string InputPath { get; private set; }
        public string ResultPath1 { get; private set; }
        public string ResultPath2 { get; private set; }
        public string ResultPath3 { get; private set; }
        public string ResultPath4 { get; private set; }

        public string ErrorMessage { get; private set; }
        

        [BindProperty]
        [Required(ErrorMessage = "Please select an image.")]
        [MaxFileSize(10485760, ErrorMessage = "Maximum allowed file size is 10 MB.")]
        [AllowedExtensions(new string[] { ".jpg", ".jpeg", ".png" }, ErrorMessage = "Only JPG, JPEG and PNG files are allowed.")]
        public IFormFile ImageFile1 { get; set; }

        [BindProperty]
        public string SelectedModel1 { get; set; }

        [BindProperty]
        public string SelectedModel2 { get; set; }

        [BindProperty]
        public string SelectedModel3 { get; set; }

        [BindProperty]
        public string SelectedModel4 { get; set; }

        public void OnGet()
        {


            Paths.SetPaths(_environment);

            SetFileList();

            Files.RemoveOldFiles(Paths.Uploads_folder, TimeSpan.FromMinutes(5));

        }

        public DoublePlainAndFusionModel(IWebHostEnvironment environment)
        {
            _environment = environment;
        }
        public IActionResult OnPost()
        {
            OnGet();

            if (SelectedModel1 == null || SelectedModel1.Length == 0 || SelectedModel2 == null || SelectedModel2.Length == 0)
            {
                ErrorMessage = "The model has not been selected";
                return Page();
            }

            if (ImageFile1 == null || ImageFile1.Length == 0)
            {
                ErrorMessage = "No image file selected or the image does not have the appropriate extension (\\\".jpg\\\", \\\".jpeg\\\", \\\".png\\\", \\.gif\\\")";
                return Page();
            }

            try
            {
                Paths.AddNewImagePath(_environment);

                using (var fileStream = new FileStream(Paths.Paths_of_images.FirstOrDefault(), FileMode.Create))
                    ImageFile1.CopyTo(fileStream);

                Paths.AddNewImagePath(_environment);
                Model_Run_Preparations.RunModelAndSaveNewResult(SelectedModel1, Paths.Paths_of_images.First(), Paths.Paths_of_images.Last(), false); // plain 1
                Paths.AddNewImagePath(_environment);
                Model_Run_Preparations.RunModelAndSaveNewResult(SelectedModel2, Paths.Paths_of_images.First(), Paths.Paths_of_images.Last(), true); // fusion 1
                Paths.AddNewImagePath(_environment);
                Model_Run_Preparations.RunModelAndSaveNewResult(SelectedModel3, Paths.Paths_of_images.First(), Paths.Paths_of_images.Last(), false); // plain 2
                Paths.AddNewImagePath(_environment);
                Model_Run_Preparations.RunModelAndSaveNewResult(SelectedModel4, Paths.Paths_of_images.First(), Paths.Paths_of_images.Last(), true); // fusion 2

                InputPath = Paths.Paths_of_images_short.FirstOrDefault();
                ResultPath1 = Paths.Paths_of_images_short[1];
                ResultPath2 = Paths.Paths_of_images_short[2];
                ResultPath3 = Paths.Paths_of_images_short[3];
                ResultPath4 = Paths.Paths_of_images_short[4];
            }
            catch (Exception ex)
            {
                ErrorMessage = "Error during running model" + ex.Message;
            }

            return Page();
        }

        public void SetFileList()
        {
            var files = Directory.GetFiles(Paths.Models_path_plain);

            ListModelsPlain = new List<SelectListItem>();
            foreach (var file in files)
            {
                ListModelsPlain.Add(new SelectListItem(System.IO.Path.GetFileNameWithoutExtension(file), file));
            }

            files = Directory.GetFiles(Paths.Models_path_fusion);

            ListModelsFusion = new List<SelectListItem>();
            foreach (var file in files)
            {
                ListModelsFusion.Add(new SelectListItem(System.IO.Path.GetFileNameWithoutExtension(file), file));
            }

            ViewData["ListModelsPlain"] = ListModelsPlain;
            ViewData["ListModelsFusion"] = ListModelsFusion;
        }
    }
}
