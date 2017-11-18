using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewValley;

namespace Entoarox.Framework.UI
{
    class ColoredRectangleComponent : BaseMenuComponent
    {
        public Color Color;
        public ColoredRectangleComponent(Rectangle area, Color color)
        {
            SetScaledArea(area);
            this.Color = color;
        }
        public override void Draw(SpriteBatch b, Point o)
        {
            b.Draw(Game1.staminaRect, new Rectangle(this.Area.X + o.X, this.Area.Y + o.Y, this.Area.Width, this.Area.Height), this.Color);
        }
    }
}