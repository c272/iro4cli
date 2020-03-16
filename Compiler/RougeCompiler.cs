using iro4cli.Compile;
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
            AddContext(mainCtx, ref text);

            //Close all the modules.
            text.TabOut();
            text.AppendLine("end");
            text.TabOut();
            text.AppendLine("end");
            text.TabOut();
            text.AppendLine("end");
        }

        //Adds a single context to the text.
        private void AddContext(IroContext mainCtx, string name, ref RubyStringMaker text)
        {
            text.AppendLine("state:" + name);
            text.TabIn();

            //

            text.TabOut();
            text.AppendLine("end");
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