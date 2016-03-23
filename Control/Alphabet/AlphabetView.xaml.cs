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
    public partial class AlphabetView : UserControl
    {
        Manuscript.Alphabet alphabet;
        Manuscript.Project currentProject;
        AlphabetToolView tool;
        public AlphabetView(Manuscript.Alphabet alphabet, Manuscript.Project project, AlphabetToolView tool)
        {
            this.tool = tool;
            this.alphabet = alphabet;
            InitializeComponent();
            currentProject = project;
            if (alphabet.MainSymbol != null)
                BackgroundBrush.ImageSource = alphabet.MainSymbol.toImage();

            if (currentProject.CurrentAlphabet == alphabet)
                Borders.BorderBrush = Brushes.BlueViolet;
        }

        private void Grid_PreviewMouseDown_1(object sender, MouseButtonEventArgs e)
        {
            currentProject.CurrentAlphabet = alphabet;
            tool.Refresh();
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
