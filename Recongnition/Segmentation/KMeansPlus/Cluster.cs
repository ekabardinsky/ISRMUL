using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISRMUL.Recognition.KMeansPlus
{
    class Cluster
    {
        Vector _C;
        public int ChangedCount = 0;
        public KMeans KMeans { get; set; }
        public List<Vector> Vectors = new List<Vector>();
        public Vector C
        {
            get { return _C; }
            set
            {
                if (_C == null)
                {
                    _C = value;
                    ChangedCount++;
                }
                else if ((Math.Abs(_C.Value[0] - value.Value[0]) > 0.01 || Math.Abs(_C.Value[1] - value.Value[1]) > 0.01))
                {
                    ChangedCount++;
                } 
                _C = value;
                
            }
        }
    }
}
