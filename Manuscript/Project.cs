using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using ISRMUL.Control;
using System.Windows;
using System.Windows.Controls;
using ISRMUL.Recognition;

namespace ISRMUL.Manuscript
{
    public class Project:DependencyObject
    {
        public List<BitmapImage> Pages { get; set; }
        public List<IRefreshable> Views { get; set; }
        Dictionary<BitmapImage,List<SymbolWindow>> SymbolWindows { get; set; }

        public Project(params IRefreshable [] controls)
        {
            Pages = new List<BitmapImage>();
            Views = new List<IRefreshable>(controls);
            SymbolWindows = new Dictionary<BitmapImage, List<SymbolWindow>>(); 
        }
        #region getters
        public List<SymbolWindow> getSymbolWindows(BitmapImage image)
        {
            
            if(!SymbolWindows.ContainsKey(image))
            {
                SymbolWindows.Add(image,new List<SymbolWindow>());
            }
            return SymbolWindows[image];
        }

        public void AddToSymbolWindows(BitmapImage image, SymbolWindow symbol)
        {
            getSymbolWindows(image).Add(symbol);
        }

        #endregion
        #region dependency property
        public static readonly DependencyProperty CurrentPageProperty;

        static Project()
        {
            CurrentPageProperty = DependencyProperty.Register("CurrentPage", typeof(BitmapImage), typeof(Project));
        }

        public BitmapImage CurrentPage
        {
            get { return (BitmapImage)GetValue(CurrentPageProperty); }
            set { SetValue(CurrentPageProperty, value); }
        }
        
        #endregion

        #region commands
        public void Refresh()
        {
            foreach (IRefreshable view in Views)
                view.Refresh();
        }
        #endregion

        #region segmentation

        public void SegmentationCurrent(double windowHeight, double windowWidth,Canvas canvas,BitmapImage image)
        {
            var points = getPointFromImage(CurrentPage);
            ISRMUL.Recognition.MeanShift.MeanShiftSolver solver = new Recognition.MeanShift.MeanShiftSolver(new double[] { windowWidth, windowHeight }, points);
            
            solver.Compute(0.2, 1000);
            solver.Clustering(3);

            var symbols = solver.Clusters.Select(x => new SymbolWindow(image, canvas, x));
            var original = getSymbolWindows(image);
            original.Clear();
            original.AddRange(symbols);
        }

        List<ISRMUL.Recognition.MeanShift.Point> getPointFromImage(BitmapImage img)
        {
            List<ISRMUL.Recognition.MeanShift.Point> points = new List<Recognition.MeanShift.Point>();

            int stride = img.PixelWidth * 4;
            int size = img.PixelHeight * stride;
            byte[] pixels = new byte[size];
            img.CopyPixels(pixels, stride, 0);

            for (int y = 0; y < img.PixelHeight; y++)
            {
                for (int x = 0; x < img.PixelWidth; x++)
                {
                    int index = y * stride + 4 * x;
                    byte red = pixels[index];
                    byte green = pixels[index + 1];
                    byte blue = pixels[index + 2];
                    byte alpha = pixels[index + 3];

                    if (!isBackground(red, green, blue))
                        points.Add(new ISRMUL.Recognition.MeanShift.Point(new double[] { x, y }) { R = red, G = green, B = blue });
                }
            }

            return points;
        }

        bool isBackground(byte r, byte g, byte b)
        {
            return (r + g + b) < 100;
        }

        #endregion
    }

    
}
