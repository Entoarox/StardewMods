using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Entoarox.Framework.Menus
{
    abstract public class BaseInteractiveMenuComponent : BaseMenuComponent, IInteractiveMenuComponent
    {
        protected BaseInteractiveMenuComponent()
        {

        }
        public BaseInteractiveMenuComponent(Rectangle area, Texture2D texture, Rectangle? crop = null) : base(area, texture, crop)
        {

        }
        public virtual bool InBounds(Point p, Point o)
        {
            Rectangle Offset = new Rectangle(Area.X + o.X, Area.Y + o.Y, Area.Width, Area.Height);
            return Offset.Contains(p);
        }
        public virtual void LeftClick(Point p, Point o, IComponentCollection c, FrameworkMenu m)
        {

        }
        public virtual void RightClick(Point p, Point o, IComponentCollection c, FrameworkMenu m)
        {

        }
        public virtual void LeftHeld(Point p, Point o, IComponentCollection c, FrameworkMenu m)
        {

        }
        public virtual void RightHeld(Point p, Point o, IComponentCollection c, FrameworkMenu m)
        {

        }
        public virtual void LeftUp(Point p, Point o, IComponentCollection c, FrameworkMenu m)
        {

        }
        public virtual void RightUp(Point p, Point o, IComponentCollection c, FrameworkMenu m)
        {

        }
        public virtual void HoverIn(Point p, Point o, IComponentCollection c, FrameworkMenu m)
        {

        }
        public virtual void HoverOut(Point p, Point o, IComponentCollection c, FrameworkMenu m)
        {

        }
        public virtual void HoverOver(Point p, Point o, IComponentCollection c, FrameworkMenu m)
        {

        }
        public virtual void FocusLost(IComponentCollection c, FrameworkMenu m)
        {

        }
        public virtual void Scroll(int d, Point p, Point o, IComponentCollection c, FrameworkMenu m)
        {

        }
    }
}
