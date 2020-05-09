using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace MultilayerPerceptron
{
    class ImageConverter
    {
        public static List<double> Convert(string path)
        {
            Bitmap image = new Bitmap(path);

            List<double> result = new List<double>();
            for (int i = 0; i < image.Width; i++)
            {
                for (int j = 0; j < image.Height; j++)
                {
                    Color pixel = image.GetPixel(i, j);
                    result.Add(ReturnDoubleFromPixel(pixel));
                }
            }

            return result;
        }

        private static double ReturnDoubleFromPixel(Color pixel)
        {
            return 0.288 * pixel.R + 0.587 * pixel.G + 0.114 * pixel.B;
        }

        public static void ConvertFolder(string fromFolderPath, string toFolderPath)
        {
            List<string> filenames = Directory.GetFiles(fromFolderPath).ToList<string>();
            if (!Directory.Exists(toFolderPath)) Directory.CreateDirectory(toFolderPath);
            BinaryFormatter formatter = new BinaryFormatter();

            for (int i = 0; i < filenames.Count; i++)
            {
                using(FileStream fs = new FileStream(toFolderPath + filenames[i], FileMode.OpenOrCreate))
                {
                    List<double> result = Convert(fromFolderPath + filenames[i]);
                    formatter.Serialize(fs, result);
                }
            }
        }

    }
}
