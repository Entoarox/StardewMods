using System;
using System.Reflection;
using System.Reflection.Emit;

using StardewValley;

using xTile.Dimensions;

namespace Entoarox.Framework.Reflection
{
    internal abstract class HookedLocation : GameLocation
    {
        public static void DrawBetweenLayer()
        {
            if (Game1.currentLocation.Map.GetLayer("Between") != null)
                Game1.currentLocation.Map.GetLayer("Between").Draw(Game1.mapDisplayDevice, Game1.viewport, Location.Origin, false, Game1.pixelZoom);
        }
        private static Type[] GetParamTypes(ParameterInfo[] parameters)
        {
            Type[] paramTypes = new Type[parameters.Length];
            for (var c = 0; c < parameters.Length; c++)
                paramTypes[c] = parameters[c].ParameterType;
            return paramTypes;
        }
        public static GameLocation Create(GameLocation location)
        {
            AssemblyName aName = new AssemblyName("Entoarox.HookedLocations");
            AssemblyBuilder assembly = AppDomain.CurrentDomain.DefineDynamicAssembly(aName,AssemblyBuilderAccess.RunAndSave);
            ModuleBuilder module = assembly.DefineDynamicModule(aName.Name, aName.Name + ".dll");
            TypeBuilder tb = module.DefineType(location.name + "_Hooked", TypeAttributes.Public,location.GetType());
            ConstructorInfo cbParent = location.GetType().GetConstructor(new Type[] { typeof(xTile.Map), typeof(string) });
            ConstructorBuilder cb = tb.DefineConstructor(cbParent.Attributes, cbParent.CallingConvention, GetParamTypes(cbParent.GetParameters()), null, null);
            MethodBody constructorBody = cbParent.GetMethodBody();
            ILGenerator cbIL = cb.GetILGenerator();
            // get [this]
            cbIL.Emit(OpCodes.Ldarg_0);
            // get [map]
            cbIL.Emit(OpCodes.Ldarg_1);
            // get [name]
            cbIL.Emit(OpCodes.Ldarg_2);
            // call [base] as [this] with [map,name]
            cbIL.Emit(OpCodes.Call, cbParent);
            // return
            cbIL.Emit(OpCodes.Ret);
            MethodInfo mbParent = location.GetType().GetMethod("drawWater", BindingFlags.Public | BindingFlags.Instance);
            MethodBuilder mb = tb.DefineMethod(mbParent.Name, mbParent.Attributes, mbParent.CallingConvention, mbParent.ReturnType, GetParamTypes(mbParent.GetParameters()));
            ILGenerator mbIL = mb.GetILGenerator();
            // call [DrawBetweenLayer]
            mbIL.Emit(OpCodes.Call, typeof(HookedLocation).GetMethod("DrawBetweenLayer", BindingFlags.Public | BindingFlags.Static));
            // get [this]
            mbIL.Emit(OpCodes.Ldarg_0);
            // get [batch]
            mbIL.Emit(OpCodes.Ldarg_1);
            // call [base]
            mbIL.Emit(OpCodes.Call, mbParent);
            // return
            mbIL.Emit(OpCodes.Ret);
            Type result = tb.CreateType();
            return (GameLocation)Activator.CreateInstance(result, new object[] { location.map, location.name });
        }
    }
}
