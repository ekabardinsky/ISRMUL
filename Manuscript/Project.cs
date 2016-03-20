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
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace ISRMUL.Manuscript
{
    [Serializable]
    public class Project:DependencyObject
    {
        public List<BitmapSource> Pages { get; set; } 
        public List<IRefreshable> Views { get; set; }

        Dictionary<BitmapSource, List<SymbolWindow>> SymbolWindows { get; set; }

        public Project(params IRefreshable [] controls)
        {
            Pages = new List<BitmapSource>();
            Views = new List<IRefreshable>(controls);
            SymbolWindows = new Dictionary<BitmapSource, List<SymbolWindow>>(); 
        }

        #region getters
        public List<SymbolWindow> getSymbolWindows(BitmapSource image)
        {
            
            if(!SymbolWindows.ContainsKey(image))
            {
                SymbolWindows.Add(image,new List<SymbolWindow>());
            }
            return SymbolWindows[image];
        }

        public void AddToSymbolWindows(BitmapSource image, SymbolWindow symbol)
        {
            getSymbolWindows(image).Add(symbol);
        }

        #endregion

        #region dependency property

        [NonSerialized]
        public static readonly DependencyProperty CurrentPageProperty;

        static Project()
        {
            CurrentPageProperty = DependencyProperty.Register("CurrentPage", typeof(BitmapSource), typeof(Project));
        }
        public BitmapSource CurrentPage
        {
            get { return (BitmapSource)GetValue(CurrentPageProperty); }
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

        public void SegmentationCurrent(double windowHeight, double windowWidth, Canvas canvas, BitmapSource image, int backThresh)
        {
            var points = Utils.ImageConverter.getPointFromImage(CurrentPage);

            ISRMUL.Recognition.MeanShift.MeanShiftSolver solver = new Recognition.MeanShift.MeanShiftSolver(new double[] { windowWidth, windowHeight }, points);
            
            solver.Compute(0.2, 1000);
            solver.Clustering(3);

            var symbols = solver.Clusters.Select(x => new SymbolWindow(image, canvas, x));
            var original = getSymbolWindows(image);
            original.Clear();
            original.AddRange(symbols);
        }

        

        #endregion
    }

    
}
