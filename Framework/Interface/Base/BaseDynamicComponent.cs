using Microsoft.Xna.Framework;

namespace Entoarox.Framework.Interface
{
    public abstract class BaseDynamicComponent : BaseComponent, IDynamicComponent
    {
        protected BaseDynamicComponent(string name, Rectangle bounds, int layer) : base(name, bounds, layer)
        {
        }
        public bool Enabled { get; set; }

        public virtual string Tooltip { get; set; }

        public virtual bool InBounds(Point offset, Point position) => new Rectangle(OuterBounds.X + offset.X, OuterBounds.Y + offset.Y, OuterBounds.Width, OuterBounds.Height).Contains(position);

        public virtual void FocusGained()
        {

        }
        public virtual void FocusLost()
        {

        }

        public virtual void HoverIn(Point offset, Point position)
        {

        }
        public virtual void HoverOut(Point offset, Point position)
        {

        }
        public virtual void HoverOver(Point offset, Point position)
        {

        }

        public virtual void LeftClick(Point offset, Point position)
        {

        }
        public virtual void LeftHeld(Point offset, Point position)
        {

        }
        public virtual void LeftUp(Point offset, Point position)
        {

        }
        public virtual void RightClick(Point offset, Point position)
        {
            
        }
        public virtual bool Scroll(Point offset, Point position, int amount) => false;
    }
}
