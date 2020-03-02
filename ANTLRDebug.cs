using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr4;
using Antlr4.Runtime;
using iro4cli.Grammar;

namespace iro4cli
{
    public static class ANTLRDebug
    {
        public static void PrintTokens(Lexer lexer)
        {
            //Getting tokens.
            var tokens = lexer.GetAllTokens();

            //Getting lexer vocabulary.
            var vocab = lexer.Vocabulary;

            //Printing, for each token.
            Console.WriteLine("ANTLR Lexed Tokens:");
            foreach (var tok in tokens)
            {
                if (vocab.GetSymbolicName(tok.Type) != "WS")
                    Console.WriteLine("[" + vocab.GetSymbolicName(tok.Type) + ", " + tok.Text.Replace("\n", "") + ", channel=" + tok.Channel + "]");
            }
            Console.WriteLine("");

            lexer.Reset();
        }

        /// <summary>
        /// Prints the parse list for debugging information.
        /// </summary>
        public static void PrintParseList(iroParser.CompileUnitContext tree, iroParser parser)
        {
            //Printing parse tree.
            Console.WriteLine("ANTLR Parse Tree:");
            Console.WriteLine(tree.ToStringTree(parser));
            Console.WriteLine("-\nStatement Length: " + tree.statement().Length);
            Console.WriteLine("");
        }
    }
}