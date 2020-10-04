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
    class Rand
    {
        static Random rand = new Random(42);
        public static double NextDouble()
        {
            return rand.NextDouble();
        }

        public static string[] GenerateNames(int x)
        {
            string[] xs = new string[x];
            for (int i = 0; i < xs.Length; i++)
            {
                xs[i] = $"P{i}";
            }
            return xs;
        }

        public static void SetRandomSeed(int Seed)
        {
            rand = new Random(Seed);
        }

        public static Table[] GenerateRandomData(int Count, int XMemberCount, Range XMemebersRange, Range YMemebersRange, Range LabelsRange)
        {
            var res = new Table[2];
            var features = new List<double[]>();
            var labels = new List<double[]>();
            for (int i = 0; i < Count; i++)
            {
                List<double> tempV = new List<double>();
                for (int j = 0; j < XMemberCount / 2; j++)
                {
                    var r = rand.NextDouble();
                    var limMax = Math.Round(XMemebersRange.max - r);
                    var limMin = Math.Round(XMemebersRange.min - r);
                    tempV.Add(r + (rand.Next((int)limMin, (int)limMax)));

                    r = rand.NextDouble();
                    limMax = Math.Round(YMemebersRange.max - r);
                    limMin = Math.Round(YMemebersRange.min - r);
                    tempV.Add(r + (rand.Next((int)limMin, (int)limMax)));
                }
                features.Add(tempV.ToArray());
                labels.Add(new double[] { rand.Next((int)LabelsRange.min, (int)LabelsRange.max + 1) });
            }
            res[0] = Table.GenerateFromList(features);
            res[1] = Table.GenerateFromList(labels);
            return res;
        }

        public class Range
        {
            public double min { get; }
            public double max { get; }
            public Range(double Min, double Max)
            {
                min = Min;
                max = Max;
            }
        }
    }
}
