using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISRMUL.Recongnition.Neokognitron
{
    [Serializable]
    class Sinaps
    {
        
        public Neuron Left { get; set; }

        public double W { get; set; }

    }
}
