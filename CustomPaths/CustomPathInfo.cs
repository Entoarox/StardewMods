using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace Entoarox.CustomPaths
{
    public class CustomPathInfo
    {
        /*********
        ** Fields
        *********/
        private readonly bool Seasonal;
        private readonly Dictionary<string, Texture2D> Textures;


        /*********
        ** Accessors
        *********/
        public int Alternates;
        public string Name;
        public int Price;
        public string Requirements;
        public string Salesman;
        public int Speed;


        /*********
        ** Public methods
        *********/
        internal CustomPathInfo(Texture2D texture, CustomPathConfig config)
        {
            this.Seasonal = false;
            this.Textures = new Dictionary<string, Texture2D> { { "default", texture } };
            this.Name = config.Name;
            this.Alternates = config.Alternatives;
            this.Price = config.Price;
            this.Salesman = config.Salesman;
            this.Requirements = config.Requirements;
            this.Speed = config.SpeedBoost;
        }

        internal CustomPathInfo(Dictionary<string, Texture2D> textures, CustomPathConfig config)
        {
            this.Seasonal = false;
            this.Textures = textures;
            this.Name = config.Name;
            this.Alternates = config.Alternatives;
            this.Price = config.Price;
            this.Salesman = config.Salesman;
            this.Requirements = config.Requirements;
            this.Speed = config.SpeedBoost;
        }

        public Texture2D GetTexture()
        {
            return this.Textures[this.Seasonal ? Game1.currentSeason : "default"];
        }
    }
}
