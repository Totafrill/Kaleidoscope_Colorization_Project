using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing;
using System.ComponentModel.DataAnnotations;

namespace Kolorowanie.Services
{
    public class Image_actions
    {
        public static Bitmap ImageResize(System.Drawing.Image image, int width, int height)
        {
            var destRect = new System.Drawing.Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.Default;
                graphics.InterpolationMode = InterpolationMode.Default;
                graphics.SmoothingMode = SmoothingMode.Default;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }

        public static void SaveImageFromLabTable(float[] lab_table, String path, int bitmap_width, int bitmap_height,
             int square_number_height, int square_number_width, int square_width, bool output_gray = false)
        {
            using (Bitmap bitmap = Converts.ConvertSquareLabToBitmap(lab_table, bitmap_width, bitmap_height, square_number_height,
                square_number_width, square_width, output_gray))
                bitmap.Save(path);
        }

        public static Bitmap CropImage(Bitmap original, System.Drawing.Rectangle cropArea)
        {
            Bitmap cropped = new Bitmap(cropArea.Width, cropArea.Height);

            using (Graphics g = Graphics.FromImage(cropped))
            {
                g.DrawImage(original, new System.Drawing.Rectangle(0, 0, cropped.Width, cropped.Height), cropArea, GraphicsUnit.Pixel);
            }

            return cropped;
        }

        public static System.Drawing.Image LoadImageFromFile(string path)
        {
            var bytes = File.ReadAllBytes(path);
            var ms = new MemoryStream(bytes);
            var img = System.Drawing.Image.FromStream(ms);
            return img;
        }
    }
}
