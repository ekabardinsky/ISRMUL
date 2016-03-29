using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ISRMUL.Recongnition.Neokognitron
{
    [Serializable]
    class Point
    { 

        public Point(int x, int y)
        {
            // TODO: Complete member initialization
            X = x;
            Y = y;
        }

        public int X { get; set; }

        public int Y { get; set; }
    }
}
