using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iro4cli.Compile
{
    /// <summary>
    /// Data required before compile targets can all be run.
    /// </summary>
    public class IroPrecompileData
    {
        //Name of this grammar.
        public string Name;
        //Textmate UUID of this grammar.
        public string UUID;
        //Description of this grammar.
        public string Description;
        //Colour of the base grammar.
        public string Colour;
        //Background colour of the base grammar.
        public string BackgroundColour;

        //File extensions of the base grammar.
        public List<string> FileExtensions = new List<string>();
        //Styles for this grammar.
        public List<IroStyle> Styles = new List<IroStyle>();
        //Contexts for this grammar.
        public List<IroContext> Contexts = new List<IroContext>();
    }
}
