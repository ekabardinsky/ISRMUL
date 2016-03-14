using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace ISRMUL.Manuscript
{
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
                RealCoordinates = new Point(value.X / dx, value.Y / dy);
            }
        }

        public BitmapImage Image { get; set; }
        public Canvas Canvas { get; set; }

        double dx { get { return Canvas.ActualWidth * 1.0 / Image.Width; } }
        double dy { get { return Canvas.ActualHeight * 1.0 / Image.Height; } }

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

        public SymbolWindow(BitmapImage image, Canvas canvas, Point CanvasCoord, double CanvasW, double CanvasH)
        { 
            
            Image = image;
            Canvas = canvas;
            CanvasHeight = CanvasH;
            CanvasWidth = CanvasW;
           
            CanvasCoordinates = CanvasCoord;
        }
    }
}
