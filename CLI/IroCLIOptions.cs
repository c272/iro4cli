using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iro4cli.CLI
{
    /// <summary>
    /// The command line options class for generation.
    /// </summary>
    public class IroCLIOptions
    {
        /// <summary>
        /// The path to the grammar file to convert.
        /// </summary>
        [Value(0, Default = null, HelpText = "The Iro file to compile into other grammars.", Required = true)]
        public string File { get; set; }

        /// <summary>
        /// Displays the version header when isolated.
        /// </summary>
        [Option("version", Default = false, HelpText = "Shows the version information for this build of Iro.")]
        public bool ShowVersionHeader { get; set; }

        /// <summary>
        /// The output directory to generate into.
        /// </summary>
        [Option('o', "output", Default = null, HelpText = "Sets the folder to output results into.")]
        public string OutputPath { get; set; }

        /// <summary>
        /// Whether to generate textmate grammars or not.
        /// </summary>
        [Option('t', "textmate", Default = false, HelpText = "Generates a textmate grammar file.")]
        public bool GenerateTextmate { get; set; }

        /// <summary>
        /// Whether to generate a VSCode extension with a textmate file.
        /// </summary>
        [Option('v', "vscode", Default = false, HelpText = "Generates a VSCode extension using a Textmate grammar.")]
        public bool GenerateVSCodeExtension { get; set; }

        /// <summary>
        /// Whether to generate an Atom extension with a CoffeeScript textmate file.
        /// </summary>
        [Option('e', "atom-ext", Default = false, HelpText = "Generates an Atom extension using a CoffeeScript Textmate grammar.")]
        public bool GenerateAtomExtension { get; set; }

        /// <summary>
        /// Whether to generate an atom grammar file.
        /// </summary>
        [Option('a', "atom", Default = false, HelpText = "Generates an Atom grammar file.")]
        public bool GenerateAtom { get; set; }

        
        /// <summary>
        /// Whether to generate an Ace Editor grammar file.
        /// </summary>
        [Option('c', "ace", Default = false, HelpText = "Generates an Ace Editor grammar file.")]
        public bool GenerateAce { get; set; }

        /// <summary>
        /// Whether to generate a Pygments grammar file.
        /// </summary>
        [Option('p', "pygments", Default = false, HelpText = "Generates a Pygments grammar file.")]
        public bool GeneratePygments { get; set; }

        /// <summary>
        /// Whether to generate a Rouge grammar file.
        /// </summary>
        [Option('r', "rouge", Default = false, HelpText = "Generates a Rouge grammar file.")]
        public bool GenerateRouge { get; set; }

        /// <summary>
        /// Whether to generate a Sublime 3 grammar file.
        /// </summary>
        [Option('s', "sublime", Default = false, HelpText = "Generates a Sublime 3 grammar file.")]
        public bool GenerateSublime3 { get; set; }

        /// <summary>
        /// Whether to generate a CSS file for all styles.
        /// </summary>
        [Option('x', "css", Default = false, HelpText = "Generates a CSS file for all styles.")]
        public bool GenerateCSS { get; set; }
    }
}
