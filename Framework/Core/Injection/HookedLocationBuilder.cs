using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using StardewValley;

namespace Entoarox.Framework.Core.Injection
{
#pragma warning disable CS0618
    internal static class HookedLocationBuilder
    {
        private static Dictionary<Type, Type> _Cache = new Dictionary<Type, Type>();

        public static GameLocation GetHookedLocation(GameLocation location)
        {
            if (location is IHookedLocation)
                return location;
            Type type = location.GetType();
            if (!_Cache.ContainsKey(type))
                _Cache.Add(type, BuildHookedLocation(type));
            GameLocation result=(GameLocation)Activator.CreateInstance(_Cache[type]);
            CopyData(location, result);
            return result;
        }
        public static GameLocation GetOriginLocation(GameLocation location)
        {
            if (!(location is IHookedLocation))
                return location;
            GameLocation result = (GameLocation)Activator.CreateInstance(location.GetType().BaseType);
            CopyData(location, result);
            return result;
        }


        private static ModuleBuilder _ModuleBuilder;
        private static void CopyData(GameLocation from, GameLocation to)
        {
//#error Implement actual copying here
        }
        private static void SetupBuilder()
        {
            AssemblyBuilder assemblyBuilder=AssemblyBuilder.DefineDynamicAssembly(new AssemblyName("EntoaroxFrameworkDynamic"), AssemblyBuilderAccess.Run);
            _ModuleBuilder = assemblyBuilder.DefineDynamicModule("Entoarox.Framework.Core.Dynamic");
        }
        private static Type BuildHookedLocation(Type parent)
        {
            if (_ModuleBuilder == null)
                SetupBuilder();
            TypeBuilder builder = _ModuleBuilder.DefineType("Hooked" + parent.Name, parent.Attributes, parent, new Type[] { typeof(IHookedLocation)});
//#error Implement actual builder here
            return builder.CreateType();
        }
    }
#pragma warning restore CS0618
}
