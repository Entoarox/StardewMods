using System;
using System.Collections.Generic;

using StardewModdingAPI.Events;

namespace Entoarox.Framework
{
    public class Observer<T> : Observer
    {
        public Observer(Func<T> reader, Action<T,T> listener, bool allowReference=false)
        {
            if (!allowReference && typeof(T).IsByRef && typeof(T) != typeof(string))
                throw new ArgumentException("Unable to observe reference types", nameof(T));
            if(Cache.Count==0)
                GameEvents.UpdateTick += Hook;
            Previous = reader();
            Reader = reader;
            Listener = listener;
            Cache.Add(new WeakReference(this));
        }
        // Internal
        private T Previous;
        private Func<T> Reader;
        private Action<T, T> Listener;
        protected override void Observe()
        {
            T current = Reader();
            if (current.Equals(Previous))
                return;
            Listener(Previous, current);
            Previous = current;
        }
    }
    abstract public class Observer
    {
        abstract protected void Observe();
        protected static List<WeakReference> Cache = new List<WeakReference>();
        protected static void Hook(object s, EventArgs e)
        {
            foreach (WeakReference reference in Cache)
                if (reference.IsAlive)
                    (reference.Target as Observer).Observe();
                else
                    Cache.Remove(reference);
            if (Cache.Count == 0)
                GameEvents.UpdateTick -= Hook;
        }
    }
}
