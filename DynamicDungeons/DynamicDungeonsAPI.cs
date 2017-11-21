using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SObject = StardewValley.Object;

using Microsoft.Xna.Framework;

namespace DynamicDungeons
{
    public static class DynamicDungeonsAPI
    {
        public static void AddLootReward(string table, double dropChance, SObject itemLoot)
        {
            if (!LootHandler.LootTables.ContainsKey(table))
                LootHandler.LootTables.Add(table, new LootHandler(0));
            LootHandler.LootTables[table].Add(dropChance, itemLoot);
        }
        public static void AddLootReward(string table, double dropChance, Func<SObject> itemLootCallback)
        {
            if (!LootHandler.LootTables.ContainsKey(table))
                LootHandler.LootTables.Add(table, new LootHandler(0));
            LootHandler.LootTables[table].Add(dropChance, itemLootCallback);
        }
        public static void AddStructure(string name, Point size, int weight, int minSpawn, int maxSpawn, int floorMod, float baseChance, float diffChance, ITile[] tiles)
        {
        }
        public static void AddStructure(string name, Point size, int weight, int minSpawn, int maxSpawn, float baseChance, float diffChance, ITile[] tiles)
        {
            AddStructure(name, size, weight, minSpawn, maxSpawn, 1, baseChance, diffChance, tiles);
        }
        public static void AddStructure(string name, Point size, int weight, int minSpawn, int maxSpawn, int floorMod, ITile[] tiles)
        {
            AddStructure(name, size, weight, minSpawn, maxSpawn, floorMod, 1, 0, tiles);
        }
        public static void AddStructure(string name, Point size, int weight, int minSpawn, int maxSpawn, ITile[] tiles)
        {
            AddStructure(name, size, weight, minSpawn, maxSpawn, 1, 1, 0, tiles);
        }
    }
}
