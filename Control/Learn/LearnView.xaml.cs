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

namespace ISRMUL.Control.Learn
{
    /// <summary>
    /// Логика взаимодействия для LearnView.xaml
    /// </summary>
    public partial class LearnView : UserControl
    {
        public LearnView()
        {
            InitializeComponent();
        }

        #region dependency

        public static readonly DependencyProperty learnViewProjectProperty = DependencyProperty.Register("learnViewProject", typeof(Manuscript.Project), typeof(LearnView));
        public Manuscript.Project learnViewProject
        {
            get
            {
                return this.GetValue(learnViewProjectProperty) as Manuscript.Project;
            }
            set
            {
                this.SetValue(learnViewProjectProperty, value);
            }
        }

        #endregion
    }
}
