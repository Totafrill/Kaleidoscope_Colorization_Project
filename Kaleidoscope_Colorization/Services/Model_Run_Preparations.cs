using Kolorowanie.Pages;
using Microsoft.ML.OnnxRuntime.Tensors;
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML;
using System.Drawing;
using Color = System.Drawing.Color;
using DocumentFormat.OpenXml.Office2021.Excel.RichDataWebImage;


namespace Kolorowanie.Services
{
    public class Model_Run_Preparations
    {
        private static float[] Table_grey { get; set; }
        private static int Original_width { get; set; }
        private static int Original_height { get; set; }
        private static int Squares_number_width { get; set; }
        private static int Squares_number_height { get; set; }
        private static int Squares_number { get; set; }
        private static int Square_width { get; set; } = 256;

        private static float[] CreateInceptionEmbedding(float[] grayscaledRgb)
        {
            var context = new MLContext();

            // Wczytaj model ONNX
            var session = new InferenceSession(Paths.Model_path_inception);


            var inputTensor = new DenseTensor<float>(grayscaledRgb, new int[] { 1, 299, 299, 3 });

            var input = new List<NamedOnnxValue> { NamedOnnxValue.CreateFromTensor<float>("input_1", inputTensor) };


            var output = session.Run(input);
            var outputTensor = output.First().AsTensor<float>();

            float[] wynik1 = new float[1 * 1000];


            for (int i = 0; i < 1000; i++)
            {
                wynik1[i] = outputTensor.GetValue(i);
            }

            session.Dispose();

            return wynik1;

        }

        private static float[] GreyInception(Bitmap bitmap)
        {
            Bitmap bitmap1 = Image_actions.ImageResize(bitmap, 299, 299);

            float[] grey_table = new float[299 * 299 * 3];
            int index = 0;

            for (int x = 0; x < bitmap1.Width; x++)
            {
                for (int y = 0; y < bitmap1.Height; y++)
                {
                    Color pixelColor1 = bitmap1.GetPixel(y, x);
                    grey_table[index++] = (float)pixelColor1.R / 255;
                    grey_table[index++] = (float)pixelColor1.G / 255;
                    grey_table[index++] = (float)pixelColor1.B / 255;
                }
            }

            return grey_table;
        }

        private static float[] LoadImageWithPreparations(string path)
        {
            float[] colorMe1;
            using (Bitmap original_bitmap = new Bitmap(path))
            {
                Original_width = original_bitmap.Width;
                Original_height = original_bitmap.Height;

                Squares_number_width = (int)Math.Ceiling((double)original_bitmap.Width / Square_width);
                Squares_number_height = (int)Math.Ceiling((double)original_bitmap.Height / Square_width);
                Squares_number = Squares_number_width * Squares_number_height;

                colorMe1 = new float[Squares_number * Square_width * Square_width];

                colorMe1 = ConvertImageToLabAndAddPadding(original_bitmap);
            }
            return colorMe1;
        }


        private static float[] ConvertImageToLabAndAddPadding(Bitmap original_bitmap)
        {
            float[] color_me = new float[Squares_number * Square_width * Square_width];

            int index = 0;

            for (int y = 0; y < Squares_number_height; y++)
            {
                for (int x = 0; x < Squares_number_width; x++)
                {
                    for (int y1 = 0; y1 < Square_width; y1++)
                    {
                        for (int x1 = 0; x1 < Square_width; x1++)
                        {
                            if (x1 + 1 + x * Square_width > Original_width || y1 + 1 + y * Square_width > Original_height)
                                color_me[index] = Converts.ConvertRgb_ToLab(0, 0, 0)[0];
                            else
                            {
                                Color pixelColor = original_bitmap.GetPixel(x * Square_width + x1, y * Square_width + y1);
                                color_me[index] = Converts.ConvertRgb_ToLab(pixelColor.R, pixelColor.G, pixelColor.B)[0];
                            }
                            index++;
                        }
                    }
                }
            }
            return color_me;
        }

        public static void RunModelAndSaveNewResult(string selected_model, string photo_path, string result_path, bool is_fusion)
        {
            Bitmap result = RunModel(selected_model, photo_path, is_fusion);

            if (Original_width % Square_width != 0 || Original_height % Square_width != 0)
            {
                using (Bitmap bitmap = (Bitmap)Image_actions.LoadImageFromFile(photo_path))
                {
                    Bitmap _bitmap = Image_actions.CropImage(bitmap, new System.Drawing.Rectangle(0, 0, Original_width, Original_height));
                    _bitmap.Save(photo_path);
                }

                result = Image_actions.CropImage(result, new System.Drawing.Rectangle(0, 0, Original_width, Original_height));
            }

            result.Save(result_path);
        }

        private static Bitmap RunModel(string selected_model, string photo_path, bool is_fusion)
        {
            float[] photo_table = LoadImageWithPreparations(photo_path);

            int new_width = Squares_number_width * Square_width;
            int new_height = Squares_number_height * Square_width;

            Image_actions.SaveImageFromLabTable(photo_table, photo_path, new_width, new_height,
                Squares_number_height, Squares_number_width, Square_width, true);

            var session = new InferenceSession(selected_model);
            int index_w = 0, squares_index = 0;

            float[] wynik = new float[Squares_number_width * Squares_number_height * Square_width * Square_width * 3];
            float[] photo_table_square = new float[Square_width * Square_width];

            for (int y = 0; y < Squares_number_height; y++)
                for (int x = 0; x < Squares_number_width; x++)
                {
                    Array.Copy(photo_table, squares_index++ * Square_width * Square_width, photo_table_square, 0, Square_width * Square_width);

                    int[] index = new int[] { 0, 0 };

                    var outputTensor = RunModelSquare(session, photo_table_square, is_fusion);

                    for (int i = 0; i < Square_width * Square_width; i++)
                    {
                        wynik[index_w++] = photo_table_square[index[0]];
                        photo_table_square[index[0]++] = 0;
                        wynik[index_w++] = outputTensor.GetValue(index[1]++) * 128;
                        wynik[index_w++] = outputTensor.GetValue(index[1]++) * 128;
                    }
                }

            session.Dispose();

            return Converts.ConvertSquareLabToBitmap(wynik, new_width, new_height, Squares_number_height, Squares_number_width, Square_width, false);
        }

        private static Tensor<float> RunModelSquare(InferenceSession session, float[] photo_table_square, bool is_fusion)
        {
            var inputTensor1 = new DenseTensor<float>(photo_table_square, new int[] { 1, Square_width, Square_width, 1 });

            var inputs = new NamedOnnxValue[]
            {
                        NamedOnnxValue.CreateFromTensor<float>("input_1", inputTensor1)
            };

            if (is_fusion)
            {
                Bitmap bitmap_temp = Converts.ConvertLabToBitmap(photo_table_square, Square_width, Square_width, true);
                Table_grey = GreyInception(bitmap_temp);
                float[] table_to_fusion = CreateInceptionEmbedding(Table_grey);
                var inputTensor2 = new DenseTensor<float>(table_to_fusion, new int[] { 1, 1000 });

                inputs = new NamedOnnxValue[]
                {
                            NamedOnnxValue.CreateFromTensor<float>("input_3", inputTensor1),
                            NamedOnnxValue.CreateFromTensor<float>("input_2", inputTensor2)
                };
            }

            var output = session.Run(inputs);

            return output.First().AsTensor<float>();
        }


    }
}
