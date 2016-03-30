using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISRMUL.Recognition.Neokognitron
{
    class WTAIntermadiateTrainer
    {
        static Random random = new Random();
        List<double[,]> trainData { get; set; }
        double Dr { get; set; }
        double LThresh { get; set; }
        double RThresh { get; set; }
        double[][] CPrevWeight { get; set; }
        double[][] CWeight { get; set; }
        double[][] DWeight { get; set; }
        double q {get;set;}
        int A { get { return CPrevWeight.Length / 2; } }
        int r { get { return (int)Math.Floor(Dr); } }
        int Height { get { return prevC[0].Neurons.GetLength(0); } }
        int Width { get { return prevC[0].Neurons.GetLength(1); } }
        List<C> prevC { get { return neo.U[layer - 1].C; } }
        List<S> newlySPlanes = new List<S>();
        NeoKognitron neo { get; set; }
        int layer { get; set; }
        double[,] inputPlane;
        double SeedThreshE { get; set; }
        Logger Logger { get; set; }
        bool Stop = false;
        public WTAIntermadiateTrainer(double q,List<double[,]> trainData, int layer, double Dr, NeoKognitron neo, double LThresh, double RThresh, double[][] CPrevWeight, double[][] CWeight, double[][] DWeight, Logger logger)
        {
            this.q = q;
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
            inputPlane = new double[Height, Width];
        }

        void computeOutInPlane()
        {
            double thresh = getSeedThreshold();
            for (int y = r; y < Height - r; y++)
            {
                for (int x = r; x < Width - r; x++)
                {
                    double sum = 0;
                    for (int c = 0; c < prevC.Count; c++)
                    {
                        sum += prevC[c].Neurons[y, x].getOut(1);
                    }
                    sum -= thresh;
                    //sum = prevC.Max(z => z.Neurons[y, x].getOut(1));
                    inputPlane[y, x] = sum < 0 ? 0 : sum;
                }
            }
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
                        sum += prevC[c].Neurons[y, x].getOut(1);
                    }
                    if (sum > max) max = sum;
                }
            }

            return max * SeedThreshE;
        }
        bool isNewlySPlanesSilent(int seedY, int seedX)
        {

            //double silentThreshold = 0.000255;
            double silentThreshold = (1 - RThresh) * neo.U[layer - 1].V.Neurons[seedY, seedX].getOut(1);
            //maxS = Math.Max(silentThreshold, maxS); 
            foreach (S s in newlySPlanes)
            {

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
        bool isWinner(int y, int x, int n, double output)
        {
            for (int c = 0; c < newlySPlanes.Count; c++)
            {
                for (int dy = -r; dy <= r; dy++)
                {
                    for (int dx = -r; dx <= r; dx++)
                    {
                        if (dx != 0 || dy != 0 || c != n)
                        {
                            double radius = Math.Sqrt(dx * dx + dy * dy);
                            if (radius < Dr && newlySPlanes[c].Neurons[y + dy, x + dx].getOut(1) > output)
                                return false;
                        }
                    }
                }
            }
            return true;
        }
        List<point3d> getWinners()
        {
            Neuron[, ,] neurons = new Neuron[newlySPlanes.Count, Height, Width];
            for (int y = A+1; y < Height - A-1; y++)
            {
                for (int x = A+1; x < Width - A-1; x++)
                {
                    for (int n = 0; n < newlySPlanes.Count; n++)
                    {
                        double output = newlySPlanes[n].Neurons[y, x].getOut(1);
                        //is winner?
                        neurons[n, y, x] = isWinner(y, x, n, output) ? newlySPlanes[n].Neurons[y, x] : null;
                    }
                }
            }

            List<point3d> winner = new List<point3d>();
            for (int i = 0; i < neurons.GetLength(0); i++)
            {
                winner.Add(null);
                for (int y = A+1; y < neurons.GetLength(1)-A-1; y++)
                {
                    for (int x = A+1; x < neurons.GetLength(2)-A-1; x++)
                    {
                        if (winner[i] == null && neurons[i, y, x] != null)
                            winner[i] = new point3d() { l = i, y = y, x = x };

                        if (winner[i] != null && neurons[i, y, x] != null && neurons[i, y, x].getOut(1) > neurons[winner[i].l, winner[i].y, winner[i].x].getOut(1))
                            winner[i] = new point3d() { l = i, y = y, x = x };
                    }
                }
            }

            return winner.Where(x => x != null).ToList();
        }
        void adoptateWinners(List<point3d> winners) 
        {
            foreach (point3d point in winners)
            {
                SCell winner = newlySPlanes[point.l].Neurons[point.y, point.x] as SCell;

                for (int c = 0; c < prevC.Count; c++)
                {
                    for (int y = 0; y < CPrevWeight.Length; y++)
                    {
                        for (int x = 0; x < CPrevWeight[y].Length; x++)
                        {
                            double u = prevC[c].Neurons[winner.Y + y - A, winner.X + x - A].getOut(1);
                            int index = (CPrevWeight.Length * CPrevWeight[y].Length * c) + CPrevWeight[y].Length * y + x;
                            newlySPlanes[point.l].Neurons[winner.Y,winner.X].Lefts[index].W += q * CPrevWeight[y][x] * u;
                        }
                    }
                }
                //replace a weigth
                for (int y = A+1; y < newlySPlanes[point.l].Neurons.GetLength(0)-A-1; y++)
                {
                    for (int x = A+1; x < newlySPlanes[point.l].Neurons.GetLength(1)-A-1; x++)
                    {
                        for (int s = 0; s < newlySPlanes[point.l].Neurons[point.y, point.x].Lefts.Count; s++)
                        {
                            newlySPlanes[point.l].Neurons[y, x].Lefts[s].W = newlySPlanes[point.l].Neurons[point.y, point.x].Lefts[s].W;
                        }
                    }
                }
                double b = getBWeigth(point.y, point.x);
                //replace b weigth to winners
                for (int y = A + 1; y < newlySPlanes[point.l].Neurons.GetLength(0) - A - 1; y++)
                {
                    for (int x = A + 1; x < newlySPlanes[point.l].Neurons.GetLength(1) - A - 1; x++)
                    {
                        (newlySPlanes[point.l].Neurons[y, x] as SCell).V.W = b;
                    }
                }
            }
        }
        double[][][] generateNewlyW(int seedY, int seedX,double b) 
        {
           
            double[][][] W = new double[prevC.Count][][];
            for (int c = 0; c < prevC.Count; c++)
            {
                W[c] = getWeigthFromIndexesAndCplane(seedY, seedX, c, b);
            }
            return W;
        }
        double[][] getWeigthFromIndexesAndCplane(int seedY, int seedX, int planeC, double b)
        {
            double[][] W = new double[2 * A + 1][];
            for (int y = -A; y <= A; y++)
            {
                W[A + y] = new double[2 * A + 1];
                for (int x = -A; x <= A; x++)
                {
                    if (seedY + y >= 0 && seedX + x >= 0 && seedY + y < Height && seedX + x < Width)
                    {
                        W[A + y][A + x] = prevC[planeC].Neurons[seedY + y, seedX + x].getOut(1) * CPrevWeight[A + y][A + x] / b;
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
        double[][][] randomW(double a,double b)
        {
            double[][][] W = new double[prevC.Count][][];
            for (int c = 0; c < prevC.Count; c++)
            {
                W[c] = new double[2 * A + 1][];
                for (int y = -A; y <= A; y++)
                {
                    W[c][A + y] = new double[2 * A + 1];
                    for (int x = -A; x <= A; x++)
                    {
                        W[c][A + y][A + x] = WTAIntermadiateTrainer.random.NextDouble() * a + b; 
                    }
                }
            }


            return W;
        }
        S generateNewSPlane(int seedY, int seedX)
        {
            double b = getBWeigth(seedY, seedX);
            double[][][] W = generateNewlyW(seedY, seedX, b);
            S s = new S() { U = neo.U[layer], PrevC = prevC, SeedW = W };
            s.Neurons = new Neuron[Height, Width];
            V v = neo.U[layer - 1].V;
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    SCell cell = new SCell(v.Neurons[y, x] as Inhibitor, b) { Plane = s, Y = y, X = x };
                    if (x > A && y > A && x < Width - A && y < Height - A)
                    {
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
                    }
                    s.Neurons[y, x] = cell;
                }
            }

            return s;
        }
        S generateRandomSPlane()
        {
            double b = random.NextDouble();
            double[][][] W = randomW(0.3, 0.0001);
            S s = new S() { U = neo.U[layer], PrevC = prevC, SeedW = W };
            s.Neurons = new Neuron[Height, Width];
            V v = neo.U[layer - 1].V;
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    SCellV2 cell = new SCellV2(v.Neurons[y, x] as Inhibitor, b) { Plane = s, Y = y, X = x }; 
                    s.Neurons[y, x] = cell;
                }
            }

            return s;
        }
        public void Train()
        {
            Stop = false;
            neo.U.Add(new U() { NeoKognitron = neo, Selectivity = LThresh });
            //for (int i = 0; i < 5; i++)
            //{
            //    newlySPlanes.Add(generateRandomSPlane());
            //}
            for (int p = 0; p < trainData.Count; p++)
            {
                Logger("Pattern " + p + " of " + trainData.Count + "\ngenerated " + newlySPlanes.Count, "input pattern");
                bool loop = false;



                neo.clearOperation();
                clearOperation();
                neo.input(trainData[p]);
                computeOutInPlane();
                adoptateWinners(getWinners());
                Logger("Pattern " + p + " of " + trainData.Count + "\ngenerated " + newlySPlanes.Count, "adoptate newly");

                //add splanes
                Point g = new Point(-1, -1);
                double max = 0;
                for (int y = r; y < Height - r; y++)
                {
                    for (int x = r; x < Width - r; x++)
                    {
                        if (inputPlane[y, x] > 0 && inputPlane[y, x] > max && isNewlySPlanesSilent(y, x))
                        {
                            g = new Point(x, y);
                            max = inputPlane[y, x];
                        }
                    }
                }
                if (max > 0)
                    newlySPlanes.Add(generateNewSPlane(g.Y, g.X));
                else
                    loop = false;
                Logger("Pattern " + p + " of " + trainData.Count + "\ngenerated " + newlySPlanes.Count, "generated plane ");
                if (Stop)
                    goto end;
            }
            end:
            neo.U[layer].S.AddRange(newlySPlanes);
            NeoKognitron.CConnectToS(neo.U[layer], DWeight);
            NeoKognitron.VConnectToC(neo.U[layer], CWeight);
        }
        void clearOperation()
        {
            foreach (S s in newlySPlanes)
            {
                s.clearOperation();
            }
        }


        internal void stop()
        {
            Stop = true;
        }
    }

    class point3d
    {
        public int l;
        public int y;
        public int x;
    }
}
