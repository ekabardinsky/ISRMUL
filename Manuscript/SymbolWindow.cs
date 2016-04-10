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
    [Serializable]
    public class SymbolWindow
    {

        public BitmapSource Image { get { return Project.Images[ImageKey]; } }

        public string Label { get; set; }

        string ImageKey {get;set;}

        public Canvas Canvas { get { return Project.Canvas; } }

        List<ISRMUL.Recognition.MeanShift.Point> cachedPoint { get; set; }

        [NonSerialized]
        public Project Project;

        #region coordinates
        Point _RC;
        double RW;
        double RH;
        double dx { get { return Canvas.Width * 1.0 / Image.Width; } }
        double dy { get { return Canvas.Height * 1.0 / Image.Height; } }
        public Point RealCoordinates { get { return _RC; } set { redacted = true; _RC = value; } }
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
        public double RealWidth { get { return RW; } set { redacted = true; RW = value; } }
        public double RealHeight { get { return RH; } set { redacted = true; RH = value; } }
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
        public List<ISRMUL.Recognition.MeanShift.Point> SegmentedPoint { get; set; }
        bool redacted { get; set; }

        public SymbolWindow(string image, Project project, Point CanvasCoord, double CanvasW, double CanvasH)
        {

            ImageKey = image;
            Project = project;
            CanvasHeight = CanvasH;
            CanvasWidth = CanvasW;
            CanvasCoordinates = CanvasCoord;
            Active = false;
            redacted = false;
        }
        public SymbolWindow(string image, Project project, ISRMUL.Recognition.MeanShift.Cluster pixels)
        {

            ImageKey = image;
            Project = project;
            Pixels = pixels;
            SegmentedPoint = pixels.Points;
            double startX = pixels.Points.Min(x => x.Original[0]);
            double startY = pixels.Points.Min(x => x.Original[1]);
            double endX = pixels.Points.Max(x => x.Original[0]);
            double endY = pixels.Points.Max(x => x.Original[1]);
            RealCoordinates = new Point(startX, startY);
            RealHeight = endY - startY;
            RealWidth = endX - startX; 
            Active = false;
            redacted = false;
        }

        #region segmentation
        ISRMUL.Recognition.MeanShift.Cluster Pixels { get; set; }
        public BitmapSource toImage()
        {

            List<ISRMUL.Recognition.MeanShift.Point> point=null;
            if ((redacted || SegmentedPoint==null) && Project.Images.ContainsKey(ImageKey))
                point = Utils.ImageConverter.getPointFromImage(Image, (int)RealCoordinates.X, (int)RealCoordinates.Y, (int)RealWidth, (int)RealHeight);
            else if (Project.Images.ContainsKey(ImageKey))
                point = SegmentedPoint;


            if (point != null)
                cachedPoint = point;
            else
                point = cachedPoint;

            return Utils.ImageConverter.pointToImage(point, (int)RealCoordinates.X, (int)RealCoordinates.Y, (int)RealWidth+1, (int)RealHeight+1);
        }

        public double[,] toRetina(int width, int heigth)
        {
            var origin = toImage();
            var scaled = Utils.ImageConverter.UniformResizeImage(origin, width, heigth);
            var array = Utils.ImageConverter.bitmapSourceToArray(scaled);

            int top = (heigth - array.GetLength(0)) / 2;
            int left = (width - array.GetLength(1)) / 2;

            if (top != 0 || left != 0)
                array = Utils.ImageConverter.printToArrayUniform(array, top, left, width, heigth);

            return array;
        }

        #endregion

        public string getLabelFromAlphabet()
        {
            var alpha = Project.Alphabets;
            var item = alpha.FirstOrDefault(x => x.Symbols.Contains(this));
            return item == null ? null : item.Code; 
        }

        public string recognize()
        {
            string label = getLabelFromAlphabet();
            if (label == null)
            {
                label = Project.Neokognitron.Compute(toRetina(Manuscript.Project.patternWidth, Manuscript.Project.patternHeight));
            }

            return label;
        }
    }
}
