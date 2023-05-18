using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace iro4cli
{
    /// <summary>
    /// Classes which represent single values.
    /// </summary>
    public class IroValue : IroVariable
    {
        public string Value;

        public IroValue(string val)
        {
            base.Type = VariableType.Value;
            Value = val;
        }
    }

    //Represents a single hexadecimal value in Iro.
    public class IroHex : IroVariable
    {
        public string Value;

        private IroHex(string hexVal)
        {
            base.Type = VariableType.Hex;
            Value = hexVal;
        }

        //Attempts to parse the given string into an Iro hex value.
        //On failure, returns null.
        public static IroHex Parse(string hexVal)
        {
            //Ensure the value conforms to the standard.
            if (!hexVal.StartsWith('#') || (hexVal.Length != 4 && hexVal.Length != 7))
                return null;
            return new IroHex(hexVal);
        }
    }
    
    public class IroRegex : IroVariable
    {
        public Regex Value;
        public string StringValue;

        public IroRegex(string rawVal)
        {
            //Trim the end and start of newlines.
            rawVal = rawVal.Trim('\n', '\r', '\t');

            //Put the regex in.
            Value = new Regex(rawVal);
            StringValue = rawVal;
            base.Type = VariableType.Regex;
        }
    }
}
