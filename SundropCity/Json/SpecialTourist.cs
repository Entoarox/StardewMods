using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using StardewValley;

namespace SundropCity.Json
{
    class SpecialTourist
    {
        public readonly string Id;
        public readonly int Lines;
        public readonly double Chance;
        public readonly string Season;
        public readonly Func<Farmer, bool> Trigger;
        public bool IsActive;

        public SpecialTourist(string id, int lines, double chance, string season=null, Func<Farmer, bool> trigger=null)
        {
            this.Id = id;
            this.Lines = lines;
            this.Chance = chance;
            this.Season = season;
            this.Trigger = trigger;
        }
    }
}
