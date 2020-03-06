using System.Collections.Generic;

namespace iro4cli.Compiler
{
    /// <summary>
    /// An abstract compile target for creating code from Iro trees.
    /// </summary>
    public interface ICompileTarget
    {
        CompileResult Compile(IroPrecompileData data);
    }
}