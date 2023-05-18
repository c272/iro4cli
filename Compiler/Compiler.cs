using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace iro4cli.Compile
{
    /// <summary>
    /// The top level compiler for Iro. Processes pre-compile data and starts target compilers individually.
    /// </summary>
    public static class Compiler
    {
        //The variables currently being compiled.
        public static Dictionary<string, IroVariable> Variables;
        
        /// <summary>
        /// Compiles a set of Algo variables given targets.
        /// </summary>
        public static List<CompileResult> Compile(Dictionary<string, IroVariable> vars, params ICompileTarget[] targets)
        {
            //Set locals for this compile.
            Variables = vars;
            var pcd = new IroPrecompileData();

            //Parse all the top level flags.
            ParseTopLevelFlags(vars, ref pcd);

            //todo: parse other top level flags
            Error.CompileWarning("Some top-level flags are missing and/or not implemented yet.");

            //Find constants.
            var constants = vars.Select(x => x.Value.Type == VariableType.Value && x.Key.StartsWith("__")).Cast<IroValue>();
            
            //Check that the "styles" and "contexts" system sets are there.
            if (!vars.ContainsKey("styles") || vars["styles"].Type != VariableType.Set)
            {
                Error.Compile("The 'styles' set does not exist, you must define a list of styles to use for your grammar.");
                return null;
            }
            if (!vars.ContainsKey("contexts") || vars["contexts"].Type != VariableType.Set)
            {
                Error.Compile("The 'contexts' set does not exist, you must define a list of contexts to pattern match your styles to.");
                return null;
            }

            //Crawl through the styles set and define them in the list.
            var stylesSet = (IroSet)vars["styles"];
            foreach (var style in stylesSet)
            {
                var thisStyle = new IroStyle(style.Key);
                
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
                    //Is the value for a colour, or another random property?
                    string value;
                    if (styleProperty.Key == "color" || styleProperty.Key == "colour"
                        || styleProperty.Key == "background_color" || styleProperty.Key == "background_colour")
                    {
                        //For a colour, so it must be hex or string type.
                        if (styleProperty.Value.Type != VariableType.Value && styleProperty.Value.Type != VariableType.Hex)
                        {
                            Error.CompileWarning("Failed to create style with name '" + style.Key + ", colour values must be of type hex or string.");
                            continue;
                        }

                        //It is, grab value out.
                        value = (styleProperty.Value is IroValue) ? ((IroValue)styleProperty.Value).Value : ((IroHex)styleProperty.Value).Value;
                    }
                    else
                    {
                        //Ensure property is a value type, then grab value out.
                        if (styleProperty.Value.Type != VariableType.Value)
                        {
                            Error.CompileWarning("Failed to create style with name '" + style.Key + "', non-string value defined in the style object.");
                            continue;
                        }
                        value = ((IroValue)styleProperty.Value).Value;
                    }

                    //Switch on the property.
                    switch (styleProperty.Key)
                    {
                        case "color":
                        case "colour":
                            thisStyle.Colour = value;
                            break;
                        case "background_colour":
                        case "background_color":
                            thisStyle.BackgroundColour = value;
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

                pcd.Contexts.Add(ProcessContext(context.Key, (IroSet)context.Value, contextsSet));
            }

            //Use precompile data to process the given targets.
            var results = new List<CompileResult>();
            foreach (var target in targets)
            {
                results.Add(target.Compile(pcd));
            }

            return results;
        }

        /// <summary>
        /// Parses all the possible top level flags for Iro.
        /// </summary>
        private static void ParseTopLevelFlags(Dictionary<string, IroVariable> vars, ref IroPrecompileData pcd)
        {
            //Verify that the required keys exist.
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
            pcd.Name = ((IroValue)vars["name"]).Value;

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

            //Parsing other top-level flags.
            GetTopLevelProperty("textmate_uuid", ref pcd.UUID, ref vars);
            GetTopLevelProperty("description", ref pcd.Description, ref vars);
            GetTopLevelProperty("color", ref pcd.Colour, ref vars);
            GetTopLevelProperty("background_color", ref pcd.BackgroundColour, ref vars);
        }
        
        /// <summary>
        /// Sets a single top level property given it's name and the value to set.
        /// </summary>
        private static void GetTopLevelProperty(string propertyName, ref string toSet, ref Dictionary<string, IroVariable> vars)
        {
            if (vars.ContainsKey(propertyName))
            {
                if (!(vars[propertyName] is IroValue))
                {
                    Error.CompileWarning("Property '" + propertyName + "' must be a value type, ignoring.");
                }
                else
                {
                    toSet = ((IroValue)vars[propertyName]).Value;
                }
            }
            else { toSet = ""; }
        }

        /// <summary>
        /// Processes a single context from the IroVariable form into an IroContext form.
        /// </summary>
        private static IroContext ProcessContext(string contextName, IroSet context, IroSet contexts)
        {
            var iroCtx = new IroContext(contextName);

            //Loop over the values in the context, process depending on type.
            foreach (var kvp in context)
            {
                var value = kvp.Value;

                //An include.
                if (value is IroInclude)
                {
                    iroCtx.Members.Add(new ContextMember()
                    {
                        Data = ((IroInclude)value).Value,
                        Type = ContextMemberType.Include
                    });
                }

                //A descriptive value for a set.
                else if (value is IroValue)
                {
                    var valueType = (IroValue)value;
                    var ctxVal = AddContextValue(valueType.Value, kvp.Key);
                    if (ctxVal != null)
                    {
                        iroCtx.Members.Add(ctxVal);
                    }
                }

                //A set of a given type.
                else if (value is IroSet)
                {
                    switch (((IroSet)value).SetType)
                    {
                        case "inline_push":
                            iroCtx.Members.Add(ParseInlinePush((IroSet)value, contexts));
                            break;
                        case "pattern":
                            iroCtx.Members.Add(ParsePattern((IroSet)value));
                            break;
                        case "pop":
                            iroCtx.Members.Add(ParsePop((IroSet)value));
                            break;
                        case "push":
                            iroCtx.Members.Add(ParsePush((IroSet)value, contexts));
                            break;
                            
                        //We have no idea what this is.
                        default:
                            throw new NotImplementedException();
                    }
                }

                //Unrecognized.
                else
                {
                    Error.Compile("Unrecognized statement inside an Iro context, must be a set or an include.");
                    return null;
                }
            }

            return iroCtx;
        }

        /// <summary>
        /// A single context value to be added to the context list.
        /// </summary>
        private static ContextMember AddContextValue(string value, string name)
        {
            switch (name)
            {
                case "description":
                    return new ContextMember()
                    {
                        Data = value,
                        Type = ContextMemberType.Description
                    };
                case "case_sensitive":
                    return new ContextMember()
                    {
                        Data = value,
                        Type = ContextMemberType.CaseSensitive
                    };
                case "default_style":
                    return new ContextMember()
                    {
                        Data = value,
                        Type = ContextMemberType.DefaultStyle
                    };
                case "enabled":
                    return new ContextMember()
                    {
                        Data = value,
                        Type = ContextMemberType.Enabled
                    };
                case "space_unimportant":
                    return new ContextMember()
                    {
                        Data = value,
                        Type = ContextMemberType.SpaceUnimportant
                    };
                case "uid":
                    return new ContextMember()
                    {
                        Data = value,
                        Type = ContextMemberType.UID
                    };
                default:
                    Error.CompileWarning("Unrecognized property '" + name + "' in context, must be one of:");
                    Error.CompileWarning("description, case_sensitive, default_style, enabled, space_unimportant, uid");
                    return null;
            }
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
            string regex;
            if (value["styles"].Type != VariableType.Array)
            {
                Error.Compile("Pattern 'styles' attribute must be an array.");
                return null;
            }
            if (value["regex"].Type != VariableType.Regex)
            {
                if (value["regex"].Type == VariableType.Reference)
                {
                    var constant = GetConstant(((IroReference)value["regex"]).Value, VariableType.Regex);
                    if (constant is IroValue)
                    {
                        regex = ((IroValue)constant).Value;
                    }
                    else
                    {
                        regex = ((IroRegex)constant).StringValue;
                    }
                }
                else
                {
                    Error.Compile("Pattern 'regex' attribute must be a regex value.");
                    return null;
                }
            }
            else
            {
                regex = ((IroRegex)value["regex"]).StringValue;
            }

            //Get them out.
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
                Type = ContextMemberType.Pattern
            };
        }

        /// <summary>
        /// Parses a single inline push context member in Iro.
        /// </summary>
        private static ContextMember ParseInlinePush(IroSet ilp, IroSet contexts)
        {
            //Find the required elements 'regex', 'styles' and 'pop'.
            if (!ilp.ContainsKey("regex") || !ilp.ContainsKey("styles"))
            {
                Error.Compile("Required attribute is missing from inline push (must have members 'regex', 'styles').");
                return null;
            }
            if (!ilp.ContainsSetOfType("pop") && !ilp.ContainsSetOfType("eol_pop"))
            {
                //No pops or eol_pops here. Try to find one in children.
                var popsFound = FindPops(ilp, contexts);
                if (popsFound.Count == 0)
                {
                    Error.Compile("Inline push patterns must have a 'pop' or 'eol_pop' set to know when to end the state.");
                    return null;
                }
                if (popsFound.Count > 1)
                {
                    Error.Compile("Inline push patterns can only contain one 'pop' or 'eol_pop' rule. It appears multiple 'pop's are imported from other contexts.");
                    return null;
                }

                //Add the relevant set to the ILP.
                ilp.Add(popsFound[0]);
            }
            if (ilp.ContainsSetOfType("pop") && ilp.ContainsSetOfType("eol_pop"))
            {
                Error.Compile("Inline push patterns cannot hav both a 'pop' and an 'eol_pop', you must use one or the other.");
                return null;
            }

            //Verify their types.
            string regex;
            if (!(ilp["regex"] is IroRegex))
            {
                //Is it a constant yet to be converted?
                if ((ilp["regex"] is IroReference))
                {
                    //Attempt to get the constant.
                    IroVariable constant = GetConstant(((IroReference)ilp["regex"]).Value, VariableType.Regex);
                    if (constant is IroRegex)
                    {
                        regex = ((IroRegex)constant).StringValue;
                    }
                    else
                    {
                        regex = ((IroValue)constant).Value;
                    }
                }
                else
                {
                    Error.Compile("Inline push attribute 'regex' must be a regex value.");
                    return null;
                }
            } else 
            {
                //Just a normal regex, get out the string value.
                regex = ((IroRegex)ilp["regex"]).StringValue;
            }
            if (!(ilp["styles"] is IroList))
            {
                Error.Compile("Inline push attribute 'styles' must be an array value.");
                return null;
            }
            if (ilp.ContainsSetOfType("pop") && !(ilp.GetFirstSetOfType("pop") is IroSet))
            {
                Error.Compile("Pop attributes must be a set.");
                return null;
            }
            if (ilp.ContainsSetOfType("eol_pop") && !(ilp.GetFirstSetOfType("eol_pop") is IroSet))
            {
                Error.Compile("End of line pop attributes must be a set.");
                return null;
            }

            //Get out the style values.
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
            if (ilp.ContainsSetOfType("pop"))
            {
                //Parse the 'pop'.
                var pop = ilp.GetFirstSetOfType("pop");
                if (!pop.ContainsKey("regex"))
                {
                    Error.Compile("Inline push 'pop' messages must contain a 'regex' property.");
                    return null;
                }
                if (!(pop["regex"] is IroValue) && !(pop["regex"] is IroRegex))
                {
                    Error.Compile("Inline push 'pop' messages must contain a 'regex' property of type 'regex'.");
                }
                if (!pop.ContainsKey("styles") || pop["styles"].Type != VariableType.Array)
                {
                    Error.Compile("Inline push 'pop' messages must have a 'styles' attribute of type 'array'.");
                    return null;
                }

                //What is the regex for the pop rule?
                if (pop["regex"] is IroReference)
                {
                    //It's a constant, uh oh. Get it.
                    var const_ = GetConstant(((IroReference)pop["regex"]).Value, VariableType.Regex);
                    if (const_ is IroRegex)
                    {
                        popRegex = ((IroRegex)const_).StringValue;
                    }
                    else
                    {
                        popRegex = ((IroValue)const_).Value;
                    }
                }
                else
                {
                    popRegex = ((IroRegex)pop["regex"]).StringValue;
                }

                //Get the styles for this pop rule.
                foreach (var style in ((IroList)pop["styles"]))
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
                popRegex = "(\\n|\\r\\n)";
            }

            //Get all patterns and includes out.
            var patterns = ilp.Where(x => x.Value is IroSet && ((IroSet)x.Value).SetType == "pattern")
                              .Select(x => x.Value)
                              .Cast<IroSet>();

            var includes = ilp.Where(x => x.Value is IroInclude)
                              .Select(x => x.Value)
                              .Cast<IroValue>();

            var values = ilp.Where(x => x.Value.Type == VariableType.Value);

            var ctxMems = new List<ContextMember>();

            //Parse them into the context list.
            foreach (var pattern in patterns)
            {
                ctxMems.Add(ParsePattern(pattern));
            }
            foreach (var include in includes)
            {
                ctxMems.Add(new ContextMember()
                {
                    Data = include.Value,
                    Type = ContextMemberType.Include
                });
            }
            foreach (var value in values)
            {
                //Ignore the horrible spaghetti, first parameter is getting out the string value from the KeyValuePair.
                //Second is getting out the name.
                var ctxVal = AddContextValue(((IroValue)value.Value).Value, value.Key);
                if (ctxVal != null)
                {
                    ctxMems.Add(ctxVal);
                }
            }

            //Create the module and return it.
            return new InlinePushContextMember()
            {
                Data = regex,
                Styles = styles,
                PopData = popRegex,
                PopStyles = popStyles,
                Patterns = ctxMems,
                Type = ContextMemberType.InlinePush
            };
        }

        /// <summary>
        /// Parses a single push context member in Iro.
        /// </summary>
        private static ContextMember ParsePush(IroSet push, IroSet contexts)
        {
            //Ensure the push has required fields 'regex', 'styles' and 'context'.
            if (!push.ContainsKey("regex") || !push.ContainsKey("styles")
                || !push.ContainsKey("context"))
            {
                Error.Compile("Required attribute is missing from push (must have members 'regex', 'styles', 'context').");
                return null;
            }

            //Ensure types of required fields are correct.
            string regex = null;
            if (!(push["regex"] is IroRegex))
            {
                //Is it a constant yet to be converted?
                if ((push["regex"] is IroReference))
                {
                    //Attempt to get the constant.
                    IroVariable constant = GetConstant(((IroReference)push["regex"]).Value, VariableType.Regex);
                    if (constant is IroRegex)
                    {
                        regex = ((IroRegex)constant).StringValue;
                    }
                    else
                    {
                        regex = ((IroValue)constant).Value;
                    }
                }
                else
                {
                    Error.Compile("Push attribute 'regex' must be a regex value.");
                    return null;
                }
            }
            else
            {
                //Just a normal regex, get out the string value.
                regex = ((IroRegex)push["regex"]).StringValue;
            }
            if (!(push["styles"] is IroList))
            {
                Error.Compile("Push attribute 'styles' must be an array value.");
                return null;
            }
            if (!(push["context"] is IroList) || ((IroList)push["context"]).Count() != 1)
            {
                Error.Compile("Push attribute 'context' must be an array value of length 1.");
                return null;
            }

            //Parse out the styles.
            List<string> styles = new List<string>();
            foreach (var style in ((IroList)push["styles"]))
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

            //Ensure that the target context value is of a valid type.
            IroVariable context_var = ((IroList)push["context"]).ElementAt(0);
            if (context_var.Type != VariableType.Value)
            {
                Error.Compile("Context items within 'push' must be a value type.");
                return null;
            }

            //Ensure that the target context exists within our context list.
            var context_name = ((IroValue)context_var).Value;
            if (!contexts.ContainsKey(context_name))
            {
                Error.Compile($"Referenced context in push '{context_name}' not found in contexts list.");
                return null;
            }

            //Everything seems valid, create our push context!
            return new PushContextMember()
            {
                Data = regex,
                TargetContext = context_name,
                Styles = styles,
                Type = ContextMemberType.Push,
            };
        }

        /// <summary>
        /// Parses a single pop context member in Iro.
        /// </summary>
        private static ContextMember ParsePop(IroSet pop)
        {
            //Ensure the pop has required fields 'regex', 'styles'.
            if (!pop.ContainsKey("regex") || !pop.ContainsKey("styles"))
            {
                Error.Compile("Required attribute is missing from pop (must have members 'regex', 'styles').");
                return null;
            }

            //Ensure types of required fields are correct.
            string regex = null;
            if (!(pop["regex"] is IroRegex))
            {
                //Is it a constant yet to be converted?
                if ((pop["regex"] is IroReference))
                {
                    //Attempt to get the constant.
                    IroVariable constant = GetConstant(((IroReference)pop["regex"]).Value, VariableType.Regex);
                    if (constant is IroRegex)
                    {
                        regex = ((IroRegex)constant).StringValue;
                    }
                    else
                    {
                        regex = ((IroValue)constant).Value;
                    }
                }
                else
                {
                    Error.Compile("Push attribute 'regex' must be a regex value.");
                    return null;
                }
            }
            else 
            {
                //Just a normal regex, get out the string value.
                regex = ((IroRegex)pop["regex"]).StringValue;
            }
            if (!(pop["styles"] is IroList))
            {
                Error.Compile("Push attribute 'styles' must be an array value.");
                return null;
            }

            //Parse out the styles.
            List<string> styles = new List<string>();
            foreach (var style in ((IroList)pop["styles"]))
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

            //Create the pop.
            return new PopContextMember()
            {
                Data = regex,
                Styles = styles,
                Type = ContextMemberType.Pop
            };
        }

        /// <summary>
        /// Finds sets recursively of type "pop" or "eol_pop".
        /// </summary>
        private static List<KeyValuePair<string, IroVariable>> FindPops(IroSet set, IroSet contexts)
        {
            //Loop over set members, for each include find pops too.
            var pops = new List<KeyValuePair<string, IroVariable>>();
            foreach (var member in set)
            {
                if (member.Value is IroSet)
                {
                    //Is it a pop or eol_pop?
                    var setMem = ((IroSet)member.Value);
                    if (setMem.SetType == "pop" || setMem.SetType == "eol_pop")
                    {
                        pops.Add(member);
                    }
                }

                //Is it an include?
                if (member.Value is IroInclude)
                {
                    var include = ((IroInclude)member.Value);
                    string includeCtx = include.Value;

                    //Try and find context to evaluate.
                    if (!contexts.ContainsKey(includeCtx) || !(contexts[includeCtx] is IroSet))
                    {
                        Error.Compile("Include statement references context '" + includeCtx + "', but it does not exist.");
                        return null;
                    }
                    var ctx = (IroSet)contexts[includeCtx];

                    //Find pops in there, add range.
                    pops.AddRange(FindPops(ctx, contexts));
                }
            }

            //Return list.
            return pops;
        }

        /// <summary>
        /// Attempts to get a constant value from the currently executing compile's variables.
        /// </summary>
        private static IroVariable GetConstant(string value, VariableType type)
        {
            //Check if the constant exists.
            if (!Variables.ContainsKey(value))
            {
                //No constant found.
                Error.Compile("Referenced constant '" + value + "' is not defined.");
                return null;
            }

            //Right type for a constant?
            if (Variables[value].Type != VariableType.Value && Variables[value].Type != VariableType.Regex)
            {
                Error.Compile("Value '" + value + "' refenced like a constant, but it is not a value or a regex.");
                return null;
            }

            //Is the variable of the right type?
            if (Variables[value].Type != type)
            {
                Error.Compile("Constant value '" + value + "' is not of the expected type '" + type.ToString() + "'.");
                return null;
            }

            //Return it.
            return Variables[value];
        }
    }
}
