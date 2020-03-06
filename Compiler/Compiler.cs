﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iro4cli.Compiler
{
    public static class Compiler
    {
        public static void Compile(Dictionary<string, IroVariable> vars, params ICompileTarget[] targets)
        {
            var pcd = new IroPrecompileData();

            //Verify that name, file extensions exist.
            if (!vars.ContainsKey("name"))
            {
                Error.Compile("No 'name' variable defined to name the grammar.");
                return;
            }
            if (!vars.ContainsKey("file_extensions"))
            {
                Error.Compile("No 'file_extensions' variable defined to name the file extensions compatible with this grammar.");
                return;
            }
            if (vars["name"].Type != VariableType.Value)
            {
                Error.Compile("The 'name' variable must be a string.");
                return;
            }
            if (vars["file_extensions"].Type != VariableType.Array)
            {
                Error.Compile("The 'file_extensions' variable must define an array of file extensions (currently not an array).");
                return;
            }

            //Set name.
            string name = ((IroValue)vars["name"]).Value;
            pcd.Name = name;

            //Set file extensions.
            IroList fileExts = (IroList)vars["file_extensions"];
            foreach (var ext in fileExts.Contents)
            {
                if (ext.Type != VariableType.Value)
                {
                    Error.Compile("All file extensions must be string values (detected type " + ext.Type.ToString() + " in array).");
                    return;
                }

                pcd.FileExtensions.Add(((IroValue)ext).Value);
            }

            //todo: parse other possible top level flags

            //Find constants.
            var constants = vars.Select(x => x.Value.Type == VariableType.Value && x.Key.StartsWith("__")).Cast<IroValue>();
            
            //Check that the "styles" and "contexts" system sets are there.
            if (!vars.ContainsKey("styles") || vars["styles"].Type != VariableType.Set)
            {
                Error.Compile("The 'styles' set does not exist, you must define a list of styles to use for your grammar.");
                return;
            }
            if (!vars.ContainsKey("contexts") || vars["contexts"].Type != VariableType.Set)
            {
                Error.Compile("The 'contexts' set does not exist, you must define a list of contexts to pattern match your styles to.");
                return;
            }

            //Crawl through the styles set and define them in the list.
            var stylesSet = (IroSet)vars["styles"];
            foreach (var style in stylesSet)
            {
                var thisStyle = new IroStyle();
                
                //Make sure this is an object, can't have any attributes in the styles list.
                if (style.Value.Type != VariableType.Set)
                {
                    //Skip the value.
                    Error.CompileWarning("A non-set value '" + style.Key + "' is defined in the styles map. Only style sets should be defined in the 'styles' map.");
                    continue;
                }

                var styleDefinition = (IroSet)style.Value;
                if (styleDefinition.SetType != "style")
                {
                    Error.CompileWarning("A set in the styles map is defined as having type '" + styleDefinition.SetType + "', which is not a style. Skipping.");
                    continue;
                }

                //Switch on all the values in that set and define style based on the keys.
                foreach (var styleProperty in styleDefinition)
                {
                    //Is the value a string?
                    if (styleProperty.Value.Type != VariableType.Value)
                    {
                        Error.CompileWarning("Failed to create style with name '" + style.Key + ", non-string value defined in the style object.");
                        continue;
                    }
                    string value = ((IroValue)styleProperty.Value).Value;

                    //Switch on the property.
                    switch (styleProperty.Key)
                    {
                        case "color":
                        case "colour":
                            thisStyle.Colour = value;
                            break;
                        case "ace_scope":
                            thisStyle.AceScope = value;
                            break;
                        case "textmate_scope":
                            thisStyle.TextmateScope = value;
                            break;
                        case "pygments_scope":
                            thisStyle.PygmentsScope = value;
                            break;
                        case "highlight_js_scope":
                            thisStyle.HighlightJSScope = value;
                            break;
                        case "bold":
                            if (value == "true")
                            {
                                thisStyle.Bold = true;
                            }
                            else if (value == "false")
                            {
                                thisStyle.Bold = false;
                            }
                            else
                            {
                                Error.CompileWarning("Unrecognized value for boolean property 'bold', not 'true' or 'false'. Assumed 'false'.");
                                thisStyle.Bold = false;
                            }
                            break;
                        case "italic":
                            if (value == "true")
                            {
                                thisStyle.Italic = true;
                            }
                            else if (value == "false")
                            {
                                thisStyle.Italic = false;
                            }
                            else
                            {
                                Error.CompileWarning("Unrecognized value for boolean property 'bold', not 'true' or 'false'. Assumed 'false'.");
                                thisStyle.Italic = false;
                            }
                            break;
                        default:
                            Error.CompileWarning("Invalid property in style set '" + style.Key + "' with name '" + styleProperty.Key + "'.");
                            continue;
                    }
                }

                //Add the style to the list.
                pcd.Styles.Add(thisStyle);
            }

            //Styles are done processing, start processing the contexts.
            var contextsSet = (IroSet)vars["contexts"];
            foreach (var context in contextsSet)
            {
                //Is the set a context?
                if (context.Value.Type != VariableType.Set)
                {
                    Error.CompileWarning("Could not create context '" + context.Key + "', contexts must be sets of type 'context'.");
                    continue;
                }
                if (((IroSet)context.Value).SetType != "context")
                {
                    Error.CompileWarning("Could not create context '" + context.Key + "', values in the contexts array must be sets of type 'context'.");
                    continue;
                }

                pcd.Contexts.Add(ProcessContext(context.Key, (IroSet)context.Value));
            }

            //Use precompile data to process the given targets.
            List<CompileResult>
            foreach (var target in targets)
            {
                compileResults.Add(target.Compile(pcd));
            }
        }

        /// <summary>
        /// Processes a single context from the IroVariable form into an IroContext form.
        /// </summary>
        private static IroContext ProcessContext(string contextName, IroSet context)
        {
            var iroCtx = new IroContext(contextName);

            //Loop over the values in the context, process depending on type.
            foreach (var value in context.Values)
            {
                //An include.
                if (value is IroInclude)
                {
                    iroCtx.Members.Add(new ContextMember()
                    {
                        Data = ((IroInclude)value).Value,
                        Type = ContextType.Include
                    });
                }

                //A set of a given type.
                else if (value is IroSet)
                {
                    switch (((IroSet)value).SetType)
                    {
                        case "inline_push":
                            iroCtx.Members.Add(ParseInlinePush((IroSet)value));
                            break;
                        case "pattern":
                            iroCtx.Members.Add(ParsePattern((IroSet)value));
                            break;
                        case "pop":
                            throw new NotImplementedException();
                        case "push":
                            throw new NotImplementedException();
                    }
                }
            }

            return iroCtx;
        }

        /// <summary>
        /// Parses a single pattern from a context.
        /// </summary>
        private static ContextMember ParsePattern(IroSet value)
        {
            //Find the mandatory "styles" and "regex" properties.
            if (!value.ContainsKey("regex") || !value.ContainsKey("styles"))
            {
                Error.Compile("Pattern missing a required attribute (must have 'styles' and 'regex').");
                return null;
            }

            //Valid types?
            if (value["styles"].Type != VariableType.Array)
            {
                Error.Compile("Pattern 'styles' attribute must be an array.");
                return null;
            }
            if (value["regex"].Type != VariableType.Regex)
            {
                Error.Compile("Pattern 'regex' attribute must be a regex value.");
                return null;
            }

            //Get them out.
            string regex = ((IroRegex)value["regex"]).StringValue;
            List<string> styles = new List<string>();
            foreach (var style in ((IroList)value["styles"]))
            {
                //Is the value a name?
                if (!(style is IroValue))
                {
                    Error.CompileWarning("Failed to add pattern style for pattern with regex '" + regex + "', array member is not a value.");
                    continue;
                }

                //Get the name out and add it.
                styles.Add(((IroValue)style).Value);
            }

            //Create a pattern.
            return new PatternContextMember()
            {
                Data = regex,
                Styles = styles,
                Type = ContextType.Pattern
            };
        }

        /// <summary>
        /// Parses a single inline push context member in Iro.
        /// </summary>
        private static ContextMember ParseInlinePush(IroSet ilp)
        {
            //Find the required elements 'regex', 'styles' and 'pop'.
            if (!ilp.ContainsKey("regex") || !ilp.ContainsKey("styles"))
            {
                Error.Compile("Required attribute is missing from inline push (must have members 'regex', 'styles').");
                return null;
            }
            if (!ilp.ContainsKey("pop") && !ilp.ContainsKey("eol_pop"))
            {
                Error.Compile("Inline push patterns must have a 'pop' or 'eol_pop' set to know when to end the state..");
                return null;
            }
            if (ilp.ContainsKey("pop") && ilp.ContainsKey("eol_pop"))
            {
                Error.Compile("Inline push patterns cannot hav both a 'pop' and an 'eol_pop', you must use one or the other.");
                return null;
            }

            //Verify their types.
            if (!(ilp["regex"] is IroRegex))
            {
                Error.Compile("Inline push attribute 'regex' must be a regex value.");
                return null;
            }
            if (!(ilp["styles"] is IroList))
            {
                Error.Compile("Inline push attribute 'styles' must be an array value.");
                return null;
            }
            if (ilp.ContainsKey("pop") && !(ilp["pop"] is IroSet))
            {
                Error.Compile("Pop attributes must be a set.");
                return null;
            }
            if (ilp.ContainsKey("eol_pop") && !(ilp["eol_pop"] is IroSet))
            {
                Error.Compile("End of line pop attributes must be a set.");
                return null;
            }

            //Get out the regex and style values.
            string regex = ((IroRegex)ilp["regex"]).StringValue;
            List<string> styles = new List<string>();
            foreach (var style in ((IroList)ilp["styles"]))
            {
                //Is the value a name?
                if (!(style is IroValue))
                {
                    Error.CompileWarning("Failed to add pattern style for pattern with regex '" + regex + "', array member is not a value.");
                    continue;
                }

                //Get the name out and add it.
                styles.Add(((IroValue)style).Value);
            }

            //Generate the pop (if it's there).
            List<string> popStyles = new List<string>();
            string popRegex;
            if (ilp.ContainsKey("pop"))
            {
                //Parse the 'pop'.
                var pop = ((IroSet)ilp["pop"]);
                if (!pop.ContainsKey("regex") || pop["regex"].Type != VariableType.Regex)
                {
                    Error.Compile("Inline push 'pop' messages must be of type 'set'.");
                    return null;
                }
                if (!pop.ContainsKey("styles") || pop["styles"].Type != VariableType.Array)
                {
                    Error.Compile("Inline push 'styles' attribute must be of type 'array'.");
                }

                //regex
                popRegex = ((IroRegex)pop["regex"]).StringValue;

                //styles
                foreach (var style in ((IroList)ilp["styles"]))
                {
                    if (!(style is IroValue))
                    {
                        Error.CompileWarning("Failed to add 'pop' style for pattern with regex '" + regex + "', array member is not a value.");
                        continue;
                    }

                    //Get the name out and add it.
                    popStyles.Add(((IroValue)style).Value);
                }
            }
            else
            {
                //eol_pop
                popStyles = styles;
                popRegex = "(\n|\r\n)";
            }

            //Create the module and return it.
            return new InlinePushContextMember()
            {
                Data = regex,
                Styles = styles,
                PopData = popRegex,
                PopStyles = popStyles,
                Type = ContextType.InlinePush
            };
        }
    }
}