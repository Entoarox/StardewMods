using System;
using System.Reflection;

namespace Entoarox.Framework.Reflection
{
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
        public static void SetField(object target, string name, object value, bool instance = true)
        {
            (target is Type ? (Type)target : target.GetType()).GetField(name, instance ? BindingFlags.Instance | BindingFlags.NonPublic : BindingFlags.NonPublic | BindingFlags.Static).SetValue(target, value);
        }
        public static object GetField(object target, string name, bool instance = true)
        {
            return (target is Type ? (Type)target : target.GetType()).GetField(name, instance ? BindingFlags.Instance | BindingFlags.NonPublic : BindingFlags.NonPublic | BindingFlags.Static).GetValue(target);
        }
        public static T GetField<T>(object target, string name, bool instance=true)
        {
            return (T)GetField(target, name, instance);
        }
    }
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
        public static void SetProperty(object target, string name, object value, bool instance = true)
        {
            (target is Type ? (Type)target : target.GetType()).GetProperty(name, instance ? BindingFlags.Instance | BindingFlags.NonPublic : BindingFlags.NonPublic | BindingFlags.Static).SetValue(target, value);
        }
        public static object GetProperty(object target, string name, bool instance = true)
        {
            return (target is Type ? (Type)target : target.GetType()).GetProperty(name, instance ? BindingFlags.Instance | BindingFlags.NonPublic : BindingFlags.NonPublic | BindingFlags.Static).GetValue(target);
        }
        public static T GetProperty<T>(object target, string name, bool instance = true)
        {
            return (T)GetProperty(target, name, instance);
        }
    }
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
        public static object InvokeMethod(object target, string name, object[] args, bool instance=true)
        {
            return (target is Type ? (Type)target : target.GetType()).GetMethod(name, instance ? BindingFlags.Instance | BindingFlags.NonPublic : BindingFlags.NonPublic | BindingFlags.Static).Invoke(target,args);
        }
        public static T InvokeMethod<T>(object target, string name, object[] args, bool instance=true)
        {
            return (T)InvokeMethod(target, name, args, instance);
        }
    }
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
