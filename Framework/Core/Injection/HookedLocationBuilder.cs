using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Newtonsoft.Json;
using StardewValley;
#pragma warning disable CS0618
#pragma warning disable CS1030

namespace Entoarox.Framework.Core.Injection
{
    internal static class HookedLocationBuilder
    {
        /*********
        ** Fields
        *********/
        private static readonly Dictionary<Type, Type> Cache = new Dictionary<Type, Type>();
        private static ModuleBuilder ModuleBuilder;


        /*********
        ** Public methods
        *********/
        public static GameLocation GetHookedLocation(GameLocation location)
        {
            if (location is IHookedLocation)
                return location;
            Type type = location.GetType();
            if (!HookedLocationBuilder.Cache.ContainsKey(type))
                HookedLocationBuilder.Cache.Add(type, HookedLocationBuilder.BuildHookedLocation(type));
            return HookedLocationBuilder.CopyData(location, HookedLocationBuilder.Cache[type]);
        }

        public static GameLocation GetOriginLocation(GameLocation location)
        {
            if (!(location is IHookedLocation))
                return location;
            return HookedLocationBuilder.CopyData(location, location.GetType().BaseType);
        }


        /*********
        ** Protected methods
        *********/
        private static GameLocation CopyData(GameLocation from, Type to)
        {
            return (GameLocation)JsonConvert.DeserializeObject(JsonConvert.SerializeObject(from), to, new JsonSerializerSettings { MissingMemberHandling = MissingMemberHandling.Ignore });
        }

        private static void SetupBuilder()
        {
            AssemblyBuilder assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName("EntoaroxFrameworkDynamic"), AssemblyBuilderAccess.Run);
            HookedLocationBuilder.ModuleBuilder = assemblyBuilder.DefineDynamicModule("Entoarox.Framework.Core.Dynamic");
        }

        private static Type BuildHookedLocation(Type parent)
        {
            if (HookedLocationBuilder.ModuleBuilder == null)
                HookedLocationBuilder.SetupBuilder();
            TypeBuilder builder = HookedLocationBuilder.ModuleBuilder.DefineType("Hooked" + parent.Name, parent.Attributes, parent, new[] { typeof(IHookedLocation) });
#warning Implement actual builder here
            return builder.CreateType();
        }
    }
}
