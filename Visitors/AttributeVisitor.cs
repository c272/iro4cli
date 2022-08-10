using Antlr4.Runtime.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iro4cli
{
    /// <summary>
    /// The attribute visitor for Iro. Defines variables based on provided attributes.
    /// </summary>
    public partial class IroVisitor : iroBaseVisitor<IroVariable>
    {
        /// <summary>
        /// Defines a variable based on this attribute definition.
        /// </summary>
        public override IroVariable VisitAttribute([NotNull] iroParser.AttributeContext context)
        {
            //Get the created variable.
            return new IroAttribute(context.IDENTIFIER().GetText(), VisitDefinition(context.definition()));
        }

        /// <summary>
        /// Returns a variable based on a RHS definition.
        /// </summary>
        public override IroVariable VisitDefinition([NotNull] iroParser.DefinitionContext context)
        {
            //Is it an array type?
            if (context.ARRAY_SYM() != null)
            {
                //Yes, must be an identifier or an array on the right.
                if (context.definition_ident() != null)
                {
                    //Single value.
                    var list = new IroList();
                    list.Add(new IroValue(context.definition_ident().IDENTIFIER().GetText()));
                    return list;
                }
                else if (context.array() != null)
                {
                    //Array, make a list of the items.
                    var list = new IroList();
                    foreach (var item in context.array().IDENTIFIER())
                    {
                        list.Add(new IroValue(item.GetText()));
                    }
                    return list;
                }
                else
                {
                    Error.Fatal(context, "An array type is specified, but no array is given for that variable.");
                    return null;
                }
            }

            //Regex type?
            else if (context.REG_EQUALS_SYM() != null)
            {
                //Check a regex is used on the right hand side.
                if (context.regex() == null)
                {
                    if (context.constant_ref() == null)
                    {
                        Error.Fatal(context, "Regular expression equality symbol used, but a regex is not assigned.");
                    }
                    else
                    {
                        //Return the literal value on the right.
                        return new IroReference(context.constant_ref().IDENTIFIER().GetText());
                    }

                    return null;
                }

                //Evaluate the regex, return it.
                IroRegex regex;
                try
                {
                    regex = new IroRegex(context.regex().GetText());
                }
                catch (Exception e)
                {
                    Error.Fatal(context, "Invalid regular expression created, '" + e.Message + "'.");
                    return null;
                }

                return regex;
            }

            //Normal equality type?
            else if (context.EQUALS_SYM() != null)
            {
                //Must be a normal identifier.
                if (context.definition_ident() == null)
                {
                    Error.Fatal(context, "Value provided for standard non-regex variable must be a string.");
                    return null;
                }

                //Return the value.
                return new IroValue(context.definition_ident().IDENTIFIER().GetText());
            }

            //Unknown.
            else
            {
                Error.Fatal(context, "Unknown definition provided, please submit an issue on the repository with this source grammar included.");
                return null;
            }
        }
    }
}
