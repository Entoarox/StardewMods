using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entoarox.Framework.Abstraction
{
    public class WrappedList<TWrapper, TType> : IEnumerable<TWrapper>, ICollection<TWrapper>
    {
        protected Func<TType, TWrapper> Wrapper;
        protected Func<TWrapper, TType> Unwrapper;
        protected List<TType> List;
        public WrappedList(List<TType> list, Func<TType, TWrapper> wrapper, Func<TWrapper, TType> unwrapper = null)
        {
            Wrapper = wrapper;
            Unwrapper = unwrapper;
            List = list;
        }
        // IEnumerator<TWrapper>
        private class WrappedListEmumerator : IEnumerator<TWrapper>
        {
            private int index = -1;
            private WrappedList<TWrapper, TType> Enumerable;
            public WrappedListEmumerator(WrappedList<TWrapper, TType> enumerable)
            {
                Enumerable = enumerable;
            }
            private TWrapper _Current
            {
                get
                {
                    if (index < 0 || Enumerable.List.Count <= index + 1)
                        throw new IndexOutOfRangeException();
                    return Enumerable.Wrapper(Enumerable.List[index]);
                }
            }
            public TWrapper Current
            {
                get
                {
                    return _Current;
                }
            }
            object IEnumerator.Current
            {
                get
                {
                    return _Current;
                }
            }
            public bool MoveNext()
            {
                index++;
                return Enumerable.List.Count < index;
            }
            public void Reset()
            {
                index = -1;
            }
            public void Dispose()
            {
                Enumerable = null;
            }
        }
        // IEnumerable<TWrapper>
        private WrappedListEmumerator _GetEnumerator()
        {
            return new WrappedListEmumerator(this);
        }
        public IEnumerator<TWrapper> GetEnumerator()
        {
            return _GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return _GetEnumerator();
        }
        public virtual TWrapper this[int index]
        {
            get
            {
                if (index < 0 || List.Count <= index)
                    throw new IndexOutOfRangeException();
                return Wrapper(List[index]);
            }
            set
            {
                if (Unwrapper == null)
                    throw new NotImplementedException();
                if (index < 0 || List.Count <= index)
                    throw new IndexOutOfRangeException();
                List[index] = Unwrapper(value);
            }
        }
        // ICollection<TWrapper>
        public bool IsReadOnly
        {
            get
            {
                return Unwrapper == null;
            }
        }
        public int Count
        {
            get
            {
                return List.Count;
            }
        }
        public void Add(TWrapper value)
        {
            if (IsReadOnly)
                throw new InvalidOperationException();
            List.Add(Unwrapper(value));
        }
        public bool Contains(TWrapper value)
        {
            return List.Contains(Unwrapper(value));
        }
        public void Clear()
        {
            if (IsReadOnly)
                throw new InvalidOperationException();
            List.Clear();
        }
        public bool Remove(TWrapper value)
        {
            if (IsReadOnly)
                throw new InvalidOperationException();
            return List.Remove(Unwrapper(value));
        }
        [Obsolete("Not Implemented",true)]
        public void CopyTo(TWrapper[] array, int count)
        {
            throw new NotImplementedException();
        }
    }
}
