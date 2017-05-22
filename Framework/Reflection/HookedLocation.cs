using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Collections.Generic;

using Microsoft.Xna.Framework.Graphics;

using StardewValley;

using xTile.Dimensions;

namespace Entoarox.Framework.Reflection
{
    public static class HookedLocation
    {
        public static void DrawBetweenLayer(GameLocation self, SpriteBatch batch)
        {
            if (self.Map.GetLayer("Between") != null)
                self.Map.GetLayer("Between").Draw(Game1.mapDisplayDevice, Game1.viewport, Location.Origin, false, Game1.pixelZoom);
        }
        private static bool Init = true;
        private static AssemblyName assemblyName;
        private static AssemblyBuilder assemblyBuilder;
        private static ModuleBuilder moduleBuilder;
        private static Dictionary<Type, Type> Cache = new Dictionary<Type, Type>();
        private static Type[] GetParamTypes(ParameterInfo[] parameters)
        {
            Type[] paramTypes = new Type[parameters.Length];
            for (var c = 0; c < parameters.Length; c++)
                paramTypes[c] = parameters[c].ParameterType;
            return paramTypes;
        }
        private static void LoadArgs(ILGenerator mbIL, int count)
        {
            for (int c = 0; c < count; c++)
                mbIL.Emit(OpCodes.Ldarg, c);
        }
        private static void HookEditable(TypeBuilder tb, MethodInfo method, MethodInfo hook)
        {
            MethodBuilder mb = tb.DefineMethod(method.Name, method.Attributes | MethodAttributes.ReuseSlot | MethodAttributes.HideBySig, method.ReturnType, GetParamTypes(method.GetParameters()));
            ILGenerator mbIL = mb.GetILGenerator();
            // load arg0 [this]
            mbIL.Emit(OpCodes.Ldarg_0);
            // load arg1 [batch]
            mbIL.Emit(OpCodes.Ldarg_1);
            // call [DrawBetweenLayer] and push stack [this, batch]
            mbIL.Emit(OpCodes.Call, typeof(HookedLocation).GetMethod("DrawBetweenLayer", new Type[] { typeof(GameLocation), typeof(SpriteBatch) }));
            // load arg0 [this]
            mbIL.Emit(OpCodes.Ldarg_0);
            // load arg1 [batch]
            mbIL.Emit(OpCodes.Ldarg_1);
            LoadArgs(mbIL, method.GetParameters().Length);
            // call [base] and push stack [this, batch]
            mbIL.Emit(OpCodes.Call, method);
            // load loc0 [base() return]
            mbIL.Emit(OpCodes.Ldloc_0);
            LoadArgs(mbIL, method.GetParameters().Length);
            // return
            mbIL.Emit(OpCodes.Ret);

            tb.DefineMethodOverride(mb, method);
        }
        public static GameLocation Create(GameLocation location)
        {
            if (Init)
            {
                assemblyName = new AssemblyName("Entoarox.HookedLocations");
                assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndSave);
                moduleBuilder = assemblyBuilder.DefineDynamicModule(assemblyName.Name, assemblyName.Name + ".dll");
                Init = false;
            }
            Type key = location.GetType();
            if (!Cache.ContainsKey(key))
            {
                TypeBuilder tb = moduleBuilder.DefineType("Entoarox.HookedLocations."+location.name + "_Hooked", TypeAttributes.Public, key);
                ConstructorInfo cbParent = location.GetType().GetConstructor(new Type[] { typeof(xTile.Map), typeof(string) });
                ConstructorBuilder cb = tb.DefineConstructor(cbParent.Attributes, cbParent.CallingConvention, GetParamTypes(cbParent.GetParameters()), null, null);
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
                MethodInfo mbParent = typeof(GameLocation).GetMethod("drawWater", BindingFlags.Public | BindingFlags.Instance);
                MethodBuilder mb = tb.DefineMethod("drawWater", mbParent.Attributes | MethodAttributes.ReuseSlot | MethodAttributes.HideBySig, mbParent.ReturnType, GetParamTypes(mbParent.GetParameters()));
                ILGenerator mbIL = mb.GetILGenerator();
                // load arg0 [this]
                mbIL.Emit(OpCodes.Ldarg_0);
                // load arg1 [batch]
                mbIL.Emit(OpCodes.Ldarg_1);
                // call [DrawBetweenLayer] and push stack [this, batch]
                mbIL.Emit(OpCodes.Call, typeof(HookedLocation).GetMethod("DrawBetweenLayer", new Type[] { typeof(GameLocation), typeof(SpriteBatch) }));
                // load arg0 [this]
                mbIL.Emit(OpCodes.Ldarg_0);
                // load arg1 [batch]
                mbIL.Emit(OpCodes.Ldarg_1);
                // call [base] and push stack [this, batch]
                mbIL.Emit(OpCodes.Call, mbParent);
                // return
                mbIL.Emit(OpCodes.Ret);

                tb.DefineMethodOverride(mb, mbParent);
                Cache.Add(key, tb.CreateType());
                assemblyBuilder.Save(assemblyName.Name + ".dll");
            }
            return (GameLocation)Activator.CreateInstance(Cache[key], new object[] { location.map, location.name });
        }
    }
}
