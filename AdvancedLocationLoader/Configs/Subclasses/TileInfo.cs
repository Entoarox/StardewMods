using System.ComponentModel;
using Newtonsoft.Json;

namespace Entoarox.AdvancedLocationLoader.Configs
{
    internal abstract class TileInfo
    {
        /*********
        ** Accessors
        *********/
#pragma warning disable CS0649
        public string Conditions;
        public string MapName;

        [DefaultValue(false)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public bool Optional;

        public int TileX;
        public int TileY;
#pragma warning restore CS0649
    }
}
