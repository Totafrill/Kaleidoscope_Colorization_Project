using Kolorowanie.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Kolorowanie.Pages
{
    public class DoubleFusionModel : PageModel
    {
        public readonly IWebHostEnvironment _environment;

        public List<SelectListItem> ListModelsFusion { get; private set; }

        public string ResultPath1 { get; private set; }

        public string ResultPath2 { get; private set; }
        public string ErrorMessage { get; private set; }

        public string InputPath { get; private set; }

        [BindProperty]
        [Required(ErrorMessage = "Please select an image.")]
        [MaxFileSize(10485760, ErrorMessage = "Maximum allowed file size is 10 MB.")]
        [AllowedExtensions(new string[] { ".jpg", ".jpeg", ".png" }, ErrorMessage = "Only JPG, JPEG and PNG files are allowed.")]
        public IFormFile ImageFile1 { get; set; }

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

        public DoubleFusionModel(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        [HttpPost]
        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                SetFileList();
                return Page();
            }

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
                Model_Run_Preparations.RunModelAndSaveNewResult(SelectedModel1, Paths.Paths_of_images.First(), Paths.Paths_of_images.Last(), true); // fusion 1
                Paths.AddNewImagePath(_environment);
                Model_Run_Preparations.RunModelAndSaveNewResult(SelectedModel2, Paths.Paths_of_images.First(), Paths.Paths_of_images.Last(), true); // fusion 2

                InputPath = Paths.Paths_of_images_short.FirstOrDefault();
                ResultPath1 = Paths.Paths_of_images_short[1];
                ResultPath2 = Paths.Paths_of_images_short[2];
            }
            catch (Exception ex)
            {
                ErrorMessage = "Error during running model" + ex.Message;
            }

            SetFileList();
            Paths.ClearImagesPaths();

            return Page();
        }

        public void SetFileList()
        {
            var files = Directory.GetFiles(Paths.Models_path_fusion);

            ListModelsFusion = new List<SelectListItem>();
            foreach (var file in files)
            {
                ListModelsFusion.Add(new SelectListItem(System.IO.Path.GetFileNameWithoutExtension(file), file));
            }

            ViewData["ListModelsFusion"] = ListModelsFusion;

        }
    }
}
