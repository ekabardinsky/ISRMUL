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
    /// Логика взаимодействия для AlphabetEditorView.xaml
    /// </summary>
    public partial class AlphabetEditorView : UserControl
    {
        public AlphabetEditorView()
        {
            InitializeComponent();
        }

        #region dependency
        public static readonly DependencyProperty alphabetEditorViewProjectProperty = DependencyProperty.Register("alphabetEditorViewProject", typeof(Manuscript.Project), typeof(AlphabetEditorView));
        public Manuscript.Project alphabetEditorViewProject
        {
            get
            {
                return this.GetValue(alphabetEditorViewProjectProperty) as Manuscript.Project;
            }
            set
            {
                this.SetValue(alphabetEditorViewProjectProperty, value);
            }
        }
        #endregion
    }
}
