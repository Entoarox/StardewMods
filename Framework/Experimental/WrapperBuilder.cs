using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Entoarox.Framework.Experimental
{
    /// <summary>
    /// Allows for soft-dependencies using interfaces to define accessible methods
    ///
    /// Be warned that this *is* a experimental feature, thus it might cause crashes or other issues, and might potentially be removed from the framework
    /// Should at any time this feature stop being experimental, it will be moved to a more correct namespace
    /// </summary>
    [Obsolete("This feature is still experimental, it may not work properly and could be removed at any time!")]
    class WrapperBuilder
    {
        private ModuleBuilder _ModuleBuilder;
        private Dictionary<string, Type> _Cache = new Dictionary<string, Type>();
        /// <summary>
        /// Initialises a new instance of the WrapperBuilder
        /// </summary>
        /// <param name="wrapperNamespace">The namespace all wrapper types are created in, should be unique so conflicts will not accidentally happen</param>
        public WrapperBuilder(string wrapperNamespace)
        {
            this._ModuleBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName("DynamicallyWrappedClassAssembly"), AssemblyBuilderAccess.RunAndCollect).DefineDynamicModule(wrapperNamespace);
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
        /*
         * EXAMPLE USAGE:
         *
         * interface IDDWrapper
         * {
         *     void RegisterLootEntry(string table, double chance, StardewValley.Object item);
         *     void RegisterLootEntry(string table, double chance, Func<StardewValley.Object> callback);
         * }
         * IDDWrapper wrapper=new WrapperBuilder("MyMod.WrapperTypes").Wrap<IDDWrapper>("Entoarox.DynamicDungeons.DynamicDungeonsAPI");
         * wrapper?.RegisterLootEntry("Default",0.3,new StardewValley.Object());
         */
    }
}
