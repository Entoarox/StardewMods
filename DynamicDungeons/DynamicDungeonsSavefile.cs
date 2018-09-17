using System.Collections.Generic;
using StardewValley.Objects;

namespace Entoarox.DynamicDungeons
{
    internal class DynamicDungeonsSavefile
    {
        /*********
        ** Accessors
        *********/
        public Dictionary<int, int> LoreFound = new Dictionary<int, int>();
        public Chest StorageChest = new Chest();
    }
}
