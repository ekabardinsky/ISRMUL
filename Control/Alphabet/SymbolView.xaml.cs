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

namespace ISRMUL.Control.Alphabet
{
    /// <summary>
    /// Логика взаимодействия для AlphabetView.xaml
    /// </summary>
    public partial class SymbolView : UserControl
    {
        public Manuscript.SymbolWindow symbol;
        Manuscript.Project currentProject;
        AlphabetEditorView tool;
        public SymbolView(Manuscript.SymbolWindow symbol, Manuscript.Project project, AlphabetEditorView tool)
        {
            this.tool = tool;
            this.symbol = symbol;
            InitializeComponent();
            currentProject = project;
            if (symbol != null)
                BackgroundBrush.ImageSource = symbol.toImage();

            if (symbol.Active)
                Borders.BorderBrush = Brushes.BlueViolet;
        }

        private void Grid_PreviewMouseDown_1(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                symbol.Active ^= true;
                tool.Refresh();
            }
        }

        private void Grid_PreviewMouseMove_1(object sender, MouseEventArgs e)
        {
            MainGrid.Background = new SolidColorBrush(Color.FromArgb(100,20,200,20));
        }

        private void MainGrid_MouseLeave_1(object sender, MouseEventArgs e)
        {
            MainGrid.Background = null;
        }
    }
}
