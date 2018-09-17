using System.Xml.Serialization;

namespace Entoarox.AdvancedLocationLoader.Locations
{
    [XmlType("ALLSewer")]
    public class Sewer : StardewValley.Locations.Sewer
    {
        /*********
        ** Public methods
        *********/
        public Sewer() { }

        public Sewer(string mapPath, string name)
            : base(mapPath, name) { }


        /*********
        ** Protected methods
        *********/
        protected override void resetLocalState()
        {
            base.resetLocalState();
            this.characters.Clear();
        }
    }
}
