using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISRMUL.Recognition.MeanShift
{
    public class Point
    {
        public double[] Value { get; set; }
        public double[] Original { get; set; }
        public double R { get; set; }
        public double G { get; set; }
        public double B { get; set; }
        public Point(double[] val)
        {
            Value = val;
            Original = val;
        }
    }
}
