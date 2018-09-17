using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Entoarox.Framework.UI
{
    public abstract class BaseInteractiveMenuComponent : BaseMenuComponent, IInteractiveMenuComponent
    {
        /*********
        ** Accessors
        *********/
        public delegate void ValueChanged<T>(IInteractiveMenuComponent component, IComponentContainer collection, FrameworkMenu menu, T value);

        public delegate void ClickHandler(IInteractiveMenuComponent component, IComponentContainer collection, FrameworkMenu menu);


        /*********
        ** Public methods
        *********/
        public virtual bool InBounds(Point p, Point o)
        {
            return this.Visible && new Rectangle(this.Area.X + o.X, this.Area.Y + o.Y, this.Area.Width, this.Area.Height).Contains(p);
        }

        public virtual void RightClick(Point p, Point o) { }

        public virtual void LeftClick(Point p, Point o) { }

        public virtual void LeftHeld(Point p, Point o) { }

        public virtual void LeftUp(Point p, Point o) { }

        public virtual void HoverIn(Point p, Point o) { }

        public virtual void HoverOut(Point p, Point o) { }

        public virtual void HoverOver(Point p, Point o) { }

        public virtual void FocusLost() { }

        public virtual void FocusGained() { }

        public virtual bool Scroll(int d, Point p, Point o)
        {
            return false;
        }


        /*********
        ** Protected methods
        *********/
        protected BaseInteractiveMenuComponent() { }

        protected BaseInteractiveMenuComponent(Rectangle area, Texture2D texture, Rectangle? crop = null)
            : base(area, texture, crop) { }
    }
}
