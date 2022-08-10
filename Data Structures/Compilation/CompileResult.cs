using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iro4cli.Compile
{
    /// <summary>
    /// A single output result from an Iro compiler.
    /// </summary>
    public class CompileResult
    {
        public Target Target;
        public string GeneratedFile;
    }

    /// <summary>
    /// Represents a single output target for the Iro compiler.
    /// </summary>
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
