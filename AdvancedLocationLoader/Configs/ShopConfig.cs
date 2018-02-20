using System.Collections.Generic;
using Newtonsoft.Json;

namespace Entoarox.AdvancedLocationLoader.Configs
{
    public class ShopConfig
    {
        [JsonIgnore]
        public string Name;

        public int ParserVersion;
        public string Portrait;
        public string Owner;
        public List<string> Messages;
        public List<ShopItem> Items;
    }
}
