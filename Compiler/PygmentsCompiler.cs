using iro4cli.Compile;
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
            //todo
            return null;
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