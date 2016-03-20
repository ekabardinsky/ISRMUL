using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISRMUL.Recognition.KMeansPlus
{
    class KMeans
    {
        public IDistansion DX;
        public List<Cluster> Clusters { get; set; }
        public List<Vector> Vectors { get; set; }
        public int VectorSize { get; set; }
        public int K { get; set;}
        public int Iteration { get { return Clusters.Max(x => x.ChangedCount); } }
        public Random r = new Random();
        public KMeans(int k, List<Vector> data, IDistansion DX)
        {
            Vectors = data;
            K = k;
            Clusters = new List<Cluster>();
            this.DX = DX;
        }

        public Vector GetFirsCentr()
        {
            return getCenter(Vectors);
        }
        public void InitializeCentroids()
        {
            for (int i = 0; i < K; i++)
            {
                Clusters.Add(new Cluster() { KMeans = this });
                Clusters[i].C = GetFirsCentr();
                List<double> dx = new List<double>();

                foreach (Vector v in Vectors)
                {
                    List<double> tmp = new List<double>();
                    foreach (Cluster c in Clusters)
                    {
                        tmp.Add(DX.Calculate(c.C, v));
                    }

                    double min = tmp.Min();
                    dx.Add(min * min);
                }

                double RND =r.NextDouble() * dx.Sum();

                double sum = 0;
                for (int j = 0; j < dx.Count && sum < RND; j++)
                {
                    sum += dx[j];
                    if (sum >= RND)
                    {
                        Clusters[i].C = Vectors[j];
                    }
                }
            }
        }

        public void CalculateCentroid()
        {
            for (int i = Clusters.Count - 1; i >= 0; i--)
            {
                Cluster c = Clusters[i];
                if (c.Vectors.Count == 0)
                    Clusters.Remove(c);
                else
                    c.C = getCenter(c.Vectors);

            }
        }
        public void ReCheckVectors()
        {
            //clear clusters
            foreach (Cluster c in Clusters)
                c.Vectors.Clear();
            //re check
            foreach (Vector v in Vectors)
            {
                List<double> dist = new List<double>();
                foreach(Cluster c in Clusters)
                    dist.Add(DX.Calculate(c.C,v)); 

                int ind = dist.IndexOf(dist.Min());
                v.Cluster = Clusters[ind];
                Clusters[ind].Vectors.Add(v);
            }
        }

        public static Vector getCenter(List<Vector> v)
        {
            int size = v[0].Value.Length;
            Vector C = new Vector(v[0].Value.Count());
            for (int i = 0; i < v.Count; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    C[j] += v[i][j];
                }

            }

            for (int i = 0; i < size; i++)
                C[i] /= v.Count;

            return C;
        }

        public void Proccess(int max)
        {
            int lastSum = 0;
            int sum = 0;
            for (int i = 0; i < max; i++)
            {
                lastSum = sum;
                
                ReCheckVectors();
                CalculateCentroid();

                sum = Clusters.Sum(x => x.ChangedCount);
                if (sum == lastSum)
                    return;
            }
        }

        public void RGBClustersInitialize()
        {
            double w = Vectors.Max(x => x[0]);
            double h = Vectors.Max(x => x[1]);
            double i = Vectors.Max(x => x[2]);
            double mo = Vectors.Sum(x => x[2]) / Vectors.Count;
            Clusters.Add(new Cluster() { KMeans = this, C = new Vector(3) { Value = new double[] { 0, 0, 0 } } });
            Clusters.Add(new Cluster() { KMeans = this, C = new Vector(3) { Value = new double[] { w , h , i } } });
            Clusters.Add(new Cluster() { KMeans = this, C = new Vector(3) { Value = new double[] { w / 2, h / 2, mo } } });
        }
        //public void RGBClustersInitialize()
        //{
        //    double rMax = Vectors.Max(x => x[0]);
        //    double gMax = Vectors.Max(x => x[1]);
        //    double bMax = Vectors.Max(x => x[2]);
        //    double rMin = Vectors.Min(x => x[0]);
        //    double gMin = Vectors.Min(x => x[1]);
        //    double bMin = Vectors.Min(x => x[2]);
        //    double mr = (rMax - rMin);
        //    double mg = (gMax - gMin);
        //    double mb = (bMax - bMin);
        //    int l =K-2;
        //    for( int i = 0; i < l;i++)
        //    {
        //        Clusters.Add(new Cluster() { KMeans = this, C = new Vector(3) { Value = new double[] { mr / l * i, mg / l * i, mb / l * i } } });
        //    }
        //    Clusters.Add(new Cluster() { KMeans = this, C = new Vector(3) { Value = new double[] { rMax , gMax , bMax } } });
        //    Clusters.Add(new Cluster() { KMeans = this, C = new Vector(3) { Value = new double[] { rMin , gMin , bMin } } });
        //}
    }
}
