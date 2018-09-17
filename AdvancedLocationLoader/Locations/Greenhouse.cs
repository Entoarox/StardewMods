using System.Xml.Serialization;
using StardewValley;

namespace Entoarox.AdvancedLocationLoader.Locations
{
    [XmlType("ALLGreenhouse")]
    public class Greenhouse : GameLocation
    {
        /*********
        ** Public methods
        *********/
        public Greenhouse() { }

        public Greenhouse(string mapPath, string name)
            : base(mapPath, name) { }

        public override void DayUpdate(int dayOfMonth)
        {
            string realName = this.Name;
            this.name.Value = "Greenhouse";
            base.DayUpdate(dayOfMonth);
            this.name.Value = realName;
        }
    }
}
