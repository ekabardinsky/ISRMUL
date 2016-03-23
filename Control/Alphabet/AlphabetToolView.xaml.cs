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
    public partial class AlphabetToolView : UserControl,Manuscript.IRefreshable
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


        #region event handler
        private void addButton_Click_1(object sender, RoutedEventArgs e)
        {
            alphabetToolViewProject.Alphabets.Add(new Manuscript.Alphabet());
            Refresh();
        }

        private void removeButton_Click_1(object sender, RoutedEventArgs e)
        {

        }

        private void unionButton_Click_1(object sender, RoutedEventArgs e)
        {

        }
        #endregion 

        #region commands

        void Add(Manuscript.Alphabet alphabet)
        {
            AlphabetView view = new AlphabetView(alphabet, alphabetToolViewProject,this);
            view.Width = 100;
            view.Height = 100;
            AlphabetWrapPanel.Children.Add(view);
        }

        public void Refresh()
        {
            AlphabetWrapPanel.Children.Clear();
            foreach (var a in alphabetToolViewProject.Alphabets)
                Add(a);
        }

        #endregion
    }
}
