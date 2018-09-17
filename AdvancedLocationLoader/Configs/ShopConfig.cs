using System.Collections.Generic;
using Newtonsoft.Json;

namespace Entoarox.AdvancedLocationLoader.Configs
{
    internal class ShopConfig
    {
        /*********
        ** Accessors
        *********/
        [JsonIgnore]
        public string Name;

        public int ParserVersion;
        public string Portrait;
        public string Owner;
        public List<string> Messages;
        public List<ShopItem> Items;
    }
}
