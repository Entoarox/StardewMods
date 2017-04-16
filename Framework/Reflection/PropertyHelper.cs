using System;
using System.Reflection;

namespace Entoarox.Framework.Reflection
{
    [Obsolete("Use the `ReflectedProperty` class instead")]
    public class PropertyHelper
    {
        private PropertyInfo Target;
        public PropertyHelper(Type t, string name, bool instance = true)
        {
            this.Target = t.GetProperty(name, instance ? BindingFlags.Instance | BindingFlags.NonPublic : BindingFlags.NonPublic | BindingFlags.Static);
        }
        public void SetValue(object target, object value)
        {
            this.Target.SetValue(target, value);
        }
        public object GetValue(object target)
        {
            return this.Target.GetValue(target);
        }
        [Obsolete("Use ReflectionUtility.SetProperty instead")]
        public static void SetProperty(object target, string name, object value, bool instance = true)
        {
            (target is Type ? (Type)target : target.GetType()).GetProperty(name, instance ? BindingFlags.Instance | BindingFlags.NonPublic : BindingFlags.NonPublic | BindingFlags.Static).SetValue(target, value);
        }
        [Obsolete("Use ReflectionUtility.GetProperty instead")]
        public static object GetProperty(object target, string name, bool instance = true)
        {
            return (target is Type ? (Type)target : target.GetType()).GetProperty(name, instance ? BindingFlags.Instance | BindingFlags.NonPublic : BindingFlags.NonPublic | BindingFlags.Static).GetValue(target);
        }
        [Obsolete("Use ReflectionUtility.GetProperty instead")]
        public static T GetProperty<T>(object target, string name, bool instance = true)
        {
            return (T)PropertyHelper.GetProperty(target, name, instance);
        }
    }
}