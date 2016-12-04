using System;
using System.Reflection;
using System.Collections;

namespace Entoarox.Framework.Reflection
{
    class ReflectedEvent
    {
        private static Hashtable Cache = new Hashtable();

        private EventInfo Event;
        private object Target;
        public ReflectedEvent(object target, string @event)
        {
            Target = target;
            bool Static = target is Type;
            Type type = Static ? target as Type : target.GetType();
            ReferenceKey key = new ReferenceKey(type, @event);
            if (!Cache.ContainsKey(key))
            {
                EventInfo _event = type.GetEvent(@event, BindingFlags.NonPublic | (Static ? BindingFlags.Static : BindingFlags.Instance));
                if (_event == null)
                    throw new ArgumentNullException("@event", "Could not find the given NonPublic event on the given object");
                Cache.Add(key, _event);
            }
            Event = (EventInfo)Cache[key];
        }
        public void AddEventHandler(Delegate handler)
        {
            Event.AddEventHandler(Target, handler);
        }
        public void AddEventHandlerAs(object target, Delegate handler)
        {
            Event.AddEventHandler(target, handler);
        }
        public void RemoveEventHandler(Delegate handler)
        {
            Event.RemoveEventHandler(Target, handler);
        }
        public void RemoveEventHandlerAs(object target, Delegate handler)
        {
            Event.RemoveEventHandler(target, handler);
        }
        public static ReflectedEvent operator +(ReflectedEvent @event, Delegate handler)
        {
            @event.AddEventHandler(handler);
            return @event;
        }
        public static ReflectedEvent operator -(ReflectedEvent @event, Delegate handler)
        {
            @event.RemoveEventHandler(handler);
            return @event;
        }
    }
}
