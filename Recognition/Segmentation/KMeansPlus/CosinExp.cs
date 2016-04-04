using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISRMUL.Recognition.KMeansPlus
{
    class CosinExp:IDistansion
    {
        public double Calculate(Vector one, Vector two)
        {
            double cosin = (one * two) / (one.Module() * two.Module());
            double degree = 1 / cosin;
            return Math.Exp(degree);
        }
    }
}
