using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using StardewModdingAPI;

namespace Entoarox.AdvancedLocationLoader.Configs
{
    public class ShopConfig : Config
    {
        public override T GenerateDefaultConfig<T>()
        {
            return this as T;
        }
        public string Portrait;
        public string Owner;
        public List<string> Messages;
        public List<ShopItem> Items;
    }
}
