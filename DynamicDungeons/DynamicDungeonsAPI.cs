using System;

using SObject = StardewValley.Object;

namespace Entoarox.DynamicDungeons
{
    public class DynamicDungeonsAPI
    {
        public void RegisterLootEntry(string table, double dropChance, SObject itemLoot)
        {
            if (!LootHandler.LootTables.ContainsKey(table))
                LootHandler.LootTables.Add(table, new LootHandler(0));
            LootHandler.LootTables[table].Add(dropChance, itemLoot);
        }
        public void RegisterLootEntry(string table, double dropChance, Func<SObject> itemLootCallback)
        {
            if (!LootHandler.LootTables.ContainsKey(table))
                LootHandler.LootTables.Add(table, new LootHandler(0));
            LootHandler.LootTables[table].Add(dropChance, itemLootCallback);
        }
    }
}
