using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iro4cli.Compile
{
    /// <summary>
    /// A single result from a compiler.
    /// </summary>
    public class CompileResult
    {
        public Target Target;
        public string GeneratedFile;
    }

    public enum Target
    {
        Textmate,
        Atom,
        Ace,
        Pygments,
        Rouge,
        Sublime3,
        CSS
    }
}
