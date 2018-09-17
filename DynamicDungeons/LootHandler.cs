using System;
using System.Collections.Generic;
using System.Linq;
using SObject = StardewValley.Object;

namespace Entoarox.DynamicDungeons
{
    internal class LootHandler
    {
        /*********
        ** Fields
        *********/
        private Dictionary<double, List<object>> LootTable = new Dictionary<double, List<object>>();
        private readonly Dictionary<int, Random> Randoms = new Dictionary<int, Random>();


        /*********
        ** Accessors
        *********/
        internal static Dictionary<string, LootHandler> LootTables = new Dictionary<string, LootHandler>();


        /*********
        ** Public methods
        *********/
        public LootHandler(Dictionary<double, List<object>> lootTable = null)
        {
            this.LootTable = lootTable?.OrderBy(a => a.Key).ToDictionary(a => a.Key, a => a.Value) ?? new Dictionary<double, List<object>>();
        }

        static LootHandler()
        {
            // Setup vanilla loot tables
            LootHandler.LootTables.Add("General", new LootHandler());
            LootHandler.LootTables.Add("Supplies", new LootHandler());
            LootHandler.LootTables.Add("Gems", new LootHandler());
            LootHandler.LootTables.Add("Fishing", new LootHandler());
            LootHandler.LootTables.Add("Digging", new LootHandler());
            // Setup vanilla loot drops
            LootHandler.LootTables["General"].Add(1, new SObject(93, 1));
            LootHandler.LootTables["Supplies"].Add(1, new SObject(93, 1));
            LootHandler.LootTables["Gems"].Add(1, new SObject(80, 1));
            LootHandler.LootTables["Fishing"].Add(1, new SObject(168, 1));
            LootHandler.LootTables["Digging"].Add(1, new SObject(330, 1));
        }

        public void Add(double chancePercentage, SObject itemLoot)
        {
            if (!this.LootTable.ContainsKey(chancePercentage))
                this.LootTable.Add(chancePercentage, new List<object> { itemLoot });
            this.LootTable = this.LootTable.OrderBy(a => a.Key).ToDictionary(a => a.Key, a => a.Value);
        }

        public void Add(double chancePercentage, Func<SObject> itemLootCallback)
        {
            if (!this.LootTable.ContainsKey(chancePercentage))
                this.LootTable.Add(chancePercentage, new List<object> { itemLootCallback });
            this.LootTable = this.LootTable.OrderBy(a => a.Key).ToDictionary(a => a.Key, a => a.Value);
        }

        public SObject[] GetDrops(int seed, int count, double chanceModifier)
        {
            SObject[] drops = new SObject[count];
            for (int c = 0; c < count; c++)
                drops[c] = this.GetDrop(seed, chanceModifier);
            return drops;
        }

        public SObject GetDrop(int seed, double chanceModifier)
        {
            foreach (KeyValuePair<double, List<object>> option in this.LootTable)
            {
                double chance = chanceModifier + option.Key;
                foreach (object item in option.Value)
                {
                    if (this.GetRandom(seed).NextDouble() < chance)
                        return item as SObject ?? (item as Func<SObject>)() ?? this.GetDrop(seed, chanceModifier);
                }
            }

            return new SObject(93, 1);
        }


        /*********
        ** Private methods
        *********/
        private Random GetRandom(int seed)
        {
            if (!this.Randoms.ContainsKey(seed))
                this.Randoms.Add(seed, new Random(seed));
            return this.Randoms[seed];
        }
    }
}
