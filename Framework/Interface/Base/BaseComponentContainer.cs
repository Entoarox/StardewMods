using Microsoft.Xna.Framework;

namespace Entoarox.Framework.Interface
{
    public abstract class BaseComponentContainer : BaseComponent, IComponentContainer
    {
        public BaseComponentContainer(string name, Rectangle bounds, int layer) : base(name, bounds, layer)
        {

        }
        public InterfaceMenu Menu => this.Owner.Menu;
        public Rectangle InnerBounds => this.OuterBounds;

        public IDynamicComponent FocusComponent => null;

        public abstract bool HasFocus(IDynamicComponent component);
        public abstract bool TabBack();
        public abstract bool TabNext();
    }
}
