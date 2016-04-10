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
    /// Логика взаимодействия для TextEditor.xaml
    /// </summary>
    public partial class TextEditor : UserControl
    {
        public TextEditor()
        {
            InitializeComponent();
        }


        #region dependency
        public static readonly DependencyProperty textProjectProperty = DependencyProperty.Register("textProject", typeof(Manuscript.Project), typeof(TextEditor));
        public Manuscript.Project textProject
        {
            get
            {
                return this.GetValue(textProjectProperty) as Manuscript.Project;
            }
            set
            {
                this.SetValue(textProjectProperty, value);
            }
        }
        #endregion

        #region event handlers

        private void RecognizeButton_Click_1(object sender, RoutedEventArgs e)
        {
            if (textProject.NeoState == Recognition.Neokognitron.NeokognitronState.Null)
            {
                MessageBox.Show("Для распознавания необходимо построить модуль \"Неокогнитрон\"");
                Windows.LearnConsole console = new Windows.LearnConsole(textProject, true);
                console.Show();
            }
            if (textProject.NeoState == Recognition.Neokognitron.NeokognitronState.FeatureExtractor)
            {
                MessageBox.Show("Для распознавания необходимо обучить модуль \"Неокогнитрон\"");
                Windows.LearnConsole console = new Windows.LearnConsole(textProject, false);
                console.Show();
            }
            if (textProject.NeoState == Recognition.Neokognitron.NeokognitronState.NonActual)
            {
                if (MessageBox.Show("Модуль распознавания использует не актуальные знания. Переобучить ?", "Переобучение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    Windows.LearnConsole console = new Windows.LearnConsole(textProject, false);
                    console.Show();
                }
                else
                {
                    recognize();
                }
            }
            if (textProject.NeoState == Recognition.Neokognitron.NeokognitronState.Ready)
            {
                recognize();
            }
        }

        #endregion

        #region

        void add(Manuscript.SymbolWindow symbol, string label)
        {
            SymbolView view = new SymbolView(symbol.toImage(), label, symbol, textProject);

            view.Height = 100;
            view.Width = 100;

            TextWrapPanel.Children.Add(view);
        }

        void recognize()
        {
            var all = getOrderedSymbol();
            var labels = new List<string>();
            TextWrapPanel.Children.Clear();

            foreach (var item in all)
            {
                string label = item.recognize();
                labels.Add(label);
                add(item, label);
            }

            //text formatter
            TextFormatter formatter = new TextFormatter(all, labels);
            formatter.Init();
            RecognizedText.Text = formatter.Compute();
        }

        List<Manuscript.SymbolWindow> getOrderedSymbol()
        {
            List<Manuscript.SymbolWindow> ordered = new List<Manuscript.SymbolWindow>();
            var all = textProject.getSymbolWindows(textProject.getCurrentKey()).ToList();
            for (int i = all.Count - 1; i >= 0; i--)
            {
                ordered.Add(getNextSymbol(all));
            }

            return ordered;
        }

        Manuscript.SymbolWindow getNextSymbol(List<Manuscript.SymbolWindow> exists)
        {
            double h = exists.Sum(x => x.RealHeight / exists.Count);
            Manuscript.SymbolWindow first = exists[0];
            bool moved = true;
            while (moved)
            {
                moved = false;
                foreach (var item in exists)
                {
                    if (item.RealCoordinates.X < first.RealCoordinates.X && Math.Abs(item.RealCoordinates.Y - first.RealCoordinates.Y) < h / 4 * 3)
                    {
                        first = item;
                        moved = true;
                    }
                    if (item.RealCoordinates.Y < first.RealCoordinates.Y - h / 4 * 3)
                    {
                        first = item;
                        moved = true;
                    }
                }
            }

            exists.Remove(first);

            return first;
        }

        #endregion
    }
}
