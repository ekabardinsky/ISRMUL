using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISRMUL.Recongnition.Neokognitron
{
    class AddIfRule:Trainer
    {
        protected Logger Logger { get; set; }
        protected List<double[,]> trainData { get; set; }
        protected NeoKognitron neo { get; set; }
        protected int layer { get; set; }
        protected int Height { get { return prevC[0].Neurons.GetLength(0); } }
        protected int Width { get { return prevC[0].Neurons.GetLength(1); } }
        protected double[][] CPrevWeight { get; set; }
        protected int r { get { return (int)Math.Floor(Dr); } }
        protected double Dr { get; set; }
        protected List<C> prevC { get { return neo.U[layer - 1].C; } }
        protected List<S> newlySPlanes = new List<S>();
        protected double[][] virtualPlane { get; set; }
        protected double LThresh { get; set; }
        protected double RThresh { get; set; }
        protected double SeedThreshE { get; set; }
        protected double[][] DWeight { get; set; }
        protected double[][] CWeight { get; set; }
        protected bool stop { get; set; }
        public AddIfRule(List<double[,]> trainData, int layer, double Dr, NeoKognitron neo, double LThresh, double RThresh, double[][] CPrevWeight, double[][] CWeight, double[][] DWeight, Logger logger)
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
        }
        protected double maxV = 0;
        protected double maxT = 0;
        protected double maxO = 0;

        public override void Train()
        {
            stop = false;
            neo.U.Add(new U() { NeoKognitron = neo, Selectivity = LThresh });
            for (int p = 0; p < trainData.Count; p++)
            {
                Logger("pattern "+p, "newly "+newlySPlanes.Count);
                neo.clearOperation();
                clearOperation();
                neo.input(trainData[p]);

                for (int i = 0; i < prevC.Count; i++)
                {
                    for (int j = i; j < prevC.Count; j++)//look here, test i+1
                    {
                        if (stop)
                            goto end;
                        Logger("pattern " + p + "\nnewly " + newlySPlanes.Count + "(i=" + i + ",j=" + j + ")", "MaxT=" + maxT +"\nMaxO=" + maxO);
                        C one = prevC[i];
                        C two = prevC[j];
                        virtualPlane = getSplitedPlanes(one, two);
                        suppressToZero();
                        Point winner = getMaximun();
                        if (winner.X > 0)
                        {
                            newlySPlanes.Add(generateSPlane(i, j, winner.Y, winner.X));
                        }
                    }
                }
            }
            end:
            neo.U[layer].S.AddRange(newlySPlanes);
            NeoKognitron.CConnectToS(neo.U[layer], DWeight);
            NeoKognitron.VConnectToC(neo.U[layer], CWeight);
        }
        protected double[][] getSplitedPlanes(C one, C two)
        {
            //calc trhreshold & prepopagate
            int H = one.Neurons.GetLength(0);
            int W = one.Neurons.GetLength(1);
            double[][] virtualPlane = new double[H][];
            double thresh = 0;
            for (int y = 0; y < H; y++)
            {
                virtualPlane[y] = new double[W];
                for (int x = 0; x < W; x++)
                {
                    virtualPlane[y][x] = one.Neurons[y, x].getOut(1) + two.Neurons[y, x].getOut(1);
                    if (virtualPlane[y][x] > thresh)
                        thresh = virtualPlane[y][x];
                }
            }
            //post propagate
            for (int y = 0; y < H; y++)
            {
                for (int x = 0; x < W; x++)
                {
                    virtualPlane[y][x] = virtualPlane[y][x] - SeedThreshE * thresh;
                    if (virtualPlane[y][x] < 0)
                        virtualPlane[y][x] = 0;
                }
            }

            return virtualPlane;
        }
        protected void suppressToZero()
        {
            for (int y = 0; y < virtualPlane.Length; y++)
            {
                for( int x = 0 ; x < virtualPlane[y].Length;x++)
                {
                    if (!newlySPlanesAreSilent(y, x))
                        virtualPlane[y][x] = 0;
                }
            }
        }
        protected Point getMaximun()
        {
            Point winner = new Point(-1, -1);
            double max = 0;
            for (int y = r; y < virtualPlane.Length-r; y++)
            {
                for (int x = r; x < virtualPlane[y].Length-r; x++)
                {
                    if (virtualPlane[y][x] > max)
                    {
                        max = virtualPlane[y][x];
                        winner = new Point(x, y);
                    }
                }
            }

            return winner;
        }
        
        bool newlySPlanesAreSilent(int seedY, int seedX)
        {
            double thresh = (LThresh - RThresh) * (1 - RThresh) * neo.U[layer - 1].V.Neurons[seedY, seedX].getOut(1);
            maxT = Math.Max(thresh, maxT);
            foreach (S s in newlySPlanes)
            {
                for (int y = -r; y <= r; y++)
                {
                    for (int x = -r; x <= r; x++)
                    {
                        double rad = Math.Sqrt(x * x + y * y);
                        if (seedX + x >= 0 && seedY + y >= 0 && seedY + y < Height && seedX + x < Width && rad < Dr)
                        {
                            double o = s.Neurons[seedY + y, seedX + x].getOut(1);
                            maxO = Math.Max(o, maxO);
                            if (o > thresh)
                                return false;
                        }
                    }
                }
            }

            return true;
        }
        double[][][] getW(int one, int two, int seedY, int seedX, double b)
        {
            int A = CPrevWeight[0].Length/2;
            double[][][] W = new double[2][][];
            W[0] = new double[A * 2 + 1][];
            W[1] = new double[A * 2 + 1][];
            for (int y = -A; y <= A; y++)
            {
                W[0][A + y] = new double[2 * A + 1];
                W[1][A + y] = new double[2 * A + 1]; 
                for (int x = -A; x <= A; x++)
                {
                    if (y + seedY >= 0 && x + seedX >= 0 && y + seedY < Height && x + seedX < Width)
                    {
                        W[0][y + A][x + A] = prevC[one].Neurons[seedY + y, seedX + x].getOut(1) * CPrevWeight[y + A][x + A] / b;
                        W[1][y + A][x + A] = prevC[two].Neurons[seedY + y, seedX + x].getOut(1) * CPrevWeight[y + A][x + A] / b;
                    }
                }
            }

            return W;
        }
        double getB(int one, int two, int seedY, int seedX)
        {
            int A = CPrevWeight[0].Length / 2;
            double sum = 0;
            for (int y = -A; y <= A; y++)
            {
                for (int x = -A; x <= A; x++)
                {
                    if (seedY + y > 0 && seedX + x > 0 && seedY + y < Height && seedX + x < Width)
                    {
                        double o = prevC[one].Neurons[seedY + y, seedX + x].getOut(1);
                        sum += o * o * CPrevWeight[y + A][x + A];
                        o = prevC[two].Neurons[seedY + y, seedX + x].getOut(1);
                        sum += o * o * CPrevWeight[y + A][x + A];
                    }
                }
            }

            return Math.Sqrt(sum);
        }
        protected S generateSPlane(int one, int two, int seedY, int seedX)
        {
            double b = getB(one, two, seedY, seedX);
            double[][][] W = getW(one, two, seedY, seedX, b);
            C oneC = prevC[one];
            C twoC = prevC[two];
            int Height = oneC.Neurons.GetLength(0);
            int Width = oneC.Neurons.GetLength(1);
            S s = new S() { U = neo.U[layer] };
            s.Neurons = new Neuron[Height, Width];
            V v = neo.U[layer - 1].V;

            int A = W[0].Length / 2;
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    double rad = Math.Sqrt((seedX - x) * (seedX - x) + (seedY - y) * (seedY - y));
                    SCell cell = new SCell(v.Neurons[y, x] as Inhibitor, b) { Plane=s};
                    //if (rad <= Dr)
                   // {
                        for (int dy = -A; dy <= A; dy++)
                        {
                            for (int dx = -A; dx <= A; dx++)
                            {
                                if (y + dy >= 0 && x + dx >= 0 && y + dy < Height && x + dx < Width)
                                {
                                    cell.leftConnect(prevC[one].Neurons[y + dy, x + dx], W[0][A + dy][A + dx]);
                                    cell.leftConnect(prevC[two].Neurons[y + dy, x + dx], W[1][A + dy][A + dx]);
                                }
                            }
                        }
                 //   }
                    s.Neurons[y, x] = cell;
                }
            }

            return s;
        }
        protected void clearOperation()
        {
            foreach (S s in newlySPlanes)
            {
                s.clearOperation();
            }
        }

        double getV(int seedY, int seedX)
        {
            double sum = 0;
            int cnt = 0;
            foreach (S s in newlySPlanes)
            {
                int r = CWeight.Length/2;
                for (int y = -r; y <= r; y++)
                {
                    for (int x = -r; x <= r; x++)
                    {
                        if (x + seedX > 0 && y + seedY>0 && seedY + y < Height && seedX + x < Width)
                        {
                            double o = s.Neurons[y + seedY, x + seedX].getOut(1);
                            sum += o;
                            cnt++;
                        }
                    }
                }

            }

            return sum/cnt;
        }
       
        internal void stopTrain()
        {
            stop = true;
        }
    }
    
}
