using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;

namespace Entoarox.Framework.UI
{
    public class ProgressbarComponent : BaseMenuComponent
    {
        /*********
        ** Fields
        *********/
        protected static readonly Rectangle Background = new Rectangle(403, 383, 6, 6);
        protected static readonly Rectangle Filler = new Rectangle(306, 320, 16, 16);
        protected int _Value;
        protected int MaxValue;
        protected int OffsetValue;


        /*********
        ** Accessors
        *********/
        public int Value
        {
            get => this._Value;
            set => this._Value = Math.Max(0, Math.Min(this.MaxValue, value));
        }


        /*********
        ** Public methods
        *********/
        public ProgressbarComponent(Point position, int value, int maxValue)
        {
            this.MaxValue = maxValue;
            this.Value = value;
            this.OffsetValue = this.Value * Game1.pixelZoom;
            this.SetScaledArea(new Rectangle(position.X, position.Y, this.MaxValue + 2, 6));
        }

        public override void Draw(SpriteBatch b, Point o)
        {
            if (DateTime.Now.Millisecond % 5 == 0)
                this.OffsetValue += this.GetDiff();
            IClickableMenu.drawTextureBox(b, Game1.mouseCursors, ProgressbarComponent.Background, this.Area.X + o.X, this.Area.Y + o.Y, this.Area.Width, this.Area.Height, Color.White, Game1.pixelZoom, false);
            b.Draw(Game1.mouseCursors, new Rectangle(this.Area.X + o.X + Game1.pixelZoom, this.Area.Y + o.Y + Game1.pixelZoom, this.OffsetValue, BaseMenuComponent.Zoom4), ProgressbarComponent.Filler, Color.White);
        }


        /*********
        ** Protected methods
        *********/
        protected int GetDiff()
        {
            int v = this._Value * Game1.pixelZoom;
            if (this.OffsetValue == v)
                return 0;
            if (this.OffsetValue > v)
                return -(int)Math.Floor((this.OffsetValue - v) / 10D + 1);
            return (int)Math.Floor((v - this.OffsetValue) / 10D + 1);
        }
    }
}
