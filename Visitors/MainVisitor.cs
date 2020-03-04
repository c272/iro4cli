using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr4.Runtime.Misc;
using iro4cli.Grammar;

namespace iro4cli
{
    /// <summary>
    /// The main visitor for the Iro4cli system. Takes the most abstract blocks.
    /// </summary>
    public partial class IroVisitor : iroBaseVisitor<IroResult>
    {
        //Master compile unit visitor.
        public override IroResult VisitCompileUnit([NotNull] iroParser.CompileUnitContext context)
        {
            foreach (var stat in context.statement())
            {
                VisitStatement(stat);
            }

            return null;
        }

        //Visits and executes a statement.
        public override IroResult VisitStatement([NotNull] iroParser.StatementContext context)
        {
            //Switch on the type of statement.
            if (context.attribute() != null)
            {
                //Attribute.
                VisitAttribute(context.attribute());
            }
            else if (context.sys_set() != null)
            {
                //System defined top level set.
                VisitSys_set(context.sys_set());
            }
            else if (context.set() != null)
            {
                //Normal set definition.
                return VisitSet(context.set());
            }
            else if (context.include() != null)
            {
                //Include.
                return VisitInclude(context.include());
            }
            else
            {
                Error.Fatal(context, "Invalid statement provided, unrecognized.");
            }

            return null;
        }
    }
}
