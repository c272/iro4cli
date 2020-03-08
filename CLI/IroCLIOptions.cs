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
    class IroCLIOptions
    {
        /// <summary>
        /// The path to the grammar file to convert.
        /// </summary>
        [Value(0, Default = null, HelpText = "The Iro file to compile into other grammars.", Required = true)]
        public string File { get; }

        /// <summary>
        /// Whether to generate textmate grammars or not.
        /// </summary>
        [Option('t', "textmate", Default = true, HelpText = "Generates a textmate grammar file.")]
        public bool GenerateTextmate { get; }

        /// <summary>
        /// Whether to generate a VSCode extension with a textmate file.
        /// </summary>
        [Option('v', "vscode", Default = false, HelpText = "Generates a VSCode extension using a Textmate grammar.")]
        public bool GenerateVSCodeExtension { get; }

        /// <summary>
        /// Whether to generate an atom grammar file.
        /// </summary>
        [Option('a', "atom", Default = false, HelpText = "Generates an Atom grammar file.")]
        public bool GenerateAtom { get; }

        
        /// <summary>
        /// Whether to generate an Ace Editor grammar file.
        /// </summary>
        [Option('c', "ace", Default = false, HelpText = "Generates an Ace Editor grammar file.")]
        public bool GenerateAce { get; }

        /// <summary>
        /// Whether to generate a Pygments grammar file.
        /// </summary>
        [Option('p', "pygments", Default = false, HelpText = "Generates a Pygments grammar file.")]
        public bool GeneratePygments { get; }

        /// <summary>
        /// Whether to generate a Rouge grammar file.
        /// </summary>
        [Option('r', "rouge", Default = false, HelpText = "Generates a Rouge grammar file.")]
        public bool GenerateRouge { get; }

        /// <summary>
        /// Whether to generate a Sublime 3 grammar file.
        /// </summary>
        [Option('s', "sublime", Default = false, HelpText = "Generates a Sublime 3 grammar file.")]
        public bool GenerateSublime3 { get; }

        /// <summary>
        /// Whether to generate a CSS file for all styles.
        /// </summary>
        [Option('x', "css", Default = false, HelpText = "Generates a CSS file for all styles.")]
        public bool GenerateCSS { get; }
    }
}
