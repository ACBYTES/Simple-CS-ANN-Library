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
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.DataVisualization.Charting;

namespace ANN
{
    class ExternalDataSetReader
    {
        public const char ARRAY_IDENTIFIER = '[';
        public const char ARRAY_CLOSER_IDENTIFIER = ']';
        public const char ARRAY_MEMBER_IDENTIFIER = '(';
        public const char ARRAY_MEMBER_CLOSER_IDENTIFIER = ')';
        public const string ARRAY_MEMBER_SPLITTER = "\n";
        public const char MEMBER_SPLITER = ',';

        public static Table ReadFromFile(string Path, char ArrayIdent = ARRAY_IDENTIFIER, char ArrayCloser = ARRAY_CLOSER_IDENTIFIER, char ArrayMemberIdent = ARRAY_MEMBER_IDENTIFIER, char ArrayMemberCloser = ARRAY_MEMBER_CLOSER_IDENTIFIER, char MemberSpliter = MEMBER_SPLITER, string ArrayMemberSplitter = ARRAY_MEMBER_SPLITTER)
        {
            string file = File.ReadAllText(Path);
            file = file.Replace(ArrayIdent.ToString(), "").Replace(ArrayCloser.ToString(), "");
            var splittedValues = file.Replace("\r", "").Split(new string[] { ArrayMemberSplitter }, StringSplitOptions.RemoveEmptyEntries);
            var list = new List<double[]>();
            foreach (var item in splittedValues)
            {
                var itemR = item.Replace(ArrayMemberIdent.ToString(), "").Replace(ArrayMemberCloser.ToString(), "").Split(new string[] { MemberSpliter.ToString() }, StringSplitOptions.RemoveEmptyEntries);
                var tempL = new List<double>();
                foreach (var res in itemR)
                {
                    tempL.Add(Convert.ToDouble(res.Replace(ArrayMemberIdent.ToString(), "").Replace(ArrayMemberCloser.ToString(), "")));
                }
                list.Add(tempL.ToArray());
            }
            return Table.GenerateFromList(list);
        }
    }
}
