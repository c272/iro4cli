using iro4cli.Templates;
using Newtonsoft.Json;
using shortid;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace iro4cli.Compile
{
    /// <summary>
    /// Iro compile target for Textmate grammars.
    /// </summary>
    public class TextmateCompiler : ICompileTarget
    {
        //The pending context members to be evaluated.
        private Dictionary<string, List<ContextMember>> pendingContexts = new Dictionary<string, List<ContextMember>>();
        
        /// <summary>
        /// Compiles a set of pre compile data into a textmate file.
        /// </summary>
        public CompileResult Compile(IroPrecompileData data)
        {
            var text = new StringBuilder();

            //Add pre-baked headers.
            text.AppendLine("<?xml  version=\"1.0\" encoding=\"UTF-8\"?>");
            text.AppendLine("<!DOCTYPE plist PUBLIC \"-//Apple Computer//DTD PLIST 1.0//EN\"   \"http://www.apple.com/DTDs/PropertyList-1.0.dtd\">");
            text.AppendLine("<plist version=\"1.0\">");
            text.AppendLine("<!-- Generated via. Iro4CLI -->");

            //Start the dictionary.
            text.AppendLine("<dict>");

            //Input the file types.
            if (data.FileExtensions == null || data.FileExtensions.Count == 0)
            {
                Error.Compile("No file extensions provided to map grammar against. Use 'file_extensions' to define them.");
                return null;
            }
            text.AppendLine("<key>fileTypes</key>");
            text.AppendLine("<array>");
            foreach (var type in data.FileExtensions)
            {
                text.AppendLine("<string>" + type + "</string>");
            }
            text.AppendLine("</array>");

            //The name of the grammar.
            text.AppendLine("<key>name</key>");
            text.AppendLine("<string>" + data.Name + "</string>");
            

            //Pattern array, just including the main context here.
            if (data.Contexts.FindIndex(x => x.Name == "main") == -1)
            {
                Error.Compile("No entrypoint context named 'main' exists. You need to make a context named 'main' to start the grammar state at.");
                return null;
            }
            text.AppendLine("<key>patterns</key>");
            text.AppendLine("<array>");
            text.AppendLine("<dict>");
            text.AppendLine("<key>include</key>");
            text.AppendLine("<string>#main</string>");
            text.AppendLine("</dict>");
            text.AppendLine("</array>");

            //Name of the source.
            text.AppendLine("<key>scopeName</key>");
            text.AppendLine("<string>source." + data.Name + "</string>");

            //UUID
            text.AppendLine("<key>uuid</key>");
            text.AppendLine("<string>" + data.UUID + "</string>");

            //Main repository for all contexts.
            text.AppendLine("<key>repository</key>");
            text.AppendLine("<dict>");

            //Define all the contexts inside the array.
            foreach (var context in data.Contexts)
            {
                //Add the pre-existing context.
                AddContext(ref text, context, data);
            }

            //Contexts done parsing, complete all queued contexts.
            foreach (var queued in pendingContexts)
            {
                AddContext(ref text, new IroContext(queued.Key)
                {
                    Members = queued.Value
                }, data);
            }
            pendingContexts = new Dictionary<string, List<ContextMember>>();

            //Close the textmate scopes.
            text.AppendLine("</dict>");
            text.AppendLine("</dict>");
            text.AppendLine("</plist>");

            return new CompileResult()
            {
                GeneratedFile = FormatXml(text.ToString()),
                Target = Target.Textmate
            };
        }

        /// <summary>
        /// Adds a single context to the builder.
        /// </summary>
        private void AddContext(ref StringBuilder text, IroContext context, IroPrecompileData data)
        {
            text.AppendLine("<key>" + context.Name + "</key>");
            text.AppendLine("<dict>");

            //Define all the patterns in this context.
            text.AppendLine("<key>patterns</key>");
            text.AppendLine("<array>");
            foreach (var pattern in context.Members)
            {
                AddPattern(ref text, pattern, data);
            }
            text.AppendLine("</array>");
            text.AppendLine("</dict>");
        }

        /// <summary>
        /// Adds a single pattern to the list of patterns.
        /// </summary>
        private void AddPattern(ref StringBuilder text, ContextMember pattern, IroPrecompileData data)
        {
            //What type is it?
            switch (pattern.Type)
            {
                case ContextMemberType.Include:
                    text.AppendLine("<dict>");
                    text.AppendLine("<key>include</key>");
                    text.AppendLine("<string>#" + pattern.Data + "</string>");
                    text.AppendLine("</dict>");
                    break;
                case ContextMemberType.Pattern:
                    AddPatternRaw(ref text, (PatternContextMember)pattern, data);
                    break;
                case ContextMemberType.InlinePush:
                    AddInlinePush(ref text, (InlinePushContextMember)pattern, data);
                    break;
                case ContextMemberType.Push:
                    throw new NotImplementedException();
                case ContextMemberType.Pop:
                    throw new NotImplementedException();
                default:
                    Error.CompileWarning("Failed to add pattern, unrecognized context member type '" + pattern.Type.ToString() + "'.");
                    return;
            }
        }

        /// <summary>
        /// Adds a InlinePushContextMember to the text.
        /// </summary>
        private void AddInlinePush(ref StringBuilder text, InlinePushContextMember pattern, IroPrecompileData data)
        {
            //Get styles from the pattern.
            var styles = GetPatternStyles(pattern.Styles, data);
            text.AppendLine("<dict>");

            //Patterns match up with context groups?
            if (!Compiler.GroupsMatch(pattern.Data, styles.Count))
            {
                Error.Compile("Mismatch between capture groups and number of styles for inline push with regex '" + pattern.Data + "'.");
                return;
            }

            //Begin capture regex.
            text.AppendLine("<key>begin</key>");
            text.AppendLine("<string>" + pattern.Data.FormatForXML() + "</string>");

            //Begin capture styles.
            text.AppendLine("<key>beginCaptures</key>");
            text.AppendLine("<dict>");
            for (int i=0; i<styles.Count; i++)
            {
                text.AppendLine("<key>" + (i + 1) + "</key>");
                text.AppendLine("<dict>");
                text.AppendLine("<key>name</key>");
                text.AppendLine("<string>" + styles[i].TextmateScope + "." + data.Name + "</string>");
                text.AppendLine("</dict>");
            }
            text.AppendLine("</dict>");

            //Is a default style assigned?
            int defaultStyleIndex = pattern.Patterns.FindIndex(x => x.Type == ContextMemberType.DefaultStyle);
            if (defaultStyleIndex != -1)
            {
                //Are other patterns defined?
                if (pattern.Patterns.FindIndex(x => x.Type == ContextMemberType.Pattern) != -1)
                {
                    //Warn the user.
                    Error.CompileWarning("You cannot define unique patterns when a 'default_style' attribute is applied. Ignoring them.");
                }

                //Get the style out.
                var style = GetPatternStyles(new List<string>()
                {
                    pattern.Patterns[defaultStyleIndex].Data
                }, data);

                //Add default content name.
                text.AppendLine("<key>contentName</key>");
                text.AppendLine("<string>" + style[0].TextmateScope + "." + data.Name + "</string>");
            }
            else
            {
                //Actual patterns defined, not just default.
                //Begin patterns, capture all "pattern" sets and includes and queue them.
                text.AppendLine("<key>patterns</key>");
                text.AppendLine("<array>");

                //Include the queued context.
                if (pattern.Patterns.Count != 0)
                {
                    string helperName = "helper_" + ShortId.Generate(7);
                    text.AppendLine("<dict>");
                    text.AppendLine("<key>include</key>");
                    text.AppendLine("<string>#" + helperName + "</string>");
                    text.AppendLine("</dict>");

                    //Queue it.
                    QueueContext(helperName, pattern.Patterns);
                }
                text.AppendLine("</array>");
            }

            //Patterns done, pop condition & styles.
            var popStyles = GetPatternStyles(pattern.PopStyles, data);

            //Patterns match up with context groups?
            if (!Compiler.GroupsMatch(pattern.Data, styles.Count))
            {
                Error.Compile("Mismatch between capture groups and number of styles for pop with regex '" + pattern.PopData + "'.");
                return;
            }

            //Okay, add pop data.
            text.AppendLine("<key>end</key>");
            text.AppendLine("<string>" + pattern.PopData.FormatForXML() + "</string>");
            text.AppendLine("<key>endCaptures</key>");
            text.AppendLine("<dict>");
            for (int i = 0; i < popStyles.Count; i++)
            {
                text.AppendLine("<key>" + (i + 1) + "</key>");
                text.AppendLine("<dict>");
                text.AppendLine("<key>name</key>");
                text.AppendLine("<string>" + popStyles[i].TextmateScope + "." + data.Name + "</string>");
                text.AppendLine("</dict>");
            }
            text.AppendLine("</dict>");

            //Close the inline push.
            text.AppendLine("</dict>");
        }

        /// <summary>
        /// Queues a context to be created upon the end of the current context being created.
        /// </summary>
        private void QueueContext(string helperName, List<ContextMember> patterns)
        {
            pendingContexts.Add(helperName, patterns);
        }

        /// <summary>
        /// Adds a PatternContextMember to the text, rather than a pattern that could possibly be any ContextMember.
        /// </summary>
        private void AddPatternRaw(ref StringBuilder text, PatternContextMember pattern, IroPrecompileData data)
        {
            //Get styles from the pattern.
            var styles = GetPatternStyles(pattern.Styles, data);

            //Is the amount of patterns equal to the amount of context groups?
            //Use a hack of replacing bracket groups with normal letters.
            if (!Compiler.GroupsMatch(pattern.Data, styles.Count))
            {
                Error.Compile("Mismatch between capture groups and number of styles for pattern with regex '" + pattern.Data + "'.");
                return;
            }

            //Add the initial match.
            text.AppendLine("<dict>");
            text.AppendLine("<key>match</key>");
            text.AppendLine("<string>" + pattern.Data.FormatForXML() + "</string>");

            //Only one style? Just use the 'name' property.
            if (pattern.Styles.Count == 1) 
            {
                text.AppendLine("<key>name</key>");
                text.AppendLine("<string>" + styles[0].TextmateScope + "." + data.Name + "</string>");
            }
            else
            {
                //Multiple styles, define capture groups.
                text.AppendLine("<key>captures</key>");
                text.AppendLine("<dict>");
                for (int i=0; i<styles.Count; i++)
                {
                    text.AppendLine("<key>" + (i + 1) + "</key>");
                    text.AppendLine("<dict>");
                    text.AppendLine("<key>name</key>");
                    text.AppendLine("<string>" + styles[i].TextmateScope + "." + data.Name + "</string>");
                    text.AppendLine("</dict>");
                }
                text.AppendLine("</dict>");
            }
            text.AppendLine("</dict>");
        }

        /// <summary>
        /// Gets a list of styles for a specific pattern context member.
        /// </summary>
        private List<IroStyle> GetPatternStyles(List<string> styleNames, IroPrecompileData data)
        {
            //Check that styles exist, and try to get the first one out.
            if (styleNames.Count == 0)
            {
                Error.Compile("No style was defined for a pattern. All styles must have patterns.");
                return null;
            }

            //Get all the patterns out.
            var styles = new List<IroStyle>();
            foreach (var style in styleNames)
            {
                //Get the styles form the style list.
                int index = data.Styles.FindIndex(x => x.Name == style);
                if (index == -1)
                {
                    Error.Compile("A style '" + style + "' is referenced in a pattern, but it is not defined in the style map.");
                    return null;
                }

                styles.Add(data.Styles[index]);
            }

            //Make sure all the patterns have textmate scopes.
            if (styles.Where(x => x.TextmateScope != null).Count() != styles.Count)
            {
                Error.Compile("One or more styles for a pattern does not have a textmate scope defined.");
                return null;
            }

            return styles;
        }
        
        /// <summary>
        /// Formats a given string into pretty print XML.
        /// </summary>
        private string FormatXml(string xml)
        {
            try
            {
                XDocument doc = XDocument.Parse(xml);
                return doc.ToString(); //this un-escapes stuff, FIX ME!!!
            }
            catch (Exception e)
            {
                //Failed, just return normal XML.
                Error.Compile("Failed to generate valid Textmate file: '" + e.Message + "'.");
                return null;
            }
        }

        /// <summary>
        /// Writes a VSCode extension to a given location.
        /// </summary>
        public static void WriteVSCExtension(string tmText, string folderPath, Dictionary<string, IroVariable> data)
        {
            //Get out the name & file extensions of the grammar.
            string grammarName = ((IroValue)data["name"]).Value;
            List<string> fileExts = ((IroList)data["file_extensions"]).Select(x => ((IroValue)x).Value).ToList();

            //Make the proper folder paths.
            folderPath = Path.Combine(folderPath, grammarName + "-vscode");
            try
            {
                Directory.CreateDirectory(folderPath);
            }
            catch (Exception e)
            {
                Error.CommandLine("Failed to create VSCode extension directory: '" + e.Message + "'.");
                return;
            }

            //Create the VSCEData.
            var vsce = new VSCEData();
            Console.Write("Enter the name of your VSCode extension: ");
            string vscName = Console.ReadLine();
            Console.Write("Enter the description of your VSCode extension: ");
            string vscDesc = Console.ReadLine();
            Console.Write("Enter a display name for your language: ");
            string vscDisplayName = Console.ReadLine();
            vsce.ExtensionName = vscName;
            vsce.Description = vscDesc;
            vsce.Name = vscDisplayName;
            vsce.Contributes.Grammars.Add(new VSCodeGrammar()
            {
                Language = grammarName,
                RootScope = "source." + grammarName,
                Path = "./syntaxes/" + grammarName + ".tmLanguage"
            });
            vsce.Contributes.Languages.Add(new VSCodeLanguage()
            {
                Aliases = new List<string>()
                {
                    vscDisplayName,
                    grammarName
                },
                ID = grammarName,
                Extensions = fileExts
            });

            //Write the package.json to file.
            WriteExtFile(Path.Combine(folderPath, "package.json"), JsonConvert.SerializeObject(vsce));

            //Make a language configuration file.
            string res = "";
            while (res.ToUpper() != "Y" && res.ToUpper() != "N")
            {
                Console.Write("Do you want to make a custom language configuration? (Y/N): ");
                res = Console.ReadLine();
            }

            var langConfig = new VSCELanguageConfiguration();

            //Does the user want to make a custom language configuration?
            if (res.ToUpper() == "Y")
            {
                langConfig.Comments.LineComment = GetSingle("What starts a single line comment in your language? (blank for none): ");
                langConfig.Comments.BlockComments = GetMultiple("What starts/ends a block comment in your language? (blank for none).", 2);
                langConfig.Brackets = GetMultiple("What represents brackets in your language (in open-close order)?", 2);
                Console.WriteLine("What pairs can VSCode auto-close in your language? (in open-close order, pair by pair)");
                while (true)
                {
                    Console.Write("Finish entering pairs? (Y/N): ");
                    if (Console.ReadLine().ToUpper() != "N") { break; }

                    langConfig.AutoClosingPairs.Add(GetMultiple("Enter a single pair.", 2));
                   
                }
                langConfig.SurroundingPairs = langConfig.AutoClosingPairs;
            }

            //Write language config to file.
            WriteExtFile(Path.Combine(folderPath, "language-configuration.json"), JsonConvert.SerializeObject(langConfig));

            //Write the grammar to file.
            try { Directory.CreateDirectory(Path.Combine(folderPath, "syntaxes")); }
            catch (Exception e)
            {
                Error.Compile("Failed to create the 'syntaxes' directory for VSCode extension: '" + e.Message + "'.");
                return;
            }
            WriteExtFile(Path.Combine(folderPath, "syntaxes", grammarName + ".tmLanguage"), tmText);

            //Write the launch.json to file.
            try { Directory.CreateDirectory(Path.Combine(folderPath, ".vscode")); }
            catch (Exception e)
            {
                Error.Compile("Failed to create the '.vscode' directory for VSCode extension: '" + e.Message + "'.");
                return;
            }
            WriteExtFile(Path.Combine(folderPath, ".vscode", "launch.json"), Resources.launch_json);

            //Write the extension changelog to file.
            string changelog = Resources.CHANGELOG_md.Replace("{extension_name}", grammarName);
            WriteExtFile(Path.Combine(folderPath, "CHANGELOG.md"), changelog);

            //Write the README to file.
            string readme = Resources.README_md.Replace("{extension_name}", grammarName);
            WriteExtFile(Path.Combine(folderPath, "README.md"), readme);

            //Write VSCode ignore to file.
            WriteExtFile(Path.Combine(folderPath, ".vscodeignore"), Resources._vscodeignore);

            //Write quickstart to file.
            WriteExtFile(Path.Combine(folderPath, "vsc-extension-quickstart.md"), Resources.vsc_extension_quickstart_md);

            //Write extension debug script to file (twice) for Linux and Windows.
            WriteExtFile(Path.Combine(folderPath, "debug_win.bat"), Resources.winDebug_bat);
            WriteExtFile(Path.Combine(folderPath, "debug_linux.sh"), Resources.linuxDebug_sh);

            //Finished writing extension.
            Console.WriteLine("Finished writing extension!");
        }

        /// <summary>
        /// Gets a list of string values, for a specific multiple if required.
        /// </summary>
        private static List<string> GetMultiple(string prompt, int multipleRequired=1)
        {
            var multiList = new List<string>();

            Console.WriteLine(prompt);
            Console.WriteLine("Enter a blank to end the list.");
            int i = 1;
            while (true)
            {
                Console.Write("Value " + i + ": ");
                string out_ = Console.ReadLine();
                if (out_ == "") { break; }

                multiList.Add(out_);
                i++;
            }

            //Check the multiple is valid.
            if (multiList.Count % multipleRequired != 0)
            {
                Console.WriteLine("Amount of members in the list must be a multiple of " + multipleRequired + ".");
                return GetMultiple(prompt, multipleRequired);
            }

            return multiList;
        }

        /// <summary>
        /// Gets a single string value, given a prompt.
        /// </summary>
        private static string GetSingle(string prompt)
        {
            Console.Write(prompt);
            return Console.ReadLine();
        }

        /// <summary>
        /// Write an extension file.
        /// </summary>
        private static void WriteExtFile(string path, string contents)
        {
            try
            {
                File.WriteAllText(path, contents);
            }
            catch (Exception e)
            {
                Error.Compile("Failed writing VSCode extension to file, '" + e.Message + "'.");
            }
        }
    }
}
