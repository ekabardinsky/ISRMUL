using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISRMUL.Recognition.KMeansPlus
{
    class Euclidean2: IDistansion
    {

        public double Calculate(Vector one, Vector two)
        {
            double sum = 0;
            for (int i = 0; i < one.Value.Length; i++)
            {
                sum += Math.Pow(one[i] - two[i],2);
            }

            return sum;
        }
    }
}
