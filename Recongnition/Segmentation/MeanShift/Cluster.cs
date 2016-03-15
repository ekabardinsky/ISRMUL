using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISRMUL.Recognition.MeanShift
{
    public class Cluster
    {
        public List<Point> Points { get; set; }
        public Point C { get; set; }
        public Cluster()
        {
            Points = new List<Point>();
        }
        public bool IsIn(Point p, double e)
        {
            return Distance(p.Value, C.Value) <= e;
        }

        double Distance(double[] d, double[] d1)
        {
            double sum = 0;

            for (int i = 0; i < d.Length; i++)
            {
                sum += Math.Pow(d[i] - d1[i], 2);
            }

            return Math.Sqrt(sum);
        }
    }
}
