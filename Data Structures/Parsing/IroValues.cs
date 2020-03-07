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
