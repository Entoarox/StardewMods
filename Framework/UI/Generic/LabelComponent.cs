using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewValley;

namespace Entoarox.Framework.UI
{
    public class LabelComponent : BaseMenuComponent
    {
        protected readonly static Rectangle Left = new Rectangle(256, 267, 6, 16);
        protected readonly static Rectangle Right = new Rectangle(263, 267, 6, 16);
        protected readonly static Rectangle Center = new Rectangle(262, 267, 1, 16);
        protected string _Label;
        public string Label
        {
            get
            {
                return this._Label;
            }
            set
            {
                this._Label = value;
                this.Area.Width = (GetStringWidth(value, Game1.smallFont) + 12) * Game1.pixelZoom;
            }
        }
        public LabelComponent(Point position, string label)
        {
            SetScaledArea(new Rectangle(position.X, position.Y, GetStringWidth(label, Game1.smallFont) + 12, 16));
            this._Label = label;
        }
        public override void Draw(SpriteBatch b, Point o)
        {
            // Left
            b.Draw(Game1.mouseCursors, new Rectangle(o.X + this.Area.X, o.Y + this.Area.Y, zoom6, this.Area.Height), Left, Color.White);
            // Right
            b.Draw(Game1.mouseCursors, new Rectangle(o.X + this.Area.X + this.Area.Width - zoom6, o.Y + this.Area.Y, zoom6, this.Area.Height), Right, Color.White);
            // Center
            b.Draw(Game1.mouseCursors, new Rectangle(o.X + this.Area.X + zoom6, o.Y + this.Area.Y, this.Area.Width - zoom12, this.Area.Height), Center, Color.White);
            // Label
            Utility.drawTextWithShadow(b, this.Label, Game1.smallFont, new Vector2(o.X + this.Area.X + zoom6, o.Y + this.Area.Y + zoom5), Game1.textColor);
        }
    }
}
