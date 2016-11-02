namespace Entoarox.AdvancedLocationLoader.Locations
{
    class Sewer : StardewValley.Locations.Sewer
    {
        public Sewer()
        {

        }
        public Sewer(xTile.Map map, string name) : base(map,name)
        {

        }
        public override void resetForPlayerEntry()
        {
            base.resetForPlayerEntry();
            characters.Clear();
        }
    }
}
