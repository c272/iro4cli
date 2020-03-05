using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iro4cli
{
    /// <summary>
    /// Represents a single set in Iro.
    /// </summary>
    public class IroSet : IroVariable, IDictionary<string, IroVariable>
    {
        public string SetType = null;
        public IroSet(string setType=null)
        {
            SetType = setType;
        }

        /// <summary>
        /// Returns whether this set has a type.
        /// </summary>
        public bool HasType() { return SetType == null; }

        /////////////////////////////////
        /// DICTIONARY HELPER METHODS ///
        /////////////////////////////////
        
        public Dictionary<string, IroVariable> Variables = new Dictionary<string, IroVariable>();

        public IroVariable this[string key] { get => ((IDictionary<string, IroVariable>)Variables)[key]; set => ((IDictionary<string, IroVariable>)Variables)[key] = value; }

        public ICollection<string> Keys => ((IDictionary<string, IroVariable>)Variables).Keys;

        public ICollection<IroVariable> Values => ((IDictionary<string, IroVariable>)Variables).Values;

        public int Count => ((IDictionary<string, IroVariable>)Variables).Count;

        public bool IsReadOnly => ((IDictionary<string, IroVariable>)Variables).IsReadOnly;

        public void Add(string key, IroVariable value)
        {
            ((IDictionary<string, IroVariable>)Variables).Add(key, value);
        }

        public void Add(KeyValuePair<string, IroVariable> item)
        {
            ((IDictionary<string, IroVariable>)Variables).Add(item);
        }

        public void Clear()
        {
            ((IDictionary<string, IroVariable>)Variables).Clear();
        }

        public bool Contains(KeyValuePair<string, IroVariable> item)
        {
            return ((IDictionary<string, IroVariable>)Variables).Contains(item);
        }

        public bool ContainsKey(string key)
        {
            return ((IDictionary<string, IroVariable>)Variables).ContainsKey(key);
        }

        public void CopyTo(KeyValuePair<string, IroVariable>[] array, int arrayIndex)
        {
            ((IDictionary<string, IroVariable>)Variables).CopyTo(array, arrayIndex);
        }

        public IEnumerator<KeyValuePair<string, IroVariable>> GetEnumerator()
        {
            return ((IDictionary<string, IroVariable>)Variables).GetEnumerator();
        }

        public bool Remove(string key)
        {
            return ((IDictionary<string, IroVariable>)Variables).Remove(key);
        }

        public bool Remove(KeyValuePair<string, IroVariable> item)
        {
            return ((IDictionary<string, IroVariable>)Variables).Remove(item);
        }

        public bool TryGetValue(string key, out IroVariable value)
        {
            return ((IDictionary<string, IroVariable>)Variables).TryGetValue(key, out value);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IDictionary<string, IroVariable>)Variables).GetEnumerator();
        }
    }
}
