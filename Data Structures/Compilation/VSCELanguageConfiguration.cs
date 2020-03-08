using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iro4cli
{
    /// <summary>
    /// Represents how different language components are defined in text.
    /// </summary>
    public class VSCELanguageConfiguration
    {
        [JsonProperty(PropertyName = "comments")]
        public VSCECommentConfig Comments;

        [JsonProperty(PropertyName = "brackets")]
        public List<string> Brackets = new List<string>()
        {
            "(", ")", "{", "}", "[", "]"
        };

        [JsonProperty(PropertyName = "autoClosingPairs")]
        public List<List<string>> AutoClosingPairs = new List<List<string>>()
        {
            new List<string>() { "{", "}" },
            new List<string>() { "[", "]" },
            new List<string>() { "(", ")" },
            new List<string>() { "\"", "\"" },
            new List<string>() { "'", "'" }
        };

        //symbols that surround automatic double click selection
        [JsonProperty(PropertyName = "surroundingPairs")]
        public List<List<string>> SurroundingPairs = new List<List<string>>()
        {
            new List<string>() { "{", "}" },
            new List<string>() { "[", "]" },
            new List<string>() { "(", ")" },
            new List<string>() { "\"", "\"" },
            new List<string>() { "'", "'" }
        };
    }

    /// <summary>
    /// Represents how comments are defined in a language.
    /// </summary>
    public class VSCECommentConfig
    {
        [JsonProperty(PropertyName = "lineComment")]
        public string LineComment = "//";

        public List<string> BlockComments = new List<string>()
        {
            "/*", "*/"
        };
    }
}
