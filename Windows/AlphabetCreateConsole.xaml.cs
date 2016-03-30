using ISRMUL.Recognition.Neokognitron;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace ISRMUL.Windows
{
    /// <summary>
    /// Логика взаимодействия для AlphabetCreateConsole.xaml
    /// </summary>
    public partial class AlphabetCreateConsole : Window
    {
        private Manuscript.Project Project;

        private Recognition.Alphabet.AlphabetMaker AlphabetMaker { get; set; }

        private List<string> Logs { get; set; }

         
        public AlphabetCreateConsole(Manuscript.Project alphabetEditorViewProject)
        {
            InitializeComponent();

            // TODO: Complete member initialization
            this.Project = alphabetEditorViewProject;
            this.AlphabetMaker = new Recognition.Alphabet.AlphabetMaker(Project);
            Logs = new List<string>();
            setState();
        }


        #region methods

        void setState()
        {
            if (Project.NeoState == Recognition.Neokognitron.NeokognitronState.Null)
            {
                LearningButton.IsEnabled = true;
                MakeButton.IsEnabled = false;
            }
            else
            {
                LearningButton.IsEnabled = true;
                MakeButton.IsEnabled = true;
            }
        }

        Task learningFirstLevel(List<double[,]> traindData)
        {
            ConsoleBox.Items.Clear();

            Logger log = new Logger((s1, s2) =>
            {
                Logs.Add(s1 + " : " + s2);
            });
            AddIfRule trainer = new AddIfRule(traindData, 2, 2.5, Project.Neokognitron, 0.45, 0.5, NeoKognitron.U1C, NeoKognitron.GenerateMexicanHat(0.7, 3, -25.4, 2.5, 3.5), NeoKognitron.GenerateGaussianKernel(0.7, 1.1, 1), log);
            //trainer.Train();
            var timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 0, 0, 200);
            timer.Tick += (a, b) => { printLastLog(); };
            timer.Start();
            return Task.Run(() =>
            {
                trainer.Train();
                timer.Stop();
            });
        }
        Task learningSecondLevel(List<double[,]> traindData)
        {
            ConsoleBox.Items.Clear();

            Logger log = new Logger((s1, s2) =>
            {
                Logs.Add(s1 + " : " + s2);
            });
            AddIfRule trainer = new AddIfRule(traindData, 2, 2.5, Project.Neokognitron, 0.45, 0.5, NeoKognitron.U1C, NeoKognitron.GenerateMexicanHat(0.7, 3, -25.4, 2.5, 3.5), NeoKognitron.GenerateGaussianKernel(0.7, 1.1, 1), log);
            //trainer.Train();
            var timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 0, 0, 100);
            timer.Tick += (a, b) => { printLastLog(); };
            timer.Start();
            return Task.Run(() =>
            {
                trainer.Train();
                timer.Stop();
            });
        }

        void printLastLog()
        {
            var lasts = Logs.ToList();
            foreach (var log in lasts)
            {
                AddLog(log);
                Logs.Remove(log);
            }

            ScrollView.ScrollToEnd();
        }
        void AddLog(string s)
        {
            TextBlock block = new TextBlock();
            block.Text = s;
            block.FontSize = 14;

            ConsoleBox.Items.Add(block);
        }
        #endregion

        #region event handler
        private async void LearningButton_Click_1(object sender, RoutedEventArgs e)
        {
            Project.Neokognitron = new NeoKognitron(31, 31, new int[] { 16 }, new double[] { 0.5, 0.62, 0.45 });
            Project.Neokognitron.init();

            var data = Project.KnowledgeBase.Select(x => x.toRetina(31,31)).ToList();
            await learningFirstLevel(data);
            await learningSecondLevel(data);

            Project.NeoState = NeokognitronState.FeatureExtractor;
            setState();
        }
        #endregion
    }
}
