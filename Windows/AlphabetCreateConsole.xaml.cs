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


        private List<string> Logs { get; set; }

         
        public AlphabetCreateConsole(Manuscript.Project alphabetEditorViewProject, bool isSegmentation)
        {
            InitializeComponent();

            // TODO: Complete member initialization
            this.Project = alphabetEditorViewProject;
            Logs = new List<string>();
            setState();
            LearnInterploatingButton.Visibility = isSegmentation ? System.Windows.Visibility.Hidden : System.Windows.Visibility.Visible;
            MakeButton.Visibility = !isSegmentation ? System.Windows.Visibility.Collapsed : System.Windows.Visibility.Visible;
        }


        #region methods

        void setState()
        {
            if (Project.NeoState == Recognition.Neokognitron.NeokognitronState.Null)
            {
                LearningButton.IsEnabled = true;
                LearnInterploatingButton.IsEnabled = false;
                MakeButton.IsEnabled = false;
            }
            else
            {
                LearningButton.IsEnabled = true;
                LearnInterploatingButton.IsEnabled = true;
                MakeButton.IsEnabled = true;
            }

            if (Project.Alphabets.Count == 0)
                LearnInterploatingButton.IsEnabled = false;
        }

        Task learningFirstLevel(List<double[,]> traindData)
        {
            ConsoleBox.Items.Clear();

            Logger log = new Logger((s1, s2) =>
            {
                Logs.Add(s1 + " : " + s2);
            });
            AddIfRule trainer = new AddIfRule(traindData, 2, 1.4, Project.Neokognitron, 0.58, 0.1, NeoKognitron.GenerateGaussianKernel(0.7, 2.5, 2), NeoKognitron.GenerateGaussianKernel(0.7, 2.5, 2), NeoKognitron.GenerateMexicanHat(0.8, 2, -1, 1.5, 2.5), log);
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
            AddIfRule trainer = new AddIfRule(traindData, 3, 1.4, Project.Neokognitron, 0.58, 0.001, NeoKognitron.GenerateGaussianKernel(0.7, 2.5, 2), NeoKognitron.GenerateGaussianKernel(0.7, 2.5, 2), NeoKognitron.GenerateMexicanHat(0.8, 2, -1, 1.5, 2.5), log);
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
        Task learningInterploatingLevel(List<double[,]> traindData, List<string> labels)
        {
            ConsoleBox.Items.Clear();

            Logger log = new Logger((s1, s2) =>
            {
                Logs.Add(s1 + " : " + s2);
            });

            InterploatingTrainer trainer = new InterploatingTrainer(Project.Neokognitron, traindData, labels, 4, log);

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
            Project.Neokognitron = new NeoKognitron(Manuscript.Project.patternWidth, Manuscript.Project.patternHeight, new int[] { 16 }, new double[] { 0.5});
            Project.Neokognitron.init();

            var data = Project.KnowledgeBase.Select(x => x.toRetina(Manuscript.Project.patternWidth, Manuscript.Project.patternHeight)).ToList();
            await learningFirstLevel(data);
            await learningSecondLevel(data);

            Project.NeoState = NeokognitronState.FeatureExtractor;
            setState();
        }

        private async void LearnInterploatingButton_Click_1(object sender, RoutedEventArgs e)
        {
            var labels = Project.KnowledgeBase.Select(x => x.getLabelFromAlphabet()).ToList();
            var data = Project.KnowledgeBase.Select(x => x.toRetina(Manuscript.Project.patternWidth, Manuscript.Project.patternHeight)).ToList();

            await learningInterploatingLevel(data, labels);

            Project.NeoState = NeokognitronState.Ready;
            setState();
        }

        private void MakeButton_Click_1(object sender, RoutedEventArgs e)
        {
            int classes;
            if (!int.TryParse(ClassNumber.Text, out classes))
            {
                MessageBox.Show("Не верный формат числа");
                return;
            }
            var vectors = Project.KnowledgeBase
                .Select(x => x.toRetina(Manuscript.Project.patternWidth, Manuscript.Project.patternHeight))
                .Select(x => Project.Neokognitron.getFeatures(x))
                .Select(x=>new ISRMUL.Recognition.KMeansPlus.Vector(x.Length,x))
                .ToList();

            for (int i = 0; i < Project.KnowledgeBase.Count; i++)
                vectors[i].Tag = Project.KnowledgeBase[i];

            ISRMUL.Recognition.KMeansPlus.KMeans means = new Recognition.KMeansPlus.KMeans(classes, vectors, new ISRMUL.Recognition.KMeansPlus.Euclidean2());
            means.InitializeAlphabetCentroids(new ISRMUL.Recognition.KMeansPlus.Euclidean2());
            means.Proccess(1000);


            Project.Alphabets.Clear();
            for (int i = 0; i < means.Clusters.Count; i++)
            {
                var alphabet = new Manuscript.Alphabet();
                alphabet.Code = "#"+(i+1);
                foreach (var symbol in means.Clusters[i].Vectors)
                    alphabet.Symbols.Add(symbol.Tag as Manuscript.SymbolWindow);

                Project.Alphabets.Add(alphabet);
            }

            Project.Refresh();
           
        }

        #endregion

        

        
    }
}
