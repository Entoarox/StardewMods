using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace Entoarox.Framework.Interface
{
    public class GradientComponent : BaseComponent
    {
        /*********
        ** Fields
        *********/
        private readonly Color FromColor;
        private readonly Color ToColor;
        private readonly bool Vertical;


        /*********
        ** Public methods
        *********/
        public GradientComponent(string name, Rectangle bounds, Color fromColor, Color toColor, bool vertical = false, int layer = 0)
            : base(name, bounds, layer)
        {
            this.FromColor = fromColor;
            this.ToColor = toColor;
            this.Vertical = vertical;
        }

        public override void Draw(Point offset, SpriteBatch batch)
        {
            Rectangle rect = Utilities.GetDrawRectangle(offset, this.OuterBounds);
            batch.Draw(Game1.staminaRect, rect, this.FromColor);
            batch.Draw(this.Vertical ? Utilities.GradientTextureVertical : Utilities.GradientTextureHorizontal, rect, this.ToColor);
        }
    }
}
