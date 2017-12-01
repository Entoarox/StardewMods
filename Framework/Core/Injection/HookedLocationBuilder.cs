using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using StardewValley;

using Newtonsoft.Json;

namespace Entoarox.Framework.Core.Injection
{
#pragma warning disable CS0618
#pragma warning disable CS1030
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
            return CopyData(location, _Cache[type]);
        }
        public static GameLocation GetOriginLocation(GameLocation location)
        {
            if (!(location is IHookedLocation))
                return location;
           return CopyData(location, location.GetType().BaseType);
        }


        private static ModuleBuilder _ModuleBuilder;
        private static GameLocation CopyData(GameLocation from, Type to)
        {
            return (GameLocation)JsonConvert.DeserializeObject(JsonConvert.SerializeObject(from), to, new JsonSerializerSettings() { MissingMemberHandling = MissingMemberHandling.Ignore });
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
#warning Implement actual builder here
            return builder.CreateType();
        }
    }
#pragma warning restore CS0618
#pragma warning restore CS1030
}
