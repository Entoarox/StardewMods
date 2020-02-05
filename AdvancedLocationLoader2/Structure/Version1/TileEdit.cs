using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entoarox.AdvancedLocationLoader2.Structure.Version1
{
    class TileEdit
    {
#pragma warning disable CS0649
        public string MapName;
        public int X;
        public int Y;
        public string Layer;
        public string TileSheet;
        public int? TileIndex;
        public Dictionary<string, string> Properties;
        public string Conditions;
#pragma warning restore CS0649
    }
}
