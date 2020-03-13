using iro4cli.Compile;
using shortid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace iro4cli
{
    /// <summary>
    /// Class for compiling Iro precompile data into a valid Pygments grammar file.
    /// </summary>
    public class PygmentsCompiler : ICompileTarget
    {
        public Dictionary<Tuple<string, string>, InlinePushContextMember> queuedContexts = new Dictionary<Tuple<string, string>, InlinePushContextMember>();

        /// <summary>
        /// Compiles pre-compile data into a Pygments grammar file.
        /// </summary>
        public CompileResult Compile(IroPrecompileData data)
        {
            var text = new PyStringMaker();

            //Header for imports and class naming.
            text.AppendLine("from pygments.lexer import RegexLexer, bygroups");
            text.AppendLine("from pygments.token import *");
            text.AppendLine("import re");
            text.AppendLine("__all__ =['" + data.Name + "Lexer']");
            text.AppendLine();
            text.AppendLine("class " + data.Name + "Lexer(RegexLexer):");
            text.TabIn();

            //Name with capital first letter, aliases.
            text.AppendLine("name = '" + data.Name[0].ToString().ToUpper() + data.Name.Substring(1) + "'");
            text.AppendLine("aliases = ['" + data.Name + "']");

            //File names.
            string fNames = "[";
            foreach (var ext in data.FileExtensions)
            {
                fNames += "'" + ext + "', ";
            }
            fNames = fNames.TrimEnd(',', ' ') + "]";
            text.AppendLine(fNames);
            
            //Template flags.
            text.AppendLine("flags = re.MULTILINE | re.UNICODE");
            text.AppendLine();
            
            //Open the tokens map.
            text.AppendLine("tokens = {");
            text.TabIn();

            //Get the root context and evaluate.
            var rootCtx = data.Contexts.FirstOrDefault(x => x.Name == "main");
            if (rootCtx == null)
            {
                Error.Compile("No 'main' context exists to create the root grammar state from.");
                return null;
            }
            AddContext(rootCtx, data, ref text);
            includedThisRun = new List<string>();

            //Evaluate the rest of the contexts.
            foreach (var context in data.Contexts)
            {
                //Ignore main, already done.
                if (context.Name == "main") { continue; }

                //Add, reset includes.
                AddContext(context, data, ref text);
                includedThisRun = new List<string>();
            }

            //Add queued contexts.
            for (int i=0; i<queuedContexts.Count; i++)
            {
                var queued = queuedContexts.ElementAt(i);
                AddQueuedContext(queued.Key.Item2, queued.Key.Item1, queued.Value, data, ref text);
            }

            text.TabOut();
            text.AppendLine("}");

            return new CompileResult()
            {
                GeneratedFile = text.ToString(),
                Target = Target.Pygments
            };
        }

        //Adds a single context to the grammar string.
        private void AddContext(IroContext ctx, IroPrecompileData data, ref PyStringMaker text)
        {
            //Open the context.
            text.AppendLine("'" + ctx.Name + "' : [");
            text.TabIn();

            //Add context members.
            foreach (var member in ctx.Members)
            {
                AddContextMember(ctx.Name, member, data, ref text);
            }
            text.TrimEnd(',', ' ');

            text.TabOut();
            text.AppendLine("],");
        }

        //Adds a single context member to the context.
        List<string> includedThisRun = new List<string>();
        private void AddContextMember(string ctxName, ContextMember member, IroPrecompileData data, ref PyStringMaker text)
        {
            //What's the context member type?
            string regTxt = "";
            switch (member.Type)
            {
                //Normal pattern.
                case ContextMemberType.Pattern:
                    var pat = (PatternContextMember)member;
                    regTxt += "(u'" + pat.Data.Replace("\\", "\\\\") + "', bygroups(";

                    //Get styles out.
                    var styles = GetPatternStyles(pat.Styles, data);
                    
                    //Apply styles.
                    foreach (var style in styles)
                    {
                        regTxt += style.PygmentsScope + ", ";
                    }
                    regTxt = regTxt.TrimEnd(',', ' ');

                    //Close bygroups, end pattern.
                    regTxt += ")),";
                    text.AppendLine(regTxt);
                    return;

                //Inline push.
                case ContextMemberType.InlinePush:
                    var ilp = (InlinePushContextMember)member;
                    regTxt += "(u'" + ilp.Data.Replace("\\", "\\\\") + "', bygroups(";

                    //Get push styles out.
                    var pushStyles = GetPatternStyles(ilp.Styles, data);

                    //Add and close bygroups scope.
                    foreach (var style in pushStyles)
                    {
                        regTxt += style.PygmentsScope + ", ";
                    }
                    regTxt = regTxt.TrimEnd(',', ' ');
                    regTxt += ")";

                    //Add the push scope with a random name.
                    string helperName = "helper_" + ShortId.Generate(7);
                    QueueContext(ctxName, helperName, ilp);
                    regTxt += ", '" + helperName + "'),";

                    text.AppendLine(regTxt);
                    return;

                //Include.
                case ContextMemberType.Include:
                    //Get the context to include.
                    var ctx = data.Contexts.FirstOrDefault(x => x.Name == member.Data);
                    if (ctx == null)
                    {
                        Error.Compile("Context mentioned to include '" + member.Data + "' does not exist.");
                        return;
                    }

                    //Skip?
                    if (includedThisRun.Contains(member.Data)) { return; }

                    //Include all members recursively with update list.
                    includedThisRun.Add(member.Data);
                    foreach (var mem in ctx.Members)
                    {
                        AddContextMember(ctxName, mem, data, ref text);
                    }
                    return;
            }
        }

        //Queues a context for creation.
        private void QueueContext(string helperName, string originalCtx, InlinePushContextMember ilp)
        {
            queuedContexts.Add(new Tuple<string, string>(helperName, originalCtx), ilp);
        }

        //Adds a queued context to the text.
        private void AddQueuedContext(string name, string originalCtx, InlinePushContextMember ilp, IroPrecompileData data, ref PyStringMaker text)
        {
            //Open the context.
            text.AppendLine("'" + name + "' : [");
            text.TabIn();

            //Add the pop rule.
            var popStyles = GetPatternStyles(ilp.PopStyles, data);
            text.AppendLine("(u'" + ilp.PopData.Replace("\\", "\\\\") + "', byGroups(" + string.Join(", ", popStyles.Select(x => x.PygmentsScope)).TrimEnd(',', ' ') + "), '" + originalCtx + "')");

            //Add context members.
            foreach (var member in ilp.Patterns)
            {
                AddContextMember(name, member, data, ref text);
            }
            text.TrimEnd(',', ' ');

            text.TabOut();
            text.AppendLine("],");
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
            if (styles.Where(x => x.PygmentsScope != null).Count() != styles.Count)
            {
                Error.Compile("One or more styles for a pattern does not have a Pygments scope defined.");
                return null;
            }

            return styles;
        }
    }
    
    /// <summary>
    /// Creates a python string like a StringBuilder.
    /// </summary>
    public class PyStringMaker
    {
        List<PyLine> Lines = new List<PyLine>();
        int CurrentTab = 0;

        /// <summary>
        /// Appends a line to the python string builder.
        /// </summary>
        public void AppendLine(string line="")
        {
            Lines.Add(new PyLine()
            {
                Tabs = CurrentTab,
                Line = line
            });
        }

        /// <summary>
        /// Trims the end of the string.
        /// </summary>
        public void TrimEnd(params char[] chars)
        {
            if (Lines.Count == 0) { return; }
            Lines[Lines.Count - 1] = new PyLine()
            {
                Line = Lines[Lines.Count - 1].Line.TrimEnd(chars),
                Tabs = Lines[Lines.Count - 1].Tabs
            };
        }

        public override string ToString()
        {
            return string.Join("\r\n", Lines.Select(x => x.Make()));
        }

        //Tabs in and out.
        public void TabIn() { CurrentTab++; }
        public void TabOut() { CurrentTab--; if (CurrentTab < 0) { CurrentTab = 0; } }
    }

    public struct PyLine
    {
        public int Tabs;
        public string Line;

        public string Make()
        {
            string made = "";

            for (int i=0; i<Tabs; i++)
            {
                made += "\t";
            }

            made += Line;
            return made;
        }
    }
}
