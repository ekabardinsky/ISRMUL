using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace ISRMUL.Control.Editor
{
    /// <summary>
    /// Логика взаимодействия для EditorView.xaml
    /// </summary>
    public partial class EditorViewControl : UserControl, Manuscript.IRefreshable
    {
        Operation currentOperation { get { return Tool.CurrentOperation; } }

        #region visual tool fields

        Point splitStart { get; set; }
        bool splitStarted { get; set; }
        Line SplitLine;

        Point NewRectangleStart { get; set; }
        bool newRectangleStarted { get; set; }
        Rectangle NewRectangle { get; set; }

        #endregion

        public EditorViewControl()
        {
            InitializeComponent();
        }

       

        #region dependency
        public static readonly DependencyProperty editorViewProjectProperty = DependencyProperty.Register("editorViewProject", typeof(Manuscript.Project), typeof(EditorViewControl));
        public Manuscript.Project editorViewProject
        {
            get
            {
                return this.GetValue(editorViewProjectProperty) as Manuscript.Project;
            }
            set
            {
                this.SetValue(editorViewProjectProperty, value);
            }
        }
        #endregion

        #region event handler

        private void Canvas_MouseDown_1(object sender, MouseButtonEventArgs e)
        {

            if (currentOperation == Operation.NewRectangle&&!newRectangleStarted)
            {
                NewRectangleStart=e.GetPosition(Canvas);
                newRectangleStarted = true;
                PaintNewRectangle(NewRectangleStart, NewRectangleStart);
            }
            else if (currentOperation==Operation.Split&&!splitStarted)
            {
                splitStart = e.GetPosition(Canvas);
                splitStarted = true;
                PaintSplitLine(splitStart, splitStart);
            }
            else if (currentOperation == Operation.Explore)
            {
                var window = editorViewProject.getSymbolWindows(editorViewProject.CurrentPage).Where(x => x.CanvasPointInRectangle(e.GetPosition(Canvas))).FirstOrDefault();
                if (window != null)
                    window.Active = true;

                editorViewProject.getSymbolWindows(editorViewProject.CurrentPage).ForEach(x => x.Active = x == window);

                Refresh();
            }
            else if (currentOperation == Operation.Delete)
            {
                var window = editorViewProject.getSymbolWindows(editorViewProject.CurrentPage).Where(x => x.CanvasPointInRectangle(e.GetPosition(Canvas))).FirstOrDefault();
                if (window != null)
                {
                    editorViewProject.getSymbolWindows(editorViewProject.CurrentPage).Remove(window);
                    Refresh();
                }
                
            }
            else if (currentOperation == Operation.Union)
            {
                var active = editorViewProject.getSymbolWindows(editorViewProject.CurrentPage).Where(x => x.Active).FirstOrDefault(); 
                var window = editorViewProject.getSymbolWindows(editorViewProject.CurrentPage).Where(x => x.CanvasPointInRectangle(e.GetPosition(Canvas))).FirstOrDefault();
                if (window != null)
                {
                    window.Active = true;
                    if (active != null)
                    {
                        var all = editorViewProject.getSymbolWindows(editorViewProject.CurrentPage);
                        var newWindow = Union(active, window);
                        all.Remove(window);
                        all.Remove(active);
                        all.Add(newWindow);
                    }
                }

                editorViewProject.getSymbolWindows(editorViewProject.CurrentPage).ForEach(x => x.Active = x == window);

                Refresh();
            }
        }
                
        private void Canvas_MouseMove_1(object sender, MouseEventArgs e)
        {
            if (currentOperation == Operation.Split && e.LeftButton == MouseButtonState.Pressed && splitStarted)
            {
                PaintSplitLine(splitStart, e.GetPosition(Canvas));
            }
            else if (currentOperation == Operation.NewRectangle && e.LeftButton == MouseButtonState.Pressed && newRectangleStarted)
            {
                PaintNewRectangle(NewRectangleStart, e.GetPosition(Canvas));
            }
        }

        private void Canvas_MouseUp_1(object sender, MouseButtonEventArgs e)
        {
            if (currentOperation == Operation.Split && splitStarted)
            {
                splitStarted = false;
                Split(splitStart, e.GetPosition(Canvas));
                Canvas.Children.Remove(SplitLine);
                SplitLine = null;
            }
            if (currentOperation == Operation.NewRectangle && newRectangleStarted)
            {
                newRectangleStarted = false;
                CreateNewRectangle(NewRectangleStart, e.GetPosition(Canvas));
                Canvas.Children.Remove(NewRectangle);
                NewRectangle = null;
            }

        }

        private void Tool_MouseDown_1(object sender, MouseButtonEventArgs e)
        {
            if (editorViewProject == null || editorViewProject.CurrentPage == null) return;
            ClearActivatedWindow();
        }

        private void Button_MouseDown_1(object sender, MouseButtonEventArgs e)
        {
            if (editorViewProject.CurrentPage == null) return;
            var project = editorViewProject;
            project.SegmentationCurrent(ySlider.Value, xSlider.Value, Canvas, project.CurrentPage);
            Refresh();
        }

        private void xSlider_ValueChanged_1(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            try
            {
                xLabel.Text = "Ширина символа " + (int)xSlider.Value + " px";
            }
            catch { }
        }

        private void ySlider_ValueChanged_1(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            try
            {
                yLabel.Text = "Высота символа " + (int)ySlider.Value + " px";
            }
            catch { }
        }

        private void Grid_PreviewMouseWheel_1(object sender, MouseWheelEventArgs e)
        {
            if (editorViewProject.CurrentPage == null) return;
            Point position = e.GetPosition(Canvas);
            try
            {
                double percent = 5;
                double k = e.Delta > 0 ? 1 : -1;
                double dH = k * Canvas.Height / 100 * percent;
                double dW = k * Canvas.Width / 100 * percent;
                double H = Canvas.Height + dH;
                double W = Canvas.Width + dW;

                if (H > 0 && W > 0)
                {
                    Canvas.Height = H;
                    Canvas.Width = W;

                    Refresh();
                }
            }
            catch { }
        }

        private void Editor_KeyDown(object sender, KeyEventArgs e)
        {
            if (currentOperation == Operation.Explore)
            {
                var window = editorViewProject.getSymbolWindows(editorViewProject.CurrentPage).Where(x => x.Active).FirstOrDefault();
                if (window != null)
                {
                    if (e.Key == Key.Up)
                    {
                        double y = window.CanvasCoordinates.Y - window.CanvasHeight / 10;
                        window.CanvasCoordinates = new Point(window.CanvasCoordinates.X, (y > 0) ? y : 0);

                        Refresh();
                    }
                    if (e.Key == Key.Down)
                    {
                        double y = window.CanvasCoordinates.Y + window.CanvasHeight / 10;
                        window.CanvasCoordinates = new Point(window.CanvasCoordinates.X, (y < Canvas.Height - window.CanvasHeight) ? y : (Canvas.Height - window.CanvasHeight));

                        Refresh();
                    }
                    if (e.Key == Key.Left)
                    {
                        double x = window.CanvasCoordinates.X - window.CanvasWidth / 10;
                        window.CanvasCoordinates = new Point((x > 0) ? x : 0, window.CanvasCoordinates.Y);

                        Refresh();
                    }
                    if (e.Key == Key.Right)
                    {
                        double x = window.CanvasCoordinates.X + window.CanvasWidth / 10;
                        window.CanvasCoordinates = new Point((x < Canvas.Width - window.CanvasWidth) ? x : (Canvas.Width - window.CanvasWidth), window.CanvasCoordinates.Y);

                        Refresh();
                    }
                    if (e.Key == Key.W)
                    {
                        window.CanvasHeight *= 0.95;

                        Refresh();
                    }
                    if (e.Key == Key.S)
                    {
                        window.CanvasHeight *= 1.05;  

                        Refresh();
                    }
                    if (e.Key == Key.A)
                    {
                        window.CanvasWidth *= 0.95; 

                        Refresh();
                    }
                    if (e.Key == Key.D)
                    {
                        window.CanvasWidth *= 1.05;

                        Refresh();
                    }

                    if (e.Key == Key.Space)
                    {
                        var allWindow = editorViewProject.getSymbolWindows(editorViewProject.CurrentPage);
                        int i = allWindow.IndexOf(window);

                        if (i == allWindow.Count - 1)
                            i = -1;

                        window.Active = false;
                        allWindow[i+1].Active = true;

                        Refresh();
                    }

                }
            }
        }

        #endregion

        #region visual tool

        void ClearActivatedWindow()
        {
            editorViewProject.getSymbolWindows(editorViewProject.CurrentPage).ForEach(x => x.Active = false);
        }
        Manuscript.SymbolWindow Union(Manuscript.SymbolWindow one, Manuscript.SymbolWindow two)
        {
            Point left = new Point(Math.Min(one.CanvasCoordinates.X, two.CanvasCoordinates.X), Math.Min(one.CanvasCoordinates.Y, two.CanvasCoordinates.Y));
            Point Right = new Point(Math.Max(one.CanvasCoordinates.X + one.CanvasWidth, two.CanvasCoordinates.X + two.CanvasWidth), Math.Max(one.CanvasCoordinates.Y + one.CanvasHeight, two.CanvasCoordinates.Y + two.CanvasHeight));

            return new Manuscript.SymbolWindow(one.Image, one.Canvas, left, Right.X - left.X, Right.Y - left.Y);
        }

        void Split(Point start, Point end)
        {
            Point center = new Point((start.X + end.X) / 2, (start.Y + end.Y) / 2);

            List<Manuscript.SymbolWindow> windows = editorViewProject.getSymbolWindows(editorViewProject.CurrentPage);

            //Manuscript.SymbolWindow original = windows.OrderBy(x => Math.Sqrt(Math.Pow(x.CanvasCoordinates.X - center.X, 2) + Math.Pow(x.CanvasCoordinates.Y - center.Y, 2))).FirstOrDefault();
            Manuscript.SymbolWindow original = windows.Where(x => x.CanvasPointInRectangle(center)).FirstOrDefault();

            if (original != null)
            {
                Manuscript.SymbolWindow one = new Manuscript.SymbolWindow(original.Image, original.Canvas, original.CanvasCoordinates, center.X - original.CanvasCoordinates.X,original.CanvasHeight);
                Point twoCenter = new Point(one.CanvasCoordinates.X + one.CanvasWidth, original.CanvasCoordinates.Y);
                Manuscript.SymbolWindow two = new Manuscript.SymbolWindow(original.Image, original.Canvas, twoCenter, original.CanvasWidth - one.CanvasWidth, original.CanvasHeight);

                windows.Remove(original);
                windows.Add(one);
                windows.Add(two);

                Refresh();
            }

        }

        void PaintSplitLine(Point start, Point stop)
        {
            if (SplitLine == null)
            {
                SplitLine = new Line();
                SplitLine.Stroke =new SolidColorBrush(Color.FromArgb(255, 255, 0, 0));
                SplitLine.StrokeThickness = 3;
                Canvas.Children.Add(SplitLine);
            }
            SplitLine.X1 = start.X;
            SplitLine.X2 = stop.X;
            SplitLine.Y1 = start.Y;
            SplitLine.Y2 = stop.Y;
        }

        void PaintNewRectangle(Point start, Point end)
        {
            if (NewRectangle == null)
            {
                NewRectangle = new Rectangle(); 
                NewRectangle.StrokeThickness = 3;
                NewRectangle.Stroke = new SolidColorBrush(Color.FromArgb(255, 0, 255, 0));
                Canvas.Children.Add(NewRectangle);
            }
            if (start.X > end.X || start.Y > end.Y) 
                return;
            NewRectangle.Width = end.X - start.X;
            NewRectangle.Height = end.Y - start.Y;
            Canvas.SetTop(NewRectangle, start.Y);
            Canvas.SetLeft(NewRectangle, start.X);
        }

        void CreateNewRectangle(Point start, Point end)
        {
            if (end.X - start.X < 20 && end.Y - start.Y < 20) return;

            Manuscript.SymbolWindow symbol = new Manuscript.SymbolWindow(editorViewProject.CurrentPage, Canvas, start, end.X - start.X, end.Y - start.Y);

            editorViewProject.AddToSymbolWindows(editorViewProject.CurrentPage, symbol);

            Refresh();
        }
        #endregion


        #region command
        void AddRectangle(Manuscript.SymbolWindow symbol)
        {
            WindowControl window = new WindowControl();
            window.Width = symbol.CanvasWidth;
            window.Height = symbol.CanvasHeight;

            if (symbol.Active)
                window.Rectangle.BorderBrush = Brushes.BlueViolet;
            else
                window.Rectangle.BorderBrush = Brushes.Lime;

            Canvas.Children.Add(window);
            Canvas.SetLeft(window, symbol.CanvasCoordinates.X);
            Canvas.SetTop(window, symbol.CanvasCoordinates.Y);
        }

        void Clear()
        {
            Canvas.Children.Clear();
        }

        public void Refresh()
        {
            Clear(); 

            if (editorViewProject.CurrentPage == null) return; 

            foreach (Manuscript.SymbolWindow symbol in editorViewProject.getSymbolWindows(editorViewProject.CurrentPage))
            {
                AddRectangle(symbol);
            }
        }

        #endregion

       

        

       

        

        

       

        

       
    }

    public enum Operation
    {
        NewRectangle,
        Union,
        Split,
        Explore,
        Delete,
        None
    }
}
