using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iro4cli.Compile;
using CommandLine;
using CommandLine.Text;
using iro4cli.CLI;
using iro4cli.Templates;
using System.Reflection;

namespace iro4cli
{
    class Program
    {
        static void Main(string[] args)
        {
            //Args just "--version"? Display version.
            if (args.Length == 1 && args[0] == "--version")
            {
                PrintVersionInfo();
                return;
            }

            //Disable help writer.
            var parser = new Parser(config => config.HelpWriter = null);

            //Get the command line options out.
            var parserResult = parser.ParseArguments<IroCLIOptions>(args)
                                    .WithParsed(Run);

            //Generate help based on parse result.
            var helpText = HelpText.AutoBuild(parserResult, h =>
            {
                h.Heading = Resources.asciiArt;
                h.Copyright = "\niro4cli (c) C272, 2020. Iro (c) Chris Ainsley.";
                return h;
            }, e => e);

            //Display help if not parsed.
            parserResult.WithNotParsed(errs => HandleParseError(helpText, errs));
        }

        //Prints version info and a nice little text banner to console.
        private static void PrintVersionInfo()
        {
            Console.WriteLine(Resources.asciiArt);
            Console.WriteLine($"v{Assembly.GetExecutingAssembly().GetName().Version.ToString()}".PadLeft(52, ' '));
            Console.WriteLine("\niro4cli (c) C272, 2020. Iro (c) Chris Ainsley.\n");
        }

        //Called when an error has occured parsing the command line options.
        private static void HandleParseError(HelpText helpText, IEnumerable<CommandLine.Error> errors)
        {
            //If it's a help request, then actually just display the help.
            if (errors.First()?.Tag == ErrorType.HelpRequestedError)
            {
                Console.WriteLine(helpText.ToString());
                return;
            }

            //Show the error help.
            Error.CommandLine(helpText.ToString());
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
            if (opts.GenerateAtom || opts.GenerateAtomExtension) { targets.Add(new AtomCompiler()); } //atom
            if (opts.GenerateCSS) { targets.Add(new CSSCompiler()); } //css
            if (opts.GeneratePygments) { targets.Add(new PygmentsCompiler()); } //pygments
            if (opts.GenerateRouge) { targets.Add(new RougeCompiler()); } //rouge
            if (opts.GenerateSublime3) { targets.Add(new Sublime3Compiler()); } //sublime3

            //Set output directory to current directory, if none supplied.
            if (opts.OutputPath == null)
                opts.OutputPath = Environment.CurrentDirectory;

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
                //Yay, successfully generated!
                Console.Write("Successfully generated for target '");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write(result.Target.ToString().Replace("Target.", ""));
                Console.ResetColor();
                Console.Write("'.\r\n");

                //Try to write the file now (with the right file extension).
                string ext = ".unknown";
                switch (result.Target)
                {
                    case Target.Ace: ext = ".js"; break;
                    case Target.Atom: 
                        ext = ".cson";
                        if (opts.GenerateAtomExtension)
                        {
                            AtomCompiler.MakeAtomExtension(((IroValue)vars["name"]).Value, result.GeneratedFile, opts.OutputPath);
                        }
                        break;
                    case Target.CSS: ext = ".css"; break;
                    case Target.Pygments: ext = ".py"; break;
                    case Target.Rouge: ext = ".rb"; break;
                    case Target.Sublime3: ext = ".yaml"; break;
                    case Target.Textmate:
                        ext = ".tmLanguage";
                        if (opts.GenerateVSCodeExtension)
                        {
                            //Write a VSCode extension to file.
                            TextmateCompiler.WriteVSCExtension(result.GeneratedFile, opts.OutputPath, vars);
                        }
                        break;
                    default:
                        Error.CommandLineWarning("Unsupported target '" + result.Target.ToString() + "', currently not implemented.");
                        continue;
                }

                //Output the generated result.
                WriteFile(result.GeneratedFile, ((IroValue)vars["name"]).Value, ext, opts.OutputPath);
            }
        }

        /// <summary>
        /// Writes a file to a given directory with a given extension.
        /// </summary>
        private static void WriteFile(string generatedFile, string projectName, string ext, string directory=null)
        {
            //Default to the current directory.
            if (directory == null)
                directory = Environment.CurrentDirectory;

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
