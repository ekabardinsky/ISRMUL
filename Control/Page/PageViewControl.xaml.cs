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

namespace ISRMUL.Control.Page
{
    /// <summary>
    /// Логика взаимодействия для PageViewControl.xaml
    /// </summary>
    public partial class PageViewControl : UserControl
    {
        public Manuscript.Project currentProject;

        public PageViewControl()
        {
            InitializeComponent();
            RefreshProject();
        }
        public void Add(BitmapImage image, string name)
        {
            Page.PageControl page = new PageControl();
            page.Image.Source = image;
            page.PageName.Text = name;
        }
        public void Clear()
        {
            Pages.Items.Clear();
        }
        public void AddRange(IEnumerable<BitmapImage> images)
        {
            int i = 0;
            foreach (BitmapImage image in images)
            {
                Add(image, "Страница № " + (++i));
            }
        }
        public void RefreshProject()
        {
            Clear();
            if (currentProject != null) AddRange(currentProject.Pages);
        }

        public static readonly DependencyProperty CurrentProjectProperty = DependencyProperty.Register("pageViewProject", typeof(Manuscript.Project), typeof(PageViewControl));
        public Manuscript.Project pageViewProject
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

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

        }
    }
}
