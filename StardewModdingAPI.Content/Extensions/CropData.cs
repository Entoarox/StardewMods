using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace StardewModdingAPI.Content.Extensions
{
    public class CropData
    {
        internal bool Registered;
        internal int Id;
        public Texture2D Crop;
        public bool Trellis;
        public bool MultipleHarvests;
        public bool RequiresSickle;
        public List<string> ValidSeasons;
        public List<int> DaysPerPhase;
        public List<Color> ColorTints;
        public int MinimumProduce;
        public int MaximumProduce;
        public int FarmerLevelProducePenalty;
        public float ExtraProduceChance;
    }
}
