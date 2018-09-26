using System.Diagnostics.CodeAnalysis;
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

        [SuppressMessage("SMAPI", "AvoidNetField", Justification = "The Name field doesn't have a setter, so we need to do it through the net field value.")]
        public override void DayUpdate(int dayOfMonth)
        {
            string realName = this.Name;
            this.name.Value = "Greenhouse";
            base.DayUpdate(dayOfMonth);
            this.name.Value = realName;
        }
    }
}
