using System.Collections.Generic;

namespace Entoarox.AdvancedLocationLoader.Configs
{
    internal class LocationConfig_1_1
    {
        /*********
        ** Accessors
        *********/
        public IList<IDictionary<string, string>> Locations { get; set; }
        public IList<IDictionary<string, string>> Overrides { get; set; }
        public IDictionary<string, IDictionary<string, IList<string>>> Minecarts { get; set; }
        public IList<IDictionary<string, string>> Tilesheets { get; set; }
        public IList<IDictionary<string, string>> Tiles { get; set; }
        public IList<IList<string>> Properties { get; set; }
        public IList<IList<string>> Warps { get; set; }
        public IDictionary<string, IDictionary<string, string>> Conditions { get; set; }
        public IList<string> Shops { get; set; }
    }
}
