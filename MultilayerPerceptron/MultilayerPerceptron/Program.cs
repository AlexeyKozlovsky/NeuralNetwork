using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace MultilayerPerceptron
{
    class Program
    {
        static void Main(string[] args)
        {
            //LearnForConjunction(0.1, 20000);
            //LearnForDisjunction(0.1, 20000);
            Console.WriteLine("Конъюнкция:");
            Console.WriteLine("0 * 0 = " + Conjunction(0, 0).ToString());
            Console.WriteLine("0 * 1 = " + Conjunction(0, 1).ToString());
            Console.WriteLine("1 * 0 = " + Conjunction(1, 0).ToString());
            Console.WriteLine("1 * 1 = " + Conjunction(1, 1).ToString());
            Console.WriteLine("Дизъюнкция:");
            Console.WriteLine("0 + 0 = " + Disjunction(0, 0).ToString());
            Console.WriteLine("0 + 1 = " + Disjunction(0, 1).ToString());
            Console.WriteLine("1 + 0 = " + Disjunction(1, 0).ToString());
            Console.WriteLine("1 + 1 = " + Disjunction(1, 1).ToString());
            Console.WriteLine("XOR");
            Console.WriteLine("0 XOR 0 = " + XOR(0, 0).ToString());
            Console.WriteLine("0 XOR 1 = " + XOR(0, 1).ToString());
            Console.WriteLine("1 XOR 0 = " + XOR(1, 0).ToString());
            Console.WriteLine("1 XOR 1 = " + XOR(1, 1).ToString());
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("Всё работает!!!");

            Console.Read();
        }

        // Обучение сети конъюнкции и сохранение в файл
        private static void LearnForConjunction(double learningRate, int epoches)
        {
            Net net = new Net(2, 1);
            List<List<double>> inputs = new List<List<double>>()
            {
                new List<double>(){0, 0},
                new List<double>(){0, 1},
                new List<double>(){1, 0},
                new List<double>(){1, 1},
            };

            List<List<double>> outputs = new List<List<double>>()
            {
                new List<double>(){0},
                new List<double>(){0},
                new List<double>(){0},
                new List<double>(){1},
            };

            Learning.LearnAndSave(net, inputs, outputs, learningRate, epoches, "conjunction.dat");
        }

        // Обучение сети дизъюнкции и сохранение в файл
        private static void LearnForDisjunction(double learningRate, int epoches)
        {
            Net net = new Net(2, 1);
            List<List<double>> inputs = new List<List<double>>()
            {
                new List<double>(){0, 0},
                new List<double>(){0, 1},
                new List<double>(){1, 0},
                new List<double>(){1, 1},
            };

            List<List<double>> outputs = new List<List<double>>()
            {
                new List<double>(){0},
                new List<double>(){1},
                new List<double>(){1},
                new List<double>(){1},
            };

            Learning.LearnAndSave(net, inputs, outputs, learningRate, epoches, "disjunction.dat");
        }

        // Конъюнкция
        private static double Conjunction(double x1, double x2)
        {
            Net net;
            BinaryFormatter formatter = new BinaryFormatter();
            using (FileStream fs = new FileStream("conjunction.dat", FileMode.OpenOrCreate))
            {
                net = (Net)formatter.Deserialize(fs);
                fs.Close();
            }

            return double.Parse(net.Return(new List<double>() { x1, x2 }));
        }

        // Дизъюнкция
        private static double Disjunction(double x1, double x2)
        {
            Net net;
            BinaryFormatter formatter = new BinaryFormatter();
            using(FileStream fs = new FileStream("disjunction.dat", FileMode.OpenOrCreate))
            {
                net = (Net)formatter.Deserialize(fs);
                fs.Close();
            }
            
            return double.Parse(net.Return(new List<double>() { x1, x2 }));
        }

        // Иключающее или (xor)
        private static double XOR(double x1, double x2)
        {
            Net net = new Net(2, 2);
            Net net2 = new Net(2, 1);

            List<List<double>> inputs = new List<List<double>>() {
                new List<double>() { 0, 0 },
                new List<double>() { 0, 1 },
                new List<double>() { 1, 0 },
                new List<double>() { 1, 1 }};

            List<List<double>> outputs = new List<List<double>>() {
                new List<double>() {0, 0},
                new List<double>() {0, 1},
                new List<double>() {1, 0},
                new List<double>() {0, 0}};

            List<List<double>> inputs2 = new List<List<double>>() {
                new List<double>() { 0, 0},
                new List<double>() { 0, 1},
                new List<double>() { 1, 0},
                new List<double>() {1, 1} };

            List<List<double>> outputs2 = new List<List<double>>() {
                new List<double>(){0},
                new List<double>(){1},
                new List<double>(){1},
                new List<double>(){1}};

            net2.Learn(inputs2, outputs2, 0.1, 20000);
            net.Learn(inputs, outputs, 0.1, 20000);

            return double.Parse(net2.Return(net.ReturnList(new List<double>() { x1, x2 })));
        }

        
    }
}
