using System;
using Microsoft.Xna.Framework;

namespace Entoarox.Framework.Interface
{
    public abstract class BaseClickableComponent : BaseDynamicComponent
    {
        /*********
        ** Accessors
        *********/
        public event Action EventClicked;


        /*********
        ** Public methods
        *********/
        public override void LeftClick(Point offset, Point position)
        {
            this.EventClicked?.Invoke();
            base.LeftClick(offset, position);
        }

        public override void LeftHeld(Point offset, Point position)
        {
            this.EventClicked?.Invoke();
            base.LeftHeld(offset, position);
        }


        /*********
        ** Protected methods
        *********/
        protected BaseClickableComponent(string name, Rectangle bounds, int layer)
            : base(name, bounds, layer) { }
    }
}
