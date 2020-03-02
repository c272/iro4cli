using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr4.Runtime;
using Antlr4;
using iro4cli.Grammar;

namespace iro4cli
{
    /// <summary>
    /// Emulates running Iro, and generating all surrounding grammars.
    /// </summary>
    public class Emulator
    {
        /// <summary>
        /// Runs the Iro emulator with a given input.
        /// </summary>
        public static void Run(string input)
        {
            var s_chars = new AntlrInputStream(input);
            var s_lexer = new iroLexer(s_chars);
            ANTLRDebug.PrintTokens(s_lexer);

            var s_tokens = new CommonTokenStream(s_lexer);
            var s_parse = new iroParser(s_tokens);
            
            //Execute.
            s_parse.BuildParseTree = true;
            var s_tree = s_parse.compileUnit();
            ANTLRDebug.PrintParseList(s_tree, s_parse);
        }
    }
}
