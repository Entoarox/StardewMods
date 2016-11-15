using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewValley;

namespace Entoarox.Framework.Menus
{
    public class LabelComponent : BaseMenuComponent
    {
        protected static Rectangle Left = new Rectangle(256, 267, 6, 16);
        protected static Rectangle Right = new Rectangle(263, 267, 6, 16);
        protected static Rectangle Center = new Rectangle(262, 267, 1, 16);
        protected string Label;
        public LabelComponent(Point position, string label)
        {
            SetScaledArea(new Rectangle(position.X, position.Y, GetStringWidth(label, Game1.smallFont) + 12, 16));
            Label = label;
        }
        public override void Draw(SpriteBatch b, Point o)
        {
            // Left
            b.Draw(Game1.mouseCursors, new Rectangle(o.X + Area.X, o.Y + Area.Y, 6 * Game1.pixelZoom, Area.Height), Left, Color.White);
            // Right
            b.Draw(Game1.mouseCursors, new Rectangle(o.X + Area.X + Area.Width - 6 * Game1.pixelZoom, o.Y + Area.Y, 6 * Game1.pixelZoom, Area.Height), Right, Color.White);
            // Center
            b.Draw(Game1.mouseCursors, new Rectangle(o.X + Area.X + 6 * Game1.pixelZoom, o.Y + Area.Y, Area.Width - 12 * Game1.pixelZoom, Area.Height), Center, Color.White);
            // Label
            Utility.drawTextWithShadow(b, Label, Game1.smallFont, new Vector2(o.X + Area.X + Game1.pixelZoom * 6, o.Y + Area.Y + Game1.pixelZoom * 5), Game1.textColor);
        }
    }
}
