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
        
        public BitmapImage Image { get; set; }
        public Canvas Canvas { get; set; }

        #region coordinates

        double dx { get { return Canvas.ActualWidth * 1.0 / Image.Width; } }
        double dy { get { return Canvas.ActualHeight * 1.0 / Image.Height; } }
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
        public Point CanvasCenter
        {
            get
            {
                return new Point(CanvasCoordinates.X + CanvasWidth / 2, CanvasCoordinates.Y + CanvasHeight / 2);
            }
        }
        public bool CanvasPointInRectangle(Point p)
        {
            double x1 = CanvasCoordinates.X;
            double x2 = p.X;
            double y1 = CanvasCoordinates.Y;
            double y2 = p.Y;
             double x3=x1+CanvasWidth;
            double y3=y1+CanvasHeight;

            return x2 < x3 && x2 > x1 && y2 < y3 && y2 > y1;
        }

        #endregion

        public bool Active { get; set; }

        public SymbolWindow(BitmapImage image, Canvas canvas, Point CanvasCoord, double CanvasW, double CanvasH)
        {

            Image = image;
            Canvas = canvas;
            CanvasHeight = CanvasH;
            CanvasWidth = CanvasW;
            CanvasCoordinates = CanvasCoord;
            Active = false;
        }
        public SymbolWindow(BitmapImage image, Canvas canvas, ISRMUL.Recognition.MeanShift.Cluster pixels)
        {

            Image = image;
            Canvas = canvas;
            Pixels = pixels;
            double startX = pixels.Points.Min(x => x.Original[0]);
            double startY = pixels.Points.Min(x => x.Original[1]);
            double endX = pixels.Points.Max(x => x.Original[0]);
            double endY = pixels.Points.Max(x => x.Original[1]);
            RealCoordinates = new Point(startX, startY);
            RealHeight = endY - startY;
            RealWidth = endX - startX; 
            Active = false;
        }

        #region segmentation
        ISRMUL.Recognition.MeanShift.Cluster Pixels { get; set; }
        #endregion
    }
}
