using System;

namespace Entoarox.Framework.Reflection
{
    [Obsolete("Use the new Reflected* classes instead")]
    public class ClassHelper
    {
        private Type Target;
        public ClassHelper(Type t)
        {
            Target = t;
        }
        public object CreateInstance(params object[] args)
        {
            return Activator.CreateInstance(Target, args);
        }
        public FieldHelper GetField(string field, bool instance = true)
        {
            return new FieldHelper(Target, field, instance);
        }
        public PropertyHelper GetProperty(string property, bool instance = true)
        {
            return new PropertyHelper(Target, property, instance);
        }
        public MethodHelper GetMethod(string method, bool instance = true)
        {
            return new MethodHelper(Target, method, instance);
        }
    }
}
