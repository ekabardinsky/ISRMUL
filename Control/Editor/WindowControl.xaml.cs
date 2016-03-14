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
    /// Логика взаимодействия для WindowControl.xaml
    /// </summary>
    public partial class WindowControl : UserControl
    {
        public Manuscript.SymbolWindow Symbol { get; set; }

        public WindowControl()
        {
            InitializeComponent(); 
        }

        public static readonly DependencyProperty VisibleProperty = DependencyProperty.Register("Visible", typeof(System.Windows.Visibility), typeof(WindowControl), new FrameworkPropertyMetadata(System.Windows.Visibility.Hidden));
        public System.Windows.Visibility Visible
        {
            get
            {
                return (System.Windows.Visibility)this.GetValue(VisibleProperty);
            }
            set
            {
                this.SetValue(VisibleProperty, value);
            }
        }
    }
}
