using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Entoarox.Framework.Interface
{
    [Obsolete]
    public abstract class BaseComponentContainer : BaseComponent, IComponentContainer
    {
        protected BaseComponentContainer(string name, IComponent component, Rectangle bounds, int layer) : base(name, bounds, layer)
        {
            this.Component = component;
        }

        protected IComponent Component;

        public InterfaceMenu Menu => this.Owner.Menu;

        public Rectangle InnerBounds => this.OuterBounds;

        public bool HasFocus(IDynamicComponent component)
        {
            return false;
        }

        public bool TabBack()
        {
            return false;
        }

        public bool TabNext()
        {
            return false;
        }
    }
}
