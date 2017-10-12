using System.Collections;
using System.Collections.Generic;

namespace Entoarox.Framework.Experimental.BinObject
{
    public sealed class BinMap : BinObject<IDictionary<string, BinObject>>, IDictionary<string, BinObject>
    {
        internal BinMap(IDictionary<string, BinObject> value) : base(BinType.Map, new Dictionary<string,BinObject>(value))
        {
        }
        // IDictionary<TKey, TValue> Properties
        public BinObject this[string key]
        {
            get => Value[key];
            set => Value[key] = value;
        }
        public ICollection<string> Keys
        {
            get => Value.Keys;
        }
        public ICollection<BinObject> Values
        {
            get => Value.Values;
        }

        // ICollection<T> Properties
        public int Count
        {
            get => Value.Count;
        }
        public bool IsReadOnly
        {
            get => false;
        }

        // IDictionary<TKey,TValue> Methods
        public bool ContainsKey(string key) => Value.ContainsKey(key);
        public void Add(string key, BinObject value) => Value.Add(key, value);
        public bool Remove(string key) => Value.Remove(key);
        public bool TryGetValue(string key, out BinObject value) => Value.TryGetValue(key, out value);

        // ICollection<T> Methods
        public void Add(KeyValuePair<string, BinObject> item) => Value.Add(item);
        public void Clear() => Value.Clear();
        public bool Contains(KeyValuePair<string, BinObject> item) => Value.Contains(item);
        public void CopyTo(KeyValuePair<string, BinObject>[] array, int arrayIndex) => Value.CopyTo(array, arrayIndex);
        public bool Remove(KeyValuePair<string, BinObject> item) => Value.Remove(item);

        // IEnumerable<T> Methods
        public IEnumerator<KeyValuePair<string, BinObject>> GetEnumerator() => Value.GetEnumerator();

        // IEnumerable Methods
        IEnumerator IEnumerable.GetEnumerator() => Value.GetEnumerator();

    }
}
