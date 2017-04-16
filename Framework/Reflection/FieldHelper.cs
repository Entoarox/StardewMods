using System;
using System.Reflection;

namespace Entoarox.Framework.Reflection
{
    [Obsolete("Use the `ReflectedField` class instead")]
    public class FieldHelper
    {
        private FieldInfo Target;
        public FieldHelper(Type t, string name, bool instance = true)
        {
            this.Target = t.GetField(name, instance ? BindingFlags.Instance | BindingFlags.NonPublic : BindingFlags.NonPublic | BindingFlags.Static);
        }
        public void SetValue(object target, object value)
        {
            this.Target.SetValue(target, value);
        }
        public object GetValue(object target)
        {
            return this.Target.GetValue(target);
        }
        [Obsolete("Use ReflectionUtility.SetField instead")]
        public static void SetField(object target, string name, object value, bool instance = true)
        {
            (target is Type ? (Type)target : target.GetType()).GetField(name, instance ? BindingFlags.Instance | BindingFlags.NonPublic : BindingFlags.NonPublic | BindingFlags.Static).SetValue(target, value);
        }
        [Obsolete("Use ReflectionUtility.GetField instead")]
        public static object GetField(object target, string name, bool instance = true)
        {
            return (target is Type ? (Type)target : target.GetType()).GetField(name, instance ? BindingFlags.Instance | BindingFlags.NonPublic : BindingFlags.NonPublic | BindingFlags.Static).GetValue(target);
        }
        [Obsolete("Use ReflectionUtility.GetField instead")]
        public static T GetField<T>(object target, string name, bool instance=true)
        {
            return (T)FieldHelper.GetField(target, name, instance);
        }
    }
}