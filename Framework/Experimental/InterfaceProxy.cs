using System;
using System.Collections.Generic;
using System.Linq;

using System.Reflection;
using System.Reflection.Emit;

namespace Entoarox.Framework.Experimental
{
    static class InterfaceProxyBuilder
    {
        private static Dictionary<Type, Type> Cache = new Dictionary<Type, Type>();
        private static ModuleBuilder MBuilder;
        static InterfaceProxyBuilder()
        {
            var aBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName("SMAPI_Proxy_Types"), AssemblyBuilderAccess.Run);
            MBuilder = aBuilder.DefineDynamicModule("StardewModdingAPI.Dynamic.ApiProxies");
        }
        public static T ProxyAs<T>(this object instance)
        {
            if (!typeof(T).IsInterface)
                throw new ArgumentException("Type is not a interface.", nameof(T));
            if(!Cache.ContainsKey(typeof(T)))
            {
                var tBuilder = MBuilder.DefineType(instance.GetType().FullName.Replace(".", "") + "_AS_" + typeof(T).FullName.Replace(".", ""), TypeAttributes.Class, typeof(object), new[] { typeof(T) });
                var t = typeof(T);
                var p = instance.GetType();
                foreach (var method in t.GetMethods())
                {
                    var r = p.GetMethod(method.Name, method.GetParameters().Select(a => a.ParameterType).ToArray());
                    if (r == null)
                        throw new ArgumentException("Interface defines a method that is not implemented by the API class.", method.Name);
                    ProxyMethod(method, r, tBuilder);
                }
                foreach (var property in t.GetProperties())
                {
                    var r = p.GetProperty(property.Name, property.PropertyType, property.GetIndexParameters().Select(a => a.ParameterType).ToArray());
                    if (r == null)
                        throw new ArgumentException("Interface defines a method that is not implemented by the API class.", property.Name);
                    ProxyProperty(property, r, tBuilder);
                }
                Cache.Add(typeof(T), tBuilder.CreateType());
            }
            return (T)Activator.CreateInstance(Cache[typeof(T)]);
        }
        private static void ProxyMethod(MethodInfo method, MethodInfo parent, TypeBuilder tBuilder)
        {
            var args = method.GetParameters().Select(a => a.ParameterType).ToArray();
            var mBuilder = tBuilder.DefineMethod(parent.Name, parent.Attributes, parent.CallingConvention, parent.ReturnType, args);
            var il = mBuilder.GetILGenerator();
            for (int c = 1; c <= args.Length; c++)
                il.Emit(OpCodes.Ldarg, c);
            il.Emit(OpCodes.Call, parent);
            il.Emit(OpCodes.Ret);
        }
        private static void ProxyProperty(PropertyInfo property, PropertyInfo parent, TypeBuilder tBuilder)
        {
            var pBuilder = tBuilder.DefineProperty(parent.Name, parent.Attributes, parent.PropertyType, parent.GetIndexParameters().Select(a => a.ParameterType).ToArray());
            if(property.CanRead)
            {
                var pMethod = parent.GetGetMethod();
                var args =pMethod.GetParameters().Select(a => a.ParameterType).ToArray();
                var mBuilder = tBuilder.DefineMethod(pMethod.Name, pMethod.Attributes, pMethod.CallingConvention, pMethod.ReturnType, args);
                var il = mBuilder.GetILGenerator();
                for (int c = 1; c <= args.Length; c++)
                    il.Emit(OpCodes.Ldarg, c);
                il.Emit(OpCodes.Call, pMethod);
                il.Emit(OpCodes.Ret);
                pBuilder.SetGetMethod(mBuilder);
            }
            if (property.CanWrite)
            {
                var pMethod = parent.GetSetMethod();
                var args = pMethod.GetParameters().Select(a => a.ParameterType).ToArray();
                var mBuilder = tBuilder.DefineMethod(pMethod.Name, pMethod.Attributes, pMethod.CallingConvention, pMethod.ReturnType, args);
                var il = mBuilder.GetILGenerator();
                for (int c = 1; c <= args.Length; c++)
                    il.Emit(OpCodes.Ldarg, c);
                il.Emit(OpCodes.Call, pMethod);
                il.Emit(OpCodes.Ret);
                pBuilder.SetSetMethod(mBuilder);
            }
        }
    }
}
