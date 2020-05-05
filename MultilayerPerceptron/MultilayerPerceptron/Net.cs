using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultilayerPerceptron
{
    // Класс, описывающий нейронную сеть
    
    [Serializable]
    class Net
    {
        // Список слоев
        private List<Layer> layers;
        public List<Layer> Layers { get { return layers; } }

        // Количество нейронов на входном слое
        private int inputsCount;
        public int InputsCount { get { return inputsCount; } }

        // Количество нейронов на выходном слое
        private int outputsCount;
        public int OutputsCount { get { return outputsCount; } }

        // Список, содержащий количества нейронов на скрытых слоях
        // Количество элементов списка - количество скрытых слоев
        private List<int> hiddensCount;
        public List<int> HiddensCount { get { return hiddensCount; } }


        // Конструктор
        // Принимает как аргументы количество нейронов на входном слое, количество нейронов на выходном слое,
        // а также массив параметров, указывающий количества нейронов на скрытых слоях (если они есть)
        public Net(int inputsCount, int outputsCount, params int[] hiddensCount)
        {
            this.inputsCount = inputsCount;
            this.outputsCount = outputsCount;
            this.hiddensCount = hiddensCount.ToList();
            this.layers = new List<Layer>();
            InitInputLayer();
            InitHiddenLayers();
            InitOutputLayer();
            InitWeights();
            FeedForward();
        }

        // Инициализирует входной слой
        private void InitInputLayer()
        {
            Layer inputLayer = new Layer(inputsCount, LayerType.Input);
            this.layers.Add(inputLayer);
        }

        // Инициализирует скрытые слои
        private void InitHiddenLayers()
        {
            Layer hiddenLayer;
            for (int i = 0; i < hiddensCount.Count; i++)
            {
                hiddenLayer = new Layer(hiddensCount[i], LayerType.Hidden);
                this.layers.Add(hiddenLayer);
            }
        }

        // Инициализирует выходной слой
        private void InitOutputLayer()
        {
            Layer outputLayer = new Layer(outputsCount, LayerType.Output);
            this.layers.Add(outputLayer);
        }

        // Инициализирует синапсы и выставляет им веса
        private void InitWeights()
        {
            for (int i = 1; i < this.layers.Count; i++)
            {
                // Прибавляем +1, так как .Length не считает нейроны смещения
                this.layers[i].InitWeights(this.layers[i - 1].Length + 1);
            }
        }

        // Обновляет нейронную сеть
        private void FeedForward()
        {
            for (int i = 1; i < this.layers.Count; i++)
                this.layers[i].FeedForward(this.layers[i - 1]);
        }

        // Выстанавливает входные значения нейронов на входном слое и обновляет сеть
        private void SetInputs(List<double> inputs)
        {
            if (inputs.Count != this.layers[0].Length)
                throw new Exception("Количество входных нейронов не совпадает с количеством" +
                    "входных данных");

            this.layers[0].SetNeuronsInputs(inputs);
            this.FeedForward();
        }

        // Обучения методом обратного распространения ошибки
        // Принимает как аргументы: список входных значений входного слоя, список ожидаемых выходных значений
        // выходного слоя, скорость обучения
        public void Backpropagation(List<double> inputs, List<double> outputs, double learningRate)
        {
            if (this.layers[0].Length != inputsCount || this.layers.Last().Length != outputs.Count)
                throw new Exception("Количество данных либо на входных, либо на выходных данных" +
                    "не совпадает по размерности с переданными либо на вход либо на выход" +
                    "данными");

            // Устанавливаем входные значения на входном слое
            this.SetInputs(inputs);

            // Считаем дельты у нейронов выходного слоя
            for (int i = 0; i < this.layers.Last().Length; i++)
                this.layers.Last()[i].Delta = outputs[i] - this.layers.Last()[i].Output;


            for (int layerNumber = this.layers.Count - 1; layerNumber > 0; layerNumber--)
            {
                // Зануляем дельты на предыдущем слое
                for (int i = 0; i < this.layers[layerNumber - 1].Length + 1; i++)
                    this.layers[layerNumber - 1][i].Delta = 0;

                for (int i = 0; i < this.layers[layerNumber].Length; i++)
                {
                    double delta = this.layers[layerNumber][i].Delta;
                    double output = this.layers[layerNumber][i].Output;
                    double weightDelta = delta * this.layers[layerNumber][i].SigmoidDerivative(output);

                    // Меняем веса, которые идут от предыдущего слоя
                    this.layers[layerNumber][i].ChangeWeights(this.layers[layerNumber - 1], learningRate);

                    // Меняем дельты нейронов предыдущего слоя
                    for (int j = 0; j < this.layers[layerNumber - 1].Length + 1; j++)
                        this.layers[layerNumber - 1][j].Delta +=
                            this.layers[layerNumber][i].Weights[j] * weightDelta * learningRate;
                }
            }
        }


        // Метод обучения
        // Принимает как аргументы: список списков входных значений,
        // список списков ожидаемых выходных знаений, скорость обучения, количество эпох
        public void Learn(List<List<double>> inputs, List<List<double>> outputs, double learningRate,
            int epoches)
        {
            if (inputs.Count != outputs.Count)
                throw new Exception("Количество ответов не совпадает с количеством входных данных");

            for (int epoch = 0; epoch < epoches; epoch++)
                for (int i = 0; i < inputs.Count; i++)
                    this.Backpropagation(inputs[i], outputs[i], learningRate);
        }



        // НЕОБЯЗАТЕЛЬНЫЕ МЕТОДА
        // Далее идут необязательные методы удобные для тестирования и отладки
        
        // Возвращает веса всех синапсов нейронной сети
        public string ReturnWeights()
        {
            string result = "";
            for (int i = 1; i < this.layers.Count; i++)
            {
                for (int j = 0; j < this.layers[i].Length; j++)
                {
                    for (int w = 0; w < this.layers[i][j].Weights.Count; w++)
                    {
                        result += this.layers[i][j].Weights[w].ToString() + " ";
                    }
                    result += "\n";
                }
                result += "\n";
            }
            return result;
        }

        // Возвращает дельты всех нейронов сети
        public string ReturnDeltas()
        {
            string result = "=================\n";
            for (int i = 0; i < this.layers.Count; i++)
            {
                for (int j = 0; j < (this.layers[i].Type == LayerType.Output ?
                    this.layers[i].Length : this.layers[i].Length + 1); j++)
                {
                    result += " " + this.layers[i][j].Delta.ToString();
                }
                result += "\n";
            }

            return result;
        }

        // Переопределяем метод ToString()
        // Возвращает строку, содержащую выходные значений нейронов выходного слоя
        public override string ToString()
        {
            return layers.Last().ToString();
        }

        // Возвращает список выходных значений нейронов выходного слоя
        public List<double> ReturnList(List<double> inputs)
        {
            this.SetInputs(inputs);
            return this.layers.Last().ReturnListOfDouble();
        }

        // Возвращает строку содержащую выходные значений нейронов выходного слоя сети
        // Принимает как аргумент входные значения нейронов для входного слоя
        public string Return(List<double> inputs)
        {
            this.SetInputs(inputs);
            this.FeedForward();
            return this.ToString();
        }


    }
}
