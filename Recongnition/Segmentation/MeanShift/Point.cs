using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISRMUL.Recognition.MeanShift
{
    [Serializable]
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

        public override int GetHashCode()
        {
            int code = (int)(R * 1000000 + G * 1000 + B);
            return code;
        }
        public override bool Equals(object obj)
        {
            if (!(obj is Point))
                return false;
            return (obj as Point).GetHashCode() == GetHashCode();
        }
    }
}
