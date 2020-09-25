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
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANN
{
    class DataDrawer
    {
        static List<double> a { get; } = new List<double>();
        static List<double> XYMins { get; } = new List<double>();

        public static void Draw(Table XValues, Table YValues, int ResultWidth = 1920, int ResultHeight = 1080)
        {
            Bitmap b = new Bitmap(ResultWidth, ResultHeight);
            GenerateLocalValues(XValues.MaxFromAllForDrawing(), YValues.MaxFromAllForDrawing(), XValues.MinFromAllForDrawing(), YValues.MinFromAllForDrawing(), ResultWidth / 2, ResultHeight / 2);
            List<double[]> points = new List<double[]>();
            for (int i = 0; i < XValues.tableStructure.YValues.Count; i++)
            {
                points.Add(CalculateLocal(XValues.tableStructure.YValues[i][0], YValues.tableStructure.YValues[i][0]).ToArray());
            }

            using (Graphics g = Graphics.FromImage(b))
            {
                g.Clear(Color.White);
                foreach (var item in points)
                {
                    var x = float.Parse(item[0].ToString());
                    var y = float.Parse(item[1].ToString());
                    g.DrawString("⚪", new Font("Calibri", 10, FontStyle.Regular), new SolidBrush(Color.Red), new PointF(x, y));
                }
                g.DrawLine(new Pen(new SolidBrush(Color.Black)), new Point(ResultWidth / 2, ResultHeight / 2), new Point(ResultWidth / 2, 20));
                g.Save();
                g.Dispose();
            }
            b.Save(@"C:\Users\Alireza\Desktop\RES.png", System.Drawing.Imaging.ImageFormat.Png);
            b.Dispose();
        }

        private static void GenerateLocalValues(double XMax, double YMax, double XMin, double YMin, int TargetX, int TargetY)
        {
            //XCalc
            a.Add(TargetX - 0 / XMax - XMin);
            XYMins.Add(XMin);
            //y - y0 = a(x - x0)
            //y = a(x - x0) + y0
            // y = ax - aXMin
            a.Add(TargetY - 0 / YMax - YMin);
            XYMins.Add(YMin);
        }

        private static List<double> CalculateLocal(double XValue, double YValue)
        {
            List<double> l = new List<double>();
            l.Add((a[0] * XValue - a[0] * XYMins[0]));
            l.Add((a[1] * YValue - a[1] * XYMins[1]));
            return l;
        }
    }
}
