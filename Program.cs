using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iro4cli.Compile;
using CommandLine;

namespace iro4cli
{
    class Program
    {
        static void Main(string[] args)
        {
            //Load the first arg as a file for testing purposes.
            string text = File.ReadAllText(args[0]);

            //Run the emulator and get the variables back.
            var outputVars = Emulator.Run(text);

            //Compile the output variables into text.
            List<CompileResult> compileResults = Compiler.Compile(outputVars, new TextmateCompiler());
        }
    }
}
