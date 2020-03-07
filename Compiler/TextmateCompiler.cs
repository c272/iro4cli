using shortid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace iro4cli.Compile
{
    public class TextmateCompiler : ICompileTarget
    {
        //The pending context members to be evaluated.
        private Dictionary<string, List<ContextMember>> pendingContexts = new Dictionary<string, List<ContextMember>>();
        
        /// <summary>
        /// Compiles a set of pre compile data into a textmate file.
        /// </summary>
        public CompileResult Compile(IroPrecompileData data)
        {
            var text = new StringBuilder();

            //Add pre-baked headers.
            text.AppendLine("<?xml  version=\"1.0\" encoding=\"UTF-8\"?>");
            text.AppendLine("<!DOCTYPE plist PUBLIC \"-//Apple Computer//DTD PLIST 1.0//EN\"   \"http://www.apple.com/DTDs/PropertyList-1.0.dtd\">");
            text.AppendLine("<plist version=\"1.0\">");
            text.AppendLine("<!-- Generated via. Iro4CLI -->");

            //Start the dictionary.
            text.AppendLine("<dict>");

            //Input the file types.
            if (data.FileExtensions == null || data.FileExtensions.Count == 0)
            {
                Error.Compile("No file extensions provided to map grammar against. Use 'file_extensions' to define them.");
                return null;
            }
            text.AppendLine("<key>fileTypes</key>");
            text.AppendLine("<array>");
            foreach (var type in data.FileExtensions)
            {
                text.AppendLine("<string>" + type + "</string>");
            }
            text.AppendLine("</array>");

            //The name of the grammar.
            text.AppendLine("<key>name</key>");
            text.AppendLine("<string>" + data.Name + "</string>");
            

            //Pattern array, just including the main context here.
            if (data.Contexts.FindIndex(x => x.Name == "main") == -1)
            {
                Error.Compile("No entrypoint context named 'main' exists. You need to make a context named 'main' to start the grammar state at.");
                return null;
            }
            text.AppendLine("<key>patterns</key>");
            text.AppendLine("<array>");
            text.AppendLine("<dict>");
            text.AppendLine("<key>include</key>");
            text.AppendLine("<string>#main</string>");
            text.AppendLine("</dict>");
            text.AppendLine("</array>");

            //Name of the source.
            text.AppendLine("<key>scopeName</key>");
            text.AppendLine("<string>source." + data.Name + "</string>");

            //UUID
            text.AppendLine("<key>uuid</key>");
            text.AppendLine("<string>" + data.UUID + "</string>");

            //Main repository for all contexts.
            text.AppendLine("<key>repository</key>");
            text.AppendLine("<dict>");

            //Define all the contexts inside the array.
            foreach (var context in data.Contexts)
            {
                //Add the pre-existing context.
                AddContext(ref text, context, data);

                //Context done parsing, complete all queued contexts.
                foreach (var queued in pendingContexts)
                {
                    AddContext(ref text, new IroContext(queued.Key)
                    {
                        Members = queued.Value
                    }, data);
                }
            }

            return null;
        }

        /// <summary>
        /// Adds a single context to the builder.
        /// </summary>
        private void AddContext(ref StringBuilder text, IroContext context, IroPrecompileData data)
        {
            text.AppendLine("<key>" + context.Name + "</key>");
            text.AppendLine("<dict>");

            //Define all the patterns in this context.
            text.AppendLine("<key>patterns</key>");
            text.AppendLine("<array>");
            foreach (var pattern in context.Members)
            {
                AddPattern(ref text, pattern, data);
            }

            text.AppendLine("</dict>");
        }

        /// <summary>
        /// Adds a single pattern to the list of patterns.
        /// </summary>
        private void AddPattern(ref StringBuilder text, ContextMember pattern, IroPrecompileData data)
        {
            //What type is it?
            switch (pattern.Type)
            {
                case ContextMemberType.Include:
                    text.AppendLine("<dict>");
                    text.AppendLine("<key>include</key>");
                    text.AppendLine("<string>#" + pattern.Data + "</string>");
                    text.AppendLine("</dict>");
                    break;
                case ContextMemberType.Pattern:
                    AddPatternRaw(ref text, (PatternContextMember)pattern, data);
                    break;
                case ContextMemberType.InlinePush:
                    AddInlinePush(ref text, (InlinePushContextMember)pattern, data);
                    break;
                case ContextMemberType.Push:
                    throw new NotImplementedException();
                case ContextMemberType.Pop:
                    throw new NotImplementedException();
                default:
                    Error.Compile("Failed to add pattern, unrecognized context member type '" + pattern.Type.ToString() + "'.");
                    return;
            }
        }

        /// <summary>
        /// Adds a InlinePushContextMember to the text.
        /// </summary>
        private void AddInlinePush(ref StringBuilder text, InlinePushContextMember pattern, IroPrecompileData data)
        {
            //Get styles from the pattern.
            var styles = GetPatternStyles(pattern, data);

            //Patterns match up with context groups?
            if (!GroupsMatch(styles, pattern.Data))
            {
                Error.Compile("Mismatch between capture groups and number of styles for inline push with regex '" + pattern.Data + "'.");
                return;
            }

            //Begin capture regex.
            text.AppendLine("<key>begin</key>");
            text.AppendLine("<string>" + pattern.Data + "</string>");

            //Begin capture styles.
            text.AppendLine("<key>beginCaptures</key>");
            text.AppendLine("<dict>");
            for (int i=0; i<styles.Count; i++)
            {
                text.AppendLine("<key>" + (i + 1) + "</key>");
                text.AppendLine("<dict>");
                text.AppendLine("<key>name</key>");
                text.AppendLine("<string>" + styles[i].TextmateScope + "." + data.Name + "</string>");
                text.AppendLine("</dict>");
            }
            text.AppendLine("</dict>");

            //Begin patterns, capture all "pattern" sets and includes and queue them.
            text.AppendLine("<key>patterns</key>");
            text.AppendLine("<array>");

            //Include the queued context.
            string helperName = "helper_" + ShortId.Generate(7);
            text.AppendLine("<dict>");
            text.AppendLine("<key>include</key>");
            text.AppendLine("<string>#" + helperName + "</string>");
            text.AppendLine("</dict>");
            text.AppendLine("</array>");

            //Queue it.
            QueueContext(helperName, pattern.Patterns);

            //...
        }

        /// <summary>
        /// Queues a context to be created upon the end of the current context being created.
        /// </summary>
        private void QueueContext(string helperName, List<ContextMember> patterns)
        {
            pendingContexts.Add(helperName, patterns);
        }

        /// <summary>
        /// Adds a PatternContextMember to the text, rather than a pattern that could possibly be any ContextMember.
        /// </summary>
        private void AddPatternRaw(ref StringBuilder text, PatternContextMember pattern, IroPrecompileData data)
        {
            //Get styles from the pattern.
            var styles = GetPatternStyles(pattern, data);

            //Is the amount of patterns equal to the amount of context groups?
            //Use a hack of replacing bracket groups with normal letters.
            if (!GroupsMatch(styles, pattern.Data))
            {
                Error.Compile("Mismatch between capture groups and number of styles for pattern with regex '" + pattern.Data + "'.");
                return;
            }

            //Add the initial match.
            text.AppendLine("<dict>");
            text.AppendLine("<key>match</key>");
            text.AppendLine("<string>" + pattern.Data + "</string>");

            //Only one style? Just use the 'name' property.
            if (pattern.Styles.Count == 1) 
            {
                text.AppendLine("<key>name</key>");
                text.AppendLine("<string>" + styles[0].TextmateScope + "." + data.Name + "</string>");
            }
            else
            {
                //Multiple styles, define capture groups.
                text.AppendLine("<key>captures</key>");
                text.AppendLine("<dict>");
                for (int i=0; i<styles.Count; i++)
                {
                    text.AppendLine("<key>" + (i + 1) + "</key>");
                    text.AppendLine("<dict>");
                    text.AppendLine("<key>name</key>");
                    text.AppendLine("<string>" + styles[i].TextmateScope + "." + data.Name + "</string>");
                    text.AppendLine("</dict>");
                }
                text.AppendLine("</dict>");
            }
        }

        /// <summary>
        /// Gets a list of styles for a specific pattern context member.
        /// </summary>
        private List<IroStyle> GetPatternStyles(PatternContextMember pattern, IroPrecompileData data)
        {
            //Check that styles exist, and try to get the first one out.
            if (pattern.Styles.Count == 0)
            {
                Error.Compile("No style was defined for pattern with regex '" + pattern.Data + "'.");
                return null;
            }

            //Get all the patterns out.
            var styles = new List<IroStyle>();
            foreach (var style in pattern.Styles)
            {
                //Get the styles form the style list.
                int index = data.Styles.FindIndex(x => x.Name == style);
                if (index == -1)
                {
                    Error.Compile("A style '" + style + "' is referenced in pattern with regex '" + pattern.Data + "', but it is not defined in the style map.");
                    return null;
                }

                styles.Add(data.Styles[index]);
            }

            //Make sure all the patterns have textmate scopes.
            if (styles.Where(x => x.TextmateScope != null).Count() != styles.Count)
            {
                Error.Compile("One or more styles for pattern with regex '" + pattern.Data + "' does not have a textmate scope defined.");
                return null;
            }

            return styles;
        }

        /// <summary>
        /// Determines whether the capture groups in the given data match the styles list.
        /// </summary>
        public bool GroupsMatch(List<IroStyle> styles, string data)
        {
            string withoutGroups = Regex.Replace(data, "\\(([^()]+)\\)", "x");
            int groupAmt = withoutGroups.Split('|').Length;
            return (groupAmt == styles.Count);
        }
    }
}
