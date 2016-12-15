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
            Target = t.GetField(name, instance ? BindingFlags.Instance | BindingFlags.NonPublic : BindingFlags.NonPublic | BindingFlags.Static);
        }
        public void SetValue(object target, object value)
        {
            Target.SetValue(target, value);
        }
        public object GetValue(object target)
        {
            return Target.GetValue(target);
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
            return (T)GetField(target, name, instance);
        }
    }
    [Obsolete("Use the `ReflectedProperty` class instead")]
    public class PropertyHelper
    {
        private PropertyInfo Target;
        public PropertyHelper(Type t, string name, bool instance = true)
        {
            Target = t.GetProperty(name, instance ? BindingFlags.Instance | BindingFlags.NonPublic : BindingFlags.NonPublic | BindingFlags.Static);
        }
        public void SetValue(object target, object value)
        {
            Target.SetValue(target, value);
        }
        public object GetValue(object target)
        {
            return Target.GetValue(target);
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
            return (T)GetProperty(target, name, instance);
        }
    }
    [Obsolete("Use the `ReflectedMethod` class instead")]
    public class MethodHelper
    {
        private MethodInfo Target;
        public MethodHelper(Type t, string name, bool instance = true)
        {
            Target = t.GetMethod(name, instance ? BindingFlags.Instance | BindingFlags.NonPublic : BindingFlags.NonPublic | BindingFlags.Static);
        }
        public object Invoke(object target, object[] args)
        {
            return Target.Invoke(target, args);
        }
        [Obsolete("Use ReflectionUtility.InvokeMethod instead")]
        public static object InvokeMethod(object target, string name, object[] args, bool instance=true)
        {
            return (target is Type ? (Type)target : target.GetType()).GetMethod(name, instance ? BindingFlags.Instance | BindingFlags.NonPublic : BindingFlags.NonPublic | BindingFlags.Static).Invoke(target,args);
        }
        [Obsolete("Use ReflectionUtility.InvokeMethod instead")]
        public static T InvokeMethod<T>(object target, string name, object[] args, bool instance=true)
        {
            return (T)InvokeMethod(target, name, args, instance);
        }
    }
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
