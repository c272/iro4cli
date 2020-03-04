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
    public partial class IroVisitor : iroBaseVisitor<IroVariable>
    {
        //Master compile unit visitor.
        public override IroVariable VisitCompileUnit([NotNull] iroParser.CompileUnitContext context)
        {
            //Define all system sets first.
            foreach (var set in context.sys_set())
            {
                //Define all the sets.
                var setAttrib = VisitSys_set(set);
                
                //Is it an attribute?
                if (!(setAttrib is IroAttribute))
                {
                    Error.Fatal(context, "Defined system set did not return an attribute. Please log an issue on the repository page with the source grammar.");
                    return null;
                }
            }

            foreach (var stat in context.statement())
            {
                var toAdd = VisitStatement(stat);

                //If anything comes back up, try to add it.
                if (toAdd != null)
                {
                    //Make sure it's an attribute so the name is there too.
                    if (!(toAdd is IroAttribute))
                    {
                        Error.Warn(stat, "Non-attribute variable reached the top of the stack, skipping.");
                        continue;
                    }

                    //Attempt to add.
                    var thisAttrib = (IroAttribute)toAdd;
                    if (IroScope.VariableExists(thisAttrib.Name))
                    {
                        Error.Fatal(stat, "Variable already exists with the name '" + thisAttrib.Name + "'.");
                        return null;
                    }
                    IroScope.AddVariable(thisAttrib.Name, thisAttrib.Value);
                }
            }

            return null;
        }

        //Visits and executes a statement.
        public override IroVariable VisitStatement([NotNull] iroParser.StatementContext context)
        {
            //Switch on the type of statement.
            if (context.attribute() != null)
            {
                //Attribute.
                return VisitAttribute(context.attribute());
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
