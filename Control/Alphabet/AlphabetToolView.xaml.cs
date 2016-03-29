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
        static int lastId;
        public AlphabetEditorView Editor;
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
            alphabetToolViewProject.Alphabets.Add(new Manuscript.Alphabet() { Code="#"+(lastId++) });
            Refresh();
        }

        private void removeButton_Click_1(object sender, RoutedEventArgs e)
        {
            if (alphabetToolViewProject.CurrentAlphabet != null)
            {
                alphabetToolViewProject.Alphabets.Remove(alphabetToolViewProject.CurrentAlphabet);
                Refresh();
            }
        }

        private void unionButton_Click_1(object sender, RoutedEventArgs e)
        {

        }
        private void TextBox_TextChanged_1(object sender, TextChangedEventArgs e)
        {
            TextBox box = sender as TextBox;
            if (alphabetToolViewProject.CurrentAlphabet != null)
            {
                alphabetToolViewProject.CurrentAlphabet.Code = box.Text;
                AlphabetWrapPanel.Children.Cast<AlphabetView>().Where(x => x.alphabet == alphabetToolViewProject.CurrentAlphabet).First().Text.Text = box.Text;
            }
        }
        #endregion 

        #region commands

        void Add(Manuscript.Alphabet alphabet)
        {
            AlphabetView view = new AlphabetView(alphabet, alphabetToolViewProject,this);
            view.Width = 100;
            view.Height = 100;
            view.Text.Text = alphabet.Code;
            AlphabetWrapPanel.Children.Add(view);
        }

        bool flag = false;
        public void Refresh()
        {
            if (flag)
                return;
            AlphabetWrapPanel.Children.Clear();
            foreach (var a in alphabetToolViewProject.Alphabets)
                Add(a);

            flag = true;
            Editor.Refresh();
            flag = false;
        }

        #endregion

        
    }
}
