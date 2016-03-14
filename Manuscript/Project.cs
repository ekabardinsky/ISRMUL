using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using ISRMUL.Control;
using System.Windows;
using System.Windows.Controls;

namespace ISRMUL.Manuscript
{
    public class Project:DependencyObject
    {
        public List<BitmapImage> Pages { get; set; }
        public List<IRefreshable> Views { get; set; }
        public List<SymbolWindow> SymbolWindows { get; set; }

        public Project(Control.Page.PageViewControl pageView)
        {
            Pages = new List<BitmapImage>();
            Views = new List<IRefreshable>();
            SymbolWindows = new List<SymbolWindow>();
            Views.Add(pageView);
        }

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
    }

    public class SymbolWindow
    {
        public Point RealCoordinates { get; set; }
        public Point CanvasCoordinates
        {
            get
            { 
                return new Point(RealCoordinates.X * dx, RealCoordinates.Y * dy);
            }
            set
            {  
                RealCoordinates = new Point(value.X * dx, value.Y * dy);
            }
        }

        public Image Image { get; set; }
        public Canvas Canvas { get; set; }

        double dx { get { return Canvas.ActualWidth * 1.0 / Image.Source.Width; } }
        double dy { get { return Canvas.ActualHeight * 1.0 / Image.Source.Height; } }

        public double RealWidth { get; set; }
        public double RealHeight { get; set; }
        public double CanvasWidth
        {
            get
            { 
                return RealWidth * dx;
            }
            set
            {
                RealWidth = value / dx;
            }
        }
        public double CanvasHeight
        {
            get { return RealHeight * dy; }
            set { RealHeight = value / dy; }
        }

        public SymbolWindow(Image image, Canvas canvas, Point CanvasCoord, double CanvasW, double CanvasH)
        {
            CanvasHeight = CanvasH;
            CanvasWidth = CanvasW;
            Image = image;
            Canvas = canvas;
            CanvasCoordinates = CanvasCoord;
        }
    }
}
