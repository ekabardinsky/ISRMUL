using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISRMUL.Recognition.KMeansPlus
{
    class Euclidean : IDistansion
    {
        public double Calculate(Vector one, Vector two)
        {
            double sum = 0;
            for (int i = 0; i < one.Value.Length; i++)
            {
                sum += (one[i] - two[i]) * (one[i] - two[i]);
            }

            return Math.Sqrt(sum);
        }
    }
}
