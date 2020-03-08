using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iro4cli.Compile;
using CommandLine;
using iro4cli.CLI;

namespace iro4cli
{
    class Program
    {
        static void Main(string[] args)
        {
            //Disable help writer.
            var parser = new Parser(config => config.HelpWriter = null);

            //Get the command line options out.
            parser.ParseArguments<IroCLIOptions>(args)
                          .WithParsed(Run)
                          .WithNotParsed(HandleParseError);
        }

        //Called when an error has occured parsing the command line options.
        private static void HandleParseError(IEnumerable<CommandLine.Error> errors)
        {
            Error.CommandLine("Invalid command line options, must include an Iro file to parse in position 0.");
        }

        //When the command line options have been successfully parsed, this is run.
        private static void Run(IroCLIOptions opts)
        {
            //Get a list of targets together.
            var targets = new List<ICompileTarget>();
            if (opts.GenerateTextmate || opts.GenerateVSCodeExtension)
            {
                //Generate textmate.
                targets.Add(new TextmateCompiler());
            }
            if (opts.GenerateAce) { targets.Add(new AceCompiler()); } //ace
            if (opts.GenerateAtom) { targets.Add(new AtomCompiler()); } //atom
            if (opts.GenerateCSS) { targets.Add(new CSSCompiler()); } //css
            if (opts.GeneratePygments) { targets.Add(new PygmentsCompiler()); } //pygments
            if (opts.GenerateRouge) { targets.Add(new RougeCompiler()); } //rouge
            if (opts.GenerateSublime3) { targets.Add(new Sublime3Compiler()); } //sublime3

            //Attempt to read the Iro grammar.
            string iroGrammar;
            try
            {
                iroGrammar = File.ReadAllText(opts.File);    
            }
            catch (Exception e)
            {
                Error.CommandLine("Could not read Iro grammar - '" + e.Message + "'.");
                return;
            }

            //Run the parser emulator.
            var vars = Emulator.Run(iroGrammar);
            var compileResults = Compiler.Compile(vars, targets.ToArray());
            foreach (var result in compileResults)
            {
                Console.Write("Successfully generated for target '");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write(result.Target.ToString().Replace("Target.", ""));
                Console.ResetColor();
                Console.Write("'.\r\n");

                string ext = ".unknown";
                switch (result.Target)
                {
                    case Target.Ace: ext = ".js"; break;
                    case Target.Atom: ext = ".cson"; break;
                    case Target.CSS: ext = ".css"; break;
                    case Target.Pygments: ext = ".py"; break;
                    case Target.Rouge: ext = ".rb"; break;
                    case Target.Sublime3: ext = ".yaml"; break;
                    case Target.Textmate:
                        ext = ".tmLanguage";
                        if (opts.GenerateVSCodeExtension)
                        {
                            //Write a VSCode extension to file.
                            TextmateCompiler.WriteVSCExtension(result.GeneratedFile, Environment.CurrentDirectory, vars);
                        }
                        break;
                    default:
                        Error.CommandLineWarning("Unsupported target '" + result.Target.ToString() + "', currently not implemented.");
                        continue;
                }

                //todo: add a directory value
                WriteFile(result.GeneratedFile, ((IroValue)vars["name"]).Value, ext, null);
            }
        }

        /// <summary>
        /// Writes a file to a given directory with a given extension.
        /// </summary>
        private static void WriteFile(string generatedFile, string projectName, string ext, string directory=null)
        {
            //Default to the current directory.
            if (directory == null)
            {
                directory = Environment.CurrentDirectory;
            }

            //Generate a file path.
            string filePath = Path.Combine(directory, projectName + ext);

            //Try to write the file.
            try
            {
                File.WriteAllText(filePath, generatedFile);
            }
            catch (Exception e)
            {
                Error.CommandLineWarning("Failed to write generated grammar to file, '" + e.Message + "'.");
            }
        }
    }
}
