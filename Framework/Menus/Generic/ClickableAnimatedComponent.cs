using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewValley;

namespace Entoarox.Framework.Menus
{
    public class ClickableAnimatedComponent : BaseInteractiveMenuComponent
    {
        protected TemporaryAnimatedSprite Sprite;
        protected IClickHandler Handler;
        protected bool ScaleOnHover;
        public ClickableAnimatedComponent(Rectangle area, TemporaryAnimatedSprite sprite, IClickHandler handler, bool scaleOnHover = true)
        {
            Handler = handler;
            ScaleOnHover = scaleOnHover;
            Sprite = sprite;
            SetScaledArea(area);
        }
        public override void HoverIn(Point p, Point o, IComponentCollection c, FrameworkMenu m)
        {
            Area.X -= 2;
            Area.Y -= 2;
            Area.Width += 4;
            Area.Height += 4;
        }
        public override void HoverOut(Point p, Point o, IComponentCollection c, FrameworkMenu m)
        {
            Area.X += 2;
            Area.Y += 2;
            Area.Width -= 4;
            Area.Height -= 4;
        }
        public override void LeftClick(Point p, Point o, IComponentCollection c, FrameworkMenu m)
        {
            Handler.LeftClick(p, o, c, m);
        }
        public override void RightClick(Point p, Point o, IComponentCollection c, FrameworkMenu m)
        {
            Handler.RightClick(p, o, c, m);
        }
        public override void Update(GameTime t, IComponentCollection c, FrameworkMenu m)
        {
            Sprite.update(t);
        }
        public override void Draw(SpriteBatch b, Point offset)
        {
            if (Visible)
                Sprite.draw(b, false, offset.X, offset.Y);
        }
    }
}
