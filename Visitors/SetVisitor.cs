using Antlr4.Runtime.Misc;
using iro4cli.Grammar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iro4cli
{
    /// <summary>
    /// The main visitor for the Iro4cli system. Takes the most abstract blocks.
    /// </summary>
    public partial class IroVisitor : iroBaseVisitor<IroVariable>
    {
        /// <summary>
        /// Visits a top level system set and defines it.
        /// </summary>
        public override IroVariable VisitSys_set([NotNull] iroParser.Sys_setContext context)
        {
            //
        }
    }
}
