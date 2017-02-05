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
            Color = color;
        }
        public override void Draw(SpriteBatch b, Point o)
        {
            b.Draw(Game1.staminaRect, new Rectangle(Area.X + o.X, Area.Y + o.Y, Area.Width, Area.Height), Color);
        }
    }
}