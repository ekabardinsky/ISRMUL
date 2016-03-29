using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISRMUL.Recongnition.Neokognitron
{
    [Serializable]
    class Plane
    { 
        public U U { get; set; }
        public Neuron[,] Neurons { get; set; }
        public void clearOperation ()
        {
            if (Neurons == null) return;
            foreach (Neuron n in Neurons)
               if(n!=null) n.clearOperation();
        }
    } 
}
