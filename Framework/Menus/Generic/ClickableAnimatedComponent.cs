using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewValley;

namespace Entoarox.Framework.Menus
{
    public class ClickableAnimatedComponent : BaseInteractiveMenuComponent
    {
        protected TemporaryAnimatedSprite Sprite;
        protected bool ScaleOnHover;
        public event ClickHandler Handler;
        public ClickableAnimatedComponent(Rectangle area, TemporaryAnimatedSprite sprite, ClickHandler handler = null, bool scaleOnHover = true)
        {
            if (handler != null)
                Handler += handler;
            ScaleOnHover = scaleOnHover;
            Sprite = sprite;
            SetScaledArea(area);
        }
        public override void HoverIn(Point p, Point o, IComponentContainer c, FrameworkMenu m)
        {
            Area.X -= 2;
            Area.Y -= 2;
            Area.Width += 4;
            Area.Height += 4;
        }
        public override void HoverOut(Point p, Point o, IComponentContainer c, FrameworkMenu m)
        {
            Area.X += 2;
            Area.Y += 2;
            Area.Width -= 4;
            Area.Height -= 4;
        }
        public override void LeftClick(Point p, Point o, IComponentContainer c, FrameworkMenu m)
        {
            Handler?.Invoke(this, c, m, true);
        }
        public override void RightClick(Point p, Point o, IComponentContainer c, FrameworkMenu m)
        {
            Handler?.Invoke(this, c, m, false);
        }
        public override void Update(GameTime t, IComponentContainer c, FrameworkMenu m)
        {
            Sprite.update(t);
        }
        public override void Draw(SpriteBatch b, Point offset)
        {
            if (Visible)
                Sprite.draw(b, false, offset.X+Area.X, offset.Y+Area.Y);
        }
    }
}
