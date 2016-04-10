using ISRMUL.Manuscript;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISRMUL.Control.Text
{
    class TextFormatter
    {
        List<SymbolWindow> windows { get; set; }
        List<string> labels { get; set; }
        double h { get; set; }
        double w { get; set; }

        public TextFormatter(List<SymbolWindow> all, List<string> labels)
        {
            windows = all;
            this.labels = labels;
        }

        public void Init()
        {
            h = windows.Sum(x => x.RealHeight / windows.Count);
            w = windows.Sum(x => x.RealWidth / windows.Count);
        }

        public string Compute()
        {
            string text = "";
            for (int i = 0; i < windows.Count - 1; i++)
            {
                SymbolWindow one = windows[i];
                SymbolWindow two = windows[i + 1];

                string separator = getSeparator(one, two);
                text += labels[i] + separator;
            }

            text += labels[labels.Count - 1];
            return text;
        }

        string getSeparator(SymbolWindow one, SymbolWindow two)
        {
            if (two.RealCoordinates.Y - h > one.RealCoordinates.Y)
                return "\n";
            if (one.RealCoordinates.X + one.RealWidth + w / 2 < two.RealCoordinates.X)
                return " ";
            return "";
        }
    }
}
