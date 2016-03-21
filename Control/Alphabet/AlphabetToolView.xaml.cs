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
    /// Логика взаимодействия для AlphabetToolView.xaml
    /// </summary>
    public partial class AlphabetToolView : UserControl
    {
        public AlphabetToolView()
        {
            InitializeComponent();
        }

        #region dependency
        public static readonly DependencyProperty alphabetToolViewProjectProperty = DependencyProperty.Register("alphabetToolViewProject", typeof(Manuscript.Project), typeof(AlphabetToolView));
        public Manuscript.Project alphabetToolViewProject
        {
            get
            {
                return this.GetValue(alphabetToolViewProjectProperty) as Manuscript.Project;
            }
            set
            {
                this.SetValue(alphabetToolViewProjectProperty, value);
            }
        }
        #endregion
    }
}
