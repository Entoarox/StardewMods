using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using xTile;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewValley;
using xTile.Layers;

namespace PlayGround
{
    class CustomBathhouse : GameLocation
    {
        private Vector2 SteamPosition;
        private Texture2D SteamAnimation;
        public CustomBathhouse(string mapPath, string name, Texture2D steam=null)
            : base(mapPath, name)
        {
            this.SteamAnimation = steam;
        }

        protected override void resetLocalState()
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
            Rectangle viewport = new Rectangle(0, 0, Game1.viewport.Width, Game1.viewport.Height);
            // Custom clamping
            b.GraphicsDevice.ScissorRectangle = Rectangle.Intersect(new Rectangle(15 * Game1.tileSize - Game1.viewport.X, 1 * Game1.tileSize - Game1.viewport.Y, 25 * Game1.tileSize, 20 * Game1.tileSize - 7*Game1.pixelZoom), viewport);
            b.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, new RasterizerState() { ScissorTestEnable = true });
            for (float num = this.SteamPosition.X; num < Game1.viewport.Width+Game1.viewport.X + 256f; num += 256f)
            {
                for (float num2 = this.SteamPosition.Y; num2 < Game1.viewport.Height+Game1.viewport.Y + 128; num2 += 256f)
                {
                    b.Draw(this.SteamAnimation, new Rectangle((int)Math.Round(num - Game1.viewport.X), (int)Math.Round(num2 - Game1.viewport.Y), 256, 256), null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 1f);
                }
            }
            b.End();
            // Left-door magic
            float px = Game1.tileSize * 2 + 1;
            double pxp = 1 / px;
            int h = 4 * Game1.tileSize - 7 * Game1.pixelZoom;
            int y = 9 * Game1.tileSize - Game1.viewport.Y;
            int x = 15 * Game1.tileSize - Game1.viewport.X;
            for (int c = 1; c < px; c++)
            {
                b.GraphicsDevice.ScissorRectangle = Rectangle.Intersect(new Rectangle(x - c, y, 1, h), viewport);
                b.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, new RasterizerState() { ScissorTestEnable = true });
                for (float num = this.SteamPosition.X; num < Game1.viewport.Width + Game1.viewport.X + 256f; num += 256f)
                {
                    for (float num2 = this.SteamPosition.Y; num2 < Game1.viewport.Height + Game1.viewport.Y + 128; num2 += 256f)
                    {
                        b.Draw(this.SteamAnimation, new Rectangle((int)Math.Round(num - Game1.viewport.X), (int)Math.Round(num2 - Game1.viewport.Y), 256, 256), null, Color.White * (float)(pxp*(px-c)), 0f, Vector2.Zero, SpriteEffects.None, 1f);
                    }
                }
                b.End();
            }
            x += (25 * Game1.tileSize) - 1;
            for (int c = 1; c < px; c++)
            {
                b.GraphicsDevice.ScissorRectangle = Rectangle.Intersect(new Rectangle(x + c, y, 1, h), viewport);
                b.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, new RasterizerState() { ScissorTestEnable = true });
                for (float num = this.SteamPosition.X; num < Game1.viewport.Width + Game1.viewport.X + 256f; num += 256f)
                {
                    for (float num2 = this.SteamPosition.Y; num2 < Game1.viewport.Height + Game1.viewport.Y + 128; num2 += 256f)
                    {
                        b.Draw(this.SteamAnimation, new Rectangle((int)Math.Round(num - Game1.viewport.X), (int)Math.Round(num2 - Game1.viewport.Y), 256, 256), null, Color.White * (float)(pxp * (px - c)), 0f, Vector2.Zero, SpriteEffects.None, 1f);
                    }
                }
                b.End();
            }
            // Restore vanilla
            Game1.spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp, null, null);
        }
        private bool reverse = false;
        public override void UpdateWhenCurrentLocation(GameTime time)
        {
            base.UpdateWhenCurrentLocation(time);

            Layer alwaysFrontLayer = this.Map.GetLayer("AlwaysFront");
            if (alwaysFrontLayer != null)
                this.Map.RemoveLayer(alwaysFrontLayer);

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
