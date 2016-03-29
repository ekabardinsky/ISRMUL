using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISRMUL.Recongnition.Neokognitron
{
    [Serializable]
    abstract class Neuron
    {
        [NonSerialized]
        protected int operationId;
        public List<Sinaps> Lefts { get; set; }

        public U U { get { return Plane.U; } }
        public Plane Plane { get; set; }

        [NonSerialized]
        protected double CachedOut;

        public double getOut(int operation)
        {
            if (operation == 0) throw new InvalidOperationException();
            if (operation != operationId)
            { CachedOut = calculateOut(operation); operationId = operation; }

            return CachedOut;
        }
        public void leftConnect(Neuron n, double w)
        {
            Sinaps s = new Sinaps() { W = w, Left = n};
            this.Lefts.Add(s);
        }
        protected abstract double calculateOut(int operation);
        public abstract void clearOperation();
    }
    [Serializable]
    class CCell : Neuron
    {
        [NonSerialized]
        public int PreSinapsOperation;
        [NonSerialized]
        public double PreSinapsCach;
        public C C { get { return Plane as C; } }
        public double CalculatePreSinaps(int operation)
        {
            if (PreSinapsOperation == operation) return PreSinapsCach;
            double sum = 0;
            foreach (Sinaps s in Lefts)
            {
                double value = s.Left.getOut(operation);
                sum += value * value * s.W;
            }


            sum = sum < 0 ? 0 : sum;

            return PreSinapsCach = Math.Sqrt(sum);
        }

        public CCell()
        {
            Lefts = new List<Sinaps>();
        } 
        protected override double calculateOut(int operation) 
        {
            U.calculateMaxC(operation);

            //double sum = PreSinapsCach / (U.MaxC * C.q + PreSinapsCach);
            double sum = PreSinapsCach / (1 + PreSinapsCach);
           // double sum = PreSinapsCach / (U.MaxC);
            return sum;
        }

        public override void clearOperation()
        {
            PreSinapsOperation = 0;
            operationId = 0;
        }
    }
    [Serializable]
    class SCell : Neuron
    {

        public Sinaps V { get; set; }
        public int Y { get; set; }
        public int X { get; set; }
        public SCell(Inhibitor inhib, double w)
        {
            V = new Sinaps() { W = w, Left = inhib};
            Lefts = new List<Sinaps>();
        }
        //protected override double calculateOut(int operation)
        //{
        //    double sum = 0;
        //    foreach (Sinaps s in Lefts)
        //    {
        //        sum += s.Left.getOut(operation) * s.W;
        //    }
        //    sum += 1;
        //    double selectivity = U.Selectivity;
        //    double I = selectivity * V.Left.getOut(operation) * V.W+1;

        //    sum = sum / I-1;

        //    return (selectivity / (1 - selectivity)) * (sum < 0 ? 0 : sum);
        //}
        protected override double calculateOut(int operation)
        {
            //double sum = 0;
            //foreach (Sinaps s in Lefts)
            //{
            //    sum += s.Left.getOut(operation) * s.W;
            //}
            double[] sum = new double[Lefts.Count];
            Parallel.For(0, Lefts.Count, i =>
                {
                    sum[i] = Lefts[i].Left.getOut(operation) * Lefts[i].W;
                });

            double value = sum.Sum();
            //double value = sum;
            double selectivity = U.Selectivity;
            double I = selectivity * V.Left.getOut(operation) * V.W;

            value = value - I;

            return (1 / (1 - selectivity)) * (value < 0 ? 0 : value);
        }
        //protected override double calculateOut(int operation)
        //{
        //    S s = Plane as S;
        //    double sum = 0;
        //    for (int c = 0; c < s.PrevC.Count; c++)
        //    {
        //        int r = s.SeedW[c].Length/2;
        //        for (int y = -r; y <=r ; y++)
        //        {
        //            for (int x = -r; x <=r; x++)
        //            {
        //                if (Y + y > 0 & X + x > 0 && Y + y < s.PrevC[c].Neurons.GetLength(0) && X + x < s.PrevC[c].Neurons.GetLength(1))
        //                    sum += s.PrevC[c].Neurons[Y + y, X + x].getOut(1) * s.SeedW[c][r + y][r + x];
        //            }
        //        }
        //    }
        //    double selectivity = U.Selectivity;
        //    double I = selectivity * V.Left.getOut(operation) * V.W;

        //    sum = sum - I;

        //    return (1 / (1 - selectivity)) * (sum < 0 ? 0 : sum);
        //}


        public override void clearOperation()
        {
            operationId = 0;
        }
    }
    class SCellV2 : Neuron
    {

        public Sinaps V { get; set; }
        public int Y { get; set; }
        public int X { get; set; }
        public SCellV2(Inhibitor inhib, double w)
        {
            V = new Sinaps() { W = w, Left = inhib};
            Lefts = new List<Sinaps>();
        }
        protected override double calculateOut(int operation)
        {
            S s = Plane as S;
            int W = s.PrevC[0].Neurons.GetLength(1);
            int H = s.PrevC[0].Neurons.GetLength(0);
            double sum = 0;
            for (int c = 0; c < s.PrevC.Count; c++)
            {
                int r = s.SeedW[c].Length / 2;
                for (int y = -r; y <= r; y++)
                {
                    for (int x = -r; x <= r; x++)
                    {
                        if (Y + y > 0 & X + x > 0 && Y + y < H && X + x < W)
                            sum += s.PrevC[c].Neurons[Y + y, X + x].getOut(1) * s.SeedW[c][r + y][r + x];
                    }
                }
            }
            double selectivity = U.Selectivity;
            double I = selectivity * V.Left.getOut(operation) * V.W;

            sum = sum - I;

            return (1 / (1 - selectivity)) * (sum < 0 ? 0 : sum);
        }


        public override void clearOperation()
        {
            operationId = 0;
        }
    }
    [Serializable]
    class GradientNeuron : Neuron
    {
        double[][] Weight { get; set; }
        public int k { get; set; }
        int RetinaX { get; set; }
        int RetinaY { get; set; }

        public GradientNeuron(int x, int y, int k, double[][] W)
        {
            this.k = k;
            RetinaX = x;
            RetinaY = y;
            Weight = W;
        }

        protected override double calculateOut(int operation)
        {
            return U.NeoKognitron.Retina[RetinaY, RetinaX];
            int marginY = Weight.Length / 2;
            int marginX = Weight[0].Length / 2;

            double sum = 0;

            for (int y = -marginY; y <= marginY; y++)
            {
                for (int x = -marginX; x <= marginX; x++)
                {

                    if ((RetinaY + y >= 0 && RetinaX + x >= 0) && (U.NeoKognitron.Retina.GetLength(0) > RetinaY+y && U.NeoKognitron.Retina.GetLength(1) > RetinaX + x))
                        sum += Weight[marginY + y][marginX + x] * U.NeoKognitron.Retina[RetinaY + y, RetinaX + x];
                }
            }
            sum = k == 0 ? sum : -sum;
            sum = sum < 0 ? 0 : sum;
            return sum;
        }

        public override void clearOperation()
        { 
            operationId = 0;
        }
    }
    [Serializable]
    class Inhibitor : Neuron
    {
        public Inhibitor()
        {
            Lefts = new List<Sinaps>();
        }
        protected override double calculateOut(int operation)
        {
            double sum = 0;
            foreach (Sinaps s in Lefts)
            {
                double o = s.Left.getOut(operation);
                sum +=  o*o*s.W;
            }

            return Math.Sqrt(sum);
        }

        public override void clearOperation()
        { 
            operationId = 0;
        }
    }
    [Serializable]
    class EdgeExtractor : Neuron
    {
        [NonSerialized]
        double PreSinapsCach;
        [NonSerialized]
        public int PreSinapsOperation;
        public int X {get;set;}
        public int Y { get; set; }

        public EdgeExtractor()
        {
            Lefts = new List<Sinaps>();
        }

        protected override double calculateOut(int operation)
        {

            double meOut = CalculatePreSinaps(operation);
            double otherOut = 0; 
            foreach (S s in U.S)
            {
                otherOut += (s.Neurons[Y, X] as EdgeExtractor).CalculatePreSinaps(operation);
            }
            
            meOut = 2*meOut - (2.0 / U.S.Count) * otherOut;

            return meOut < 0 ? 0 : meOut;
        }

        public double CalculatePreSinaps(int operation)
        {
            if (operation == PreSinapsOperation) return PreSinapsCach;
            double sum = 0;
            foreach (Sinaps s in Lefts)
            {
                sum += s.Left.getOut(operation) * s.W;
            }
            PreSinapsOperation = operation;
            PreSinapsCach = sum < 0 ? 0 : sum;
            return sum;
        }

        public override void clearOperation()
        {
            PreSinapsOperation = 0;
            operationId = 0;
        }
    }
    [Serializable]
    class SCellInteploating : Neuron
    {
        public string Name { get { return (Plane as SInterploating).Clazz.Name; } }
        
        public List<C> prevC { get { return (Plane as SInterploating).PrevC; } }
        int Height { get { return prevC[0].Neurons.GetLength(0); } }
        int Width { get { return prevC[0].Neurons.GetLength(1); } }

        protected override double calculateOut(int operation)
        {
            Vector v = getPattern(operation);
            Clazz clazz = (Plane as SInterploating).Clazz;

            return clazz.Compute(v);
        }

        public override void clearOperation()
        {
            operationId = 0;
        }
        Vector getPattern(int operation)
        {
            double[] pattern = new double[prevC.Count * Width * Height];
            int dimension = 0;
            foreach (C c in prevC)
                for (int y = 0; y < Height; y++)
                    for (int x = 0; x < Width; x++)
                        pattern[dimension++] = c.Neurons[y, x].getOut(operation);

            return new Vector(pattern);
        }
    }

}
