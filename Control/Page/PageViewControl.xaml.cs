using Microsoft.Win32;
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
    public partial class PageViewControl : UserControl, Manuscript.IRefreshable
    {

        public PageViewControl()
        {
            InitializeComponent();
            Refresh();
        }

        #region commands
        public void Add(BitmapImage image, string name)
        {
            Page.PageControl page = new PageControl();
            page.Image.Source = image;
            page.PageName.Text = name;
            Pages.Items.Add(page);
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
        public void Refresh()
        {
            Clear();
            if (pageViewProject != null) AddRange(pageViewProject.Pages);
        }
        #endregion

        #region dependency
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
        #endregion

        #region event handler
        private void Button_Click_1(object sender, RoutedEventArgs e)
        { 
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Multiselect = true;
            dialog.Filter = "Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png, *.bmp) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png; *.bmp";
            if (dialog.ShowDialog()==true)
            {
                List<BitmapImage> images = new List<BitmapImage>(dialog.FileNames.Length);
                foreach (string path in dialog.FileNames)
                    images.Add(new BitmapImage(new Uri(path)));

                pageViewProject.Pages.AddRange(images);
                Refresh();
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            int index = Pages.SelectedIndex;
            if (index > -1)
            {
                pageViewProject.Pages.RemoveAt(index);
                Refresh();
            }
        }

        private void Pages_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            PageControl page = Pages.SelectedItem as PageControl;
            pageViewProject.CurrentPage = page.Image.Source as BitmapImage;
        }

        #endregion

   
         
    }
}
