using ISRMUL.Control.Text;
using ISRMUL.Manuscript;
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
using System.Windows.Shapes;

namespace ISRMUL.Windows
{
    /// <summary>
    /// Логика взаимодействия для ToKnowledgeBaseConsole.xaml
    /// </summary>
    public partial class ToKnowledgeBaseConsole : Window
    {
        Project current { get; set; }
        SymbolWindow window { get; set; }
        bool full;

        public ToKnowledgeBaseConsole(bool full,Project p, SymbolWindow symbol)
        {
            InitializeComponent();

            this.full = full;
            window = symbol;
            current = p;
            Title = full ? "Консоль переобучения" : "Консоль быстрого исправления";
            fill(full);
        }

        void fill(bool full)
        {
            //window
            MainSymbol.SymbolImage.Source = window.toImage();
            //combo
            if (full)
            {
                foreach (var item in current.Alphabets)
                {
                    ISRMUL.Control.Alphabet.AlphabetComboBoxItem comboItem = new Control.Alphabet.AlphabetComboBoxItem(item);
                    MainComboBox.Items.Add(comboItem);
                }
            }
            else
            {
                foreach (Recognition.Neokognitron.SInterploating item in current.Neokognitron.U[4].S)
                {
                    MainComboBox.Items.Add(item);
                }
            }
        }

        private void FastLearnButton_Click_1(object sender, RoutedEventArgs e)
        {
            current.KnowledgeBase.Add(window);
            var s = MainComboBox.SelectedItem as Recognition.Neokognitron.SInterploating;
            if (s == null)
                return;
            var feature = current.Neokognitron.getFeatures(window.toRetina(Project.patternWidth, Project.patternHeight));
            s.Clazz.AddReferenceVector(new Recognition.Neokognitron.Vector(feature));
            current.NeoState = Recognition.Neokognitron.NeokognitronState.NonActual;
            window.cachedCode = s.ToString();
            this.Close();
        }

        private void FullLearnButton_Click_1(object sender, RoutedEventArgs e)
        {
            current.KnowledgeBase.Add(window);
            var alpha = MainComboBox.SelectedItem as Control.Alphabet.AlphabetComboBoxItem;
            alpha.alphabet.Symbols.Add(window);
            
            Windows.LearnConsole console = new LearnConsole(current, false);
            console.ShowDialog();
        }

        private void LearnButton_Click_1(object sender, RoutedEventArgs e)
        {
            if (full)
                FullLearnButton_Click_1(null, null);
            else
                FastLearnButton_Click_1(null, null);
        }
    }
}
