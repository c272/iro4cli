using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iro4cli.Compile
{
    public class TextmateCompiler : ICompileTarget
    {
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
            text.AppendLine("<!-- Generated via Iro4CLI -->");

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
            text.AppendLine("<string>source." + data.UUID + "</string>");

            //todo
            return null;
        }
    }
}
