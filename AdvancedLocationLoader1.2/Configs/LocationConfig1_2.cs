using System.Collections.Generic;

namespace Entoarox.AdvancedLocationLoader.Configs
{
    public class LocationConfig1_2
    {
        public string LoaderVersion;
        public About About=new About();
        public List<Location> Locations=new List<Location>();
        public List<Override> Overrides=new List<Override>();
        public List<Redirect> Redirects=new List<Redirect>();
        public List<Tilesheet> Tilesheets=new List<Tilesheet>();
        public List<Tile> Tiles=new List<Tile>();
        public List<Property> Properties=new List<Property>();
        public List<Warp> Warps=new List<Warp>();
        public List<Conditional> Conditionals=new List<Conditional>();
        public List<TeleporterList> Teleporters=new List<TeleporterList>();
        public List<string> Shops;
    }
}
