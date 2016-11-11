using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewValley;

namespace Entoarox.Framework.Menus
{
    public class AnimatedComponent : BaseMenuComponent
    {
        protected TemporaryAnimatedSprite Sprite;
        public AnimatedComponent(Rectangle area, TemporaryAnimatedSprite sprite)
        {
            SetScaledArea(area);
            Sprite = sprite;
        }
        public override void Update(GameTime t, IComponentCollection c, FrameworkMenu m)
        {
            Sprite.update(t);
        }
        public override void Draw(SpriteBatch b, Point o)
        {
            if (Visible)
                Sprite.draw(b, false, o.X, o.Y);
        }
    }
}
