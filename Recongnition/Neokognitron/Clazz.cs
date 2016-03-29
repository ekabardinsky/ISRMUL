using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISRMUL.Recongnition.Neokognitron
{
    [Serializable]
    class Clazz
    {
        public List<Vector> ReferenceVectors { get; set; }
        public string Name { get; set; }

        public Clazz(string name) 
        { 
            ReferenceVectors = new List<Vector>();
            this.Name = name;
        }

        public bool isNotLearned()
        {
            return ReferenceVectors.Count < 2;
        }
        public void AddReferenceVector(Vector vector)
        {
            ReferenceVectors.Add(vector);
        }
        public double Compute(Vector pattern)
        {
            double maxS = 0;

            for (int i = 0; i < ReferenceVectors.Count-1; i++)
            {
                for (int j = i + 1; j < ReferenceVectors.Count; j++)
                {
                    Vector one = ReferenceVectors[i];
                    Vector two = ReferenceVectors[j];
                    Vector interploating = getInterploatingVector(one,two,pattern);
                    double s = getSimilary(pattern, interploating);
                    if (s > maxS) maxS = s;
                }
            }

            return maxS;
        }
        double getSimilary(Vector one, Vector two)
        {
            return (one * two) / (one.Module() * two.Module());
        }
        private Vector getInterploatingVector(Vector one, Vector two, Vector pattern)
        {
            double Si = getSimilary(one, pattern);
            double Sj = getSimilary(two, pattern);
            double Sij = getSimilary(one, two);
            double Pi = (Si - Sj * Sij) / ((Si + Sj) * (1 - Sij));
            double Pj = (Sj - Si * Sij) / ((Si + Sj) * (1 - Sij));
            double moduleI = one.Module();
            double moduleJ = two.Module();

            double[] interploating = new double[one.Length];

            for (int i = 0; i < interploating.Length; i++)
            {
                interploating[i] = Pi / moduleI * one[i] + Pj / moduleJ * two[i];
            }

            return new Vector(interploating);
        }
    }
}
