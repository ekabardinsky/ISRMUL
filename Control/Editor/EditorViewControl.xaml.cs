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
        Operation currentOperation;

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
            Manuscript.SymbolWindow symbol = new Manuscript.SymbolWindow(editorViewProject.CurrentPage, Canvas, e.GetPosition(Canvas), 45, 45);

            editorViewProject.AddToSymbolWindows(editorViewProject.CurrentPage,symbol);

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
        Explore
    }
}
