using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ISRMUL.Recognition.Neokognitron
{
    [Serializable]
    public class S : Plane
    {
        public double[][][] SeedW;
        public List<C> PrevC;
    }
    [Serializable]
    public class SInterploating : S
    {
        public Clazz Clazz {get;set;}
    }
}
