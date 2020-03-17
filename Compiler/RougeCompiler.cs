using iro4cli.Compile;
using shortid;
using System;
using System.Collections.Generic;
using System.Linq;

namespace iro4cli
{
    /// <summary>
    /// Compiles a target for Rouge.
    /// </summary>
    public class RougeCompiler : ICompileTarget
    {
        //Contexts that are queued to generate.
        public Dictionary<string, InlinePushContextMember> queuedContexts = new Dictionary<string, InlinePushContextMember>();

        /// <summary>
        /// Turns Iro precompile data into a compiled Rouge target.
        /// </summary>
        public CompileResult Compile(IroPrecompileData data)
        {
            var text = new RubyStringMaker();

            //Add the coding header and modules.
            text.AppendLine("# -*- coding: utf-8 -*- #");
            text.AppendLine();
            text.AppendLine("module Rouge");
            text.TabIn();
            text.AppendLine("module Lexers");
            text.TabIn();
            text.AppendLine("class " + data.Name[0].ToString().ToUpper() + data.Name.Substring(1) + " < RegexLexer");
            text.TabIn();

            //Add basic metadata.
            text.AppendLine("title \"" + data.Name[0].ToString().ToUpper() + data.Name.Substring(1) + "\"");
            text.AppendLine("tag '" + data.Name[0].ToString().ToUpper() + data.Name.Substring(1) + "'");
            text.AppendLine("mimetypes 'text/x-" + data.Name[0].ToString().ToUpper() + data.Name.Substring(1) + "'");
            string fileTypes = "";
            foreach (var ext in data.FileExtensions)
            {
                fileTypes += "'*." + ext + "', ";
            }
            fileTypes = fileTypes.TrimEnd(',', ' ');
            text.AppendLine("filenames " + fileTypes);
            text.AppendLine();

            //Check the main context exists.
            var mainCtx = data.Contexts.FirstOrDefault(x => x.Name == "main");
            if (mainCtx == null)
            {
                Error.Compile("No context exists named 'main' to use as the root context. Add a 'main' context.");
                return null;
            }
            AddContext(mainCtx, "root", data, ref text);

            //Loop over all other contexts, create.
            foreach (var ctx in data.Contexts)
            {
                //Already did main.
                if (ctx.Name == "main") { continue; }

                AddContext(mainCtx, ctx.Name, data, ref text);
            }

            //Add all the queued helpers.
            while (queuedContexts.Count != 0)
            {
                var item = queuedContexts.ElementAt(0);
                AddILPContext(item.Key, item.Value, data, ref text);
                queuedContexts.Remove(item.Key);
            }

            //Close all the modules.
            text.TabOut();
            text.AppendLine("end");
            text.TabOut();
            text.AppendLine("end");
            text.TabOut();
            text.AppendLine("end");

            return new CompileResult()
            {
                GeneratedFile = text.ToString(),
                Target = Target.Rouge
            };
        }

        //Adds a single context to the text.
        private void AddContext(IroContext ctx, string name, IroPrecompileData data, ref RubyStringMaker text)
        {
            //Reset includes, create new state.
            includedThisRun = new List<string>();
            text.AppendLine("state:" + name);
            text.TabIn();

            //Loop over rules, add them.
            foreach (var member in ctx.Members)
            {
                AddContextMember(member, data, ref text);
            }

            text.TabOut();
            text.AppendLine("end");
        }

        //Adds a single context member to the text.
        List<string> includedThisRun = new List<string>();
        private void AddContextMember(ContextMember member, IroPrecompileData data, ref RubyStringMaker text)
        {
            //What type is it?
            switch (member.Type)
            {
                case ContextMemberType.Pattern:

                    //Normal pattern.
                    var pattern = (PatternContextMember)member;
                    string rule = "rule /";
                    rule += pattern.Data + "/";

                    //Get styles for pattern. Do they match up?
                    var styles = GetPatternStyles(pattern.Styles, data);
                    if (styles.Count > 1)
                    {
                        //Grouped.
                        rule += " do";
                        text.AppendLine(rule);
                        text.TabIn();
                        rule = "groups ";

                        foreach (var style in styles)
                        {
                            rule += style.PygmentsScope.Replace(".", "::") + ", ";
                        }

                        rule = rule.TrimEnd(' ', ',');

                        text.AppendLine(rule);
                        text.TabOut();
                        text.AppendLine("end");
                    }
                    else if (styles.Count != 0)
                    {
                        //Ungrouped.
                        rule += ", " + styles[0].PygmentsScope.Replace(".", "::");
                        text.AppendLine(rule);
                    }
                    else
                    {
                        Error.Compile("No styles given for rule '" + pattern.Data + "'.");
                        return;
                    }
                    break;

                case ContextMemberType.InlinePush:

                    //Inline push pattern.
                    var ilp = (InlinePushContextMember)member;
                    string ruleIlp = "rule /";
                    ruleIlp += ilp.Data + "/, ";

                    //Get styles for pattern. Do they match up?
                    var pushStyles = GetPatternStyles(ilp.Styles, data);
                    string helperName = "helper_" + ShortId.Generate(7);
                    if (pushStyles.Count > 1)
                    {
                        //Grouped.
                        ruleIlp += " do";
                        text.AppendLine(ruleIlp);
                        text.TabIn();

                        ruleIlp = "groups ";

                        foreach (var style in pushStyles)
                        {
                            ruleIlp += style.PygmentsScope.Replace(".", "::") + ", ";
                        }

                        ruleIlp = ruleIlp.TrimEnd(' ', ',');
                        text.AppendLine(ruleIlp);

                        //Add the push.
                        ruleIlp = "push :" + helperName;

                        //Tab out, end.
                        text.TabOut();
                        text.AppendLine("end");
                    }
                    else if (pushStyles.Count != 0)
                    {
                        //Ungrouped.
                        ruleIlp += ", " + pushStyles[0].PygmentsScope.Replace(".", "::") + ", :" + helperName;
                        text.AppendLine(ruleIlp);
                    }
                    else
                    {
                        Error.Compile("No styles given for rule '" + ilp.Data + "'.");
                        return;
                    }

                    //Queue the push scope.
                    QueueILPScope(helperName, ilp);
                    break;

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
                        AddContextMember(mem, data, ref text);
                    }
                    break;

                //A simple pop rule.
                case ContextMemberType.Pop:
                    //Inline push pattern.
                    var pop = (PatternContextMember)member;
                    string rulePop = "rule /";
                    rulePop += pop.Data + "/, ";

                    //Get styles for pattern. Do they match up?
                    var popStyles = GetPatternStyles(pop.Styles, data);
                    if (popStyles.Count > 1)
                    {
                        //Grouped.
                        rulePop += " do";
                        text.AppendLine(rulePop);
                        text.TabIn();

                        ruleIlp = "groups ";

                        foreach (var style in popStyles)
                        {
                            ruleIlp += style.PygmentsScope.Replace(".", "::") + ", ";
                        }

                        ruleIlp = ruleIlp.TrimEnd(' ', ',');
                        text.AppendLine(ruleIlp);

                        //Add the pop.
                        ruleIlp = "push :pop!";

                        //Tab out, end.
                        text.TabOut();
                        text.AppendLine("end");
                    }
                    else if (popStyles.Count != 0)
                    {
                        //Ungrouped.
                        rulePop += ", " + popStyles[0].PygmentsScope.Replace(".", "::") + ", :pop!";
                        text.AppendLine(rulePop);
                    }
                    else
                    {
                        Error.Compile("No styles given for rule '" + pop.Data + "'.");
                        return;
                    }

                    break;
            }
        }

        //Queues an inline push scope to be generated.
        private void QueueILPScope(string helperName, InlinePushContextMember ilp)
        {
            queuedContexts.Add(helperName, ilp);
        }

        //Adds an inline push context member.
        private void AddILPContext(string name, InlinePushContextMember mem, IroPrecompileData data, ref RubyStringMaker text)
        {
            //Create a pop.
            var pop = new PatternContextMember()
            {
                Data = mem.PopData,
                Styles = mem.Styles,
                Type = ContextMemberType.Pop
            };
            var members = new List<ContextMember>(mem.Patterns);
            members.Add(pop);

            //Add the context.
            AddContext(new IroContext(name)
            {
                Members = members,
                Name = name
            }, name, data, ref text);
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
    public class RubyStringMaker
    {
        List<RubyLine> Lines = new List<RubyLine>();
        int CurrentTab = 0;

        /// <summary>
        /// Appends a line to the python string builder.
        /// </summary>
        public void AppendLine(string line = "")
        {
            Lines.Add(new RubyLine()
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
            Lines[Lines.Count - 1] = new RubyLine()
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

    public struct RubyLine
    {
        public int Tabs;
        public string Line;

        public string Make()
        {
            string made = "";

            for (int i = 0; i < Tabs; i++)
            {
                made += "\t";
            }

            made += Line;
            return made;
        }
    }
}