using Kolorowanie.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Kolorowanie.Pages
{
    public class PlainAndFusionMultiModel : PageModel
    {
        public readonly IWebHostEnvironment _environment;

        public List<SelectListItem> ListModelsPlain { get; private set; }
        public List<SelectListItem> ListModelsFusion { get; private set; }

        public string[] InputsPaths { get; private set; } = new string[3];
        public string[] ResultsPaths { get; private set; } = new string[6];

        public string ErrorMessage { get; private set; }

        [BindProperty]
        [Required(ErrorMessage = "Please select an image.")]
        [MaxFileSize(10485760, ErrorMessage = "Maximum allowed file size is 10 MB.")]
        [AllowedExtensions(new string[] { ".jpg", ".jpeg", ".png" }, ErrorMessage = "Only JPG, JPEG and PNG files are allowed.")]
        public IFormFile ImageFile1 { get; set; }

        [BindProperty]
        [MaxFileSize(10485760, ErrorMessage = "Maximum allowed file size is 10 MB.")]
        [AllowedExtensions(new string[] { ".jpg", ".jpeg", ".png" }, ErrorMessage = "Only JPG, JPEG and PNG files are allowed.")]

        public IFormFile ImageFile2 { get; set; }

        [BindProperty]
        [MaxFileSize(10485760, ErrorMessage = "Maximum allowed file size is 10 MB.")]
        [AllowedExtensions(new string[] { ".jpg", ".jpeg", ".png" }, ErrorMessage = "Only JPG, JPEG and PNG files are allowed.")]
        public IFormFile ImageFile3 { get; set; }

        [BindProperty]
        [MaxFileSize(10485760, ErrorMessage = "Maximum allowed file size is 10 MB.")]
        [AllowedExtensions(new string[] { ".jpg", ".jpeg", ".png" }, ErrorMessage = "Only JPG, JPEG and PNG files are allowed.")]
        public string SelectedModel1 { get; set; }

        [BindProperty]
        [MaxFileSize(10485760, ErrorMessage = "Maximum allowed file size is 10 MB.")]
        [AllowedExtensions(new string[] { ".jpg", ".jpeg", ".png" }, ErrorMessage = "Only JPG, JPEG and PNG files are allowed.")]
        public string SelectedModel2 { get; set; }


        public void OnGet()
        {
            Paths.SetPaths(_environment);

            SetFileList();

            Files.RemoveOldFiles(Paths.Uploads_folder, TimeSpan.FromMinutes(5));
        }

        public PlainAndFusionMultiModel(IWebHostEnvironment environment)
        {
            _environment = environment;
        }
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

            List<IFormFile> ImagesFiles = new List<IFormFile>
            {
                ImageFile1,
                ImageFile2,
                ImageFile3,
            };

            if (ImagesFiles.Count == 0)
            {
                ErrorMessage = "No image file selected or the image does not have the appropriate extension (\\\".jpg\\\", \\\".jpeg\\\", \\\".png\\\", \\.gif\\\")";
                return Page();
            }

            int index = 0;

            foreach (IFormFile file in ImagesFiles)
            {
                if (file != null)
                {
                    try
                    {
                        Paths.AddNewImagePath(_environment);

                        using (var fileStream = new FileStream(Paths.Paths_of_images[index], FileMode.Create))
                            file.CopyTo(fileStream);

                        Paths.AddNewImagePath(_environment);
                        Model_Run_Preparations.RunModelAndSaveNewResult(SelectedModel1, Paths.Paths_of_images[index], Paths.Paths_of_images.Last(), false); // plain 1
                        Paths.AddNewImagePath(_environment);
                        Model_Run_Preparations.RunModelAndSaveNewResult(SelectedModel2, Paths.Paths_of_images[index], Paths.Paths_of_images.Last(), true); // fusion 1

                        index = index + 3;
                    }

                    catch (Exception ex)
                    {
                        ErrorMessage = "Error during running model" + ex.Message;
                    }
                }
            }

            for (int i = 0; i < Paths.Paths_of_images_short.Count / 3; i++)
            {
                InputsPaths[i] = Paths.Paths_of_images_short[i * 3];
                ResultsPaths[i*2] = Paths.Paths_of_images_short[i * 3 + 1];
                ResultsPaths[i*2+1] = Paths.Paths_of_images_short[i * 3 + 2];
            }

            SetFileList();
            Paths.ClearImagesPaths();

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
