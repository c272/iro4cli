using iro4cli.Compile;
using iro4cli.Templates;
using shortid;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace iro4cli
{
    /// <summary>
    /// Compiles precompile data into an Atom grammar file.
    /// </summary>
    public class AtomCompiler : ICompileTarget
    {
        //Queue of contexts to be created.
        public Dictionary<string, List<ContextMember>> queuedContexts = new Dictionary<string, List<ContextMember>>();

        /// <summary>
        /// Compiles precompile data into an Atom Textmate file.
        /// </summary>
        public CompileResult Compile(IroPrecompileData data)
        {
            var text = new StringBuilder();

            //Add the file extensions.
            text.AppendLine("'fileTypes': [");
            foreach (var ext in data.FileExtensions)
            {
                text.AppendLine("'" + ext + "'");
            }
            text.AppendLine("]");

            //Name of the grammar.
            text.AppendLine("'name': '" + data.Name + "'");

            //Template for including main.
            text.AppendLine("'patterns' : [");
            text.AppendLine("{");
            text.AppendLine("'include': '#main'");
            text.AppendLine("}");
            text.AppendLine("]");

            //Root scope name.
            text.AppendLine("'scopeName': '" + data.Name + "'");

            //UUID (if one exists).
            text.AppendLine("'uuid': '" + data.UUID + "'");

            //Main repo for all patterns. 
            text.AppendLine("'repository': {");

            //Loop over contexts and add them. Make sure main is first.
            var mainCtx = data.Contexts.FirstOrDefault(x => x.Name == "main");
            if (mainCtx == null)
            {
                Error.Compile("No main context exists to start the grammar with.");
                return null;
            }

            //Add main.
            AddContext(mainCtx, data, ref text);
            ProcessQueuedContexts(data, ref text);

            //Loop over other contexts and add.
            foreach (var ctx in data.Contexts)
            {
                //Skip main, already processed.
                if (ctx.Name == "main") { continue; }

                AddContext(ctx, data, ref text);
                ProcessQueuedContexts(data, ref text);
            }

            //Close set, return.
            text.AppendLine("}");
            return new CompileResult()
            {
                GeneratedFile = FormatCScript(text.ToString()),
                Target = Target.Atom
            };
        }

        /// <summary>
        /// Processes the currently queued contexts.
        /// </summary>
        private void ProcessQueuedContexts(IroPrecompileData data, ref StringBuilder text)
        {
            foreach (var context in queuedContexts)
            {
                AddContext(new IroContext(context.Key)
                {
                    Members = context.Value
                }, data, ref text);
            }

            queuedContexts = new Dictionary<string, List<ContextMember>>();
        }

        /// <summary>
        /// Adds a single context to the text grammar.
        /// </summary>
        private void AddContext(IroContext context, IroPrecompileData data, ref StringBuilder text)
        {
            //Name of context, start of patterns.
            text.AppendLine("'" + context.Name + "': {");
            text.AppendLine("'patterns': [");

            //Loop over, add members.
            foreach (var member in context.Members)
            {
                AddMember(member, data, ref text);
            }

            text.AppendLine("]");
            text.AppendLine("}");
        }

        /// <summary>
        /// Adds a single context member to the text.
        /// </summary>
        private void AddMember(ContextMember member, IroPrecompileData data, ref StringBuilder text)
        {
            if (member.Type == ContextMemberType.Pattern)
            {
                //Normal pattern. Get styles out.
                var pattern = ((PatternContextMember)member);
                var styles = GetPatternStyles(pattern.Styles, data);

                //Check if the groups match.
                if (!GroupsMatch(styles, pattern.Data))
                {
                    Error.Compile("Amount of capture groups does not line up with the amount of assigned styles.");
                    return;
                }

                //Add to text.
                text.AppendLine("{");
                //Make sure to replace backslashes and quotes.
                text.AppendLine("'match': '" + pattern.Data.Replace("\\", "\\\\").Replace("'", "\\'") + "'");
                if (styles.Count > 1)
                {
                    //Multiple styles.
                    text.AppendLine("'captures': {");
                    for (int i=0; i<styles.Count; i++)
                    {
                        //Numbered styles.
                        text.AppendLine("'" + (i + 1) + "': {");
                        text.AppendLine("'name': '" + styles[i].TextmateScope + "." + data.Name + "'");
                        text.AppendLine("}");
                    }
                    text.AppendLine("}");
                }
                else
                {
                    //Single style.
                    text.AppendLine("'name': '" + styles[0].TextmateScope + "." + data.Name + "'");
                }
                text.AppendLine("}");
            }
            else if (member.Type == ContextMemberType.InlinePush)
            {
                //Inline push pattern.
                var ilp = ((InlinePushContextMember)member);
                var styles = GetPatternStyles(ilp.Styles, data);
                
                //Add starting styles.
                text.AppendLine("{");
                text.AppendLine("'begin': '" + ilp.Data.Replace("\\", "\\\\").Replace("'", "\\'") + "'");
                text.AppendLine("'beginCaptures': {");
                for (int i=0; i<styles.Count; i++)
                {
                    text.AppendLine("'" + (i + 1) + "': {");
                    text.AppendLine("'name': '" + styles[i].TextmateScope + "." + data.Name + "'");
                    text.AppendLine("}");
                }
                text.AppendLine("}");

                //Is a default style assigned?
                int defaultStyleIndex = ilp.Patterns.FindIndex(x => x.Type == ContextMemberType.DefaultStyle);
                if (defaultStyleIndex != -1)
                {
                    //Are other patterns defined?
                    if (ilp.Patterns.FindIndex(x => x.Type == ContextMemberType.Pattern) != -1)
                    {
                        //Warn the user.
                        Error.CompileWarning("You cannot define unique patterns when a 'default_style' attribute is applied. Ignoring them.");
                    }

                    //Get the style out.
                    var style = GetPatternStyles(new List<string>()
                    {
                        ilp.Patterns[defaultStyleIndex].Data
                    }, data);

                    //Add default content name.
                    text.AppendLine("'contentName': '" + style[0].TextmateScope + "." + data.Name + "'");
                }
                else
                {
                    //Actual patterns defined, not just default.
                    //Begin patterns, capture all "pattern" sets and includes and queue them.
                    text.AppendLine("'patterns': [");

                    //Include the queued context.
                    if (ilp.Patterns.Count != 0)
                    {
                        string helperName = "helper_" + ShortId.Generate(7);
                        text.AppendLine("{");
                        text.AppendLine("'include': '#" + helperName + "'");
                        text.AppendLine("}");

                        //Queue it.
                        QueueContext(helperName, ilp.Patterns);
                    }
                    text.AppendLine("]");
                }

                //Patterns done, pop condition & styles.
                var popStyles = GetPatternStyles(ilp.PopStyles, data);

                //Patterns match up with context groups?
                if (!GroupsMatch(popStyles, ilp.PopData))
                {
                    Error.Compile("Mismatch between capture groups and number of styles for pop with regex '" + ilp.PopData + "'.");
                    return;
                }

                //Okay, add pop data.
                text.AppendLine("'end': '" + ilp.PopData.Replace("\\", "\\\\").Replace("'", "\\'") + "'");
                text.AppendLine("'endCaptures': {");
                for (int i = 0; i < popStyles.Count; i++)
                {
                    text.AppendLine("'" + (i + 1) + "': {");
                    text.AppendLine("'name': '" + popStyles[i].TextmateScope + "." + data.Name + "'");
                    text.AppendLine("}");
                }
                text.AppendLine("}");

                //Close whole ILP.
                text.AppendLine("}");
            }
            else if (member.Type == ContextMemberType.Include)
            {
                //Append an include.
                text.AppendLine("{");
                text.AppendLine("'include': '#" + member.Data + "'");
                text.AppendLine("}");
            }
        }

        /// <summary>
        /// Queues a context to be created.
        /// </summary>
        private void QueueContext(string helperName, List<ContextMember> patterns)
        {
            queuedContexts.Add(helperName, patterns);
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

        /// <summary>
        /// Gets a list of styles for a specific pattern context member.
        /// </summary>
        private List<IroStyle> GetPatternStyles(List<string> styleNames, IroPrecompileData data)
        {
            //Check that styles exist, and try to get the first one out.
            if (styleNames.Count == 0)
            {
                Error.Compile("No style was defined for a pattern. All styles must have patterns.");
                return null;
            }

            //Get all the patterns out.
            var styles = new List<IroStyle>();
            foreach (var style in styleNames)
            {
                //Get the styles form the style list.
                int index = data.Styles.FindIndex(x => x.Name == style);
                if (index == -1)
                {
                    Error.Compile("A style '" + style + "' is referenced in a pattern, but it is not defined in the style map.");
                    return null;
                }

                styles.Add(data.Styles[index]);
            }

            //Make sure all the patterns have textmate scopes.
            if (styles.Where(x => x.TextmateScope != null).Count() != styles.Count)
            {
                Error.Compile("One or more styles for a pattern does not have a textmate scope defined.");
                return null;
            }

            return styles;
        }

        /// <summary>
        /// Format CoffeeScript.
        /// </summary>
        private string FormatCScript(string text)
        {
            string[] lines = text.Replace("\r", "").Split('\n');
            int indents = 0;
            string indent = "  ";
            for (int i=0; i<lines.Length; i++)
            {
                //Add indents.
                bool indentsPost = true;
                if (lines[i].Where(x => x == '}' || x == ']').Count() != lines[i].Length)
                {
                    indentsPost = false;
                    for (int j = 0; j < indents; j++)
                    {
                        lines[i] = indent + lines[i];
                    }
                }

                //Get the splitters, move indents accordingly.
                foreach (var char_ in lines[i])
                {
                    switch (char_)
                    {
                        case '[':
                        case '{':
                            indents++;
                            break;
                        case '}':
                        case ']':
                            indents--;
                            break;
                    }
                }

                //Do the indents post-calculate?
                if (indentsPost)
                {
                    for (int j = 0; j < indents; j++)
                    {
                        lines[i] = indent + lines[i];
                    }
                }

                //Reset indents if too far below.
                if (indents < 0) { indents = 0; }
            }

            return string.Join("\r\n", lines);
        }

        /// <summary>
        /// Creates an atom extension when given precompile data and a generated file.
        /// </summary>
        public static void MakeAtomExtension(string grammarName, string atomGrammar, string folder)
        {
            //Make the folder to put the extension in.
            MakeFolder(folder);
            MakeFolder(folder, grammarName + "-atom");
            folder = Path.Combine(folder, grammarName + "-atom");

            //Make the "grammars" folder and dump the grammar into it.
            MakeFolder(folder, "grammars");
            WriteFile(atomGrammar, folder, "grammars", grammarName + ".cson");

            //Make the "spec" folder and dump the template JS into it.
            MakeFolder(folder, "spec");
            WriteFile(AtomResources.atom_spec, folder, "spec", "atom-spec.js");
            WriteFile(AtomResources.atom_view_spec, folder, "spec", "atom-view-spec.js");

            //Make generic atom files.
            WriteFile(AtomResources.gitignore, folder, ".gitignore");
            WriteFile(AtomResources.CHANGELOG.Replace("{extension_name}", grammarName), folder, "CHANGELOG.md");
            WriteFile(AtomResources.LICENSE.Replace("{extension_name}", grammarName), folder, "LICENSE.md");
            WriteFile(AtomResources.README.Replace("{extension_name}", grammarName), folder, "README.md");

            //Successful!
            Console.Write("Successfully created an ");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("Atom Extension");
            Console.ResetColor();
            Console.Write(".\r\n");
        }

        /// <summary>
        /// Attempts to write the given file.
        /// </summary>
        private static void WriteFile(string fileContents, params string[] pathParts)
        {
            string path = Path.Combine(pathParts);
            try
            {
                File.WriteAllText(path, fileContents);
            }
            catch (Exception e)
            {
                Error.Compile("Failed writing file for Atom extension '" + string.Join("/", pathParts) + "', '" + e.Message + "'.");
            }
        }

        /// <summary>
        /// Makes the given folder.
        /// </summary>
        private static void MakeFolder(string folder, params string[] extras)
        {
            if (extras.Length!=0)
            {
                var list = new List<string>() { folder };
                list.AddRange(extras);
                folder = Path.Combine(list.ToArray());
            }

            try
            {
                Directory.CreateDirectory(folder);
            }
            catch (Exception e)
            {
                Error.Compile("Failed to create folder for Atom extension: '" + e.Message + "'.");
            }
        }
    }
}