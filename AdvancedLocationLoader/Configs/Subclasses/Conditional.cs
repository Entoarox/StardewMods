using System.ComponentModel;
using Newtonsoft.Json;

namespace Entoarox.AdvancedLocationLoader.Configs
{
    internal class Conditional
    {
        /*********
        ** Accessors
        *********/
        public int Amount;
        public int Item;
        public string Name;
        public string Question;

        [DefaultValue(null)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public string Success;


        /*********
        ** Public methods
        *********/
        public override string ToString()
        {
            return $"Conditional({this.Name}[{this.Item}{':'}{this.Amount}] = `{this.Question}`)";
        }
    }
}
