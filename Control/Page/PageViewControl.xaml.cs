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
        public void Add(BitmapSource image, string name)
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
        public void AddRange(IEnumerable<BitmapSource> images)
        {
            int i = 0;
            foreach (BitmapSource image in images)
            {
                Add(image, "Страница № " + (++i));
            }
        }
        public void Refresh()
        {
            Clear();
            if (pageViewProject != null) AddRange(pageViewProject.Images.Select(x=>x.Value));
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
        public void Insert_Click(object sender, RoutedEventArgs e)
        { 
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Multiselect = true;
            dialog.Filter = "Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png, *.bmp) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png; *.bmp";
            if (dialog.ShowDialog()==true)
            {
                List<BitmapSource> images = new List<BitmapSource>(dialog.FileNames.Length);
                foreach (string path in dialog.FileNames)
                {
                    var image = new BitmapImage(new Uri(path));

                    try { pageViewProject.Images.Add(path, Utils.ImageConverter.cutBackground(image)); }
                    catch { }
                }

                Refresh();
            }
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            int index = Pages.SelectedIndex;
            if (index > -1)
            {
                var page = Pages.SelectedItem as PageControl;
                var source = pageViewProject.Images.Where(x => x.Value == page.Image.Source).Select(x=>x.Key).FirstOrDefault();

                if (source != null)
                    pageViewProject.Images.Remove(source);
                Refresh();
            }
        }

        private void Pages_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                PageControl page = Pages.SelectedItem as PageControl;
                pageViewProject.CurrentPage = page.Image.Source as BitmapSource;
                pageViewProject.Views.Where(x=>x is Editor.EditorViewControl).FirstOrDefault().Refresh();
            }
            catch { }
        }

        #endregion
    }

    public static class CustomPageEditCommands
    {
        public static readonly RoutedUICommand Insert = new RoutedUICommand("Ctrl+I", "Insert", typeof(PageViewControl), new InputGestureCollection(new KeyGesture[] { new KeyGesture(Key.I, ModifierKeys.Control) }));
        public static readonly RoutedUICommand Delete = new RoutedUICommand("Delete", "Delete", typeof(PageViewControl), new InputGestureCollection(new KeyGesture[] { new KeyGesture(Key.Delete) }));

    }
}
