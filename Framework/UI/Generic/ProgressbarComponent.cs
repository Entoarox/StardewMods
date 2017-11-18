using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewValley;
using StardewValley.Menus;

namespace Entoarox.Framework.UI
{
    public class ProgressbarComponent : BaseMenuComponent
    {
        protected readonly static Rectangle Background = new Rectangle(403, 383, 6, 6);
        protected readonly static Rectangle Filler = new Rectangle(306,320,16,16);
        public int Value
        {
            get
            {
                return this._Value;
            }
            set
            {
                this._Value = Math.Max(0, Math.Min(this.MaxValue, value));
            }
        }
        protected int _Value;
        protected int MaxValue;
        protected int OffsetValue;
        public ProgressbarComponent(Point position, int value, int maxValue)
        {
            this.MaxValue = maxValue;
            this.Value = value;
            this.OffsetValue = this.Value * Game1.pixelZoom;
            SetScaledArea(new Rectangle(position.X, position.Y, this.MaxValue + 2, 6));
        }
        protected int GetDiff()
        {
            int v = this._Value * Game1.pixelZoom;
            if (this.OffsetValue == v)
                return 0;
            if (this.OffsetValue > v)
                return -((int)Math.Floor((this.OffsetValue - v) / 10D + 1));
            return (int)Math.Floor((v - this.OffsetValue) / 10D + 1);
        }
        public override void Draw(SpriteBatch b, Point o)
        {
            if (DateTime.Now.Millisecond % 5 == 0)
                this.OffsetValue += GetDiff();
            IClickableMenu.drawTextureBox(b, Game1.mouseCursors, Background, this.Area.X + o.X, this.Area.Y + o.Y, this.Area.Width, this.Area.Height, Color.White, Game1.pixelZoom, false);
            b.Draw(Game1.mouseCursors, new Rectangle(this.Area.X + o.X + Game1.pixelZoom, this.Area.Y + o.Y + Game1.pixelZoom, this.OffsetValue, zoom4), Filler, Color.White);
        }
    }
}
