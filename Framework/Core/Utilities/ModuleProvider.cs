using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Entoarox.Framework.Core.Utilities
{
    internal static class ModuleProvider
    {
        /*********
        ** Fields
        *********/
        private static AssemblyBuilder AssemblyBuilder;


        /*********
        ** Public methods
        *********/
        public static ModuleBuilder GetModuleBuilder(string namespacePath)
        {
            if (ModuleProvider.AssemblyBuilder == null)
                ModuleProvider.AssemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName("Entoarox.Framework.Dynamic"), AssemblyBuilderAccess.Run);
            return ModuleProvider.AssemblyBuilder.DefineDynamicModule(namespacePath);
        }
    }
}
