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
        //The global list of variables.
        public static Dictionary<string, IroVariable> Variables = new Dictionary<string, IroVariable>();

        //Adds a variable to the global list.
        public static void AddVariable(string name, IroVariable var)
        {
            Variables.Add(name, var);
        }

        public static Func<string, bool> VariableExists = (string name) => Variables.ContainsKey(name);
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
        Value,
        Set,
        Include,
        Reference
    }
}
