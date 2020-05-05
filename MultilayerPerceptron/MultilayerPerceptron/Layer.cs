using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultilayerPerceptron
{
    // Перечисление типов слоев
    public enum LayerType
    {
        Input,                          // Входной слой
        Output,                         // Выходной слой
        Hidden                          // Скрытый слой
    }

    // Класс, описывающий слой
    [Serializable]
    class Layer
    {
        // Тип слоя
        private LayerType type;
        public LayerType Type { get { return type; } }

        // Количество нейронов в слое
        // Считается без нейронов смещения
        private int length;
        public int Length { get { return length; } }

        // Список нейронов слоя
        private List<Neuron> neurons;

        // Индексатор для более удобного обращения к нейронам слоя
        public Neuron this[int index] { get { return neurons[index]; } }


        // Конструктор
        // Принимает как аргументы количество нейронов в слое и тип
        // Количество нейронов считается БЕЗ нейронов смещения (если такие есть в слое)
        public Layer(int length, LayerType type)
        {
            this.length = length;
            this.type = type;

            InitNeurons();
        }

        // Инициализирует список нейронов с входными значениями 0
        // Если слой не выходной, то генерирует ещё и нейрон смещения (в конце списка) со значением 1
        public void InitNeurons()
        {
            this.neurons = new List<Neuron>();
            for (int i = 0; i < this.length; i++)
            {
                NeuronType type = NeuronType.Hidden;
                switch (this.type)
                {
                    case LayerType.Input:
                        type = NeuronType.Input;
                        break;
                    case LayerType.Output:
                        type = NeuronType.Output;
                        break;
                    case LayerType.Hidden:
                        type = NeuronType.Hidden;
                        break;
                }
                Neuron neuron = new Neuron(0, type);
                this.neurons.Add(neuron);
            }

            // Если слой не является выходным, добавляем туда ещё нейрон смещения
            if (this.type != LayerType.Output)
                this.neurons.Add(new Neuron(1, NeuronType.Bias));
        }

        // Инициализирует веса входнящих в нейроны слоя синапсов
        // Принимает как аргументы количество нейронов не предыдущем слое
        // Количество, в данном случе, считается с нейроном смещения
        public void InitWeights(int inputsCount)
        {
            if (this.type == LayerType.Input) return;

            for (int i = 0; i < this.length; i++)
                this.neurons[i].SetWeights(inputsCount);
        }

        // Метод обновляет текущий слой
        // Принимает как аргументы предыдущий слой
        public void FeedForward(Layer previousLayer)
        {
            if (this.type == LayerType.Input) return;

            List<double> inputs = new List<double>();
            for (int i = 0; i < previousLayer.length + 1; i++)
                inputs.Add(previousLayer[i].Output);

            for (int i = 0; i < this.length; i++)
                this.neurons[i].FeedForward(inputs);
        }

        // Устанавливает входные значения нейронов
        // Принимает как аргуемнты список из входных значений
        public void SetNeuronsInputs(List<double> inputs)
        {
            if (this.length != inputs.Count)
                throw new Exception("Количество входных нейронов не совпадает с количеством" +
                    "данных, переданных на вход");

            for (int i = 0; i < this.length; i++)
                this.neurons[i].Input = inputs[i];
        }

        // НЕОБЯЗАТЕЛЬНЫЕ МЕТОДЫ
        // Далее идут необязательные методы просто для удобной проверки и отладки


        // Переопределяем метод ToString() для удобного просмотра выходных значений нейронов слоя
        public override string ToString()
        {
            string result = "";
            foreach (var neuron in this.neurons)
                result += neuron.Output.ToString() + "\n";
            result = result.Substring(0, result.Length - 1);

            return result;
        }

        // Метод возвращяет список выходных значений нейронов слоя
        public List<double> ReturnListOfDouble()
        {
            List<double> result = new List<double>();
            for (int i = 0; i < this.length; i++)
                result.Add(this[i].Output);
            return result;
        }

    }
}
