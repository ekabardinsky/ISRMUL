using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISRMUL.Manuscript
{
    class Alphabet
    {
        public SymbolWindow MainSymbol { get { if (Symbols.Count > 0) return Symbols[0]; return null; } }
        public List<SymbolWindow> Symbols { get; set; }
        public string Code { get; set; }

        public Alphabet()
        {
            Symbols = new List<SymbolWindow>();
        }
    }
}
