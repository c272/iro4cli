using Antlr4.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iro4cli
{
    /// <summary>
    /// Represents error handling in iro4cli.
    /// </summary>
    public static class Error
    {
        /// <summary>
        /// Exits the program with an error message.
        /// </summary>
        public static void Fatal(ParserRuleContext ctx, string msg)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("[ERROR] Line " + ctx.Start.Line + " - " + msg);
            Console.ResetColor();

            Environment.Exit(-1);
        }

        /// <summary>
        /// Logs a warning to the console.
        /// </summary>
        public static void Warn(ParserRuleContext ctx, string msg)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("[WARN] Line " + ctx.Start.Line + " - " + msg);
            Console.ResetColor();
        }
    }
}
