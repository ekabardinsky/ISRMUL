using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISRMUL.Recognition.Neokognitron
{
    [Serializable]
    public class Vector
    {
        double[] data;
        public int Length { get { return data.Length; } }
        public Vector(double[] Data)
        {
            data = Data;
        }

        public double this[int i]
        {
            get { return data[i]; }
            set { data[i] = value; }
        }

        public static double operator * (Vector one, Vector two)
        {
            double sum = 0;
            for (int i = 0; i < one.Length; i++)
            {
                sum += one[i] * two[i];
            }

            return sum;
        }

        public double Module()
        {
            double sum = 0;
            for (int i = 0; i < Length; i++)
                sum += data[i] * data[i];

            return Math.Sqrt(sum);
        }
    }
}
