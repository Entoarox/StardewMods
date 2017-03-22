using System;
using System.Reflection;
using System.Reflection.Emit;

using StardewValley;

using xTile.Dimensions;

using Microsoft.Xna.Framework.Graphics;

namespace Entoarox.Framework.Reflection
{
    internal abstract class HookedLocation : GameLocation
    {
        public override void drawWater(SpriteBatch b)
        {
            if (Game1.currentLocation.Map.GetLayer("Between") != null)
                Game1.currentLocation.Map.GetLayer("Between").Draw(Game1.mapDisplayDevice, Game1.viewport, Location.Origin, false, Game1.pixelZoom);
        }
        private static void BuildConstructor(TypeBuilder tb, ConstructorInfo parent)
        {
            ParameterInfo[] parameters = parent.GetParameters();
            Type[] paramTypes = new Type[parameters.Length];
            for (var c = 0; c < parameters.Length; c++)
                paramTypes[c] = parameters[c].ParameterType;
            ConstructorBuilder cb = tb.DefineConstructor(parent.Attributes, parent.CallingConvention, paramTypes, null, null);
            MethodBody constructorBody =parent.GetMethodBody();
            ILGenerator generator = cb.GetILGenerator();
            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Ldarg_1);
            generator.Emit(OpCodes.Ldarg_2);
            generator.Emit(OpCodes.Call, parent);
            generator.Emit(OpCodes.Ret);
        }
        private static void BuildMethod(TypeBuilder tb, MethodInfo parent, MethodInfo from)
        {
            ParameterInfo[] parameters = from.GetParameters();
            Type[] paramTypes = new Type[parameters.Length];
            for (var c = 0; c < parameters.Length; c++)
                paramTypes[c] = parameters[c].ParameterType;
            MethodBuilder mb = tb.DefineMethod(from.Name, from.Attributes, from.CallingConvention, from.ReturnType, paramTypes);
            ILGenerator generator = mb.GetILGenerator();
            generator.Emit(OpCodes.Ldarg_1);
            generator.Emit(OpCodes.Call, from);
            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Ldarg_1);
            generator.Emit(OpCodes.Call, parent);
            generator.Emit(OpCodes.Ret);
        }
        public static GameLocation Create(GameLocation location)
        {
            Type type = typeof(HookedLocation);
            AssemblyName aName = new AssemblyName("Entoarox.HookedLocations");
            AssemblyBuilder ab = AppDomain.CurrentDomain.DefineDynamicAssembly(aName,AssemblyBuilderAccess.RunAndSave);
            ModuleBuilder mb = ab.DefineDynamicModule(aName.Name, aName.Name + ".dll");
            TypeBuilder tb = mb.DefineType(location.name + "_Hooked", TypeAttributes.Public,location.GetType());
            BuildConstructor(tb, location.GetType().GetConstructor(new Type[] { typeof(xTile.Map), typeof(string) }));
            BuildMethod(tb, location.GetType().GetMethod("drawWater", BindingFlags.Public | BindingFlags.Instance), type.GetMethod("drawWater", BindingFlags.Public | BindingFlags.Instance));
            Type result = tb.CreateType();
            return (GameLocation)Activator.CreateInstance(result, new object[] { location.map, location.name });
        }
    }
}
