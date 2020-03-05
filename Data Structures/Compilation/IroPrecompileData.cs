using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iro4cli.Compiler
{
    /// <summary>
    /// Data required before compile targets can all be run.
    /// </summary>
    public class IroPrecompileData
    {
        public string Name;
        public List<string> FileExtensions = new List<string>();
        public List<IroStyle> Styles = new List<IroStyle>();
    }
}
