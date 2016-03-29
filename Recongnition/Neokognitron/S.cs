using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ISRMUL.Recongnition.Neokognitron
{
    [Serializable]
    class S:Plane
    {
        public double[][][] SeedW;
        public List<C> PrevC;
    }
    [Serializable]
    class SInterploating : S
    {
        public Clazz Clazz {get;set;}
    }
}
