using System.Collections.Generic;

namespace iro4cli.Compile
{
    /// <summary>
    /// An abstract compile target for creating code from Iro trees.
    /// </summary>
    public interface ICompileTarget
    {
        CompileResult Compile(IroPrecompileData data);
    }
}