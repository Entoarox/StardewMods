using Entoarox.Utilities.UI.Interfaces;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Entoarox.Utilities.UI.Abstract
{
    public abstract class AbstractInteractiveComponent : AbstractComponent, IInteractiveComponent
    {
        public virtual bool Enabled { get; set; }
        public virtual Rectangle FocusRegion => this.DisplayRegion;

        public virtual IInteractiveComponent NextUp { get; set; }
        public virtual IInteractiveComponent NextDown { get; set; }
        public virtual IInteractiveComponent NextLeft { get; set; }
        public virtual IInteractiveComponent NextRight { get; set; }

        public virtual void Draw(Rectangle screenRect, Rectangle drawRect, SpriteBatch batch)
        {
            this.Draw(drawRect, batch);
        }

        public virtual void FocusGained()
        {
        }
        public virtual void FocusLost()
        {
        }

        public virtual void KeyDown(Keys key)
        {
        }
        public virtual void KeyHeld(Keys key)
        {
        }
        public virtual void KeyUp(Keys key)
        {
        }

        public virtual void LeftDown(Point position)
        {
        }
        public virtual void LeftHeld(Point position)
        {
        }
        public virtual void LeftUp(Point position)
        {
        }

        public virtual void MouseIn(Point position)
        {
        }
        public virtual void MouseMove(Point position)
        {
        }
        public virtual void MouseOut(Point position)
        {
        }

        public virtual void RightDown(Point position)
        {
        }
        public virtual void RightHeld(Point position)
        {
        }
        public virtual void RightUp(Point position)
        {
        }

        public int Scroll(int scrollAmount)
        {
            return scrollAmount;
        }
    }
}
