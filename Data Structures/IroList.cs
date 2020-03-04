using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iro4cli
{
    /// <summary>
    /// Represents a list data structure in Iro.
    /// </summary>
    public class IroList : IroVariable
    {
        //Define the type in constructor.
        public IroList()
        {
            base.Type = VariableType.Array;
        }

        //List of other variables contained within this one.
        public List<IroVariable> Contents = new List<IroVariable>();
    }
}
