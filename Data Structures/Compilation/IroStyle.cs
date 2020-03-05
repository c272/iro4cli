using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iro4cli
{
    /// <summary>
    /// Represents a single style in Iro.
    /// </summary>
    public class IroStyle
    {
        //Whether the text should be bold/italic for CSS generation.
        public bool Bold = false;
        public bool Italic;
        public string Colour = null;

        //Scopes for various text grammar formats.
        public string HighlightJSScope = null;
        public string AceScope = null;
        public string TextmateScope = null;
        public string PygmentsScope = null;
    }
}
