using iro4cli.Compile;
using shortid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace iro4cli
{
    /// <summary>
    /// Compiles pre-compile data into the Ace grammar format.
    /// </summary>
    public class AceCompiler : ICompileTarget
    {
        //Queued contexts to be created.
        private Dictionary<string, InlinePushContextMember> queuedContexts = new Dictionary<string, InlinePushContextMember>();
        private Dictionary<Tuple<string, string>, string> contextsCreated = new Dictionary<Tuple<string, string>, string>();

        /// <summary>
        /// Compiles a set of Iro precompile data into a generated file.
        /// </summary>
        public CompileResult Compile(IroPrecompileData data)
        {
            var text = new StringBuilder();

            //Add the header.
            text.AppendLine("/*");
            text.AppendLine("* To try in Ace editor, copy and paste into the mode creator");
            text.AppendLine("*here : http://ace.c9.io/tool/mode_creator.html");
            text.AppendLine("*/");
            text.AppendLine();
            text.AppendLine("define(function(require, exports, module) {");
            text.AppendLine("\"use strict\";");
            text.AppendLine("var oop = require(\"../lib/oop\");");
            text.AppendLine("var TextHighlightRules = require(\"./text_highlight_rules\").TextHighlightRules;");
            text.AppendLine("/* --------------------- START ----------------------------- */");
            text.AppendLine("var MysampleHighlightRules = function() {");
            text.AppendLine("this.$rules = { ");

            //Define the main ruleset.
            var mainCtx = data.Contexts.FirstOrDefault(x => x.Name == "main");
            if (mainCtx == null)
            {
                Error.Compile("No 'main' context exists to create patterns from.");
                return null;
            }

            //Add the main context.
            AddContext(mainCtx, "start", data, ref text);

            //Add all other contexts.
            foreach (var context in data.Contexts)
            {
                //Ignore main, already processed.
                if (context.Name == "main") { continue; }

                AddContext(context, context.Name, data, ref text);
            }

            //Add the queued contexts from inline pushes.
            while (queuedContexts.Count > 0)
            {
                AddQueuedContext(queuedContexts.First().Key, queuedContexts.First().Value, data, ref text);
                queuedContexts.Remove(queuedContexts.First().Key);
            }

            //Trim the trailing ",".
            text = text.TrimEnd(',');

            //Close the set.
            text.AppendLine("};");
            text.AppendLine("this.normalizeRules();");
            text.AppendLine("};");

            //Footer.
            text.AppendLine("/* ------------------------ END ------------------------------ */");
            text.AppendLine("oop.inherits(MysampleHighlightRules, TextHighlightRules);");
            text.AppendLine("exports.MysampleHighlightRules = MysampleHighlightRules;");
            text.AppendLine("});");

            return new CompileResult()
            {
                GeneratedFile = text.ToString(),
                Target = Target.Ace
            };
        }

        //Adds a single context to the compiling string.
        private void AddContext(IroContext thisCtx, string ctxName, IroPrecompileData data, ref StringBuilder text)
        {
            //Begin this context.
            text.AppendLine("\"" + ctxName + "\": [");

            //Loop over the context, add all members.
            for (int i = 0; i < thisCtx.Members.Count; i++)
            {
                var member = thisCtx.Members[i];
                AddMember(member, ctxName, ref text, data);
            }

            //Add the default member.
            text.AppendLine("{");
            text.AppendLine("defaultToken: \"text\"");
            text.AppendLine("}");

            //Close the context.
            text.AppendLine("],");
        }

        //Adds a single context member to the compiling string.
        private void AddMember(ContextMember member, string ctxName, ref StringBuilder text, IroPrecompileData data)
        {
            if (member.Type == ContextMemberType.Pattern)
            {
                //Normal pattern.
                var pattern = ((PatternContextMember)member);
                var patternStyles = GetPatternStyles(pattern.Styles, data);

                text.AppendLine("{");
                text.AppendLine("\"token\": \"" + patternStyles[0].AceScope + "\",");
                //Replace normal '\' with '\\', since it's inside another string.
                text.AppendLine("\"regex\": \"" + pattern.Data.Replace("\\", "\\\\") + "\"");

                //Close pattern.
                text.AppendLine("},");
            }
            else if (member.Type == ContextMemberType.InlinePush)
            {
                //Inline push pattern.
                var ilp = ((InlinePushContextMember)member);
                var ilpStyles = GetPatternStyles(ilp.Styles, data);

                //Get the push data.
                text.AppendLine("{");
                text.AppendLine("\"token\": \"" + ilpStyles[0].AceScope + "\",");
                //Replace normal '\' with '\\', since it's inside another string.
                text.AppendLine("\"regex\": \"" + ilp.Data.Replace("\\", "\\\\").Replace("\"", "\\\"") + "\",");

                //Queue an ILP context to be created later (if it's not already created).
                string pushCtxName;
                var ilpPushTuple = new Tuple<string, string>(ctxName, ilp.Data);
                if (!contextsCreated.ContainsKey(ilpPushTuple))
                {
                    pushCtxName = "helper_" + ShortId.Generate(7);

                    //Queue and add to the "done" list.
                    QueueILPContext(pushCtxName, ilp);
                    contextsCreated.Add(ilpPushTuple, pushCtxName);
                }
                else
                {
                    //Already created one for this regex.
                    pushCtxName = contextsCreated[ilpPushTuple];
                }

                text.AppendLine("\"push\": \"" + pushCtxName + "\"");

                //Close pattern.
                text.AppendLine("},");

            }
            else if (member.Type == ContextMemberType.Include)
            {
                //Include.
                //Find the top-level context being included.
                var toInclude = data.Contexts.FirstOrDefault(x => x.Name == member.Data);
                if (toInclude == null)
                {
                    Error.Compile("No context with name '" + member.Data + "' exists to include.");
                    return;
                }

                //Make sure toInclude isn't this one.
                if (toInclude.Name == ctxName)
                {
                    Error.CompileWarning("Recursive include loop detected, ignoring it.");
                    return;
                }

                //Loop over all members in toInclude, include them.
                foreach (var memberInc in toInclude.Members)
                {
                    AddMember(memberInc, toInclude.Name, ref text, data);
                }
            }
        }

        /// <summary>
        /// Enqueues an inline push helper context to be created later.
        /// </summary>
        private void QueueILPContext(string name, InlinePushContextMember ilp)
        {
            queuedContexts.Add(name, ilp);
        }

        /// <summary>
        /// Adds a context that has been queued to the builder.
        /// </summary>
        private void AddQueuedContext(string ctxName, InlinePushContextMember ilp, IroPrecompileData data, ref StringBuilder text)
        {
            //Begin this context.
            text.AppendLine("\"" + ctxName + "\": [");

            //Get styles out for pop rule.
            var popStyles = GetPatternStyles(ilp.PopStyles, data);

            //Add the pop rule.
            text.AppendLine("{");
            text.AppendLine("\"token\": \"" + popStyles[0].AceScope + "\",");
            text.AppendLine("\"regex\": \"" + ilp.PopData.Replace("\\", "\\\\").Replace("\"", "\\\"") + "\",");
            text.AppendLine("\"next\": \"pop\"");
            text.AppendLine("},");

            //Add all patterns for the ILP.
            foreach (var pattern in ilp.Patterns)
            {
                AddMember(pattern, ctxName, ref text, data);
            }

            //Add the default style.
            text.AppendLine("{");
            text.AppendLine("defaultToken: \"text\"");
            text.AppendLine("}");

            //End this context.
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
            foreach (var style in styles)
            {
                if (style.AceScope == null)
                {
                    //Missing ace, can inherit?
                    if (style.TextmateScope == null)
                    {
                        Error.Compile("No Textmate or Atom scope is defined for style '" + style.Name + "'.");
                        return null;
                    }

                    style.AceScope = style.TextmateScope;
                }
            }

            return styles;
        }
    }

    /// <summary>
    /// Helper for this compiler's string builder.
    /// </summary>
    public static class StringBuilderExtensions
    {
        public static StringBuilder TrimEnd(this StringBuilder sb, char c)
        {
            if (sb == null || sb.Length == 0) return sb;

            int i = sb.Length - 1;
            for (; i >= 0; i--)
                if (sb[i] != c)
                    break;

            if (i < sb.Length - 1)
                sb.Length = i + 1;

            return sb;
        }
    }
}