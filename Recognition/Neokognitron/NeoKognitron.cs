using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace ISRMUL.Recognition.Neokognitron
{
    public enum IntermediateLayerTrainRule
    {
        WTA, WKL, AddIf
    }
    [Serializable]
    public class NeoKognitron
    {
        public List<U> U { get; set; }

        [NonSerialized]
        public double[,] Retina;

        public double[] Selectivity { get; set; }
        
        int[] LayersArch { get; set; }
        int Width { get; set; }
        int Height { get; set; }

        public NeoKognitron(int width, int height, int [] arch, double[] Selectivity)
        {
            LayersArch = arch;
            Width = width;
            Height = height;
            Retina = new double[Height, Width];
            this.Selectivity = Selectivity;
        }

        public void init()
        {
            U = new List<U>();
            gradLayerInit();
            edgeExtractionLayerInit();
           
        }
        public void input(double[][] data)
        {
            if(Retina==null)
                Retina = new double[Height, Width];

            for (int i = 0; i < Retina.GetLength(0); i++)
                for (int j = 0; j < Retina.GetLength(1); j++)
                    Retina[i, j] = data[i][j];
        }
        public void input(double[,] bmp)
        {
            if (Retina == null)
                Retina = new double[Height, Width];

            for (int i = 0; i < bmp.GetLength(0); i++)
            {
                for (int j = 0; j < bmp.GetLength(1); j++)
                {
                    Retina[i, j] = bmp[i, j];
                }
            }
        }
        public void clearOperation()
        {
            foreach (U u in U)
                u.clearOperation();
        }
        public string Compute(double[,] data, int operation)
        {
            input(data);
            double max = 0;
            int index = -1;
            for (int i = 0; i < U[4].S.Count; i++)
            {
                double o = U[4].S[i].Neurons[0, 0].getOut(operation);
                if (o > max)
                {
                    max = o;
                    index = i;
                }
            }

            return (U[4].S[index] as SInterploating).Clazz.Name;
        }
        public string Compute(double[,] data)
        {
            clearOperation();
            input(data);
            double max = 0;
            int index = -1;
            for (int i = 0; i < U[4].S.Count; i++)
            {
                double o = U[4].S[i].Neurons[0, 0].getOut(1);
                if (o > max)
                {
                    max = o;
                    index = i;
                }
            }

            return (U[4].S[index] as SInterploating).Clazz.Name;
        }
        void gradLayerInit()
        {
            U.Add(new U());
            U[0].NeoKognitron = this;
            U[0].Name = "U0G";
            int marginY = UgKernelWeight.Length / 2;
            int marginX = UgKernelWeight[0].Length / 2;
            int i = 0;
            //gradient layer 
           // for ( i = 0; i < 2; i++)
          //  {

            C tmpC = new C() { U = U[0] };
                tmpC.Neurons = new Neuron[Retina.GetLength(0), Retina.GetLength(1)];
                for (int y = 0; y < Retina.GetLength(0); y++)
                {
                    for (int x = 0; x < Retina.GetLength(1); x++)
                    {
                        GradientNeuron ex = new GradientNeuron(x, y, i, UgKernelWeight) { Plane = tmpC };
                        tmpC.Neurons[y, x] = ex;
                    }
                }
                U[0].C.Add(tmpC);
        //    }
        }
        void edgeExtractionLayerInit()
        {
            //edge extractor layer
            int featuresCount = LayersArch[0];
            U.Add(new U());
            U[1].Selectivity = Selectivity[0];
            U[1].NeoKognitron = this;
            U[1].Name = "U1";
            //s1 planes
            double[][][] W = GenerateEdgesWeight(featuresCount, 0.05, 2);
            //double[][][] W = EdgesWeight(8);

            int marginY = W[0].Length / 2;
            int marginX = W[0][0].Length / 2;
            int prevH = U[0].C[0].Neurons.GetLength(0);
            int prevW = U[0].C[0].Neurons.GetLength(1);

            for (int f = 0; f < featuresCount; f++)
            {
                S tmpS = new S() { U = U[1] };
                tmpS.Neurons = new Neuron[prevH, prevW];
                for (int y = 0; y < prevH; y++)
                {
                    for (int x = 0; x < prevW; x++)
                    {
                        EdgeExtractor ex = new EdgeExtractor() { X = x, Y = y, Plane = tmpS };
                        tmpS.Neurons[y, x] = ex;

                        for (int i = -marginY; i <= marginY; i++)
                        {
                            for (int j = -marginX; j <= marginX; j++)
                            {
                                double w = W[f][marginY + i][marginX + j];
                                if (y + i >= 0 && x + j >= 0 && y + i < prevH && x + j < prevW)
                                {
                                    for (int c = 0; c < 1; c++)
                                    {
                                        ex.leftConnect(U[0].C[c].Neurons[y + i, x + j], w);
                                    }
                                }
                            }
                        }
                    }

                }
                U[1].S.Add(tmpS);
            }           

            //V
            //CConnectToS(U[1], GenerateMexicanHat(0.7,2,-25.4,1.5,2.5));
            //CConnectToS(U[1], GenerateMexicanHat(0.7, 3, -25.4, 2.5, 3.5));
            CConnectToS(U[1], U1C);
           // CConnectToS(U[1], GenerateGaussianKernel(0.7, 1.1, 1));
            VConnectToC(U[1], GenerateGaussianKernel(0.7, 2.5, 2));
        }

        public static void VConnectToC(U u, double[][] W)
        {
            V v = u.V;
            List<C> c = u.C;
            int marginY = W.Length / 2;
            int marginX = W[0].Length / 2;

            v.Neurons = new Neuron[c[0].Neurons.GetLength(0), c[0].Neurons.GetLength(1)];
            for (int y = 0; y < v.Neurons.GetLength(0); y++)
            {
                for (int x = 0; x < v.Neurons.GetLength(1); x++)
                {
                    var tmp = new Inhibitor();
                    //connect to all planes and its receptiv location
                    for (int i = -marginY; i <= marginY; i++)
                    {
                        for (int j = -marginX; j <= marginX; j++)
                        {
                            for (int k = 0; k < c.Count; k++)
                            {
                                tmp.leftConnect(c[k].Neurons[y, x], W[marginY + i][marginX + j]);
                            }
                        }
                    }
                    v.Neurons[y, x] = tmp;
                }
            }
        }
        public static void CConnectToS(U u, double[][] W)
        {
            for (int p = 0; p < u.S.Count; p++)
            {
                C newC = new C() { U=u};
                u.C.Add(newC);
                int marginY = W.Length / 2;
                int marginX = W[0].Length / 2;
                List<List<Neuron>> neurons = new List<List<Neuron>>(); 
                for (int y = marginY; y < u.S[p].Neurons.GetLength(0)-marginY; y+=marginY)
                {
                    neurons.Add(new List<Neuron>());
                    for (int x = marginX; x < u.S[p].Neurons.GetLength(1) -marginX; x+=marginX)
                    {
                        CCell cell = new CCell() { Plane=newC};

                        for (int i = -marginY; i <= marginY; i++)
                        {
                            for (int j = -marginX; j <= marginX; j++)
                            {
                                Neuron left = u.S[p].Neurons[y+i,x+j];
                                cell.leftConnect(left, W[marginY + i][marginX + j]);
                            }
                        }
                       neurons[y / marginY-1].Add(cell);
                    }
                }


                 newC.Neurons = new Neuron[neurons.Count,neurons[0].Count];

                 for (int i = 0; i < neurons.Count; i++)
                     for (int j = 0; j < neurons[i].Count; j++)
                         newC.Neurons[i, j] = neurons[i][j];
               
            }
        }
       

        public static double[][][] EdgesWeight(int edgesCnt)
        {
            List<double[][]> result = new List<double[][]>();
            for (int i = 0; i < edgesCnt; i++)
            {
                List<double[]> pattern = new List<double[]>();
                string[] lines = File.ReadAllLines("weight/" + (i + 1) + ".txt");
                for (int j = 0; j < lines.Length; j++)
                {
                    List<double> pixels = new List<double>();
                    string[] line = lines[j].Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                    for (int l = 0; l < line.Length; l++)
                    {
                        pixels.Add(Convert.ToDouble(line[l]));
                    }
                    pattern.Add(pixels.ToArray());
                }
                result.Add(pattern.ToArray());
                result.Add(pattern.ToArray());
            }

            return result.ToArray();
        }
        public static double[][][] GenerateEdgesWeight(int edgesCnt, double q, int margin)
        {
            
            double one = (1.0 / 2*Math.PI*Math.Pow(q,4));
            int l = margin * 2 + 1;
            int edges = edgesCnt;
            int K = edgesCnt;
            double[][][] data = new double[edges][][];
            for (int i = 0; i < edges; i++)
            {
                data[i] = new double[l][];
                for (int y = -margin; y <= margin; y++)
                { 
                    data[i][margin + y] = new double[l];
                    for (int x = -margin; x <= margin; x++)
                    {
                        int angle = i+K;
                        double radius = Math.Sqrt(x * x + y * y);
                        if (radius <= 2.5)
                        {
                            double degree = -(x * x + y * y) / (2 * q * q);
                            double exp = Math.Exp(degree);
                            double left = (x * Math.Cos(2 * Math.PI * angle / K) - y * Math.Sin(2 * Math.PI * angle / K));
                            left *= one *Math.Exp(200);
                            data[i][margin + y][margin + x] = left * exp;
                        }
                    }
                }
            }

            return data;
        }
        public static double[][] GenerateGaussianKernel(double q, double radius, int margin)
        { 
            double[][]  data = new double[margin*2+1][] ; 
                for (int y = -margin; y <= margin; y++)
                {
                    data[margin + y] = new double[margin*2+1];
                    for (int x = -margin; x <= margin; x++)
                    {
                        double l = Math.Sqrt(x * x + y * y);
                        double left = 1.0/(q*Math.Sqrt(2*Math.PI));
                        double degree = -(x*x + y * y)/(2*q*q);
                        data[margin + y][margin + x] = (l <= radius) ? left * Math.Exp(degree) : 0;
                    }
                } 

            return data;
        }
        public static double[][] GenerateMexicanHat(double q, int margin,double beta, double rPlus, double rMinus)
        {
            double[][] data = new double[margin * 2 + 1][];
            for (int y = -margin; y <= margin; y++)
            {
                data[margin + y] = new double[margin * 2 + 1];
                for (int x = -margin; x <= margin; x++)
                {
                    double l = Math.Sqrt(x * x + y * y);
                    double left = 1.0 / (q * Math.Sqrt(2 * Math.PI));
                    double degree = -(l*l) / (2 * q * q);

                    if (l < rPlus)
                        data[margin + y][margin + x] = left * Math.Exp(degree);
                    else if (l < rMinus)
                        data[margin + y][margin + x] = beta*left * Math.Exp(degree); 
                }
            }
            double sum = data.Sum(x => x.Sum());
            return data;
        }


        public static void TrainIntermediateLayers(List<double[,]> trainData, IntermediateLayerTrainRule rule, int layer, int r, NeoKognitron neo, double learningThreshold, double recognizeThreshold, double[][] CWeight)
        {
            neo.U.Add(new U() { NeoKognitron = neo, Selectivity = recognizeThreshold, Name = "U" + layer.ToString() });
            U u = neo.U[layer];
            U uPrevious = neo.U[layer - 1];
            double silentMultiplayer = (learningThreshold - recognizeThreshold) * (1 - recognizeThreshold);
            int width = uPrevious.C[0].Neurons.GetLength(1);
            int height = uPrevious.C[0].Neurons.GetLength(0);

            for (int p = 0; p < trainData.Count; p++)
            {
                //input
                neo.clearOperation();
                neo.input(trainData[p]);

                //iterate areas
                for (int y = r; y < height - r; y++)
                {
                    for (int x = r; x < width - r; x++)
                    {
                        //check are silent
                        bool silent = true;
                        for (int dy = -r; dy <= r; dy++)
                        {
                            for (int dx = -r; dx <= r; dx++)
                            {
                                for (int s = 0; s < u.S.Count; s++)
                                {
                                    double o = u.S[s].Neurons[y + dy, x + dx].getOut(1);
                                    double v = uPrevious.V.Neurons[y + dy, x + dx].getOut(1);
                                    //if (o > silentMultiplayer * (Math.Pow(2 * r + 1, 2)))
                                        //if (o > silentMultiplayer * v)
                                        //if (o > silentMultiplayer)
                                         if (o > v)
                                    //if (o > (1 - recognizeThreshold)*v)
                                        silent = false;
                                }
                            }
                        }
                        if (silent)
                        {
                            //get layer with max response
                            int maxCLayer = -1;
                            double max = double.MinValue;
                            for (int c = 0; c < uPrevious.C.Count; c++)
                            {
                                double sum = 0;
                                for (int dy = -r; dy <= r; dy++)
                                {
                                    for (int dx = -r; dx <= r; dx++)
                                    {
                                        sum += uPrevious.C[c].Neurons[y + dy, x + dx].getOut(1);
                                    }
                                }
                                if (sum > max) { max = sum; maxCLayer = c; }
                            }
                            if (max > 0)
                            {
                                //get weights
                                double[,] W = new double[2 * r + 1, 2 * r + 1];
                                for (int dy = -r; dy <= r; dy++)
                                {
                                    for (int dx = -r; dx <= r; dx++)
                                    {
                                        W[r + dy, r + dx] = uPrevious.C[maxCLayer].Neurons[y + dy, x + dx].getOut(1);
                                    }
                                }
                                //create S plane
                                u.S.Add(CreateSPlane(rule, u, uPrevious, height, width, W, r, maxCLayer));
                            }
                        }

                    }
                }
            }

            //connect C
            CConnectToS(u, CWeight);
            VConnectToC(u, GenerateGaussianKernel(0.7, 3.5, 3));
        }
        public static S CreateSPlane(IntermediateLayerTrainRule rule, U u, U uPrevious, int height, int width, double[,] W, int r, int cLayer)
        {
            S sPlane = new S() { U = u };
            sPlane.Neurons = new Neuron[height, width];
            if (rule == IntermediateLayerTrainRule.WTA)
            {
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        SCell cell = new SCell(uPrevious.V.Neurons[y, x] as Inhibitor, 1) { Plane = sPlane };
                        for (int dy = -r; dy <= r; dy++)
                        {
                            for (int dx = -r; dx <= r; dx++)
                            {
                                if ((dx + x > 0 && dy + y > 0) && (dx + x < width && dy + y < height))
                                {
                                    cell.leftConnect(uPrevious.C[cLayer].Neurons[y + dy, x + dx], W[dy + r, dx + r]);
                                }
                            }
                        }
                        sPlane.Neurons[y, x] = cell;
                    }
                }
            }

            return sPlane;
        }

        public static void Serialize(string filename,NeoKognitron neo)
        {
            FileStream fs = new FileStream(filename, FileMode.Create);

            // Construct a BinaryFormatter and use it to serialize the data to the stream.
            BinaryFormatter formatter = new BinaryFormatter();
            try
            {
                formatter.Serialize(fs, neo);
            } 
            finally
            {
                fs.Close();
            }
        }
        public static NeoKognitron DeSerialize(string filename)
        {
            FileStream fs = new FileStream(filename, FileMode.Open);
            NeoKognitron neo = null;
            try
            {
                BinaryFormatter formatter = new BinaryFormatter();

                // Deserialize the hashtable from the file and 
                // assign the reference to the local variable.
                neo = (NeoKognitron)formatter.Deserialize(fs);
            } 
            finally
            {
                fs.Close();
            }

            return neo;
        }

        static NeoKognitron()
        {
            UgKernelWeight = new double[][]
            {
                new double[]{1.312,-2.653,-3.58325,-2.653,	1.312},
	            new double[]{-2.653,-1.401,4.599,-1.401,-2.653},
	            new double[]{-3.58325, 4.599,17.517,4.599,-3.58325},
	            new double[]{ -2.653,-1.401, 4.599,-1.401,-2.653},
	            new double[]{ 1.312,-2.653, -3.58325,-2.653,1.312}
            };

            U0V = new double[][]
            {
                new double[]{0.002, 0.004, 0.004, 0.004, 0.002, },
                new double[]{0.004, 0.005, 0.005, 0.005, 0.004, },
                new double[]{0.004, 0.005, 0.006, 0.005, 0.004, },
                new double[]{0.004, 0.005, 0.005, 0.005, 0.004, },
                new double[]{0.002, 0.004, 0.004, 0.004, 0.002, },
            };
            UGV = new double[][]
            {
                new double[]{0.012, 0.018, 0.02, 0.018, 0.012, },
                new double[]{0.018, 0.024, 0.026, 0.024, 0.018, },
                new double[]{0.02, 0.026, 0.028, 0.026, 0.02, },
                new double[]{0.018, 0.024, 0.026, 0.024, 0.018, },
                new double[]{0.012, 0.018, 0.02, 0.018, 0.012, },
            };
            U1C = new double[][]
            {
                new double[]{-0.24, 0.184, -0.24, },
                new double[]{0.184, 1, 0.184, },
                new double[]{-0.24, 0.184, -0.24, }
            };
        }


        [NonSerialized]
        public static double[][] UgKernelWeight;
        [NonSerialized]
        public static double[][] U0V;
        [NonSerialized]
        public static double[][] UGV;
        [NonSerialized]
        public static double[][] U1C;




        public double crossValidation(List<double[,]> trainData, List<string> labelSet)
        {
            clearOperation();
            double percent = 0;
            for (int i = 0; i < trainData.Count; i++)
            { 
                string answer = Compute(trainData[i],i+1);
                if (answer.Equals(labelSet[i]))
                    percent += 1;
            }

            return percent / trainData.Count;
        }
    }

    public enum NeokognitronState
    {
        Null,
        FeatureExtractor,
        Ready
    }
}
