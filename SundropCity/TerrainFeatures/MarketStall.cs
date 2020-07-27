using System.Collections.Generic;
using System.Runtime.CompilerServices;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewValley;
using StardewValley.Menus;
using StardewValley.TerrainFeatures;

namespace SundropCity.TerrainFeatures
{
    using Json;
    using Internal;
    internal class MarketStall : LargeTerrainFeature, ISundropTransient
    {
        public static List<ShopData> Stalls;
        private readonly Dictionary<ISalable, int[]> Stock;
        private readonly int Stall;
        private readonly bool PlayerStall = false;
        private int Sprite;
        private Rectangle SpriteRect;
        public MarketStall(Vector2 tilePosition, int stall) : base(false)
        {
            this.tilePosition.Value = tilePosition;
            this.Stall = stall;
            this.Stock = stall < Stalls.Count ? Stalls[stall].GetStock() : new Dictionary<ISalable, int[]>();
            this.Setup(Stalls[this.Stall].Sprite);
        }

        public MarketStall(Vector2 tilePosition, bool playerStall) : base(false)
        {
            this.PlayerStall = playerStall;
            this.Setup(playerStall ? SundropCityMod.SystemData.PlayerStall : SundropCityMod.SystemData.EmptyStalls[Game1.random.Next(SundropCityMod.SystemData.EmptyStalls.Length - 1)]);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private Texture2D GetTexture()
        {
            return SundropCityMod.SHelper.Content.Load<Texture2D>("assets/Maps/spring_SundropMarketStalls.png");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Setup(int sprite)
        {
            this.Sprite = sprite;
            this.SpriteRect = Game1.getSourceRectForStandardTileSheet(this.GetTexture(), sprite, 96, 80);
            this.SpriteRect.Y += 16;
        }

        public override bool performUseAction(Vector2 tileLocation, GameLocation location)
        {
                if (this.PlayerStall)
                    return false;
            if (this.Stock == null)
                Game1.drawObjectDialogue(SundropCityMod.SHelper.Translation.Get("Message.MarketStall.NoOwner"));
            else
                Game1.activeClickableMenu = new ShopMenu(this.Stock);
            return true;
        }

        public override Rectangle getBoundingBox(Vector2 tileLocation)
        {
            return new Rectangle((int)tileLocation.X * 64, (int)(tileLocation.Y - 1) * 64, 6 * 64, 2 * 64);
        }

        public override Rectangle getRenderBounds(Vector2 tileLocation)
        {
            return new Rectangle((int)tileLocation.X * 64, (int)(tileLocation.Y - 4) * 64, 6 * 64, 5 * 64);
        }

        public override void draw(SpriteBatch spriteBatch, Vector2 tileLocation)
        {
            spriteBatch.Draw(this.GetTexture(), Game1.GlobalToLocal(Game1.viewport, new Vector2(tileLocation.X * 64, (tileLocation.Y - 4) * 64)), this.SpriteRect, Color.White, 0, Vector2.Zero, 0, SpriteEffects.None, (this.getBoundingBox(tileLocation).Center.Y + 48) / 10000f - tileLocation.X / 1000000f);
        }
    }
}
