using System;

using StardewValley;
using SObject = StardewValley.Object;

namespace Entoarox.DynamicDungeons
{
    public class DynamicDungeonsAPI
    {
        public void RegisterLootEntry(string table, double dropChance, SObject itemLoot)
        {
            if (!LootHandler.LootTables.ContainsKey(table))
                LootHandler.LootTables.Add(table, new LootHandler());
            LootHandler.LootTables[table].Add(dropChance, itemLoot);
        }
        public void RegisterLootEntry(string table, double dropChance, Func<SObject> itemLootCallback)
        {
            if (!LootHandler.LootTables.ContainsKey(table))
                LootHandler.LootTables.Add(table, new LootHandler());
            LootHandler.LootTables[table].Add(dropChance, itemLootCallback);
        }
        public int CurrentFloorLevel => DynamicDungeonsMod.Location?.Floor ?? 0;
        public bool InDungeon => Game1.currentLocation != null && Game1.currentLocation is DynamicDungeon;
    }
}
