using ISRMUL.Manuscript;
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

namespace ISRMUL.Control.Text
{
    /// <summary>
    /// Логика взаимодействия для SymbolView.xaml
    /// </summary>
    public partial class SymbolView : UserControl
    {
        Manuscript.SymbolWindow window;
        Project current;

        public SymbolView(BitmapSource source, string code, Manuscript.SymbolWindow window, Project p)
        {
            InitializeComponent();
            SymbolImage.Source = source;
            Code.Text = code;
            this.window = window;
            current = p;
        }
        public SymbolView()
        {
            InitializeComponent();
        }

        private void Grid_PreviewMouseMove_1(object sender, MouseEventArgs e)
        {
            MainGrid.Background = new SolidColorBrush(Color.FromArgb(100, 20, 200, 20));
        }

        private void Grid_MouseLeave_1(object sender, MouseEventArgs e)
        {
            MainGrid.Background = null;
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            Windows.ToKnowledgeBaseConsole console = new Windows.ToKnowledgeBaseConsole(false, current, window);
            console.Show();
        }

        private void MenuItem_Click_2(object sender, RoutedEventArgs e)
        {

            Windows.ToKnowledgeBaseConsole console = new Windows.ToKnowledgeBaseConsole(true, current, window);
            console.Show();
        }
    }
}
