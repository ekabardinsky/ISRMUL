using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISRMUL.Manuscript
{
    [Serializable]
    public class Alphabet
    {
        public delegate void TextChangeCallBack(string text);
        public SymbolWindow MainSymbol { get { return Symbols.FirstOrDefault(); } }
        public HashSet<SymbolWindow> Symbols { get; set; }

        public string Code { get; set; }

        public Alphabet()
        {
            Symbols = new HashSet<SymbolWindow>();
            Code = string.Empty;
        }

        #region getters
        #endregion

        public override string ToString()
        {
            return Code;
        }
    }
}
