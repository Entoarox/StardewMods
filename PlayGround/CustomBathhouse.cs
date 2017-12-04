using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using xTile;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewValley;

namespace PlayGround
{
    class CustomBathhouse : GameLocation
    {
        private Vector2 SteamPosition;
        private Texture2D SteamAnimation;
        public CustomBathhouse(Map map, string name, Texture2D steam=null) : base(map, name)
        {
            this.SteamAnimation = steam;
        }
        public override void resetForPlayerEntry()
        {
            base.resetForPlayerEntry();
            Game1.changeMusicTrack("pool_ambient");
            this.SteamPosition = new Vector2(0f, 0f);
            this.SteamAnimation = this.SteamAnimation ?? Game1.temporaryContent.Load<Texture2D>("LooseSprites\\steamAnimation");
        }

        public override void drawAboveAlwaysFrontLayer(SpriteBatch b)
        {
            base.drawAboveAlwaysFrontLayer(b);
            // End vanilla
            b.End();
            // Custom clamping
            b.GraphicsDevice.ScissorRectangle = Rectangle.Intersect(new Rectangle(15 * Game1.tileSize - Game1.viewport.X, 1 * Game1.tileSize - Game1.viewport.Y, 25 * Game1.tileSize, 20 * Game1.tileSize - 7*Game1.pixelZoom), new Rectangle(0, 0, Game1.viewport.Width, Game1.viewport.Height));
            b.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, new RasterizerState() { ScissorTestEnable = true });
            for (float num = this.SteamPosition.X; num < Game1.graphics.GraphicsDevice.Viewport.Width+Game1.viewport.X + 256f; num += 256f)
            {
                for (float num2 = this.SteamPosition.Y; num2 < Game1.graphics.GraphicsDevice.Viewport.Height+Game1.viewport.Y + 128; num2 += 256f)
                {
                    b.Draw(this.SteamAnimation, new Rectangle((int)Math.Round(num - Game1.viewport.X), (int)Math.Round(num2 - Game1.viewport.Y), 256, 256), null, Color.White * 0.8f, 0f, Vector2.Zero, SpriteEffects.None, 1f);
                }
            }
            b.End();
            // Restore vanilla
            Game1.spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp, null, null);
        }
        private bool reverse = false;
        public override void UpdateWhenCurrentLocation(GameTime time)
        {
            base.UpdateWhenCurrentLocation(time);
            this.SteamPosition.Y = this.SteamPosition.Y - time.ElapsedGameTime.Milliseconds * 0.1f;
            this.SteamPosition.Y = this.SteamPosition.Y % -256f;
            if (this.SteamPosition.X < -32f || this.SteamPosition.X > 32f)
                this.reverse = !this.reverse;
            if (this.reverse)
                this.SteamPosition.X = this.SteamPosition.X + (float)Math.Sqrt(Math.Pow(time.ElapsedGameTime.Milliseconds * 0.05f, 33 - (this.SteamPosition.X % 32)));
            else
                this.SteamPosition.X = this.SteamPosition.X - (float)Math.Sqrt(Math.Pow(time.ElapsedGameTime.Milliseconds * 0.05f, 33 - (this.SteamPosition.X % 32)));
        }
    }
}
