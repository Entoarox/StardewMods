using System.Xml.Serialization;

namespace Entoarox.AdvancedLocationLoader.Locations
{
    [XmlType("ALLGreenhouse")]
    public class Greenhouse : StardewValley.GameLocation
    {
        public Greenhouse()
        {

        }
        public Greenhouse(string mapPath, string name)
            : base(mapPath, name)
        {

        }
        public override void DayUpdate(int dayOfMonth)
        {
            string realName = this.name;
            this.name.Value = "Greenhouse";
            base.DayUpdate(dayOfMonth);
            this.name.Value = realName;
        }
    }
}
