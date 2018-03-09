using System;

using Microsoft.Xna.Framework;

namespace Entoarox.Framework.Interface
{
    public abstract class BaseClickableComponent : BaseDynamicComponent
    {
        public BaseClickableComponent(string name, Rectangle bounds, int layer) : base(name, bounds, layer)
        {
        }
        public event Action EventClicked;
        public override void LeftClick(Point offset, Point position)
        {
            EventClicked?.Invoke();
            base.LeftClick(offset, position);
        }
        public override void LeftHeld(Point offset, Point position)
        {
            EventClicked?.Invoke();
            base.LeftHeld(offset, position);
        }
    }
}
