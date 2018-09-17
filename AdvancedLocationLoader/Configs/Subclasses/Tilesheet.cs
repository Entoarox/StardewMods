using System.Collections.Generic;
using System.ComponentModel;
using Newtonsoft.Json;

namespace Entoarox.AdvancedLocationLoader.Configs
{
    internal class Tilesheet : MapFileLink
    {
        /*********
        ** Accessors
        *********/
        public Dictionary<string, string> Properties = new Dictionary<string, string>();

        [DefaultValue(false)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public bool Seasonal;

        public string SheetId;


        /*********
        ** Public methods
        *********/
        public override string ToString()
        {
            return $"Tilesheet({this.MapName}:{this.SheetId},{this.FileName ?? "null"}){{Seasonal={this.Seasonal}}}";
        }
    }
}
