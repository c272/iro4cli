using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iro4cli
{
    /// <summary>
    /// The VSCode engine data for package.json.
    /// </summary>
    public class VSCEData
    {
        [JsonProperty(PropertyName = "name")]
        public string ExtensionName;

        [JsonProperty(PropertyName = "displayName")]
        public string Name;

        [JsonProperty(PropertyName = "description")]
        public string Description;

        [JsonProperty(PropertyName = "version")]
        public string Version = "0.0.1";

        [JsonProperty(PropertyName = "engines")]
        public EnginesList Engines = new EnginesList();

        [JsonProperty(PropertyName = "categories")]
        public List<string> ExtensionCategories = new List<string>()
        {
            "Programming Languages"
        };

        [JsonProperty(PropertyName = "contributes")]
        public VSCodeContributes Contributes = new VSCodeContributes();
    }

    /// <summary>
    /// VSCode contribution point inside package.json.
    /// </summary>
    public class VSCodeContributes
    {
        [JsonProperty(PropertyName = "languages")]
        public List<VSCodeLanguage> Languages = new List<VSCodeLanguage>();

        [JsonProperty(PropertyName = "grammars")]
        public List<VSCodeGrammar> Grammars = new List<VSCodeGrammar>();
    }

    /// <summary>
    /// A single grammar definition in VSCode.
    /// </summary>
    public class VSCodeGrammar
    {
        [JsonProperty(PropertyName = "language")]
        public string Language;

        [JsonProperty(PropertyName = "scopeName")]
        public string RootScope;

        //Path to the grammar file.
        [JsonProperty(PropertyName = "path")]
        public string Path;
    }

    /// <summary>
    /// A single language definition in VSCode.
    /// </summary>
    public class VSCodeLanguage
    {
        [JsonProperty(PropertyName = "id")]
        public string ID;

        [JsonProperty(PropertyName = "aliases")]
        public List<string> Aliases;

        [JsonProperty(PropertyName = "extensions")]
        public List<string> Extensions = new List<string>();

        [JsonProperty(PropertyName = "configuration")]
        public string Configuration = "./language-configuration.json";
    }

    /// <summary>
    /// A list of required engine versions.
    /// </summary>
    public class EnginesList
    {
        [JsonProperty(PropertyName = "vscode")]
        public string VSCodeVersion = "^1.42.0";
    }
}
