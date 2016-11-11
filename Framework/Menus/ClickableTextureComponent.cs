using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Entoarox.Framework.Menus
{
    public class ClickableTextureComponent : BaseInteractiveMenuComponent
    {
        protected IClickHandler Handler;
        protected bool ScaleOnHover;
        public ClickableTextureComponent(Rectangle area, Texture2D texture, IClickHandler handler, Rectangle? crop = null, bool scaleOnHover = true) : base(area, texture, crop)
        {
            Handler = handler;
            ScaleOnHover = scaleOnHover;
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
    }
}
