using System.ComponentModel;
using Newtonsoft.Json;

namespace Entoarox.AdvancedLocationLoader.Configs
{
    internal abstract class TileInfo
    {
        /*********
        ** Accessors
        *********/
        public string Conditions;
        public string MapName;

        [DefaultValue(false)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public bool Optional;

        public int TileX;
        public int TileY;
    }
}
