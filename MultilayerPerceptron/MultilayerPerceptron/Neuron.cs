using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MultilayerPerceptron
{
    // Перечисление возможных типов нейронов
    public enum NeuronType
    {
        Input,                          // Нейрон находится во входном слое
        Hidden,                         // Нейрон находится в скрытом слое
        Bias,                           // Нейрон смещения (не имеет входящих синапсов)
        Output                          // Нейрон находится на выходном слое
    }

    // Класс, описывающий нейрон
    [Serializable]
    class Neuron
    {
        // Входное значение нейрона
        private double input;
        public double Input { get { return input; }
            set 
            {
                input = value;
                output = Sigmoid(input);
            }
        }

        // Выходное значение нейрона
        private double output;
        public double Output { get { return output; } }

        // Тип нейрона
        private NeuronType type;
        public NeuronType Type { get { return type; } }

        // Дельта нейрона
        private double delta;
        public double Delta { get { return delta; } set { delta = value; } }

        // Список весов входящих в нейрон синапсов
        // Если входящих синапсов нет, то список пустой
        private List<double> weights;
        public List<double> Weights { get { return weights; } }


        // Конструктор
        // Принимает как аргументы входное значение нейрона и тип нейрона
        public Neuron(double input, NeuronType type)
        {
            this.input = input;
            this.type = type;
            this.output = Sigmoid(input);
            this.weights = new List<double>();
        }

        // Метод, устанавливающий список весов.
        // Принимает как аргументы количество синапсов, входящих в нейрон
        // Устанавливает веса рандомно в промежутке от -0.5 до 0.5
        public void SetWeights(int inputsCount)
        {
            // Если нейрон является нейроном смещения или нейроном входного слоя,
            // он не имеет входных синапсов
            if (this.type == NeuronType.Bias || this.type == NeuronType.Input) return;

            this.weights = new List<double>();
            for (int i = 0; i < inputsCount; i++)
            {
                Random rnd = new Random();
                weights.Add(rnd.NextDouble() - 0.5);
                Thread.Sleep(10);
            }
        }


        // Активационная функция нейрона. Сигмоида
        private double Sigmoid(double x)
        {
            return 1 / (1 + Math.Exp(-x));
        }

        // Производная сигмоиды
        public double SigmoidDerivative(double x)
        {
            return Sigmoid(x) * (1 - Sigmoid(x));
        }


        // Обновляет нейрон, в соответствии со списком весов входящих синапсов
        // Принимает как аргумент выходные значения нейронов на предыдущем слое
        public void FeedForward(List<double> inputs)
        {
            double result = 0;
            for (int i = 0; i < this.weights.Count; i++)
                result += inputs[i] * this.weights[i];

            this.input = result;
            this.output = this.Sigmoid(result);
        }

        // Меняет веса входящий в нейрон синапсов
        // Метод вызывается в процессе обучения методом обратного распространения ошибки
        // Как аргументы принимает предыдущий слой, скорость обучения
        public void ChangeWeights(Layer prevLayer, double learningRate)
        {
            for (int i = 0; i < weights.Count; i++)
                this.weights[i] += prevLayer[i].output * this.delta * learningRate;
        }




    }
}
