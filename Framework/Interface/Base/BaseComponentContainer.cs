using Microsoft.Xna.Framework;

namespace Entoarox.Framework.Interface
{
    public abstract class BaseComponentContainer : BaseComponent, IComponentContainer
    {
        /*********
        ** Accessors
        *********/
        public InterfaceMenu Menu => this.Owner.Menu;
        public Rectangle InnerBounds => this.OuterBounds;
        public IDynamicComponent FocusComponent => null;


        /*********
        ** Public methods
        *********/
        public abstract bool HasFocus(IDynamicComponent component);
        public abstract bool TabBack();
        public abstract bool TabNext();


        /*********
        ** Protected methods
        *********/
        protected BaseComponentContainer(string name, Rectangle bounds, int layer)
            : base(name, bounds, layer) { }
    }
}
