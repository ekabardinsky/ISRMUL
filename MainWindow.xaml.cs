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
        }

        #region event handlers

        private void NewProjectMenuItem_Click(object sender, RoutedEventArgs e)
        {
            newProject();
        }

        private void SaveProjectMenuItem_Click(object sender, RoutedEventArgs e)
        {
            saveProject();
        }
        #endregion

        #region commands

        void newProject()
        {
            CurrentProject = new Manuscript.Project(new Manuscript.IRefreshable[] { Pages, Editor });
            ProjectReady = true;
            CurrentProject.Refresh();
        }

        void saveProject()
        {
            SaveFileDialog dialog = new SaveFileDialog(); 
            dialog.Filter = "Project files (*.pro) | *.pro";
            if (dialog.ShowDialog() == true)
            {
                
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
}
