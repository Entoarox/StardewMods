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
        public HookedLocation(xTile.Map map, string name) : base(map, name)
        {

        }
        public override void drawWater(SpriteBatch b)
        {
            if (Game1.currentLocation.Map.GetLayer("Between") != null)
                Game1.currentLocation.Map.GetLayer("Between").Draw(Game1.mapDisplayDevice, Game1.viewport, Location.Origin, false, Game1.pixelZoom);
            base.drawWater(b);
        }
        private static void BuildConstructor(TypeBuilder tb, ConstructorInfo from)
        {
            ParameterInfo[] parameters = from.GetParameters();
            Type[] paramTypes = new Type[parameters.Length];
            for (var c = 0; c < parameters.Length; c++)
                paramTypes[c] = parameters[c].ParameterType;
            ConstructorBuilder cb = tb.DefineConstructor(from.Attributes, from.CallingConvention, paramTypes, null, null);
            MethodBody constructorBody =from.GetMethodBody();
            byte[] constructorIL = constructorBody.GetILAsByteArray();
            cb.SetMethodBody(constructorIL, constructorBody.MaxStackSize, null, null, null);
        }
        private static void BuildMethod(TypeBuilder tb, MethodInfo from)
        {
            ParameterInfo[] parameters = from.GetParameters();
            Type[] paramTypes = new Type[parameters.Length];
            for (var c = 0; c < parameters.Length; c++)
                paramTypes[c] = parameters[c].ParameterType;
            MethodBuilder mb = tb.DefineMethod(from.Name, from.Attributes, from.CallingConvention, from.ReturnType, paramTypes);
            byte[] data = from.GetMethodBody().GetILAsByteArray();
            mb.CreateMethodBody(data, data.Length);
        }
        public static GameLocation Create(GameLocation location)
        {
            Type type = typeof(HookedLocation);
            AssemblyName aName = new AssemblyName("Entoarox.HookedLocations");
            AssemblyBuilder ab = AppDomain.CurrentDomain.DefineDynamicAssembly(aName,AssemblyBuilderAccess.RunAndSave);
            ModuleBuilder mb = ab.DefineDynamicModule(aName.Name, aName.Name + ".dll");
            TypeBuilder tb = mb.DefineType(location.name + "_Hooked", TypeAttributes.Public,location.GetType());
            BuildConstructor(tb, type.GetConstructor(new Type[] { typeof(xTile.Map), typeof(string) }));
            BuildMethod(tb, type.GetMethod("drawWater", BindingFlags.Public | BindingFlags.Instance));
            Type result = tb.CreateType();
            return (GameLocation)Activator.CreateInstance(result, new object[] { location.map, location.name });
        }
    }
}
