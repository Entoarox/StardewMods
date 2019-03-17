using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using StardewModdingAPI;

namespace DialogueFramework
{
    public class DialogueFrameworkMod : Mod
    {
        internal static ModApi Api;
        public override void Entry(IModHelper helper)
        {
            Api = new ModApi();
        }
        public override object GetApi()
        {
            return Api;
        }
    }
}
