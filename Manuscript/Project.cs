using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using ISRMUL.Control;
using System.Windows;
using System.Windows.Controls;
using ISRMUL.Recognition;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace ISRMUL.Manuscript
{
    
    public class Project:DependencyObject
    {
        public List<IRefreshable> Views { get; set; }
        public Canvas Canvas { get; set; }
        Dictionary<string, List<SymbolWindow>> SymbolWindows { get; set; }
        public Dictionary<string, BitmapSource> Images { get; set; }
        public List<SymbolWindow> KnowledgeBase {get;set;}
        public List<Alphabet> Alphabets { get; set; }

        public Project(Canvas Canvas, params IRefreshable [] controls)
        {
            this.Canvas = Canvas;
            Views = new List<IRefreshable>(controls);
            SymbolWindows = new Dictionary<string, List<SymbolWindow>>();
            Images = new Dictionary<string, BitmapSource>();
            KnowledgeBase = new List<SymbolWindow>();
            Alphabets = new List<Alphabet>();
        }

        #region getters
        public List<SymbolWindow> getSymbolWindows(string key)
        {

            if (!SymbolWindows.ContainsKey(key))
            {
                SymbolWindows.Add(key, new List<SymbolWindow>());
                Images.Add(key, new BitmapImage(new Uri(key)));
            }
            return SymbolWindows[key];
        }

        public void AddToSymbolWindows(string key, SymbolWindow symbol)
        {
            getSymbolWindows(key).Add(symbol);
        }

        public string getCurrentKey()
        {
            return Images.Where(x => x.Value == CurrentPage).Select(x => x.Key).FirstOrDefault();
        }

        #endregion

        #region dependency property

        [NonSerialized]
        public static readonly DependencyProperty CurrentPageProperty;

        static Project()
        {
            CurrentPageProperty = DependencyProperty.Register("CurrentPage", typeof(BitmapSource), typeof(Project));
            CurrentAlphabetProperty = DependencyProperty.Register("CurrentAlphabet", typeof(Alphabet), typeof(Project));

        }
        public BitmapSource CurrentPage
        {
            get { return (BitmapSource)GetValue(CurrentPageProperty); }
            set { SetValue(CurrentPageProperty, value); }
        }

        [NonSerialized]
        public static readonly DependencyProperty CurrentAlphabetProperty;

        public Alphabet CurrentAlphabet
        {
            get { return (Alphabet)GetValue(CurrentAlphabetProperty); }
            set { SetValue(CurrentAlphabetProperty, value); }
        }
        
        #endregion

        #region commands
        public void Refresh()
        {
            foreach (IRefreshable view in Views)
                view.Refresh();
        }
        #endregion

        #region segmentation

        public void SegmentationCurrent(double windowHeight, double windowWidth, string key, int backThresh)
        {
            var points = Utils.ImageConverter.getPointFromImage(CurrentPage);

            ISRMUL.Recognition.MeanShift.MeanShiftSolver solver = new Recognition.MeanShift.MeanShiftSolver(new double[] { windowWidth, windowHeight }, points);
            
            solver.Compute(10, 1000);
            solver.Clustering(3);

            var symbols = solver.Clusters.Select(x => new SymbolWindow(key,this, x));
            var original = getSymbolWindows(key);
            original.Clear();
            original.AddRange(symbols);
        }

        

        #endregion

        #region serialize
        public static void Serialize(string filename, Project p)
        {
            FileStream fs = new FileStream(filename, FileMode.Create);

            List<Object> o = new List<object>();

            o.Add(p.SymbolWindows);
            o.Add(p.Images.Select(x => x.Key).ToList());
            o.Add(p.getCurrentKey());
            o.Add(p.KnowledgeBase);
            o.Add(p.Alphabets);
            // Construct a BinaryFormatter and use it to serialize the data to the stream.
            BinaryFormatter formatter = new BinaryFormatter();
            try
            {
                formatter.Serialize(fs, o);
            }
            finally
            {
                fs.Close();
            }
        }

        public static Project DeSerialize(string filename,Canvas Canvas, params IRefreshable [] controls)
        {
            FileStream fs = new FileStream(filename, FileMode.Open);
            Project p = null;
            try
            {
                BinaryFormatter formatter = new BinaryFormatter();

                // Deserialize the hashtable from the file and 
                // assign the reference to the local variable.
                List<Object> o;
                o = (List<Object>)formatter.Deserialize(fs);

                p = new Project(Canvas, controls); 
                //symbol
                p.SymbolWindows = o[0] as Dictionary<string, List<SymbolWindow>>;
                foreach (var item in p.SymbolWindows)
                {
                    item.Value.ForEach(x => x.Project = p);
                }
                //images
                var images = o[1] as List<string>;
                foreach (string s in images)
                    p.Images.Add(s, Utils.ImageConverter.cutBackground(new BitmapImage(new Uri(s))));

                p.CurrentPage = p.Images[o[2] as string];
                p.KnowledgeBase = o[3] as List<SymbolWindow>;
                p.Alphabets = o[4] as List<Alphabet>;
            }
            finally
            {
                fs.Close();
            }

            return p;
        }
        #endregion
    }

    
}
