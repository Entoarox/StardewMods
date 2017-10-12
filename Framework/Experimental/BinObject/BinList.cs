using System.Collections;
using System.Collections.Generic;

namespace Entoarox.Framework.Experimental.BinObject
{
    public sealed class BinList : BinObject<IList<BinObject>>, IList<BinObject>
    {
        internal BinList(IList<BinObject> value) : base(BinType.List, new List<BinObject>(value))
        {
        }
        // IList<T> Properties
        public BinObject this[int index]
        {
            get => Value[index];
            set => Value[index] = value;
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

        // IList<T> Methods
        public int IndexOf(BinObject item) => Value.IndexOf(item);
        public void Insert(int index, BinObject item) => Value.Insert(index, item);
        public void RemoveAt(int index) => Value.RemoveAt(index);

        // ICollection<T> Methods
        public void Add(BinObject item) => Value.Add(item);
        public void Clear() => Value.Clear();
        public bool Contains(BinObject item) => Value.Contains(item);
        public void CopyTo(BinObject[] array, int arrayIndex) => Value.CopyTo(array, arrayIndex);
        public bool Remove(BinObject item) => Value.Remove(item);

        // IEnumerable<T> Methods
        public IEnumerator<BinObject> GetEnumerator() => Value.GetEnumerator();

        // IEnumerable Methods
        IEnumerator IEnumerable.GetEnumerator() => Value.GetEnumerator();
    }
}
