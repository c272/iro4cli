using System.Collections.Generic;

namespace iro4cli.Compiler
{
    /// <summary>
    /// An abstract compile target for creating code from Iro trees.
    /// </summary>
    public abstract class IroCompileTarget
    {
        public string GrammarName;
        public List<string> FileExtensions = new List<string>();
    }
}