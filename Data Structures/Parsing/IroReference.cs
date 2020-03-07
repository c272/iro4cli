using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iro4cli
{
    /// <summary>
    /// Represents a single reference to a variable in Iro.
    /// </summary>
    public class IroReference : IroVariable 
    {
        public string Value = null;

        public IroReference(string val)
        {
            Value = val;
            Type = VariableType.Reference;
        }
    }
}
