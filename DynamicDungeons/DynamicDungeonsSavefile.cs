using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using StardewValley.Objects;

namespace Entoarox.DynamicDungeons
{
    class DynamicDungeonsSavefile
    {
        public Dictionary<int, int> LoreFound = new Dictionary<int, int>();
        public Chest StorageChest = new Chest();
    }
}
