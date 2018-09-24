using System.ComponentModel;
using Newtonsoft.Json;

namespace Entoarox.AdvancedLocationLoader.Configs
{
    internal class TeleporterDestination
    {
        /*********
        ** Accessors
        *********/
        [DefaultValue(0)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public int Direction;

        public string ItemText;
        public string MapName;
        public int TileX;
        public int TileY;


        /*********
        ** Public methods
        *********/
        public override bool Equals(object a)
        {
            if (a is TeleporterDestination b)
                return b.MapName == this.MapName && b.TileX == this.TileX && b.TileY == this.TileY;

            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return $"{this.MapName}@[{this.TileX},{this.TileY}]:{this.Direction}";
        }
    }
}
