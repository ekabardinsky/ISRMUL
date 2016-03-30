using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISRMUL.Recognition.KMeansPlus
{
    class Vector
    { 

        public Vector(int count,double[] value)
        {
            Value = new double[count];
            Original = new double[count];

            for (int i = 0; i < count; i++)
                Value[i] = Original[i] = value[i];
        }

        public Vector(int count)
        {
            Value = new double[count];
            Original = new double[count];
        }
        public double[] Value { get; set; }
        public double[] Original { get; set; }

        public Cluster Cluster { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public double this[int i]
        {
            get { return Value[i]; }
            set { Value[i] = value; }
        }
    }
}
