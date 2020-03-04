using System.Collections.Generic;
using System.Runtime.Serialization;
using System.IO;
using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewValley;

namespace SundropCity.Json
{
    enum FeatureType
    {
        Inline,
        BelowAlwaysFront,
        AboveAlwaysFront
    }
    class CustomFeature
    {
#pragma warning disable CS0649
        public string Texture;
        public int[] Position;
        public Dictionary<string, int[]> Regions;
#pragma warning restore CS0649
        public FeatureType Type = FeatureType.Inline;

        internal float Alpha = 1;
        internal Texture2D TextureFile;
        internal Point AlphaPoint;
        internal Vector2 TileVector;
        internal Dictionary<string, Rectangle> ClipRects = new Dictionary<string, Rectangle>();

        [OnDeserialized]
        internal void Init(StreamingContext context)
        {
            this.TextureFile = SundropCityMod.SHelper.Content.Load<Texture2D>(Path.Combine("assets", "TerrainFeatures", this.Texture));
            this.AlphaPoint = new Point(this.Position[0] * 64 + 32, this.Position[1] * 64 + 32);
            this.TileVector = new Vector2(this.Position[0] * 64, this.Position[1] * 64);
            foreach (var entry in this.Regions)
                this.ClipRects.Add(entry.Key.ToLower(), new Rectangle(entry.Value[0] * 16, entry.Value[1] * 16, entry.Value[2] * 16, entry.Value[3] * 16));
        }
        internal void Render(SpriteBatch b)
        {
            var rect = this.ClipRects.ContainsKey(Game1.currentSeason) ? this.ClipRects[Game1.currentSeason] : this.ClipRects["default"];
            var bounds = new Rectangle(this.AlphaPoint.X, this.AlphaPoint.Y, rect.Width * 4 - 64, rect.Height * 4 - 64);
            this.Alpha = Math.Min(1f, this.Alpha + 0.05f);
            if (Game1.player.GetBoundingBox().Intersects(bounds))
                this.Alpha = Math.Max(0.6f, this.Alpha - 0.09f);
            b.Draw(this.TextureFile, Game1.GlobalToLocal(this.TileVector), rect, Color.White * this.Alpha, 0, Vector2.Zero, 4f, SpriteEffects.None, (bounds.Bottom + 32) / 10000f - this.TileVector.X / 1000000f);
        }
    }
}
