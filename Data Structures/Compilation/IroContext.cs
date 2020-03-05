using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iro4cli
{
    /// <summary>
    /// Represents a single context within Iro.
    /// </summary>
    public class IroContext
    {
        public string Name;
        public List<ContextMember> Members = new List<ContextMember>();

        public IroContext(string name)
        {
            Name = name;
        }
    }

    /// <summary>
    /// A single member inside the Iro context.
    /// </summary>
    public class ContextMember
    {
        public ContextType Type;
        public string Data;
    }

    /// <summary>
    /// Types of context members.
    /// </summary>
    public enum ContextType 
    { 
        EOLPop,
        EOLPush,
        Include,
        InlinePush,
        Pattern,
        Pop,
        Push
    }
}
