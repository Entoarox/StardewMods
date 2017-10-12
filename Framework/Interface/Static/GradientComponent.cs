using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Entoarox.Framework.Interface
{
    public class GradientComponent : BaseComponent
    {
        protected Color FromColor;
        protected Color ToColor;
        protected bool Vertical;
        public GradientComponent(string name, Rectangle bounds, Color fromColor, Color toColor, bool vertical=false, int layer=0) : base(name, bounds, layer)
        {
            FromColor = fromColor;
            ToColor = toColor;
            Vertical = vertical;
        }
        public override void Draw(Point offset, SpriteBatch batch)
        {
            Rectangle rect = GetDrawRectangle(offset, OuterBounds);
            batch.Draw(StardewValley.Game1.staminaRect, rect, FromColor);
            batch.Draw(Vertical ? Cache.GradientTextureVertical : Cache.GradientTextureHorizontal, rect, ToColor);
        }
    }
}