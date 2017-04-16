using System;
using System.Reflection;

namespace Entoarox.Framework.Reflection
{
    [Obsolete("Use the `ReflectedMethod` class instead")]
    public class MethodHelper
    {
        private MethodInfo Target;
        public MethodHelper(Type t, string name, bool instance = true)
        {
            this.Target = t.GetMethod(name, instance ? BindingFlags.Instance | BindingFlags.NonPublic : BindingFlags.NonPublic | BindingFlags.Static);
        }
        public object Invoke(object target, object[] args)
        {
            return this.Target.Invoke(target, args);
        }
        [Obsolete("Use ReflectionUtility.InvokeMethod instead")]
        public static object InvokeMethod(object target, string name, object[] args, bool instance=true)
        {
            return (target is Type ? (Type)target : target.GetType()).GetMethod(name, instance ? BindingFlags.Instance | BindingFlags.NonPublic : BindingFlags.NonPublic | BindingFlags.Static).Invoke(target,args);
        }
        [Obsolete("Use ReflectionUtility.InvokeMethod instead")]
        public static T InvokeMethod<T>(object target, string name, object[] args, bool instance=true)
        {
            return (T)MethodHelper.InvokeMethod(target, name, args, instance);
        }
    }
}