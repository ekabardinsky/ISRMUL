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
    public partial class AlphabetEditorView : UserControl, Manuscript.IRefreshable
    {
        public AlphabetEditorView()
        {
            InitializeComponent();
            AlphabetTool.Editor = this;
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

        #region command

        void Add(Manuscript.SymbolWindow window, WrapPanel panel)
        {
            SymbolView view = new SymbolView(window, alphabetEditorViewProject, this);
            view.Width = 100;
            view.Height = 100;
            panel.Children.Add(view);
        }

        public void Refresh()
        {
            AlphabetTool.Refresh();
            SymbolWrapPanel.Children.Clear();

            var key = alphabetEditorViewProject.getCurrentKey();
            if (key == null) return;
            var symbol = alphabetEditorViewProject.getSymbolWindows(key);
            foreach (var item in symbol)
                if (!(alphabetEditorViewProject.Alphabets.Any(x => x.Symbols.Any(y => y == item)) || alphabetEditorViewProject.KnowledgeBase.Any(x => x == item)))
                    Add(item, SymbolWrapPanel);

            //current alphabet
            CurrentAlphabetWrapPanel.Children.Clear();
            if(alphabetEditorViewProject.CurrentAlphabet!=null){ 
                foreach (var item in alphabetEditorViewProject.CurrentAlphabet.Symbols)
                    Add(item, CurrentAlphabetWrapPanel);
            }

            //knowledge
            KnowledgeBaseWrapPanel.Children.Clear();
            foreach (var item in alphabetEditorViewProject.KnowledgeBase)
                Add(item, KnowledgeBaseWrapPanel);

            //comboBox
            AlphabetCombo.Items.Clear();
            foreach (var alpha in alphabetEditorViewProject.Alphabets)
                AlphabetCombo.Items.Add(new AlphabetComboBoxItem(alpha));
        }
        #endregion

        #region event handle

        private void toCurrentAlphabetButton_Click_1(object sender, RoutedEventArgs e)
        {
            if (alphabetEditorViewProject.CurrentAlphabet != null)
            {

                var symbols = SymbolWrapPanel.Children.Cast<SymbolView>().Where(x => x.symbol.Active).Select(x => x.symbol);

                foreach (var item in symbols)
                    alphabetEditorViewProject.CurrentAlphabet.Symbols.Add(item);

                alphabetEditorViewProject.KnowledgeBase.AddRange(symbols);
                alphabetEditorViewProject.getSymbolWindows(alphabetEditorViewProject.getCurrentKey()).ForEach(x => x.Active = false);

                Refresh();
            }
        }

        private void createAlphabetButton_Click_1(object sender, RoutedEventArgs e)
        {
            Windows.AlphabetCreateConsole console = new Windows.AlphabetCreateConsole(alphabetEditorViewProject);
            console.ShowDialog();
            Refresh();
        }

        private void toBaseButton_Click_1(object sender, RoutedEventArgs e)
        {
            var symbols = SymbolWrapPanel.Children.Cast<SymbolView>().Where(x => x.symbol.Active).Select(x => x.symbol);
            alphabetEditorViewProject.KnowledgeBase.AddRange(symbols);
            alphabetEditorViewProject.getSymbolWindows(alphabetEditorViewProject.getCurrentKey()).ForEach(x => x.Active = false);
            
            Refresh();
        }

        private void toCurrentAlphabetButton1_Click_1(object sender, RoutedEventArgs e)
        {
            if (alphabetEditorViewProject.CurrentAlphabet != null)
            {
                var symbols = KnowledgeBaseWrapPanel.Children.Cast<SymbolView>().Where(x => x.symbol.Active).Select(x => x.symbol);

                foreach (var item in symbols)
                    alphabetEditorViewProject.CurrentAlphabet.Symbols.Add(item);

                alphabetEditorViewProject.getSymbolWindows(alphabetEditorViewProject.getCurrentKey()).ForEach(x => x.Active = false);

                Refresh();
            }
        }

        private void deleteSymbolFromBaseButton_Click_1(object sender, RoutedEventArgs e)
        {
           var symbols = KnowledgeBaseWrapPanel.Children.Cast<SymbolView>().Where(x => x.symbol.Active).Select(x => x.symbol);

           foreach (var item in symbols)
               alphabetEditorViewProject.KnowledgeBase.Remove(item);

           if (alphabetEditorViewProject.Alphabets != null)
           {
               foreach (var alpha in alphabetEditorViewProject.Alphabets)
               {
                   foreach (var item in symbols)
                   {
                       alpha.Symbols.Remove(item);
                   }
               }
           }
           Refresh();
        }

        private void changeAlphabetButton_Click_1(object sender, RoutedEventArgs e)
        {
            var symbols = CurrentAlphabetWrapPanel.Children.Cast<SymbolView>().Where(x => x.symbol.Active).Select(x => x.symbol);
            var alphabet = (AlphabetCombo.SelectedItem as AlphabetComboBoxItem).alphabet;

            if (alphabet == null) return;
            foreach (var s in symbols)
            {
                foreach (var alpha in alphabetEditorViewProject.Alphabets)
                    alpha.Symbols.Remove(s);
            }

            foreach (var s in symbols)
                alphabet.Symbols.Add(s);
            Refresh();
        }

        private void removeFromCurrentAlphabetButton_Click_1(object sender, RoutedEventArgs e)
        {
            var symbols = CurrentAlphabetWrapPanel.Children.Cast<SymbolView>().Where(x => x.symbol.Active).Select(x => x.symbol);
            var alpha = alphabetEditorViewProject.CurrentAlphabet;

            foreach (var s in symbols)
            {
                alpha.Symbols.Remove(s);
                alphabetEditorViewProject.KnowledgeBase.Remove(s);
            }

            Refresh();
        }

        #endregion

        

       

        


    }
}
