using System;
using System.Collections.Generic;
using System.Linq;

using SObject = StardewValley.Object;

namespace Entoarox.DynamicDungeons
{
    class LootHandler
    {
        internal static Dictionary<string, LootHandler> LootTables = new Dictionary<string, LootHandler>();
        static LootHandler()
        {
            // Setup vanilla loot tables
            LootTables.Add("General", new LootHandler());
            LootTables.Add("Supplies", new LootHandler());
            LootTables.Add("Gems", new LootHandler());
            LootTables.Add("Fishing", new LootHandler());
            LootTables.Add("Digging", new LootHandler());
            // Setup vanilla loot drops
            LootTables["General"].Add(1, new SObject(93, 1));
            LootTables["Supplies"].Add(1, new SObject(93, 1));
            LootTables["Gems"].Add(1, new SObject(80, 1));
            LootTables["Fishing"].Add(1, new SObject(168, 1));
            LootTables["Digging"].Add(1, new SObject(330, 1));
        }
        private Dictionary<double, List<object>> _LootTable = new Dictionary<double, List<object>>();
        private Dictionary<int, Random> _Randoms = new Dictionary<int, Random>();
        public LootHandler(Dictionary<double, List<object>> lootTable = null)
        {
            this._LootTable = lootTable?.OrderBy(a => a.Key).ToDictionary(a => a.Key, a => a.Value) ?? new Dictionary<double, List<object>>();
        }
        private Random GetRandom(int seed)
        {
            if (!this._Randoms.ContainsKey(seed))
                this._Randoms.Add(seed, new Random(seed));
            return this._Randoms[seed];
        }
        public void Add(double chancePercentage, SObject itemLoot)
        {
            if (!this._LootTable.ContainsKey(chancePercentage))
                this._LootTable.Add(chancePercentage, new List<object>() { itemLoot});
            this._LootTable=this._LootTable.OrderBy(a => a.Key).ToDictionary(a => a.Key, a => a.Value);
        }
        public void Add(double chancePercentage, Func<SObject> itemLootCallback)
        {
            if (!this._LootTable.ContainsKey(chancePercentage))
                this._LootTable.Add(chancePercentage, new List<object>() { itemLootCallback });
            this._LootTable = this._LootTable.OrderBy(a => a.Key).ToDictionary(a => a.Key, a => a.Value);
        }
        public SObject[] GetDrops(int seed, int count, double chanceModifier)
        {
            var drops = new SObject[count];
            for (int c = 0; c < count; c++)
                drops[c] = this.GetDrop(seed, chanceModifier);
            return drops;
        }
        public SObject GetDrop(int seed, double chanceModifier)
        {
            foreach(var option in this._LootTable)
            {
                double chance = chanceModifier + option.Key;
                foreach (object item in option.Value)
                    if (this.GetRandom(seed).NextDouble() < chance)
                        return (item as SObject) ?? (item as Func<SObject>)() ?? GetDrop(seed, chanceModifier);
            }
            return new SObject(93, 1);
        }
    }
}
