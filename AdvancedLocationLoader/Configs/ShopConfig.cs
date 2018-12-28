using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
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

        [JsonIgnore]
        public Texture2D PortraitTexture;

        public int ParserVersion;
        public string Portrait;
        public string Owner;
        public List<string> Messages;
        public List<ShopItem> Items;
    }
}
