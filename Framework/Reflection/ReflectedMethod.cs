using System;
using System.Reflection;
using System.Collections;

namespace Entoarox.Framework.Reflection
{
    class ReflectedMethod
    {
        private static Hashtable Cache = new Hashtable();

        private MethodInfo Method;
        private object Target;
        public ReflectedMethod(object target, string method)
        {
            Target = target;
            bool Static = target is Type;
            Type type = Static ? target as Type : target.GetType();
            ReferenceKey key = new ReferenceKey(type, method);
            if (!Cache.ContainsKey(key))
            {
                MethodInfo _method = type.GetMethod(method, BindingFlags.NonPublic | (Static ? BindingFlags.Static : BindingFlags.Instance));
                if (_method == null)
                    throw new ArgumentNullException("method", "Could not find the given NonPublic method on the given object");
                if (_method.ReturnType != typeof(void))
                    throw new TargetException("Expected Type does not match the actuality.");
                Cache.Add(key, _method);
            }
            Method = (MethodInfo)Cache[key];
        }
        public void Invoke(params object[] arguments)
        {
            Method.Invoke(Target, arguments);
        }
        public void InvokeAs(object target, params object[] arguments)
        {
            Method.Invoke(target, arguments);
        }
        public Delegate CreateDelegate(Type type)
        {
            return Target is Type ? Method.CreateDelegate(type) : Method.CreateDelegate(type, Target);
        }
        public Delegate CreateDelegateAs(object target, Type type)
        {
            return Method.CreateDelegate(type, target);
        }
    }
    class ReflectedMethod<T>
    {
        private static Hashtable Cache = new Hashtable();

        private MethodInfo Method;
        private object Target;
        public ReflectedMethod(object target, string method)
        {
            Target = target;
            bool Static = target is Type;
            Type type = Static ? target as Type : target.GetType();
            ReferenceKey key = new ReferenceKey(type, method);
            if (!Cache.ContainsKey(key))
            {
                MethodInfo _method = type.GetMethod(method, BindingFlags.NonPublic | (Static ? BindingFlags.Static : BindingFlags.Instance));
                if (_method.ReturnType != typeof(T))
                    throw new TargetException("Expected Type does not match the actuality.");
                Cache.Add(key, _method);
            }
            Method = (MethodInfo)Cache[key];
        }
        public T Invoke(params object[] arguments)
        {
            return (T)Method.Invoke(Target, arguments);
        }
        public T InvokeAs(object target, params object[] arguments)
        {
            return (T)Method.Invoke(target, arguments);
        }
        public Delegate CreateDelegate(Type type)
        {
            return Target is Type ? Method.CreateDelegate(type) : Method.CreateDelegate(type, Target);
        }
        public Delegate CreateDelegateAs(object target, Type type)
        {
            return Method.CreateDelegate(type, target);
        }
    }
}
