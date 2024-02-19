using Kolorowanie.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.ML;
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using Microsoft.Scripting.Utils;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using Color = System.Drawing.Color;

namespace Kolorowanie.Pages
{
    public class PlainModel : PageModel
    {
        public readonly IWebHostEnvironment _environment;

        public List<SelectListItem> ListModelsPlain { get; private set; }

        public string ResultPath1 { get; private set; }

        public string ErrorMessage { get; private set; }

        public string InputPath { get; private set; }

        [BindProperty]
        [Required(ErrorMessage = "Please select an image.")]
        [MaxFileSize(10485760, ErrorMessage = "Maximum allowed file size is 10 MB.")]
        [AllowedExtensions(new string[] { ".jpg", ".jpeg", ".png" }, ErrorMessage = "Only JPG, JPEG and PNG files are allowed.")]
        public IFormFile ImageFile1 { get; set; }

        [BindProperty]
        public string SelectedModel1 { get; set; }



        public void OnGet()
        {
            SetFileList();

            Files.RemoveOldFiles(Paths.Uploads_folder, TimeSpan.FromMinutes(5));
        }

        public PlainModel(IWebHostEnvironment environment)
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


            if (SelectedModel1 == null || SelectedModel1.Length == 0)
            {
                ErrorMessage = "The model has not been selected";
                SetFileList();

                return Page();
            }

            if (ImageFile1 == null || ImageFile1.Length == 0)
            {
                var v = ImageFile1.ContentType;
                ErrorMessage = "No image file";
                SetFileList();

                return Page();
            }

            try
            {
                Paths.AddNewImagePath(_environment);

                using (var fileStream = new FileStream(Paths.Paths_of_images.FirstOrDefault(), FileMode.Create))
                    ImageFile1.CopyTo(fileStream);

                Paths.AddNewImagePath(_environment);
                Model_Run_Preparations.RunModelAndSaveNewResult(SelectedModel1, Paths.Paths_of_images.First(), Paths.Paths_of_images.Last(), false);

                InputPath = Paths.Paths_of_images_short.FirstOrDefault();
                ResultPath1 = Paths.Paths_of_images_short[1];
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
            var files = Directory.GetFiles(Paths.Models_path_plain);

            ListModelsPlain = new List<SelectListItem>();
            foreach (var file in files)
            {
                ListModelsPlain.Add(new SelectListItem(System.IO.Path.GetFileNameWithoutExtension(file), file));
            }

            ViewData["ListModelsPlain"] = ListModelsPlain;

        }
    }

    public class MaxFileSizeAttribute : ValidationAttribute
    {
        private readonly int _maxFileSize;
        public MaxFileSizeAttribute(int maxFileSize)
        {
            _maxFileSize = maxFileSize;
        }

        protected override ValidationResult IsValid(
        object value, ValidationContext validationContext)
        {
            var file = value as IFormFile;
            if (file != null)
            {
                if (file.Length > _maxFileSize)
                {
                    return new ValidationResult(GetErrorMessage());
                }
            }

            return ValidationResult.Success;
        }

        public string GetErrorMessage()
        {
            return $"Maximum allowed file size is {_maxFileSize} bytes.";
        }
    }

    public class AllowedExtensionsAttribute : ValidationAttribute
    {
        private readonly string[] _extensions;
        public AllowedExtensionsAttribute(string[] extensions)
        {
            _extensions = extensions;
        }

        protected override ValidationResult IsValid(
        object value, ValidationContext validationContext)
        {
            var file = value as IFormFile;
            if (file != null)
            {
                var extension = Path.GetExtension(file.FileName);
                if (!_extensions.Contains(extension.ToLower()))
                {
                    return new ValidationResult(GetErrorMessage());
                }
            }

            return ValidationResult.Success;
        }

        public string GetErrorMessage()
        {
            return $"This photo extension is not allowed!";
        }
    }
}
