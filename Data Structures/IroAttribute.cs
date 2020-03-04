using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iro4cli
{
    /// <summary>
    /// Represents a single named attribute in the Iro parse tree.
    /// </summary>
    public class IroAttribute : IroVariable
    {
        public string Name { get; private set; }
        public IroVariable Value { get; private set; }
        public IroAttribute(string name, IroVariable var)
        {
            Name = name;
            Value = var;
        }
    }
}
