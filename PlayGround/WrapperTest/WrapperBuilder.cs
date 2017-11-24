using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace PlayGround.WrapperTest
{
    /// <summary>
    /// Allows for soft-dependencies using interfaces to define accessible methods
    /// </summary>
    class WrapperBuilder
    {
        private ModuleBuilder _ModuleBuilder;
        private Dictionary<string, Type> _Cache = new Dictionary<string, Type>();
        /// <summary>
        /// Initialises a new instance of the WrapperBuilder
        /// </summary>
        /// <param name="wrapperNamespace">The namespace all wrapper types are created in, should be unique so conflict will not accidentally happen</param>
        public WrapperBuilder(string wrapperNamespace)
        {
            AssemblyBuilder aBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName("DynamicallyWrappedClassAssembly"), AssemblyBuilderAccess.RunAndCollect);
            this._ModuleBuilder = aBuilder.DefineDynamicModule(wrapperNamespace);
        }
        /// <summary>
        /// Creates a dynamic wrapper around a type to allow for soft-dependency access
        ///
        /// The required path must be the full namespace path as you would define if no `using` was in place
        /// </summary>
        /// <typeparam name="T">The interface defining the class methods you wish to have access to</typeparam>
        /// <param name="typePath">The full path to the type, including the type itself</param>
        /// <returns></returns>
        public T Wrap<T>(string typePath) where T : class
        {
            try
            {
                Assembly target = AppDomain.CurrentDomain.GetAssemblies().First(a => a.GetType(typePath) != null);
                if (target == null)
                    return null;
                Type parent = target.GetType(typePath);
                string cls = "Wrapped" + parent.Name;
                if (!this._Cache.ContainsKey(cls))
                {
                    TypeBuilder tBuilder = this._ModuleBuilder.DefineType(cls, TypeAttributes.Class | TypeAttributes.Public, parent, new Type[] { typeof(T) });
                    this._Cache.Add(cls, tBuilder.CreateType());
                }
                return (T)Activator.CreateInstance(this._Cache[cls]);
            }
            catch
            {
                return null;
            }
        }
    }
}
