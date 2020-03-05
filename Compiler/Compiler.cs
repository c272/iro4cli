using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iro4cli.Compiler
{
    public static class Compiler
    {
        public static void Compile(Dictionary<string, IroVariable> vars)
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

                var thisContext = ProcessContext(context.Key, (IroSet)context.Value);
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
        }

        /// <summary>
        /// Parses a single pattern from a context.
        /// </summary>
        private static ContextMember ParsePattern(IroSet value)
        {
            throw new NotImplementedException();
        }

        private static ContextMember ParseInlinePush(IroSet value)
        {
            throw new NotImplementedException();
        }
    }
}
