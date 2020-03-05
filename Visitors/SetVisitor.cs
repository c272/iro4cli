using Antlr4.Runtime.Misc;
using iro4cli.Grammar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using shortid;

namespace iro4cli
{
    /// <summary>
    /// The main visitor for the Iro4cli system. Takes the most abstract blocks.
    /// </summary>
    public partial class IroVisitor : iroBaseVisitor<IroVariable>
    {
        //The length of the random IDs, must be >= 7.
        public const int RAND_ID_LEN = 7;

        /// <summary>
        /// Visits a top level system set and defines it.
        /// </summary>
        public override IroVariable VisitSys_set([NotNull] iroParser.Sys_setContext context)
        {
            //Parse the set on its own.
            var set = new IroSet();
            
            //Add individual attributes to the set, checking for uniqueness.
            foreach (var statement in context.statement())
            {
                string name;
                IroVariable value;

                //Attribute?
                if (statement.attribute() != null)
                {
                    //Get name, value.
                    var attribute = statement.attribute();
                    name = attribute.IDENTIFIER().GetText();
                    value = VisitAttribute(attribute);
                }

                //Set?
                else if (statement.set() != null)
                {
                    //Get name, value.
                    var childSet = statement.set();
                    
                    //Is it an anonymous set?
                    if (childSet.IDENTIFIER() == null)
                    {
                        //Automatic naming.
                        name = "set_" + ShortId.Generate(RAND_ID_LEN);
                    }
                    else
                    {
                        //Named, just set it normally.
                        name = childSet.IDENTIFIER().GetText();
                    }

                    value = VisitSet(childSet);
                }

                //Include?
                else if (statement.include() != null)
                {
                    //Add it to the list.
                    name = "include_" + ShortId.Generate(RAND_ID_LEN);
                    value = new IroInclude(statement.include().IDENTIFIER().GetText());
                }
                
                //Unrecognized statement in set.
                else
                {
                    Error.Fatal(context, "Unrecognized statement in set, please open an issue on the repository with this source grammar.");
                    return null;
                }

                //Check if the name already exists.
                if (set.ContainsKey(name))
                {
                    Error.Fatal(context, "Set already contains a variable with name '" + name + "'.");
                    return null;
                }

                //Safe to add.
                set.Add(name, value);
            }

            //Return the created set.
            return set;
        }
    }
}
