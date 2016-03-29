using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISRMUL.Recongnition.Neokognitron
{
    public delegate void Logger(string level,string log);
    class AddIfIntermediateTrainer
    {
        List<double[,]> trainData { get; set; }
        double Dr { get; set; }
        double LThresh { get; set; }
        double RThresh { get; set; }
        double As { get; set; }
        int A { get { return CPrevWeight.Length/2; } }
        double[][] CPrevWeight { get; set; }
        double[][] CWeight { get; set; }
        double[][] DWeight { get; set; }
        int r { get { return (int)Math.Floor(Dr); } }
        int Height { get { return prevC[0].Neurons.GetLength(0); } }
        int Width { get { return prevC[0].Neurons.GetLength(1); } }
        NeoKognitron neo { get; set; }
        int layer { get; set; }
        List<C> prevC { get { return neo.U[layer - 1].C; } }
        List<S> newlySPlanes = new List<S>();
        double[,] seedPlane;
        double SeedThreshE { get; set; }
        Logger Logger {get;set;}
        public AddIfIntermediateTrainer(List<double[,]> trainData, int layer, double Dr, NeoKognitron neo, double LThresh, double RThresh, double[][] CPrevWeight,double[][] CWeight, double[][] DWeight, Logger logger)
        {
            Logger = logger;
            this.trainData = trainData;
            this.layer = layer;
            this.Dr = Dr;
            this.CPrevWeight = CPrevWeight;
            this.DWeight = DWeight;
            this.neo = neo;
            this.LThresh = LThresh;
            this.RThresh = RThresh;
            this.CWeight = CWeight;
            SeedThreshE = 0.3;
            seedPlane = new double[Height,Width];
        }
        void computeOutSeedPlane()
        {
            //double thresh = getSeedThreshold();
            for (int y = r; y < Height-r; y++)
            {
                for (int x = r; x < Width-r; x++)
                {
                    double sum = 0; 
                    //for( int c = 0;c < prevC.Count;c++)
                    //{
                    //    sum += prevC[c].Neurons[y, x].getOut(1);
                    //}
                    //sum -= thresh;
                    sum = prevC.Max(z => z.Neurons[y, x].getOut(1));
                    seedPlane[y, x] = sum < 0 ? 0 : sum;
                } 
            } 
        }
        void suppresedToZero()
        {
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    if (!isNewlySPlanesSilent(y, x))
                        seedPlane[y, x] = 0;
                    else if (seedPlane[y, x] > 0)
                        y+=0;
                }
            }
             
            foreach (S s in newlySPlanes)
            {
                for (int y = 0; y < Height; y++)
                {
                    for (int x = 0; x < Width; x++)
                    {
                        maxV = Math.Max(neo.U[layer - 1].V.Neurons[y, x].getOut(1), maxV);
                        maxI = Math.Max(s.Neurons[y, x].getOut(1), maxI);
                    }
                }
            }
        }
        double maxS = 0;
        double maxV = 0;
        double maxI = 0;
        bool isNewlySPlanesSilent(int seedY, int seedX)
        {
            double v = 0;
            double min = 0;
            for (int i = 0; i < newlySPlanes.Count; i++)
            {
                for (int y = 0; y < newlySPlanes[i].Neurons.GetLength(0); y++)
                {
                    for (int x = 0; x < newlySPlanes[i].Neurons.GetLength(1); x++)
                    {
                        double o = newlySPlanes[i].Neurons[y, x].getOut(1);
                        v=Math.Max(o,v);
                        if (o > 0)
                            min = Math.Min(o, min);
                    }
                }
            }
           // if (newlySPlanes.Count>0) v /= newlySPlanes[0].Neurons.Length * newlySPlanes.Count;
            foreach (S s in newlySPlanes)
            {

                double silentThreshold = (1- RThresh)*neo.U[layer-1].V.Neurons[seedY,seedX].getOut(1);
                maxS = Math.Max(silentThreshold, maxS); 
                for (int dy = -r; dy <= r; dy++)
                {
                    for (int dx = -r; dx <= r; dx++)
                    {
                        double rad = Math.Sqrt(dx * dx + dy * dy);
                        if (seedX + dx >= 0 && seedY + dy >= 0 && seedY + dy < Height && seedX + dx < Width && rad < Dr)
                        {
                            double o = s.Neurons[seedY + dy, seedX + dx].getOut(1);
                            if (o > silentThreshold)
                                return false;
                        }
                    }
                }
            }
            return true;
        }
        double getSeedThreshold()
        {
            double max = double.MinValue;
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    double sum = 0; 
                    for (int c = 0; c < prevC.Count; c++)
                    {
                        sum+=prevC[c].Neurons[y, x].getOut(1);
                    }
                    if (sum > max) max = sum; 
                }
            }

            return max * SeedThreshE;
        }
        bool isNotCompletelyZero()
        {
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    if (seedPlane[y, x] > 0) return true;
                }
            }

            return false;
        }
        int[] maxSeedCellResponseIndexes()
        {
            double max = double.MinValue;
            int[] ind = { -1, -1 };
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    if (seedPlane[y, x] > max)
                    {
                        max = seedPlane[y, x];
                        ind[0] = y;
                        ind[1] = x;
                    }
                }
            }
            return ind;
        }
        int maxSeedPlaneIndex(int seedY, int seedX)
        {
            int ind = -1;
            double max = double.MinValue;
            for (int c = 0; c < prevC.Count; c++)
            {
                double value = prevC[c].Neurons[seedY, seedX].getOut(1);
                if (max < value)
                {
                    max = value;
                    ind = c;
                }
            }


            return ind;
        }
        double[][][] getWeigthFromIndexes(int[] indexes, double b)
        {
            double[][][] W = new double[prevC.Count][][]; 
            for (int c = 0; c < prevC.Count; c++)
            {
                W[c] = getWeigthFromIndexesAndCplane(indexes[0], indexes[1], c, b);
            }
            return W;
        }
        double[][] getWeigthFromIndexesAndCplane(int seedY, int seedX, int planeC,double b)
        {
            double[][] W = new double[2*A+1][];
            for (int y = -A; y <= A; y++)
            {
                W[A+y]=new double[2*A+1];
                for (int x = -A; x <= A; x++)
                {
                    if (seedY + y >= 0 && seedX + x >= 0 && seedY + y < Height && seedX + x < Width)
                    {
                        W[A + y][A + x] = prevC[planeC].Neurons[seedY + y, seedX + x].getOut(1) * CPrevWeight[A + y][A + x]/b;
                    }
                }
            }

            return W;
        }
        double getBWeigth(int seedY, int seedX)
        {
            double b = 0;
            for (int c = 0; c < prevC.Count; c++)
            {
                for (int y = -A; y <= A; y++)
                {
                    for (int x = -A; x <= A; x++)
                    {
                        if (seedY + y >= 0 && seedX + x >= 0 && seedY + y < Height && seedX + x < Width)
                        {
                            double value = prevC[c].Neurons[seedY + y, seedX + x].getOut(1);
                            b += CPrevWeight[A + y][A + x] * value * value;
                        }
                    }
                }

            }

            return Math.Sqrt(b);
        }
        public void Train()
        {
            neo.U.Add(new U() { NeoKognitron=neo,Selectivity=LThresh});
            for (int p = 0; p < trainData.Count; p++)
            {
                bool loop =true; 
                neo.clearOperation();
                clearOperation();
                neo.input(trainData[p]);
                computeOutSeedPlane();

                while (loop)
                { 

                    Logger("Pattern " + p + " of " + trainData.Count + "\ngenerated " + newlySPlanes.Count, "input pattern\nmaxSilent=" + maxS + "\nmaxV=" + maxV + "\nmaxI" + maxI);
                   
                    suppresedToZero();
                    Logger("Pattern " + p + " of " + trainData.Count+"\ngenerated "+newlySPlanes.Count, "input pattern\nmaxSilent="+maxS+"\nmaxV="+maxV+"\nmaxI"+maxI);
                    if (isNotCompletelyZero())
                    {
                        int[] indexes = maxSeedCellResponseIndexes();
                        int maxCPlane = maxSeedPlaneIndex(indexes[0], indexes[1]);
                        double b = getBWeigth(indexes[0], indexes[1]);

                        double[][][] W = getWeigthFromIndexes(indexes, b);

                        newlySPlanes.Add(generateSPlane(W, b));
                    }
                    else loop = false;
                }
            }

            neo.U[layer].S.AddRange(newlySPlanes);
            NeoKognitron.CConnectToS(neo.U[layer], DWeight);
            NeoKognitron.VConnectToC(neo.U[layer], CWeight);
        }
        S generateSPlane(double[][][] W, double b)
        {
            S s = new S() { U = neo.U[layer] };
            s.Neurons = new Neuron[Height, Width];
            V v = neo.U[layer-1].V;
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    SCell cell = new SCell(v.Neurons[y, x] as Inhibitor, b) { Plane=s };

                    for (int dy = -A; dy <= A; dy++)
                    {
                        for (int dx = -A; dx <= A; dx++)
                        {
                            if (y + dy >= 0 && x + dx >= 0 && y + dy < Height && x + dx < Width)
                            {
                                for (int c = 0; c < prevC.Count; c++)
                                {
                                    cell.leftConnect(prevC[c].Neurons[y + dy, x + dx], W[c][A + dy][A + dx]);
                                }
                            }
                        }
                    }
                    s.Neurons[y, x] = cell;
                }
            }

            return s;
        }
        void clearOperation()
        {
            foreach (S s in newlySPlanes)
            {
                s.clearOperation();
            }
        }
    }
}
