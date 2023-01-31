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
    /// Represents a single push context member.
    /// </summary>
    public class PopContextMember : PatternContextMember
    {
    }

    /// <summary>
    /// Represents a single push context member.
    /// </summary>
    public class PushContextMember : PatternContextMember
    {
        //The context we are pushing to.
        public string TargetContext;

        //The styles associated with the push.
        public List<string> PushStyles = new List<string>();
    }

    /// <summary>
    /// Represents a single inline push context member.
    /// </summary>
    public class InlinePushContextMember : PatternContextMember
    {
        public string PopData;
        public List<string> PopStyles = new List<string>();
        public List<ContextMember> Patterns = new List<ContextMember>();
    }

    /// <summary>
    /// A single pattern member inside the context.
    /// </summary>
    public class PatternContextMember : ContextMember
    {
        public List<string> Styles = new List<string>();
    }

    /// <summary>
    /// A single member inside the Iro context.
    /// </summary>
    public class ContextMember
    {
        public ContextMemberType Type;
        public string Data;
    }

    /// <summary>
    /// Types of context members.
    /// </summary>
    public enum ContextMemberType 
    { 
        Include,
        InlinePush,
        Pattern,
        Pop,
        Push,
        SpaceUnimportant,
        Enabled,
        DefaultStyle,
        Description,
        CaseSensitive,
        UID
    }
}
