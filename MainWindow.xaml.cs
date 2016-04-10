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
        }

        #region command handler

        private void CloseProjectCommand(object sender, RoutedEventArgs e)
        {
            if (ProjectReady)
            {
                var answer = MessageBox.Show("Сохранить проект ?", "Выход", MessageBoxButton.YesNoCancel);
                if (answer == MessageBoxResult.Cancel) return;
                else if (answer == MessageBoxResult.No) CloseProject();
                else saveProject();
            }
        }
        private void NewProjectCommand(object sender, RoutedEventArgs e)
        {
            if (ProjectReady)
                trySave();
            newProject();
        }
        private void OpenProjectCommand(object sender, RoutedEventArgs e)
        {
            if (ProjectReady)
                trySave();
            openProject();
        }
        private void SaveProjectCommand(object sender, RoutedEventArgs e)
        {
            saveProject();
        }

        private void InsertCommand(object sender, RoutedEventArgs e)
        {
            if (ProjectReady)
                Pages.Insert_Click(null, null);
        }
        private void KBSaveCommand(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "Knowledge Base files (*.kb) | *.kb";
            if (dialog.ShowDialog() == true)
            {
                Manuscript.Project.SerializeKB(dialog.FileName, CurrentProject);
            }
        }
        private void KBLoadCommand(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Knowledge Base files (*.kb) | *.kb";
            if (dialog.ShowDialog() == true)
            {
                CurrentProject.KnowledgeBase = Manuscript.Project.DeSerializeKB(dialog.FileName, CurrentProject);
                CurrentProject.Refresh(); 
            }
            
        }
        private void KBImportCommand(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Knowledge Base files (*.kb) | *.kb";
            if (dialog.ShowDialog() == true)
            {
                CurrentProject.KnowledgeBase.AddRange(Manuscript.Project.DeSerializeKB(dialog.FileName, CurrentProject));
                CurrentProject.Refresh();
            }
        }

        private void LearnCommand(object sender, RoutedEventArgs e)
        {
            Windows.LearnConsole console = new Windows.LearnConsole(CurrentProject, false);
            console.Show();
            CurrentProject.Refresh();
        }
        private void CreateAlphabetCommand(object sender, RoutedEventArgs e)
        {
            Windows.LearnConsole console = new Windows.LearnConsole(CurrentProject, true);
            console.Show();
            CurrentProject.Refresh();
        }


        #endregion

        #region event handlers
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

       

        

        
    }

    public static class CustomCommands
    {
        public static readonly RoutedUICommand KBSave = new RoutedUICommand();
        public static readonly RoutedUICommand KBLoad = new RoutedUICommand();
        public static readonly RoutedUICommand KBImport = new RoutedUICommand();

        public static readonly RoutedUICommand CreateAlphabet = new RoutedUICommand();
        public static readonly RoutedUICommand Learn = new RoutedUICommand();
         
    }
}
