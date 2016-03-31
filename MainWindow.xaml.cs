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
using ISRMUL.Control.Page;
using Microsoft.Win32;

namespace ISRMUL
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
            ProjectReady = false;
            Editor.alphabetEditor = AlphabetEditor;

            registerCommand(ApplicationCommands.New, NewProjectMenuItem_Click);
            registerCommand(ApplicationCommands.Open, OpenProjectMenuItem_Click);
            registerCommand(ApplicationCommands.Save, SaveProjectMenuItem_Click);
            registerCommand(ApplicationCommands.Close, CloseProjectMenuItem_Click_1);
        }

        void registerCommand(ICommand command, ExecutedRoutedEventHandler handler)
        {
            CommandBinding binding = new CommandBinding(command);
            binding.Executed += handler;
            this.CommandBindings.Add(binding);
        } 

        #region event handlers

        private void CloseProjectMenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            if (ProjectReady)
            {
                var answer = MessageBox.Show("Сохранить проект ?", "Выход", MessageBoxButton.YesNoCancel);
                if (answer == MessageBoxResult.Cancel) return;
                else if (answer == MessageBoxResult.No) CloseProject();
                else saveProject();
            }
        }
        private void NewProjectMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (ProjectReady)
                trySave();
            newProject();
        }
        private void OpenProjectMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (ProjectReady)
                trySave();
            openProject();
        }

        private void SaveProjectMenuItem_Click(object sender, RoutedEventArgs e)
        {
            saveProject();
        }
        #endregion

        #region commands

        void trySave()
        {
            if (MessageBox.Show("Есть не сохраненные изменения, сохранить?","Сохранить изменения",MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                saveProject();
            }
        }
        void newProject()
        {
            CurrentProject = new Manuscript.Project(Editor.Canvas, new Manuscript.IRefreshable[] { Pages, Editor, AlphabetEditor });
            ProjectReady = true;
            CurrentProject.Refresh();
        }

        void saveProject()
        {
            SaveFileDialog dialog = new SaveFileDialog(); 
            dialog.Filter = "Project files (*.pro) | *.pro";
            if (dialog.ShowDialog() == true)
            {
                Manuscript.Project.Serialize(dialog.FileName, CurrentProject);
            }
        }

        void openProject()
        {
            OpenFileDialog dialog = new OpenFileDialog(); 
            dialog.Filter = "Project files (*.pro) | *.pro";
            if (dialog.ShowDialog() == true)
            {
                CurrentProject = Manuscript.Project.DeSerialize(dialog.FileName, Editor.Canvas, new Manuscript.IRefreshable[] { Pages, Editor, AlphabetEditor });
                CurrentProject.Refresh();
                ProjectReady = true;
            }
            else
            {
                CloseProject();
            }
        }

        void CloseProject()
        {
            ProjectReady = false;
            CurrentProject = null;
            
        }

        #endregion

        #region dependency

        public static readonly DependencyProperty CurrentProjectProperty = DependencyProperty.Register("CurrentProject", typeof(Manuscript.Project), typeof(MainWindow));
        public Manuscript.Project CurrentProject
        {
            get
            {
                return this.GetValue(CurrentProjectProperty) as Manuscript.Project;
            }
            set
            {
                this.SetValue(CurrentProjectProperty, value);
            }
        }

        public static readonly DependencyProperty ProjectReadyProperty = DependencyProperty.Register("ProjectReady", typeof(Boolean), typeof(MainWindow), new FrameworkPropertyMetadata(false));
        public Boolean ProjectReady
        {
            get
            {
                return (Boolean)this.GetValue(ProjectReadyProperty);
            }
            set
            {
                this.SetValue(ProjectReadyProperty, value);
            }
        }

        #endregion

        private void MainForm_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (ProjectReady)
            {
                var answer = MessageBox.Show("Сохранить проект ?", "Выход", MessageBoxButton.YesNoCancel);
                if (answer == MessageBoxResult.Cancel) e.Cancel = true;
                else if (answer == MessageBoxResult.No) CloseProject();
                else saveProject();
            }
        }

        

        
    }
}
