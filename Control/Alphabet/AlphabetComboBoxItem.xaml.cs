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
    /// Логика взаимодействия для AlphabetComboBoxItem.xaml
    /// </summary>
    public partial class AlphabetComboBoxItem : UserControl
    {
        public Manuscript.Alphabet alphabet;
        public AlphabetComboBoxItem(Manuscript.Alphabet alpha)
        {
            InitializeComponent();
            alphabet = alpha;
            Image.Source = alpha.MainSymbol.toImage()??null;
            Text.Text = alpha.Code;
        }
    }
}
