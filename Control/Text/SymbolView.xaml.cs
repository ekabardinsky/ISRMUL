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
        public SymbolView(BitmapSource source, string code)
        {
            InitializeComponent();
            SymbolImage.Source = source;
            Code.Text = code;
        }
    }
}
