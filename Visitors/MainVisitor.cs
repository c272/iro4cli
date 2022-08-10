using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr4.Runtime.Misc;

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
            foreach (var block in context.block())
            {
                VisitBlock(block);
            }

            return null;
        }

        /// <summary>
        /// Visits a top level statement containing an attribute definition or system set.
        /// </summary>
        public override IroVariable VisitBlock([NotNull] iroParser.BlockContext context)
        {
            //Prepare locals.
            IroAttribute thisAttrib = null;

            //System set?
            if (context.sys_set() != null)
            {
                //Define all the sets.
                var set = context.sys_set();
                var setAttrib = VisitSys_set(set);

                //Is it an attribute?
                if (!(setAttrib is IroAttribute))
                {
                    Error.Fatal(context, "Defined system set did not return an attribute. Please log an issue on the repository page with the source grammar.");
                    return null;
                }

                thisAttrib = (IroAttribute)setAttrib;
            }

            //Is it a statement?
            else if (context.statement() != null)
            {
                //Statement.
                var stat = context.statement();
                var toAdd = VisitStatement(stat);

                //If anything comes back up, try to add it.
                if (toAdd != null)
                {
                    //Make sure it's an attribute so the name is there too.
                    if (!(toAdd is IroAttribute))
                    {
                        Error.Warn(stat, "Non-attribute variable reached the top of the stack, skipping.");
                        return null;
                    }

                    //Attempt to add.
                    thisAttrib = (IroAttribute)toAdd;
                }
            }

            //Unknown operation.
            else
            {
                Error.Fatal(context, "Unknown block statement type, please submit an issue on the repository with your source grammar.");
            }

            //If a variable has been generated, try to add it to scope.
            if (thisAttrib != null)
            {
                if (IroScope.VariableExists(thisAttrib.Name))
                {
                    Error.Fatal(context, "Variable already exists with the name '" + thisAttrib.Name + "'.");
                    return null;
                }
                IroScope.AddVariable(thisAttrib.Name, thisAttrib.Value);
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
