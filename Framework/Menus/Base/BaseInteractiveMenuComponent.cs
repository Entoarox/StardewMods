using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Entoarox.Framework.Menus
{
    abstract public class BaseInteractiveMenuComponent : BaseMenuComponent, IInteractiveMenuComponent
    {
        public delegate void ValueChanged<T>(IInteractiveMenuComponent component, IComponentCollection collection, FrameworkMenu menu, T value);
        public delegate void ClickHandler(IInteractiveMenuComponent component, IComponentCollection collection, FrameworkMenu menu, bool leftClicked);
        protected BaseInteractiveMenuComponent()
        {

        }
        public BaseInteractiveMenuComponent(Rectangle area, Texture2D texture, Rectangle? crop = null) : base(area, texture, crop)
        {

        }
        public virtual bool InBounds(Point p, Point o)
        {
            if (!Visible)
                return false;
            Rectangle Offset = new Rectangle(Area.X + o.X, Area.Y + o.Y, Area.Width, Area.Height);
            return Offset.Contains(p);
        }
        public virtual void RightClick(Point p, Point o, IComponentCollection c, FrameworkMenu m)
        {

        }
        public virtual void LeftClick(Point p, Point o, IComponentCollection c, FrameworkMenu m)
        {

        }
        public virtual void LeftHeld(Point p, Point o, IComponentCollection c, FrameworkMenu m)
        {

        }
        public virtual void LeftUp(Point p, Point o, IComponentCollection c, FrameworkMenu m)
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
        public virtual void FocusGained(IComponentCollection c, FrameworkMenu m)
        {

        }
        public virtual void Scroll(int d, Point p, Point o, IComponentCollection c, FrameworkMenu m)
        {

        }
    }
}
