using Antlr4.Runtime.Misc;
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

                    value = ((IroAttribute)VisitSet(childSet)).Value;
                }

                //Include?
                else if (statement.include() != null)
                {
                    if (statement.include().IDENTIFIER()[0].GetText() != "include")
                    {
                        Error.Fatal(statement.include(), "Invalid inline set statement.");
                        return null;
                    }

                    //Add it to the list.
                    name = "include_" + ShortId.Generate(RAND_ID_LEN);
                    value = new IroInclude(statement.include().IDENTIFIER()[1].GetText());
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
            return new IroAttribute(context.IDENTIFIER().GetText(), set);
        }

        /// <summary>
        /// Visiting a standard set (non system).
        /// </summary>
        public override IroVariable VisitSet([NotNull] iroParser.SetContext context)
        {
            //Create a name for this set if necessary.
            string name;
            if (context.IDENTIFIER() == null)
            {
                name = "set_" + ShortId.Generate(RAND_ID_LEN);
            }
            else
            {
                name = context.IDENTIFIER().GetText();
            }

            //Get the value of the set from the typed definition.
            var set = VisitTyped_set(context.typed_set());

            //Return attribute.
            return new IroAttribute(name, set);
        }

        /// <summary>
        /// Visitor for the anonymous, unnamed part of a set.
        /// </summary>
        public override IroVariable VisitTyped_set([NotNull] iroParser.Typed_setContext context)
        {
            //Create the set.
            var set = new IroSet(context.IDENTIFIER().GetText());

            //Add all variables to the set.
            foreach (var statement in context.statement())
            {
                var result = VisitStatement(statement);

                //Is it an attribute?
                if (result is IroAttribute)
                {
                    var resultAttrib = (IroAttribute)result;
                    if (set.ContainsKey(resultAttrib.Name))
                    {
                        Error.Fatal(context, "A variable already exists in this set with name '" + resultAttrib.Name + "'.");
                        return null;
                    }

                    set.Add(resultAttrib.Name, resultAttrib.Value);
                }

                //An include?
                else if (result is IroInclude)
                {
                    set.Add("include_" + ShortId.Generate(RAND_ID_LEN), result);
                }

                //Unrecognized.
                else
                {
                    Error.Fatal(context, "Unrecognized result from set statement, please create an issue on the repository with the source grammar.");
                    return null;
                }
            }

            //Return the set.
            return set;
        }
    }
}
