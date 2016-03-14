using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using ISRMUL.Control;
using System.Windows;
using System.Windows.Controls;

namespace ISRMUL.Manuscript
{
    public class Project:DependencyObject
    {
        public List<BitmapImage> Pages { get; set; }
        public List<IRefreshable> Views { get; set; }
        Dictionary<BitmapImage,List<SymbolWindow>> SymbolWindows { get; set; }

        public Project(params IRefreshable [] controls)
        {
            Pages = new List<BitmapImage>();
            Views = new List<IRefreshable>(controls);
            SymbolWindows = new Dictionary<BitmapImage, List<SymbolWindow>>(); 
        }
        #region getters
        public List<SymbolWindow> getSymbolWindows(BitmapImage image)
        {
            
            if(!SymbolWindows.ContainsKey(image))
            {
                SymbolWindows.Add(image,new List<SymbolWindow>());
            }
            return SymbolWindows[image];
        }

        public void AddToSymbolWindows(BitmapImage image, SymbolWindow symbol)
        {
            getSymbolWindows(image).Add(symbol);
        }

        #endregion
        #region dependency property
        public static readonly DependencyProperty CurrentPageProperty;

        static Project()
        {
            CurrentPageProperty = DependencyProperty.Register("CurrentPage", typeof(BitmapImage), typeof(Project));
        }

        public BitmapImage CurrentPage
        {
            get { return (BitmapImage)GetValue(CurrentPageProperty); }
            set { SetValue(CurrentPageProperty, value); }
        }
        
        #endregion

        #region commands
        public void Refresh()
        {
            foreach (IRefreshable view in Views)
                view.Refresh();
        }
        #endregion
    }

    
}
