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
        public static readonly DependencyProperty CurrentProjectProperty = DependencyProperty.Register("editorViewProject", typeof(Manuscript.Project), typeof(EditorViewControl));
        public Manuscript.Project editorViewProject
        {
            get
            {
                return this.GetValue(CurrentProjectProperty) as Manuscript.Project;
            }
            set
            {
                this.SetValue(CurrentProjectProperty, value);
            }
        }
        #endregion

        #region event handler
        private void Canvas_MouseDown_1(object sender, MouseButtonEventArgs e)
        {
            Manuscript.SymbolWindow symbol = new Manuscript.SymbolWindow(FindResource("Image") as Image,FindResource("Canvas") as Canvas,e.GetPosition(sender as Canvas),40,40);
            AddRectangle(symbol);
        }
        #endregion

        #region command

        void AddRectangle(Manuscript.SymbolWindow symbol)
        {

        } 
        
        public void Refresh()
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    enum Operation
    {
        NewRectangle,
        Concatinate,
        Split
    }
}
