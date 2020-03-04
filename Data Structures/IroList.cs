using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iro4cli
{
    /// <summary>
    /// Represents a list data structure in Iro.
    /// </summary>
    public class IroList : IroVariable, IEnumerable<IroVariable>
    {
        //Define the type in constructor.
        public IroList()
        {
            base.Type = VariableType.Array;
        }

        //Adds a value to the list.
        public void Add(IroVariable v)
        {
            Contents.Add(v);
        }

        //Helper functions to make this class enumerable.
        public IEnumerator<IroVariable> GetEnumerator()
        {
            return Contents.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        //List of other variables contained within this one.
        public List<IroVariable> Contents = new List<IroVariable>();
    }
}
