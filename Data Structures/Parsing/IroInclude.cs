using Antlr4.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iro4cli
{
    /// <summary>
    /// Represents a single include directive in Iro.
    /// </summary>
    public class IroInclude : IroValue
    {
        public IroInclude(string val) : base(val)
        {
            Type = VariableType.Include;
        }
    }
}
