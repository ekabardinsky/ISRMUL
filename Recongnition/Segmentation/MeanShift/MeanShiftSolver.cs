using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISRMUL.Recognition.MeanShift
{ 
    public class MeanShiftSolver
    {
        public double[] H { get; set; }
        public List<Point> Points { get; set; }

        public List<Cluster> Clusters { get; set; }

        public MeanShiftSolver(double[] h, List<Point> points)
        {
            H = h;
            Points = points;
        }

        double G(Point p, int i)
        {
            Point pi = Points[i];

            if (Math.Abs(pi.Value[0] - p.Value[0]) < H[0] && Math.Abs(pi.Value[1] - p.Value[1]) < H[1])
                return 1;
            else
                return 0;

            //if (IsLowest(Abs(Substract(p.Value, pi.Value))))
            //    return 1;
            //else 
            //    return 0;
        }
        double[] M(Point p)
        {
            double[] res = new double[p.Value.Length];
            double gi = 0;
            for (int i = 0; i < Points.Count; i++)//res == sum(pi*gi)
            {
                double g = G(p, i);
                if (g != 0)
                {
                    res = Add(res, ConstMultiply(Points[i].Value, g));
                    gi += g;
                }
            }

            res = ConstDivision(res, gi);

            res = Substract(res, p.Value);

            return res;


        }
        double[] ConstMultiply(double[] p1, double c)
        {
            return p1.Select(x => x * c).ToArray();
        }
        double[] ConstDivision(double[] p1, double c)
        {
            return p1.Select(x => x / c).ToArray();
        }
        double[] Substract(double[] p1, double[] p2)
        {
            double[] res = new double[p1.Length];

            for (int i = 0; i < p1.Length; i++)
                res[i] = p1[i] - p2[i];

            return res;
        }
        double[] Add(double[] p1, double[] p2)
        {
            double[] res = new double[p1.Length];

            for (int i = 0; i < p1.Length; i++)
                res[i] = p1[i] + p2[i];

            return res;
        }
        double[] Abs(double[] p1)
        {
            return p1.Select(x => Math.Abs(x)).ToArray();
        }
        bool IsLowest(double[] p1)
        {
            bool b = true;
            for (int i = 0; i < H.Length; i++)
            {
                b &= H[i] >= p1[i];
            }

            return b;
        }
        double Distance(double[] d)
        {
            return Math.Sqrt(d.Sum(x => x * x));
        }

        public double Shift()
        {
            double E = 0;
            double[][] ms = new double[Points.Count][];

            Parallel.For(0, Points.Count, (p) =>
                {
                    ms[p] = M(Points[p]); 
                });
            for (int p = 0; p < Points.Count; p++)
            {
                E += Distance(ms[p]);
                Points[p].Value = Add(Points[p].Value, ms[p]);
            }
            return E;
        }
        public void Compute(double e, int iteration)
        {
            double E = double.MaxValue;
            for (int t = 0; t < iteration&&E>e; t++)
            {
                E = Shift();
            }
        }

        public void Clustering(double maxDistance)
        {
            Clusters = new List<Cluster>();

            for (int p = 0; p < Points.Count; p++)
            {
                Cluster c = Clusters.Where(x => x.IsIn(Points[p], maxDistance)).FirstOrDefault();
                if (c != null)
                {
                    c.Points.Add(Points[p]);
                }
                else
                {
                    Cluster clust = new Cluster() { C = new Point(Points[p].Value) };
                    clust.Points.Add(Points[p]);
                    Clusters.Add(clust);
                }
            }
        }
    }
}
