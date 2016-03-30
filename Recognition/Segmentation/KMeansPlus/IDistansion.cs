using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISRMUL.Recognition.KMeansPlus
{
    interface IDistansion
    {
        double Calculate(Vector one, Vector two);
    }
}
