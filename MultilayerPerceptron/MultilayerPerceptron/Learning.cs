using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace MultilayerPerceptron
{
    // Статический класс для обучения
    static class Learning
    {
        // Обучение и сохранение нейросети в файл
        public static void LearnAndSave(Net net, List<List<double>> inputs, List<List<double>> outputs,
            double learningRate, int epoches, string filename)
        {
            net.Learn(inputs, outputs, learningRate, epoches);

            BinaryFormatter formatter = new BinaryFormatter();

            using(FileStream fs = new FileStream(filename, FileMode.OpenOrCreate))
            {
                formatter.Serialize(fs, net);
                fs.Close();
            }

            Console.WriteLine("Обучение успешно прошло");
        }
    }
}
