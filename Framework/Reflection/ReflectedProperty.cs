using System;
using System.Reflection;
using System.Collections;

namespace Entoarox.Framework.Reflection
{
    class ReflectedProperty<T>
    {
        private static Hashtable Cache = new Hashtable();
        
        private PropertyInfo Property;
        private object Target;
        public ReflectedProperty(object target, string property)
        {
            Target = target;
            bool Static = target is Type;
            Type type = Static ? target as Type : target.GetType();
            ReferenceKey key = new ReferenceKey(type, property);
            if (!Cache.ContainsKey(key))
            {
                PropertyInfo _property = type.GetProperty(property, BindingFlags.NonPublic|(Static?BindingFlags.Static:BindingFlags.Instance));
                if (_property == null)
                    throw new ArgumentNullException("property", "Could not find the given NonPublic property on the given object");
                if (_property.PropertyType != typeof(T))
                    throw new TargetException("Expected Type does not match the actuality.");
                Cache.Add(key, _property);
            }
            Property = (PropertyInfo)Cache[key];
        }
        public T GetValue()
        {
            return (T)Property.GetValue(Target);
        }
        public T GetValueAs(object target)
        {
            return (T)Property.GetValue(target);
        }
        public void SetValue(T value)
        {
            Property.SetValue(Target, value);
        }
        public void SetValueAs(object target, T value)
        {
            Property.SetValue(target, value);
        }
    }
}
