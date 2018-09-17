using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace Entoarox.Framework.UI
{
    public class HeartsComponent : BaseMenuComponent
    {
        /*********
        ** Fields
        *********/
        protected static readonly Rectangle HeartEmpty = new Rectangle(218, 428, 7, 6);
        protected static readonly Rectangle HeartFull = new Rectangle(211, 428, 7, 6);
        protected int _Value;
        protected int MaxValue;


        /*********
        ** Accessors
        *********/
        public int Value
        {
            get => this._Value;
            set => this._Value = Math.Min(Math.Max(0, value), this.MaxValue);
        }


        /*********
        ** Public methods
        *********/
        public HeartsComponent(Point position, int value, int maxValue)
        {
            if (maxValue % 2 != 0)
                maxValue++;
            this.SetScaledArea(new Rectangle(position.X, position.Y, 8 * (maxValue / 2), HeartsComponent.HeartEmpty.Height));
            this.MaxValue = maxValue;
            this.Value = value;
        }

        public override void Draw(SpriteBatch b, Point o)
        {
            if (!this.Visible)
                return;
            for (int c = 0; c < this.MaxValue / 2; c++)
                b.Draw(Game1.mouseCursors, new Vector2(o.X + this.Area.X + Game1.pixelZoom + c * BaseMenuComponent.Zoom8, o.Y + this.Area.Y), new Rectangle(HeartsComponent.HeartEmpty.X, HeartsComponent.HeartEmpty.Y, 7, 6), Color.White, 0, Vector2.Zero, Game1.pixelZoom, SpriteEffects.None, 1f);
            for (int c = 0; c < this.Value; c++)
                b.Draw(Game1.mouseCursors, new Vector2(o.X + this.Area.X + Game1.pixelZoom + c * BaseMenuComponent.Zoom4, o.Y + this.Area.Y), new Rectangle(HeartsComponent.HeartFull.X + (c % 2 == 0 ? 0 : 4), HeartsComponent.HeartFull.Y, c % 2 == 0 ? 4 : 3, 6), Color.White, 0, Vector2.Zero, Game1.pixelZoom, SpriteEffects.None, 1f);
        }
    }
}
