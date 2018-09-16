using System.Xml;
using System.Xml.Serialization;

namespace Entoarox.AdvancedLocationLoader.Locations
{
    [XmlType("ALLSewer")]
    public class Sewer : StardewValley.Locations.Sewer
    {
        public Sewer()
        {

        }
        public Sewer(string mapPath, string name)
            : base(mapPath, name)
        {

        }

        protected override void resetLocalState()
        {
            base.resetLocalState();
            this.characters.Clear();
        }
    }
}
