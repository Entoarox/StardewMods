using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entoarox.AdvancedLocationLoader2.Structure.Version1
{
    class ContentPack
    {
        public int Version;
        public string[] Includes=new string[0];
        public MapFileLink[] Locations = new MapFileLink[0];
        public MapFileLink[] Tilesheets = new MapFileLink[0];
        public Warp[] Warps = new Warp[0];
        public Patch[] Patches = new Patch[0];
        public TileEdit[] TileEdits = new TileEdit[0];
        public Shop[] Shops = new Shop[0];
    }
}
