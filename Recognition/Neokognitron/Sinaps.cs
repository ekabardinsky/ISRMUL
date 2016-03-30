using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISRMUL.Recognition.Neokognitron
{
    [Serializable]
    public class Sinaps
    {
        
        public Neuron Left { get; set; }

        public double W { get; set; }

    }
}
