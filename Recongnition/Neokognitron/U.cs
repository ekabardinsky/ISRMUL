using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISRMUL.Recongnition.Neokognitron
{
    [Serializable]
    class U
    {
        public NeoKognitron NeoKognitron { get; set; }
        public List<S> S { get; set; }
        public List<C> C { get; set; }
        public V V { get; set; }
        [NonSerialized]
        public int operationId;
        public double Selectivity { get; set; }
        [NonSerialized]
        public double MaxC = double.MinValue;
        public string Name { get; set; }
        public U()
        {
            V = new V();
            S = new List<S>();
            C = new List<C>();
        }

        public void calculateMaxC(int operation)
        {
            if (operation == operationId) return;
            foreach (C c in C)
            {
                foreach (Neuron n in c.Neurons)
                    MaxC=Math.Max((n as CCell).CalculatePreSinaps(operation),MaxC);
            }
            operationId = operation;
        }

        public void clearOperation()
        {
            V.clearOperation();
            foreach (S s in S)
                s.clearOperation();


            foreach (C c in C)
                c.clearOperation();
            operationId = 0;
        }
        //public Bitmap Paint(int Width, int Height,int row, int column)
        //{
        //    int W = Width / 2;
        //    int H = Height;
        //    NeoKognitron.clearOperation();
        //    int operation = (int)DateTime.Now.Ticks;
        //    int marginTop = H / 20;
        //    int marginLeft = W / 20;
        //    int cellH = (H - row * marginTop - 2 * marginTop) / (row + 1);
        //    int cellW = (W - (column + 1) * marginLeft) / column;
        //    if (S.Count == 0)
        //    {
        //        cellW = (Width - (column + 1) * marginLeft) / column;
        //    }

        //    double[][][] OutputsS = new double[S.Count][][];
        //    double[][][] OutputsC = new double[C.Count][][];

        //    Bitmap bmp = new Bitmap(Width, Height);
        //    Graphics g = Graphics.FromImage(bmp);
        //    Font font = new Font(FontFamily.GenericMonospace, marginTop / 3f);
        //    Font labelFont = new Font(FontFamily.GenericMonospace, marginTop / 4f);
        //    double maxS = double.MinValue;
        //    double maxC = double.MinValue;
        //    for (int i = 0; i < S.Count; i++)
        //    {
        //        OutputsS[i] = new double[S[i].Neurons.GetLength(0)][];
        //        for (int y = 0; y < S[i].Neurons.GetLength(0); y++)
        //        {
        //            OutputsS[i][y] = new double[S[i].Neurons.GetLength(1)];
        //            for (int x = 0; x < S[i].Neurons.GetLength(1); x++)
        //            {
        //                double value = S[i].Neurons[y, x].getOut(operation);
        //                OutputsS[i][y][x] = value;
        //                if (value > maxS)
        //                    maxS = value;
        //            }
        //        }
        //    }

        //    for (int i = 0; i < C.Count; i++)
        //    {
        //        OutputsC[i] = new double[C[i].Neurons.GetLength(0)][];
        //        for (int y = 0; y < C[i].Neurons.GetLength(0); y++)
        //        {
        //            OutputsC[i][y] = new double[C[i].Neurons.GetLength(1)];
        //            for (int x = 0; x < C[i].Neurons.GetLength(1); x++)
        //            {
        //                double value = C[i].Neurons[y, x].getOut(operation);
        //                OutputsC[i][y][x] = value;
        //                if (value > maxC)
        //                    maxC = value;
        //            }
        //        }
        //    }

        //    //layer s draw
        //    paintS(g, marginTop, marginLeft, row, column, cellH, cellW, labelFont, font, OutputsS, maxS);
        //    //vplane
        //    paintC(S.Count == 0 ? 0 : W, g, marginTop, marginLeft, row, column, cellH, cellW, labelFont, font, OutputsC, maxC);

        //    paintV(H, g, marginTop, marginLeft, row, column, cellH, cellW, labelFont, font);
        //    return bmp;
        //}
        //public Bitmap PaintS(int Width, int Height, int row, int column)
        //{
        //    int W = Width;
        //    int H = Height;
        //    NeoKognitron.clearOperation();
        //    int operation = (int)DateTime.Now.Ticks;
        //    int marginTop = H / 20;
        //    int marginLeft = W / 20;
        //    int cellH = (H - row * marginTop - marginTop) / (row);
        //    int cellW = (W - (column + 1) * marginLeft) / column;

        //    double[][][] OutputsS = new double[S.Count][][];

        //    Bitmap bmp = new Bitmap(Width, Height);
        //    Graphics g = Graphics.FromImage(bmp);
        //    Font font = new Font(FontFamily.GenericMonospace, marginTop / 3f);
        //    Font labelFont = new Font(FontFamily.GenericMonospace, marginTop / 4f);
        //    double maxS = double.MinValue;
        //    for (int i = 0; i < S.Count; i++)
        //    {
        //        OutputsS[i] = new double[S[i].Neurons.GetLength(0)][];
        //        for (int y = 0; y < S[i].Neurons.GetLength(0); y++)
        //        {
        //            OutputsS[i][y] = new double[S[i].Neurons.GetLength(1)];
        //            for (int x = 0; x < S[i].Neurons.GetLength(1); x++)
        //            {
        //                double value = S[i].Neurons[y, x].getOut(operation);
        //                OutputsS[i][y][x] = value;
        //                if (value > maxS)
        //                    maxS = value;
        //            }
        //        }
        //    }

        //    //layer s draw
        //    paintS(g, marginTop, marginLeft, row, column, cellH, cellW, labelFont, font, OutputsS, maxS);
        //    return bmp;
        //}
        //public Bitmap PaintC(int Width, int Height, int row, int column)
        //{
        //    int W = Width;
        //    int H = Height;
        //    NeoKognitron.clearOperation();
        //    int operation = (int)DateTime.Now.Ticks;
        //    int marginTop = H / 20;
        //    int marginLeft = W / 20;
        //    int cellH = (H - row * marginTop - marginTop) / (row);
        //    int cellW = (W - (column + 1) * marginLeft) / column;
             
        //    double[][][] OutputsC = new double[C.Count][][];

        //    Bitmap bmp = new Bitmap(Width, Height);
        //    Graphics g = Graphics.FromImage(bmp);
        //    Font font = new Font(FontFamily.GenericMonospace, marginTop / 3f);
        //    Font labelFont = new Font(FontFamily.GenericMonospace, marginTop / 4f);
           
        //    double maxC = double.MinValue;
            

        //    for (int i = 0; i < C.Count; i++)
        //    {
        //        OutputsC[i] = new double[C[i].Neurons.GetLength(0)][];
        //        for (int y = 0; y < C[i].Neurons.GetLength(0); y++)
        //        {
        //            OutputsC[i][y] = new double[C[i].Neurons.GetLength(1)];
        //            for (int x = 0; x < C[i].Neurons.GetLength(1); x++)
        //            {
        //                double value = C[i].Neurons[y, x].getOut(operation);
        //                OutputsC[i][y][x] = value;
        //                if (value > maxC)
        //                    maxC = value;
        //            }
        //        }
        //    }

        //    //layer s draw
          
        //    //vplane
        //    paintC(0, g, marginTop, marginLeft, row, column, cellH, cellW, labelFont, font, OutputsC, maxC);
             
        //    return bmp;
        //}
        //public Bitmap PaintV(int Width, int Height)
        //{
        //    int W = Width;
        //    int H = Height;
        //    NeoKognitron.clearOperation();
        //    int operation = (int)DateTime.Now.Ticks;
        //    int marginTop = H / 20;
        //    int marginLeft = W / 20;
        //    int cellH = (H - 2 * marginTop);
        //    int cellW = (W - 2 * marginLeft);
        //    Bitmap bmp = new Bitmap(Width, Height);
        //    Graphics g = Graphics.FromImage(bmp);
        //    Font font = new Font(FontFamily.GenericMonospace, marginTop / 3f);
        //    Font labelFont = new Font(FontFamily.GenericMonospace, marginTop / 4f); 
        //    paintV(H, g, marginTop, marginLeft, 1, 1, cellH, cellW, labelFont, font);
        //    return bmp;
        //}


        //void paintV(int H, Graphics g, int marginTop, int marginLeft, int row, int column, int cellH, int cellW, Font labelFont, Font font)
        //{

        //    if (V.Neurons == null) return;
        //    int Y = H - marginTop - cellH;
        //    int X = marginLeft; 

        //    double maxV = double.MinValue;
        //    double[][] OutputsV = new double[V.Neurons.GetLength(0)][];
        //    //v out
        //    int id = DateTime.Now.Millisecond;
        //    for (int y = 0; y < V.Neurons.GetLength(0); y++)
        //    {
        //        OutputsV[y] = new double[V.Neurons.GetLength(1)];
        //        for (int x = 0; x < V.Neurons.GetLength(1); x++)
        //        {
        //            double value = V.Neurons[y, x].getOut(id);
        //            OutputsV[y][x] = value;
        //            if (value > maxV)
        //                maxV = value;
        //        }
        //    }
        //    g.DrawString("Layer " + Name + "V[" + V.Neurons.GetLength(0) + "X" + V.Neurons.GetLength(1) + "] (max " + Math.Round(maxV, 4) + ")", font, Brushes.Black, new PointF(X, Y - font.Size * 1.5f));

        //    for (int y = 0; y < V.Neurons.GetLength(0); y++)
        //    {
        //        for (int x = 0; x < V.Neurons.GetLength(1); x++)
        //        {
        //            int value = 255 - (int)(OutputsV[y][x] / maxV * 255);
        //            if (value > 0)
        //            {
        //                double dy = cellH * 1.0 / V.Neurons.GetLength(0);
        //                double dx = cellW * 1.0 / V.Neurons.GetLength(1);
        //                int pointY = (int)(Y + dy * y);
        //                int pointX = (int)(X + dx * x);

        //                Color color = Color.FromArgb(value, value, value);
        //                g.FillRectangle(new SolidBrush(color), pointX, pointY, (int)dx, (int)dy);
        //            }
        //        }

        //    }
        //    //div
        //    g.DrawRectangle(Pens.Black, X, Y, cellW, cellH);
           
        //}
        //void paintS(Graphics g, int marginTop, int marginLeft, int row, int column, int cellH, int cellW, Font labelFont, Font font, double[][][] OutputsS, double maxS)
        //{
        //    int p = 0;
        //    if (S.Count == 0) return;

        //    g.DrawString("Layer " + Name + "S[" + S[0].Neurons.GetLength(0) + "X" + S[0].Neurons.GetLength(1) + "] Length["+S.Count+"] (max " + Math.Round(maxS, 4) + ")", font, Brushes.Black, new Point(marginLeft, 0));

        //    for (int r = 0; r < row; r++)
        //    {
        //        for (int c = 0; c < column; c++)
        //        {
        //            try
        //            {
        //                int startY = marginTop * (r + 1) + cellH * r;
        //                int startX = marginLeft * (c + 1) + cellW * c;
        //                int endY = startY + cellH;
        //                int endX = startX + cellW;
        //                Plane plane = S[p];
        //                double dy = cellH * 1.0 / plane.Neurons.GetLength(0);
        //                double dx = cellW * 1.0 / plane.Neurons.GetLength(1);
        //                //plane label
        //                g.DrawString("S[" + p + "]", labelFont, Brushes.Black, new PointF(startX, startY - labelFont.Size * 1.5f));


        //                for (int y = 0; y < OutputsS[p].Length; y++)
        //                {
        //                    for (int x = 0; x < OutputsS[p][y].Length; x++)
        //                    {
        //                        int value = 255 - (int)(OutputsS[p][y][x] / maxS * 255);
        //                        if (value >= 0)
        //                        {
        //                            int pointY = (int)(startY + dy * y);
        //                            int pointX = (int)(startX + dx * x);

        //                            Color color = Color.FromArgb(value, value, value);
        //                            g.FillRectangle(new SolidBrush(color), pointX, pointY, (int)dx, (int)dy);
        //                        }
        //                    }

        //                }
        //                //div
        //                g.DrawRectangle(Pens.Black, startX, startY, cellW, cellH);
        //                p++;
        //            }
        //            catch { return; }
        //        }
        //    }

        //}
        //void paintC(int W,Graphics g, int marginTop, int marginLeft, int row, int column, int cellH, int cellW, Font labelFont, Font font, double[][][] OutputsC, double maxC)
        //{
        //    int marginHalf = W;
        //    //layer c draw
        //    g.DrawString("Layer " + Name + "C[" + C[0].Neurons.GetLength(0) + "X" + C[0].Neurons.GetLength(1) + "] (max " + Math.Round(maxC, 4) + ")", font, Brushes.Black, new Point(marginLeft + marginHalf, 0));
        //    int p = 0;
        //    for (int r = 0; r < row; r++)
        //    {
        //        for (int c = 0; c < column; c++)
        //        {
        //            try
        //            {
        //                int startY = marginTop * (r + 1) + cellH * r;
        //                int startX = marginLeft * (c + 1) + cellW * c + marginHalf;
        //                int endY = startY + cellH;
        //                int endX = startX + cellW;
        //                Plane plane = C[p];
        //                double dy = cellH * 1.0 / plane.Neurons.GetLength(0);
        //                double dx = cellW * 1.0 / plane.Neurons.GetLength(1);
        //                //plane label
        //                g.DrawString("C[" + p + "]", labelFont, Brushes.Black, new PointF(startX, startY - labelFont.Size * 1.5f));

        //                for (int y = 0; y < OutputsC[p].Length; y++)
        //                {
        //                    for (int x = 0; x < OutputsC[p][y].Length; x++)
        //                    {
        //                        int value = 255 - (int)(OutputsC[p][y][x] / maxC * 255);
        //                        if (value > 0)
        //                        {
        //                            int pointY = (int)(startY + dy * y);
        //                            int pointX = (int)(startX + dx * x);

        //                            Color color = Color.FromArgb(value, value, value);
        //                            g.FillRectangle(new SolidBrush(color), pointX, pointY, (int)dx, (int)dy);
        //                        }
        //                    }

        //                }
        //                //div
        //                g.DrawRectangle(Pens.Black, startX, startY, cellW, cellH);
        //                p++;
        //            }
        //            catch { return; }
        //        }
        //    }
        //}
        

    }
}
