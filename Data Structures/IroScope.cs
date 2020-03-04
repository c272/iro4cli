using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iro4cli
{
    /// <summary>
    /// The global Iro scope, for holding variables.
    /// </summary>
    public static class IroScope
    {
        public static Dictionary<string, IroVariable> Variables = new Dictionary<string, IroVariable>();
    }

    /// <summary>
    /// Represents a single variable in Iro.
    /// </summary>
    public abstract class IroVariable
    {
        public VariableType Type;
    }

    /// <summary>
    /// Different types of variables in iro.
    /// </summary>
    public enum VariableType
    {
        Array,
        Regex,
        Value
    }
}
