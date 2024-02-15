using Kolorowanie.Pages;
using System.Drawing;
using Color = System.Drawing.Color;


namespace Kolorowanie.Services
{
    public class Converts
    {
        public static Color ConvertLabToRgb(float l, float a, float b)
        {
            float[] xyz = new float[3];
            float[] col = new float[] { l, a, b };

            xyz[1] = (col[0] + 16.0f) / 116.0f;
            xyz[0] = (col[1] / 500.0f) + xyz[1];
            xyz[2] = xyz[1] - (col[2] / 200.0f);

            for (int i = 0; i < 3; i++)
            {
                float pow = xyz[i] * xyz[i] * xyz[i];
                float ratio = (6.0f / 29.0f);
                if (xyz[i] > ratio)
                {
                    xyz[i] = pow;
                }
                else
                {
                    xyz[i] = (3.0f * (6.0f / 29.0f) * (6.0f / 29.0f) * (xyz[i] - (4.0f / 29.0f)));
                }
            }
            xyz[0] = xyz[0] * 95.047f;
            xyz[1] = xyz[1] * 100.0f;
            xyz[2] = xyz[2] * 108.883f;

            return ConvertXyzToRgb(xyz);
        }

        public static Color ConvertXyzToRgb(float[] color)
        {
            float[] rgb = new float[3];
            float[] xyz = new float[3];
            float[] col = new float[] { color[0], color[1], color[2] };



            for (int i = 0; i < 3; i++)
            {
                xyz[i] = col[i] / 100.0f;
            }

            rgb[0] = (xyz[0] * 3.240479f) + (xyz[1] * -1.537150f) + (xyz[2] * -.498535f);
            rgb[1] = (xyz[0] * -.969256f) + (xyz[1] * 1.875992f) + (xyz[2] * .041556f);
            rgb[2] = (xyz[0] * .055648f) + (xyz[1] * -.204043f) + (xyz[2] * 1.057311f);

            for (int i = 0; i < 3; i++)
            {
                if (rgb[i] > .0031308f)
                {
                    rgb[i] = (1.055f * (float)Math.Pow(rgb[i], (1.0f / 2.4f))) - .055f;
                }
                else
                {
                    rgb[i] = rgb[i] * 12.92f;
                }
            }

            rgb[0] = rgb[0] * 255.0f;
            rgb[1] = rgb[1] * 255.0f;
            rgb[2] = rgb[2] * 255.0f;

            return Color.FromArgb(Math.Max(0, Math.Min(255, (int)rgb[0])), Math.Max(0, Math.Min(255, (int)rgb[1])), Math.Max(0, Math.Min(255, (int)rgb[2])));
        }

        public static Bitmap ConvertLabToBitmap(float[] lab_table, int bitmap_width, int bitmap_height, bool output_gray = false)
        {
            Bitmap bitmap = new Bitmap(bitmap_width, bitmap_height);
            int index = 0;
            float l = 0, a = 0, b = 0;

            for (int y = 0; y < bitmap_height; y++)
                for (int x = 0; x < bitmap_width; x++)
                {
                    l = lab_table[index];  // L value from LAB
                    index++;

                    if (!output_gray)
                    {
                        a = lab_table[index];  // A value from LAB
                        index++;
                        b = lab_table[index];  // B value from LAB
                        index++;
                    }

                    Color pixel_color = Converts.ConvertLabToRgb(l, a, b);
                    bitmap.SetPixel(x, y, pixel_color);
                }
            return bitmap;
        }

        public static Bitmap ConvertSquareLabToBitmap(float[] lab_table, int bitmap_width, int bitmap_height, int square_number_height,
            int square_number_width, int square_width, bool output_gray = false)
        {
            Bitmap bitmap = new Bitmap(bitmap_width, bitmap_height);
            int index = 0;
            float l = 0, a = 0, b = 0;


            for (int y = 0; y < square_number_height; y++)
            {
                for (int x = 0; x < square_number_width; x++)
                {

                    for (int y1 = 0; y1 < square_width; y1++)
                    {
                        for (int x1 = 0; x1 < square_width; x1++)
                        {
                            l = lab_table[index];  // L value from LAB
                            index++;

                            if (!output_gray)
                            {
                                a = lab_table[index];  // A value from LAB
                                index++;
                                b = lab_table[index];  // B value from LAB
                                index++;
                            }

                            Color pixel_color = Converts.ConvertLabToRgb(l, a, b);
                            bitmap.SetPixel(x * square_width + x1, y * square_width + y1, pixel_color);
                        }
                    }
                }
            }

            return bitmap;
        }

        public static float[] ConvertRgb_ToLab(int r, int g, int b)
        {
            float[] lab = new float[3];

            float[] xyz = new float[3];
            float[] rgb = new float[] { (float)(r / 255.0), (float)(g / 255.0), (float)(b / 255.0) };

            for (int i = 0; i < 3; i++)
            {
                if (rgb[i] > 0.04045)
                    rgb[i] = (float)Math.Pow((rgb[i] + 0.055) / 1.055, 2.4);
                else
                    rgb[i] /= (float)12.92;

                rgb[i] *= 100;
            }

            xyz[0] = (float)(rgb[0] * 0.4124564 + rgb[1] * 0.3575761 + rgb[2] * 0.1804375);
            xyz[1] = (float)(rgb[0] * 0.2126729 + rgb[1] * 0.7151522 + rgb[2] * 0.0721750);
            xyz[2] = (float)(rgb[0] * 0.0193339 + rgb[1] * 0.1191920 + rgb[2] * 0.9503041);

            xyz[0] /= (float)95.047;
            xyz[1] /= (float)100.000;
            xyz[2] /= (float)108.883;

            for (int i = 0; i < 3; i++)
            {
                if (xyz[i] > 0.008856)
                    xyz[i] = (float)Math.Pow(xyz[i], 1.0 / 3.0);
                else
                    xyz[i] = (float)((xyz[i] * 903.3 + 16.0) / 116.0);
            }

            lab[0] = (float)Math.Max(0.0, 116.0 * xyz[1] - 16.0);
            lab[1] = (float)(500.0 * (xyz[0] - xyz[1]));
            lab[2] = (float)(200.0 * (xyz[1] - xyz[2]));

            return lab;
        }
    }
}
