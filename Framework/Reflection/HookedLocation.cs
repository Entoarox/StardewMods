using System;
using System.Reflection;
using System.Reflection.Emit;

using Microsoft.Xna.Framework.Graphics;

using StardewValley;

using xTile.Dimensions;

namespace Entoarox.Framework.Reflection
{
    internal abstract class HookedLocation : GameLocation
    {
        public static void DrawBetweenLayer(GameLocation self)
        {
            if (self.Map.GetLayer("Between") != null)
                self.Map.GetLayer("Between").Draw(Game1.mapDisplayDevice, Game1.viewport, Location.Origin, false, Game1.pixelZoom);
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
            // load arg0 [this]
            cbIL.Emit(OpCodes.Ldarg_0);
            // load arg1 [map]
            cbIL.Emit(OpCodes.Ldarg_1);
            // load arg2 [name]
            cbIL.Emit(OpCodes.Ldarg_2);
            // call [base] and push stack [this, map, name]
            cbIL.Emit(OpCodes.Call, cbParent);
            // return
            cbIL.Emit(OpCodes.Ret);
            MethodInfo mbParent = location.GetType().GetMethod("drawWater", BindingFlags.Public | BindingFlags.Instance);
            MethodBuilder mb = tb.DefineMethod(mbParent.Name, mbParent.Attributes, mbParent.CallingConvention, mbParent.ReturnType, GetParamTypes(mbParent.GetParameters()));
            ILGenerator mbIL = mb.GetILGenerator();
            // Console.WriteLine
            mbIL.EmitWriteLine("Begin: hooked drawWater");
            // load arg0 [this]
            mbIL.Emit(OpCodes.Ldarg_0);
            // call [DrawBetweenLayer] and push stack [this]
            mbIL.Emit(OpCodes.Call, typeof(HookedLocation).GetMethod("DrawBetweenLayer", BindingFlags.Public | BindingFlags.Static));
            // load arg0 [this]
            mbIL.Emit(OpCodes.Ldarg_0);
            // load arg1 [batch]
            mbIL.Emit(OpCodes.Ldarg_1);
            // call [base] and push stack [this, batch]
            mbIL.Emit(OpCodes.Call, mbParent);
            // Console.WriteLine
            mbIL.EmitWriteLine("End: hooked drawWater");
            // return
            mbIL.Emit(OpCodes.Ret);
            Type result = tb.CreateType();
            return (GameLocation)Activator.CreateInstance(result, new object[] { location.map, location.name });
        }
    }
}
