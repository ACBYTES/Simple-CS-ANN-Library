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
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace ANN
{
    public class Table
    {
        public struct Structure
        {
            public int Xs;
            public int Ys;
            public string[] XValues;
            public List<double[]> YValues;
        }

        public Structure tableStructure;


        public Table(int Xs, int Ys, string[] XValues, List<double[]> YValues)
        {
            List<Tuple<string, double[]>> Table = new List<Tuple<string, double[]>>(); //string is going to be the state's name and the int array is going to be the rewards collected by the agent over each state taking an action.
            for (int i = 0; i < Xs; i++)
            {
                Table.Add(new Tuple<string, double[]>(XValues[i], YValues[i]));
            }

            tableStructure.Xs = Xs;
            tableStructure.Ys = Ys;
            tableStructure.XValues = XValues;
            tableStructure.YValues = YValues;
        }

        public static Table Zeros(int Count, int MemberCount)
        {
            List<double[]> values = new List<double[]>();
            for (int i = 0; i < Count; i++)
            {
                var tempV = new double[MemberCount];
                for (int j = 0; j < MemberCount; j++)
                {
                    tempV[j] = 0;
                }
                values.Add(tempV);
            }
            return Table.GenerateFromList(values);
        }

        public static string InitStructure(Table Table)
        {
            string res = "[\n  ";
            for (int i = 0; i < Table.tableStructure.YValues.Count; i++)
            {
                for (int j = 0; j < Table.tableStructure.YValues[0].Length; j++)
                {
                    res += $"{Table.tableStructure.YValues[i][j]}, ";
                }
                res += "\n  ";
            }

            return res + "\b\b]";
        }

        public static string InitInternalStringReturn(Table Table)
        {
            string res = "[";
            for (int i = 0; i < Table.tableStructure.YValues.Count; i++)
            {
                res += "[";
                for (int j = 0; j < Table.tableStructure.YValues[0].Length; j++)
                {
                    res += $"{Table.tableStructure.YValues[i][j]}, ";
                }
                res += "] ";
            }

            return res + "]";
        }

        public static string InitFileStructure(Table Table)
        {
            string res = "[\n  ";
            for (int i = 0; i < Table.tableStructure.YValues.Count; i++)
            {
                for (int j = 0; j < Table.tableStructure.YValues[0].Length; j++)
                {
                    res += $"{Table.tableStructure.YValues[i][j]}, ";
                }
                res += "\n  ";
            }

            return res + "\b\b]";
        }

        public void SaveTableToFile(string Name)
        {
            var st = InitFileStructure(this);
            string path = $@"C:\Users\Alireza\Desktop\TableValues({DateTime.Now.DayOfYear} {Name}).ACCOQT";
            if (!File.Exists(path))
                File.Create(path).Close();
            var prev = File.ReadAllText(path);
            File.WriteAllText(path, "\n\n" + prev + st);
        }

        public void ReadExistingStructure()
        {
            string path = $@"C:\Users\Alireza\Desktop\TableValues({DateTime.Now.DayOfYear}).ACCOQT";
            if (File.Exists(path))
            {
                var res = File.ReadAllText(path);
                var resA = res.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();
                resA.RemoveAt(0);
                resA.RemoveAt(16);
                for (int j = 0; j < resA.Count; j++)
                {
                    var splitedVs = resA[j].Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries);
                    for (int i = 0; i < splitedVs.Count() - 1; i++)
                    {
                        this.UpdateValue(j.ToString(), 0, double.Parse(splitedVs[0]));
                        this.UpdateValue(j.ToString(), 1, double.Parse(splitedVs[1]));
                        this.UpdateValue(j.ToString(), 2, double.Parse(splitedVs[2]));
                        this.UpdateValue(j.ToString(), 3, double.Parse(splitedVs[3]));
                    }
                }
            }
        }

        public string Max(string State)
        {
            string maxValueAsString = string.Empty;
            for (int i = 0; i < tableStructure.XValues.Count(); i++)
            {
                if (tableStructure.XValues[i] == State)
                {
                    var res = tableStructure.YValues[i];
                    var max = res.Max();
                    for (int j = 0; j < res.Count(); j++)
                    {
                        if (res[j] == max)
                        {
                            if (j == 0)
                            {
                                maxValueAsString = "Up";
                                break;
                            }
                            else if (j == 1)
                            {
                                maxValueAsString = "Down";
                                break;
                            }
                            else if (j == 2)
                            {
                                maxValueAsString = "Right";
                                break;
                            }
                            else if (j == 3)
                            {
                                maxValueAsString = "Left";
                                break;
                            }
                        }
                    }
                }
            }
            return maxValueAsString;
        }

        public double MaxFromAllForDrawing()
        {
            List<double> res = new List<double>();
            foreach (var item in tableStructure.YValues)
            {
                res.Add(item.Max());
            }
            return res.Max();
        }

        public double MinFromAllForDrawing()
        {
            List<double> res = new List<double>();
            foreach (var item in tableStructure.YValues)
            {
                res.Add(item.Min());
            }
            return res.Min();
        }

        public double MaxInt(string State)
        {
            double max = 0;
            for (int i = 0; i < tableStructure.XValues.Count(); i++)
            {
                if (tableStructure.XValues[i] == State)
                {
                    var res = tableStructure.YValues[i];
                    max = res.Max();
                }
            }
            return max;
        }

        public double All(int state)
        {
            double res = 0;
            foreach (var item in tableStructure.YValues[state])
            {
                res += item;
            }
            return res;
        }

        public void UpdateValue(string State, int Action, double NewValue)
        {
            for (int i = 0; i < tableStructure.XValues.Count(); i++)
            {
                if (tableStructure.XValues[i] == State)
                {
                    var res = tableStructure.YValues[i];
                    res[Action] = NewValue;
                }
            }
        }

        public double GetValue(string State, int Action)
        {
            double rRes = 0;
            for (int i = 0; i < tableStructure.XValues.Count(); i++)
            {
                if (tableStructure.XValues[i] == State)
                {
                    var res = tableStructure.YValues[i];
                    rRes = res[Action];
                }
            }
            return rRes;
        }

        public static Table Random(int Count, int Dimensions = 1)
        {
            List<double[]> ys = new List<double[]>();
            //System.Random rand = new Random(42);
            for (int i = 0; i < Dimensions; i++)
            {
                double[] n = new double[Count];
                for (int j = 0; j < n.Length; j++)
                {
                    n[j] = Rand.NextDouble();
                }
                ys.Add(n);
            }

            List<string> xNames = new List<string>();
            for (int i = 0; i < Dimensions; i++)
            {
                xNames.Add(i.ToString());
            }

            Table t = new Table(Dimensions, Count, xNames.ToArray(), ys);
            return t;
        }

        public static int[] GetDotValues(Table a, Table b)
        {
            int[] res = new int[3];
            res[0] = a.tableStructure.YValues.Count;
            res[1] = b.tableStructure.YValues[0].Length;
            res[2] = b.tableStructure.YValues.Count;
            return res;
        }

        public static List<double[]> Dot(Table a, Table b)
        {
            var ats = a.tableStructure.YValues;
            var bts = b.tableStructure.YValues;
            List<double> res = new List<double>();

            //foreach (var item in ats)
            //{
            //    if (item.Length != bts.Count)
            //    {
            //        //throw new Exception($"Lengths are not the same. {item} != {bts.Count}");
            //    }
            //}

            //ITEMS COUNT = BTS ITEMS COUNT
            //ARRAYS COUNT = ATS ARRAYS COUNT GETDOTVALUES()[0]
            var dotValues = GetDotValues(a, b);
            for (int i = 0; i < dotValues[0]; i++)
            {
                for (int m = 0; m < dotValues[1]; m++)
                {
                    List<double> pres = new List<double>();
                    for (int j = 0; j < dotValues[2]; j++)
                    {
                        //ats[i][j] * bts[j][i] + ats[i][j + 1] * bts[j + 1][i]
                        pres.Add(ats[i][j] * bts[j][m]);
                    }
                    var ppres = pres.Sum();
                    res.Add(ppres);
                }
            }

            List<double[]> zres = new List<double[]>();
            //foreach (var item in res)
            //{
            //    zres.Add(new double[] { item });
            //}

            var iL = res.Count / dotValues[1];
            for (int i = 0; i < iL; i++)
            {
                List<double> tempL = new List<double>();
                for (int j = 0; j < dotValues[1]; j++)
                {
                    tempL.Add(res[j]);
                }
                zres.Add(tempL.ToArray());
                res.RemoveRange(0, dotValues[1]);
            }

            //var tempD = new double[dotValues[1]];
            //for (int j = 0; j < res.Count / dotValues[1]; j++)
            //{
            //    for (int i = 0; i < tempD.Length; i++)
            //    {
            //        tempD[i] = res[i];
            //    }
            //    for (int r = 0; r < dotValues[1]; r++)
            //    {
            //        res.RemoveAt(r);
            //    }
            //    zres.Add(tempD);
            //}

            return zres;
        }
        public static Table Plus(Table a, Table b)
        {
            List<double[]> res = new List<double[]>();
            //if (b.tableStructure.YValues.Count == 1)
            //{
            //foreach (var item in a.tableStructure.YValues)
            //{
            //    double[] pres = new double[item.Count()];
            //    for (int i = 0; i < pres.Length; i++)
            //    {
            //        pres[i] = item[i] + b.tableStructure.YValues[0][0];
            //    }
            //    res.Add(pres);
            //}
            //Table t = new Table(a.tableStructure.Xs, a.tableStructure.Ys, a.tableStructure.XValues, res);
            //return t;

            Table t0 = (a.tableStructure.YValues[0].Length >= b.tableStructure.YValues[0].Length) ? a : b;
            Table t1 = (t0 == a) ? b : a;

            //t1.tableStructure.YValues.Count == 1
            if (t1.tableStructure.YValues.Count == 1)
            {
                if (t1.tableStructure.YValues[0].Length == 1)
                {
                    //Values of t0 + value[0][0] of t1
                    foreach (var item in t0.tableStructure.YValues)
                    {
                        List<double> tl = new List<double>();
                        foreach (var d in item)
                        {
                            tl.Add(d + t1.tableStructure.YValues[0][0]);
                        }
                        res.Add(tl.ToArray());
                    }
                    return Table.GenerateFromList(res);
                }

                else
                {
                    //Values of t0 + value[0][i] of t1
                    //if (t0.tableStructure.YValues[0].Length != t1.tableStructure.YValues[0].Length)
                    //    throw new Exception("Operands could not be broadcast together with these shapes.");
                    foreach (var item in t0.tableStructure.YValues)
                    {
                        List<double> tl = new List<double>();
                        for (int i = 0; i < item.Length; i++)
                        {
                            tl.Add(item[i] + t1.tableStructure.YValues[0][i]);
                        }
                        res.Add(tl.ToArray());
                    }
                    return Table.GenerateFromList(res);
                }
            }

            else
            {
                t0 = (a.tableStructure.YValues.Count >= b.tableStructure.YValues.Count) ? a : b;
                t1 = (t0 == a) ? b : a;

                if (t0.tableStructure.YValues[0].Length == 1)
                {
                    for (int i = 0; i < t1.tableStructure.YValues.Count; i++)
                    {
                        List<double> tl = new List<double>();
                        foreach (var item in t1.tableStructure.YValues[i])
                        {
                            tl.Add(item + t0.tableStructure.YValues[i][0]);
                        }
                        res.Add(tl.ToArray());
                    }
                    return Table.GenerateFromList(res);
                }

                else
                {
                    if (t0.tableStructure.YValues.Count != t1.tableStructure.YValues.Count)
                        throw new Exception("Matrices don't have the same amount of rows.");
                    else if (!CheckMatricesColumnsOverAS(t0, t1))
                        throw new Exception("Matrices don't have the same amount of columns.");
                    else
                    {
                        for (int i = 0; i < t0.tableStructure.YValues.Count; i++)
                        {
                            List<double> tl = new List<double>();
                            for (int j = 0; j < t1.tableStructure.YValues[i].Length; j++)
                            {
                                tl.Add(t0.tableStructure.YValues[i][j] + t1.tableStructure.YValues[i][j]);
                            }
                            res.Add(tl.ToArray());
                        }
                        return Table.GenerateFromList(res);
                    }
                }
            }


            //for (int i = 0; i < a.tableStructure.YValues.Count; i++)
            //{
            //    List<double> pres = new List<double>();
            //    for (int j = 0; j < a.tableStructure.YValues[i].Length; j++)
            //    {
            //        var bj = -1;
            //        var bi = -1;
            //        if (b.tableStructure.YValues.Count == 1)
            //        {
            //            bi = 0;
            //        }
            //        else
            //            bi = i;
            //        if (b.tableStructure.YValues[bi].Length == 1)
            //        {
            //            bj = 0;
            //        }
            //        else
            //            bj = j;
            //        pres.Add(a.tableStructure.YValues[i][j] + b.tableStructure.YValues[bi][bj]);
            //    }
            //    res.Add(pres.ToArray());
            //}
            //Table t = Table.GenerateFromList(res);
            //return t;
            //}
            //else
            //{
            //    for (int i = 0; i < a.tableStructure.YValues.Count; i++)
            //    {
            //        double[] pres = new double[a.tableStructure.YValues[i].Count()];
            //        for (int j = 0; j < pres.Length; j++)
            //        {
            //            var yvj = (b.tableStructure.YValues[i].Length == 1) ? 0 : j;
            //            pres[j] = a.tableStructure.YValues[i][j] + b.tableStructure.YValues[i][yvj];
            //        }
            //        res.Add(pres);
            //    }
            //    Table t = new Table(a.tableStructure.Xs, a.tableStructure.Ys, a.tableStructure.XValues, res);
            //    return t;
            //}
        }

        public static Table operator +(Table a, Table b) => Plus(a, b);

        public static Table Plus(Table a, double b)
        {
            List<double[]> res = new List<double[]>();

            for (int i = 0; i < a.tableStructure.YValues.Count; i++)
            {
                List<double> pres = new List<double>();
                for (int j = 0; j < a.tableStructure.YValues[i].Length; j++)
                {
                    pres.Add(a.tableStructure.YValues[i][j] + b);
                }
                res.Add(pres.ToArray());
            }
            Table t = Table.GenerateFromList(res);
            return t;
        }

        public static Table operator +(Table a, double b) => Plus(a, b);

        public static Table Subtract(Table a, Table b)
        {
            List<double[]> res = new List<double[]>();
            //List<double> pres = new List<double>();
            ////foreach (var item in a.tableStructure.YValues)
            ////{
            ////double[] pres = new double[item.Count()];
            //for (int i = 0; i < a.tableStructure.YValues.Count; i++)
            //{
            //    pres.Add(a.tableStructure.YValues[i][0] - b.tableStructure.YValues[i][0]);
            //}
            //foreach (var item in pres)
            //{
            //    res.Add(new double[] { item });
            //}
            ////res.Add(pres);
            ////}
            //Table t = new Table(a.tableStructure.Xs, a.tableStructure.Ys, a.tableStructure.XValues, res);
            //return t;

            Table t0 = (a.tableStructure.YValues.Count >= b.tableStructure.YValues.Count) ? a : b;
            Table t1 = (t0 == a) ? b : a;

            if (t1.tableStructure.YValues.Count == 1)
            {
                t0 = (a.tableStructure.YValues[0].Length >= b.tableStructure.YValues[0].Length) ? a : b;
                t1 = (t0 == a) ? b : a;
                if (t1.tableStructure.YValues[0].Length == 1)
                {
                    //Values of t0 + value[0][0] of t1
                    foreach (var item in t0.tableStructure.YValues)
                    {
                        List<double> tl = new List<double>();
                        foreach (var d in item)
                        {
                            if (t0 == a)
                                tl.Add(d - t1.tableStructure.YValues[0][0]);
                            else
                                tl.Add(-d + t1.tableStructure.YValues[0][0]);
                        }
                        res.Add(tl.ToArray());
                    }
                    return Table.GenerateFromList(res);
                }

                else
                {
                    //Values of t0 + value[0][i] of t1
                    //if (t0.tableStructure.YValues[0].Length != t1.tableStructure.YValues[0].Length)
                    //    throw new Exception("Operands could not be broadcast together with these shapes.");
                    foreach (var item in t0.tableStructure.YValues)
                    {
                        List<double> tl = new List<double>();
                        for (int i = 0; i < item.Length; i++)
                        {
                            if (t0 == a)
                                tl.Add(item[i] - t1.tableStructure.YValues[0][i]);
                            else
                                tl.Add(-item[i] - t1.tableStructure.YValues[0][i]);
                        }
                        res.Add(tl.ToArray());
                    }
                    return Table.GenerateFromList(res);
                }
            }

            else
            {

                if (t0.tableStructure.YValues[0].Length == 1)
                {
                    for (int i = 0; i < t1.tableStructure.YValues.Count; i++)
                    {
                        List<double> tl = new List<double>();
                        foreach (var item in t1.tableStructure.YValues[i])
                        {
                            if (t0 == a)
                                tl.Add(-item + t0.tableStructure.YValues[i][0]);
                            else
                                tl.Add(-t0.tableStructure.YValues[i][0] + item);
                        }
                        res.Add(tl.ToArray());
                    }
                    return Table.GenerateFromList(res);
                }
                else
                {
                    if (t0.tableStructure.YValues.Count != t1.tableStructure.YValues.Count)
                        throw new Exception("Matrices don't have the same amount of rows.");
                    else if (!CheckMatricesColumnsOverAS(t0, t1))
                        throw new Exception("Matrices don't have the same amount of columns.");
                    else
                    {
                        for (int i = 0; i < t0.tableStructure.YValues.Count; i++)
                        {
                            List<double> tl = new List<double>();
                            for (int j = 0; j < t1.tableStructure.YValues[i].Length; j++)
                            {
                                if (t0 == a)
                                    tl.Add(t0.tableStructure.YValues[i][j] - t1.tableStructure.YValues[i][j]);
                                else
                                    tl.Add(t1.tableStructure.YValues[i][j] - t0.tableStructure.YValues[i][j]);
                            }
                            res.Add(tl.ToArray());
                        }
                        return Table.GenerateFromList(res);
                    }
                }
            }

            //for (int i = 0; i < a.tableStructure.YValues.Count; i++)
            //{
            //    List<double> pres = new List<double>();
            //    for (int j = 0; j < a.tableStructure.YValues[i].Length; j++)
            //    {
            //        var bj = -1;
            //        var bi = -1;
            //        if (b.tableStructure.YValues.Count == 1)
            //        {
            //            bi = 0;
            //        }
            //        else
            //            bi = i;
            //        if (b.tableStructure.YValues[bi].Length == 1)
            //        {
            //            bj = 0;
            //        }
            //        else
            //            bj = j;
            //        pres.Add(a.tableStructure.YValues[i][j] - b.tableStructure.YValues[bi][bj]);
            //    }
            //    res.Add(pres.ToArray());
            //}
            //Table t = Table.GenerateFromList(res);
            //return t;
            //List<double[]> res = new List<double[]>();
            //if (b.tableStructure.YValues.Count == 1)
            //{
            //    foreach (var item in a.tableStructure.YValues)
            //    {
            //        double[] pres = new double[item.Count()];
            //        for (int i = 0; i < pres.Length; i++)
            //        {
            //            pres[i] = item[i] - b.tableStructure.YValues[0][i];
            //        }
            //        res.Add(pres);
            //    }
            //    Table t = new Table(a.tableStructure.Xs, a.tableStructure.Ys, a.tableStructure.XValues, res);
            //    return t;
            //}
            //else
            //{
            //    for (int i = 0; i < a.tableStructure.YValues.Count; i++)
            //    {
            //        double[] pres = new double[a.tableStructure.YValues[i].Count()];
            //        for (int j = 0; j < pres.Length; j++)
            //        {
            //            var yvj = (b.tableStructure.YValues[i].Length == 1) ? 0 : j;
            //            pres[j] = a.tableStructure.YValues[i][j] - b.tableStructure.YValues[i][yvj];
            //        }
            //        res.Add(pres);
            //    }
            //    Table t = new Table(a.tableStructure.Xs, a.tableStructure.Ys, a.tableStructure.XValues, res);
            //    return t;
            //}
        }

        public static Table operator -(Table a, Table b) => Subtract(a, b);

        public static Table Subtract(Table a, double b)
        {
            List<double[]> res = new List<double[]>();

            for (int i = 0; i < a.tableStructure.YValues.Count; i++)
            {
                List<double> pres = new List<double>();
                for (int j = 0; j < a.tableStructure.YValues[i].Length; j++)
                {
                    pres.Add(a.tableStructure.YValues[i][j] - b);
                }
                res.Add(pres.ToArray());
            }
            Table t = Table.GenerateFromList(res);
            return t;
        }

        public static Table operator -(Table a, double b) => (Subtract(a, b));

        public static Table Multiply(Table a, Table b)
        {
            List<double[]> res = new List<double[]>();
            //for (int i = 0; i < a.tableStructure.YValues.Count; i++)
            //{
            //    double[] pres = new double[a.tableStructure.YValues[0].Count()];
            //    for (int j = 0; j < pres.Length; j++)
            //    {
            //        pres[j] = a.tableStructure.YValues[i][j] * b.tableStructure.YValues[i][j];
            //    }
            //    res.Add(pres);
            //}
            //Table t = new Table(a.tableStructure.Xs, a.tableStructure.Ys, a.tableStructure.XValues, res);
            //return t;

            Table t0 = (a.tableStructure.YValues[0].Length >= b.tableStructure.YValues[0].Length) ? a : b;
            Table t1 = (t0 == a) ? b : a;

            for (int i = 0; i < t0.tableStructure.YValues.Count; i++)
            {
                double[] pres = new double[t0.tableStructure.YValues[0].Count()];
                for (int j = 0; j < pres.Length; j++)
                {
                    var yvj = (t1.tableStructure.YValues[i].Length == 1) ? 0 : j;
                    pres[j] = t0.tableStructure.YValues[i][j] * t1.tableStructure.YValues[i][yvj];
                }
                res.Add(pres);
            }
            Table t = new Table(a.tableStructure.Xs, a.tableStructure.Ys, a.tableStructure.XValues, res);
            return t;
        }

        public static Table operator *(Table a, Table b) => Multiply(a, b);

        public static Table ReturnMinusValues(Table a)
        {
            var res = new List<double[]>();
            for (int i = 0; i < a.tableStructure.YValues.Count; i++)
            {
                var tempr = new List<double>();
                for (int j = 0; j < a.tableStructure.YValues[i].Length; j++)
                {
                    var temp = a.tableStructure.YValues[i][j];
                    tempr.Add(-temp);
                }
                res.Add(tempr.ToArray());
            }
            return Table.GenerateFromList(res);
        }
        public static Table operator -(Table a) => ReturnMinusValues(a);

        public static Table Multiply(double[] a, double b)
        {
            List<double[]> res = new List<double[]>();
            double[] pres = new double[a.Length];
            for (int j = 0; j < pres.Length; j++)
            {
                pres[j] = a[j] * b;
            }
            res.Add(pres);
            Table t = new Table(1, pres.Length, Rand.GenerateNames(pres.Length), res);
            return t;
        }

        public static Table Multiply(Table a, double multiplier)
        {
            List<double[]> res = new List<double[]>();
            for (int i = 0; i < a.tableStructure.YValues.Count; i++)
            {
                double[] pres = new double[a.tableStructure.YValues[0].Count()];
                for (int j = 0; j < pres.Length; j++)
                {
                    pres[j] = a.tableStructure.YValues[i][j] * multiplier;
                }
                res.Add(pres);
            }
            Table t = new Table(a.tableStructure.Xs, a.tableStructure.Ys, a.tableStructure.XValues, res);
            return t;
        }

        public static Table operator *(Table a, double multiplier) => Multiply(a, multiplier);

        public static Table Transpose(Table a)
        {
            List<double[]> res = new List<double[]>();
            for (int i = 0; i < a.tableStructure.YValues[0].Length; i++)
            {
                double[] pres = new double[a.tableStructure.YValues.Count];
                for (int j = 0; j < a.tableStructure.YValues.Count; j++)
                {
                    pres[j] = a.tableStructure.YValues[j][i];
                }
                res.Add(pres);
            }
            Table t = Table.GenerateFromList(res)/*new Table(a.tableStructure.Ys, a.tableStructure.Xs, a.tableStructure.XValues, res)*/;

            return t;
        }

        public double Sum()
        {
            double sum = 0;
            //foreach (var item in tableStructure.YValues)
            //{
            //    sum += item[0];
            //}

            for (int i = 0; i < tableStructure.YValues.Count; i++)
            {
                for (int j = 0; j < tableStructure.YValues[i].Length; j++)
                {
                    sum += tableStructure.YValues[i][j];
                }
            }
            return sum;
        }

        public static Table Sum(Table a, int Axis = 1)
        {
            List<double[]> res = new List<double[]>();
            if (Axis == 1)
            {
                foreach (var item in a.tableStructure.YValues)
                {
                    res.Add(new double[] { item.Sum() });
                }
            }

            else if (Axis == 0)
            {
                List<double[]> r = new List<double[]>();
                List<double> tld = new List<double>();
                for (int i = 0; i < a.tableStructure.Ys; i++)
                {
                    double tempRes = 0;
                    foreach (var item in a.tableStructure.YValues)
                    {
                        tempRes += item[i];
                    }
                    tld.Add(tempRes);
                }
                r.Add(tld.ToArray());
                res = r;
                r = null;
                tld = null;
            }
            return Table.GenerateFromList(res);
        }

        public static Table GenerateFromList(List<double[]> Input)
        {
            Table t = new Table(Input.Count, Input[0].Length, Rand.GenerateNames(Input.Count), Input);
            return t;
        }

        public static Table Exp(Table a)
        {
            var res = new List<double[]>();
            for (int i = 0; i < a.tableStructure.YValues.Count; i++)
            {
                var tempR = new List<double>();
                for (int j = 0; j < a.tableStructure.YValues[i].Length; j++)
                {
                    tempR.Add(Math.Exp(a.tableStructure.YValues[i][j]));
                }
                res.Add(tempR.ToArray());
            }
            return Table.GenerateFromList(res);
        }

        public static Table Divide(Table a, Table b)
        {
            List<double[]> res = new List<double[]>();

            Table t0 = (a.tableStructure.YValues[0].Length >= b.tableStructure.YValues[0].Length) ? a : b;
            Table t1 = (t0 == a) ? b : a;

            for (int i = 0; i < t0.tableStructure.YValues.Count; i++)
            {
                double[] pres = new double[t0.tableStructure.YValues[0].Count()];
                for (int j = 0; j < pres.Length; j++)
                {
                    var yvj = (t1.tableStructure.YValues[i].Length == 1) ? 0 : j;
                    if (t0 == a)
                        pres[j] = t0.tableStructure.YValues[i][j] / t1.tableStructure.YValues[i][yvj];
                    else
                        pres[j] = t1.tableStructure.YValues[i][yvj] / t0.tableStructure.YValues[i][j];
                }
                res.Add(pres);
            }
            Table t = new Table(a.tableStructure.Xs, a.tableStructure.Ys, a.tableStructure.XValues, res);
            return t;
        }

        public static Table operator /(Table a, Table b) => Divide(a, b);

        public static Table Divide(double[] a, double b)
        {
            List<double[]> res = new List<double[]>();
            double[] pres = new double[a.Length];
            for (int j = 0; j < pres.Length; j++)
            {
                pres[j] = a[j] / b;
            }
            res.Add(pres);
            Table t = new Table(1, pres.Length, Rand.GenerateNames(pres.Length), res);
            return t;
        }

        public static Table Divide(Table a, double denominator)
        {
            List<double[]> res = new List<double[]>();
            for (int i = 0; i < a.tableStructure.YValues.Count; i++)
            {
                double[] pres = new double[a.tableStructure.YValues[0].Count()];
                for (int j = 0; j < pres.Length; j++)
                {
                    pres[j] = a.tableStructure.YValues[i][j] / denominator;
                }
                res.Add(pres);
            }
            Table t = new Table(a.tableStructure.Xs, a.tableStructure.Ys, a.tableStructure.XValues, res);
            return t;
        }
        public static Table operator /(Table a, double denominator) => Divide(a, denominator);


        public static Table Log(Table a)
        {
            var res = new List<double[]>();
            for (int i = 0; i < a.tableStructure.YValues.Count; i++)
            {
                var tempr = new List<double>();
                for (int j = 0; j < a.tableStructure.YValues[i].Length; j++)
                {
                    var temp = a.tableStructure.YValues[i][j];
                    temp = Math.Log(temp);
                    tempr.Add(temp);
                }
                res.Add(tempr.ToArray());
            }
            var ns = new List<double>();
            foreach (var item in res)
            {
                for (int i = 0; i < item.Length; i++)
                {
                    ns.Add(item[i]);
                }
            }
            return Table.GenerateFromList(new List<double[]>() { ns.ToArray() });
        }

        public static Table Square(Table a)
        {
            var res = new List<double[]>();
            for (int i = 0; i < a.tableStructure.YValues.Count; i++)
            {
                var tres = new List<double>();
                for (int j = 0; j < a.tableStructure.YValues[i].Length; j++)
                {
                    tres.Add((Math.Pow(a.tableStructure.YValues[i][j], 2)));
                }
                res.Add(tres.ToArray());
            }
            return Table.GenerateFromList(res);
        }

        public double GetMax(int Axis)
        {
            return tableStructure.YValues[Axis - 1].Max();
        }

        public static Table Power(Table a, double b)
        {
            var res = new List<double[]>();
            for (int i = 0; i < a.tableStructure.YValues.Count; i++)
            {
                var tres = new List<double>();
                for (int j = 0; j < a.tableStructure.YValues[i].Length; j++)
                {
                    tres.Add((Math.Pow(a.tableStructure.YValues[i][j], b)));
                }
                res.Add(tres.ToArray());
            }
            return Table.GenerateFromList(res);
        }

        public static bool CheckMatricesColumnsOverAS(Table a, Table b)
        {
            bool res = true;
            for (int i = 0; i < a.tableStructure.YValues.Count; i++)
            {
                if (a.tableStructure.YValues[i].Length != b.tableStructure.YValues[i].Length)
                {
                    res = false;
                }

                else
                    continue;
            }
            return res;
        }

        public Table RangedXY(Table y)
        {
            List<double[]> res = new List<double[]>();
            List<double> tempRes = new List<double>();
            for (int j = 0; j < tableStructure.Xs; j++)
            {
                for (int i = 0; i < tableStructure.Ys; i++)
                {
                    tempRes.Add(tableStructure.YValues[i][(int)y.tableStructure.YValues[i][0]]);
                }
                res.Add(tempRes.ToArray());
            }
            return GenerateFromList(res);
        }

        public static Table ArgMax(Table a)
        {
            var res = new List<double>();
            foreach (var item in a.tableStructure.YValues)
            {
                res.Add(item.ToList().IndexOf(item.Max()));
            }
            return Table.GenerateFromList(new List<double[]>() { res.ToArray() });
        }

        public override string ToString()
        {
            return InitInternalStringReturn(this);
        }
    }
}