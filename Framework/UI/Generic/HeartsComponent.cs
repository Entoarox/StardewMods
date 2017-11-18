using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewValley;

namespace Entoarox.Framework.UI
{
    public class HeartsComponent : BaseMenuComponent
    {
        protected readonly static Rectangle HeartFull = new Rectangle(211, 428, 7, 6);
        protected readonly static Rectangle HeartEmpty = new Rectangle(218, 428, 7, 6);
        public int Value
        {
            get
            {
                return this._Value;
            }
            set
            {
                this._Value = Math.Min(Math.Max(0, value), this.MaxValue);
            }
        }
        protected int _Value;
        protected int MaxValue;
        public HeartsComponent(Point position, int value, int maxValue)
        {
            if (maxValue % 2 != 0)
                maxValue++;
            SetScaledArea(new Rectangle(position.X, position.Y, 8 * (maxValue / 2), HeartEmpty.Height));
            this.MaxValue = maxValue;
            this.Value = value;
        }
        public override void Draw(SpriteBatch b, Point o)
        {
            if (!this.Visible)
                return;
            for (int c = 0; c < this.MaxValue / 2; c++)
                b.Draw(Game1.mouseCursors, new Vector2(o.X + this.Area.X + Game1.pixelZoom + c * zoom8, o.Y + this.Area.Y), new Rectangle(HeartEmpty.X, HeartEmpty.Y, 7, 6), Color.White, 0, Vector2.Zero, Game1.pixelZoom, SpriteEffects.None, 1f);
            for (int c = 0; c < this.Value; c++)
                b.Draw(Game1.mouseCursors, new Vector2(o.X + this.Area.X + Game1.pixelZoom + c * zoom4, o.Y + this.Area.Y), new Rectangle(HeartFull.X + (c % 2 == 0 ? 0 : 4), HeartFull.Y, (c % 2 == 0 ? 4 : 3), 6), Color.White, 0, Vector2.Zero, Game1.pixelZoom, SpriteEffects.None, 1f);
        }
    }
}
