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
        #endregion

        #region visual tool

        void Split(Point start, Point end)
        {
            Point center = new Point((start.X + end.X) / 2, (start.Y + end.Y) / 2);

            List<Manuscript.SymbolWindow> windows = editorViewProject.getSymbolWindows(editorViewProject.CurrentPage);

            /// insert code here
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
            if (editorViewProject.CurrentPage == null) return;
            Clear(); 

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
        None
    }
}
