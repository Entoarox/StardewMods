using System.Xml.Serialization;

namespace Entoarox.AdvancedLocationLoader.Locations
{
    [XmlType("ALLGreenhouse")]
    public class Greenhouse : StardewValley.GameLocation
    {
        public Greenhouse()
        {

        }
        public Greenhouse(xTile.Map map, string name) : base(map,name)
        {

        }
        public override void DayUpdate(int dayOfMonth)
        {
            string realName = name;
            name = "Greenhouse";
            base.DayUpdate(dayOfMonth);
            name = realName;
        }
    }
}
