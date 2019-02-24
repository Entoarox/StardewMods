using System.ComponentModel;
using Newtonsoft.Json;

namespace Entoarox.AdvancedLocationLoader.Configs
{
    internal class Conditional
    {
#pragma warning disable CS0649
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
#pragma warning restore CS0649


        /*********
        ** Public methods
        *********/
        public override string ToString()
        {
            return $"Conditional({this.Name}[{this.Item}{':'}{this.Amount}] = `{this.Question}`)";
        }
    }
}
