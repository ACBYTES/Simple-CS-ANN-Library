// MIT License

// Copyright (c) 2020 Alireza Shahbazi (ACBYTES) (alirezashahbazi641@yahoo.com)

// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANN
{
    class NN
    {
        Table Inputs;
        Table Labels;
        ActivationFunction AF;
        LossFunction LF;
        double LR;
        int epochs;
        Table Weights;
        Table Bias;
        int trainingSize;
        int inputDimension;
        int outputDimension;
        double regLambda;
        public NN(Table DInputs, Table DLabels, List<HiddenLayer> HiddenLayers = null,
            ActivationFunction Activation = ActivationFunction.ReLU, LossFunction LossFunc = LossFunction.SGD, double LearningRate = .05f, int Lepochs = 1)
        {
            Inputs = DInputs;
            Labels = DLabels;
            AF = Activation;
            LF = LossFunc;
            LR = LearningRate;
            epochs = Lepochs;
        }

        public NN(Model AModel, int TrainingSetSize, int InputDim, int OutputDim, double RegularizationStrength,
            double LearningRate = .05f, int Lepochs = 1)
        {
            Inputs = AModel.inputs;
            Labels = AModel.labels;
            AF = AModel.activationFunction;
            LF = AModel.lossFunction;
            LR = LearningRate;
            epochs = Lepochs;
            trainingSize = TrainingSetSize;
            inputDimension = InputDim;
            outputDimension = OutputDim;
            regLambda = RegularizationStrength;
        }

        public enum ActivationFunction
        {
            ReLU,
            Sigmoid,
            TanH
        }

        public enum LossFunction
        {
            SGD,
            MSE,
            L2Norm,
        }

        #region ActivationFunctions

        #region ReLU
        private Table ReLU(Table x)
        {
            //y = max(0, x)
            List<double[]> da = new List<double[]>();
            for (int i = 0; i < x.tableStructure.YValues.Count; i++)
            {
                da.Add(new double[] { ReLU(x.tableStructure.YValues[i][0]) });
            }
            Table t = new Table(x.tableStructure.Xs, x.tableStructure.Ys, x.tableStructure.XValues, da);
            return t;
        }

        private double ReLU(double x)
        {
            //y = max(0, x)
            double res = Math.Min(Math.Max(x, 0), Math.Sqrt(x * x));
            return res;
        }

        private Table ReLUDerivative(Table x)
        {
            List<double[]> da = new List<double[]>();
            for (int i = 0; i < x.tableStructure.YValues.Count; i++)
            {
                double res = 0;
                if (x.tableStructure.YValues[i][0] == 0)
                    throw new Exception("Undefined for ReLU");
                res = (x.tableStructure.YValues[i][0] < 0) ? 0.01f : 1; //Leaky ReLU
                da.Add(new double[] { res });
            }
            Table t = new Table(x.tableStructure.Xs, x.tableStructure.Ys, x.tableStructure.XValues, da);
            return t;
        }
        #endregion

        #region Sigmoid
        private Table Sigmoid(Table x)
        {
            List<double[]> da = new List<double[]>();
            for (int i = 0; i < x.tableStructure.YValues.Count; i++)
            {
                var cr = x.tableStructure.YValues[i][0];
                da.Add(new double[] { 1 / (1 + Math.Exp(-cr)) });
            }
            Table t = Table.GenerateFromList(da); //new Table(x.tableStructure.Xs, x.tableStructure.Ys, x.tableStructure.XValues, da);
            return t;
        }

        private double Sigmoid(double x)
        {
            return 1 / (1 + Math.Exp(-x));
        }

        private Table SigmoidDerivative(Table x)
        {
            List<double[]> da = new List<double[]>();
            for (int i = 0; i < x.tableStructure.YValues.Count; i++)
            {
                var cr = x.tableStructure.YValues[i][0];
                da.Add(new double[] { Sigmoid(cr) * (1 - Sigmoid(cr)) });
            }
            Table t = Table.GenerateFromList(da); //new Table(x.tableStructure.Xs, x.tableStructure.Ys, x.tableStructure.XValues, da);
            return t;
        }
        #endregion

        #region TanH
        public static Table TanH(Table x)
        {
            List<double[]> da = new List<double[]>();
            for (int i = 0; i < x.tableStructure.YValues.Count; i++)
            {
                List<double> tv = new List<double>();
                for (int j = 0; j < x.tableStructure.YValues[i].Length; j++)
                {
                    var cr = x.tableStructure.YValues[i][j];
                    tv.Add(TanH(cr));
                }
                da.Add(tv.ToArray());
            }
            Table t = Table.GenerateFromList(da); //new Table(x.tableStructure.Xs, x.tableStructure.Ys, x.tableStructure.XValues, da);
            return t;
        }

        public static double TanH(double x)
        {
            return (Math.Exp(x) - Math.Exp(-x)) / (Math.Exp(x) + Math.Exp(-x));
        }

        public static Table TanHDerivative(Table x)
        {
            List<double[]> da = new List<double[]>();
            for (int i = 0; i < x.tableStructure.YValues.Count; i++)
            {
                List<double> tv = new List<double>();
                for (int j = 0; j < x.tableStructure.YValues[i].Length; j++)
                {
                    var cr = x.tableStructure.YValues[i][j];
                    tv.Add(1 - (Math.Pow(TanH(cr), 2)));
                }
                da.Add(tv.ToArray());
            }
            Table t = Table.GenerateFromList(da); //new Table(x.tableStructure.Xs, x.tableStructure.Ys, x.tableStructure.XValues, da);
            return t;
        }
        #endregion

        #endregion

        #region LossFunctions
        private double MSE()
        {
            //double res = // 1/n SUM(Predicted Y - Actual Y)Sqaured
            return 0;
        }
        #endregion

        private double GradientDescent()
        {
            //New Weight = Old Weight - alpha(Error / Weight)
            return 0;
        }

        public void Train()
        {
            #region NO HIDDEN LAYER CALCULATION
            var weights = Table.Random(1, 3);
            var bias = Table.Random(1);
            //bias = Table.GenerateFromList(new List<double[]>() { new double[] { -.4f } });
            for (int i = 0; i < epochs; i++)
            {
                var inputs = Inputs;
                var doted = Table.Dot(inputs, weights);

                //var XW = Table.Plus(new Table(1, doted.Count, Rand.GenerateNames(doted.Count), doted), bias); 1, doted.Count, Rand.GenerateNames(doted.Count), doted)
                var XW = Table.GenerateFromList(doted) + bias;

                var z = Sigmoid(XW);
                //var z = ReLU(XW);

                //END OF FEEDFORWARD

                //START OF BACKPROPAGATION

                var error = z - Labels /*Table.Subtract(z, Labels)*/;
                Console.WriteLine($"#### ERROR #### \n{error.Sum()}\n ########");

                var dcost = error;
                var dpred = SigmoidDerivative(z);
                //var dpred = ReLUDerivative(z);

                var zDel = dcost * dpred /*Table.Multiply(dcost, dpred)*/;
                inputs = Table.Transpose(Inputs);
                var wdoted = Table.Dot(inputs, zDel);
                //weights = Table.Subtract(weights, (Table.Multiply(new Table(1, wdoted.Count, Rand.GenerateNames(wdoted.Count), wdoted), LR)));
                weights = weights - (Table.GenerateFromList(wdoted) * LR);
                foreach (var item in zDel.tableStructure.YValues)
                {
                    //bias = Table.Subtract(bias, (Table.Multiply(item, LR)));
                    bias -= (Table.GenerateFromList(new List<double[]>() { item }) * LR);
                }
                //inputs.SaveTableToFile("INPUTS");
                //weights.SaveTableToFile("WEIGHTS");
                //bias.SaveTableToFile("BIASES");
                Weights = weights;
                Bias = bias;
                GC.Collect();
            }

            #endregion


            Console.WriteLine($"DATATABLE \n{Inputs}");
            Console.WriteLine($"LABELS \n{Labels}");
            Console.WriteLine($"WEIGHTS \n{weights}");
            Console.WriteLine($"BIAS \n{bias}");
            var dts = Table.Dot(Inputs, weights);
            Console.WriteLine($"DOT \n{new Table(dts.Count, dts[0].Length, Rand.GenerateNames(dts.Count), dts)}");
            Console.WriteLine($"DOT + BIAS \n{Table.Plus(new Table(dts.Count, dts[0].Length, Rand.GenerateNames(dts.Count), dts), bias)}");
        }

        public void Predict(Table data)
        {
            #region NO HIDDEN LAYER CALCULATION
            var doted = Table.Dot(data, Weights);
            /*var res = Sigmoid(Table.Plus(new Table(1, doted.Count, Rand.GenerateNames(doted.Count), doted), Bias));*/
            //var res = ReLU(Table.Plus(new Table(1, doted.Count, Rand.GenerateNames(doted.Count), doted), Bias));
            var res = Sigmoid((Table.GenerateFromList(doted) + Bias));
            foreach (var item in res.tableStructure.YValues[0])
            {
                Console.WriteLine(item);
            }
            #endregion
        }

        public Table Predict(Model MModel, Table Data)
        {
            var w1 = MModel.weights[0];
            var b1 = MModel.biases[0];
            var w2 = MModel.weights[1];
            var b2 = MModel.biases[1];

            var z1 = Table.GenerateFromList(Table.Dot(Data, w1)) + b1;
            var a1 = TanH(z1);
            var z2 = Table.GenerateFromList(Table.Dot(a1, w2)) + b2;

            var expScores = Table.Exp(z2);
            var probs = expScores / Table.Sum(expScores, 1);
            return Table.ArgMax(probs);
        }

        public static double CalculateLoss(Model MModel, Table DInputs, Table DLabels)
        {
            var examplesCount = DInputs.tableStructure.YValues.Count;
            var w1 = MModel.weights[0];
            var b1 = MModel.biases[0];
            var w2 = MModel.weights[1];
            var b2 = MModel.biases[1];

            var z1 = Table.GenerateFromList(Table.Dot(DInputs, w1)) + b1;
            var a1 = TanH(z1);
            var z2 = Table.GenerateFromList(Table.Dot(a1, w2)) + b2;

            var expScores = Table.Exp(z2);
            var probs = expScores / Table.Sum(expScores, 1);
            var correctLogProbes = Table.Sum(-Table.Log(probs.RangedXY(DLabels)));
            var dataLoss = correctLogProbes.Sum();
            dataLoss += MModel.regLambda / (2 * (Table.Square(w1).Sum() + Table.Square(w2).Sum()));
            return (1 / (examplesCount * dataLoss));
        }

        public class Model
        {
            public Table inputs;
            public Table labels;
            public List<HiddenLayer> hiddenLayers;
            public ActivationFunction activationFunction;
            public LossFunction lossFunction;
            public List<Table> biases = new List<Table>();
            public List<Table> weights = new List<Table>();
            public double regLambda;
            public int passCount;
            public double epsilon;
            public bool printLoss;
            public Model(Table DInputs, Table DLabels, List<HiddenLayer> HiddenLayerStructure, int OutputDim, double RegularizationLambda, int PassCount, double LearningRate, bool PrintLoss
                , ActivationFunction MActivationFunction, LossFunction MLossFunction)
            {
                inputs = DInputs;
                labels = DLabels;
                hiddenLayers = HiddenLayerStructure;
                lossFunction = MLossFunction;
                activationFunction = MActivationFunction;
                var examplesCount = DInputs.tableStructure.Ys;
                Rand.SetRandomSeed(0);
                weights.Add(Table.Random(hiddenLayers[0].neurons, examplesCount) / Math.Sqrt(examplesCount)); //SQRT?
                biases.Add(Table.Zeros(1, hiddenLayers[0].neurons));
                weights.Add(Table.Random(OutputDim, hiddenLayers[0].neurons) / Math.Sqrt(hiddenLayers[0].neurons));
                biases.Add(Table.Zeros(1, OutputDim));
                regLambda = RegularizationLambda;
                passCount = PassCount;
                epsilon = LearningRate;
                printLoss = PrintLoss;
            }
            public void Train()
            {
                for (int i = 0; i < passCount; i++)
                {
                    var w1 = weights[0];
                    var b1 = biases[0];
                    var w2 = weights[1];
                    var b2 = biases[1];

                    var z1 = Table.GenerateFromList(Table.Dot(inputs, w1)) + b1;
                    var a1 = TanH(z1);
                    var z2 = Table.GenerateFromList(Table.Dot(a1, w2)) + b2;

                    var expScores = Table.Exp(z2);
                    var probs = expScores / Table.Sum(expScores, 1);

                    var delta3 = probs;
                    delta3 = delta3.RangedXY(labels) - 1;
                    var tt = Table.Transpose(a1);
                    var tttt = Table.Dot(tt, delta3);
                    var dW2 = Table.GenerateFromList(Table.Dot(Table.Transpose(a1), delta3));
                    var db2 = Table.Sum(delta3, 0);
                    var delta2 = Table.GenerateFromList(Table.Dot(delta3, Table.Transpose(w2))) * (-Table.Power(a1, 2) + 1);
                    var dw1 = Table.GenerateFromList(Table.Dot(Table.Transpose(inputs), delta2));
                    var db1 = Table.Sum(delta2, 0);

                    dW2 += w2 * regLambda;
                    dw1 += w1 * regLambda;

                    w1 += dw1 * -epsilon;
                    weights[0] = w1;
                    b1 += db1 * -epsilon;
                    biases[0] = b1;
                    w2 += dW2 * -epsilon;
                    weights[1] = w2;
                    b2 += db2 * -epsilon;
                    biases[1] = b2;

                    if (printLoss)
                    {
                        if (i % 1000 == 0)
                        {
                            Console.ForegroundColor = ConsoleColor.DarkRed;
                            var loss = CalculateLoss(this, inputs, labels);
                            Console.WriteLine($"Loss after {i}: {loss}");
                        }
                    }
                }
            }
        }

        public class HiddenLayer
        {
            public int neurons { get; }
            public ActivationFunction activationFunction { get; }
            public Table bias { get; }
            public HiddenLayer(int Neurons, ActivationFunction HActivationFunction)
            {
                neurons = Neurons;
                activationFunction = HActivationFunction;
            }
        }
    }
}
