using System;
using System.Reflection;
using System.Collections;

namespace Entoarox.Framework.Reflection
{
    public class ReflectedField<T>
    {
        private static Hashtable Cache = new Hashtable();
        
        private FieldInfo Field;
        private object Target;
        public ReflectedField(object target, string field)
        {
            Target = target;
            bool Static = target is Type;
            Type type = Static ? target as Type : target.GetType();
            ReferenceKey key = new ReferenceKey(type, field);
            if (!Cache.ContainsKey(key))
            {
                FieldInfo _field = type.GetField(field, BindingFlags.NonPublic | (Static ? BindingFlags.Static : BindingFlags.Instance));
                if (_field == null)
                    throw new ArgumentNullException("field", "Could not find the given NonPublic field on the given object");
                if (_field.FieldType != typeof(T))
                    throw new TargetException("Expected Type does not match the actuality.");
                Cache.Add(key, _field);
            }
            Field = (FieldInfo)Cache[key];
        }
        public T GetValue()
        {
            return (T)Field.GetValue(Target);
        }
        public T GetValueAs(object target)
        {
            return (T)Field.GetValue(target);
        }
        public void SetValue(T value)
        {
            Field.SetValue(Target, value);
        }
        public void SetValueAs(object target, T value)
        {
            Field.SetValue(target, value);
        }
    }
}
