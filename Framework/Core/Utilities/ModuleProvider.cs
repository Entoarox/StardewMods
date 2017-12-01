using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Reflection.Emit;

namespace Entoarox.Framework.Core.Utilities
{
    static class ModuleProvider
    {
        private static AssemblyBuilder AssemblyBuilder;
        public static ModuleBuilder GetModuleBuilder(string namespacePath)
        {
            if (AssemblyBuilder == null)
                AssemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName("Entoarox.Framework.Dynamic"), AssemblyBuilderAccess.Run);
            return AssemblyBuilder.DefineDynamicModule(namespacePath);
        }
    }
}
