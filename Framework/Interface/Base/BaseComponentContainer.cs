using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;



namespace Entoarox.Framework.Interface
{
    public abstract class BaseComponentContainer : BaseDynamicComponent, IComponentContainer
    {
        protected BaseComponentContainer(string name, Rectangle bounds, int layer) : base(name, bounds, layer)
        {
        }
        public InterfaceMenu Menu => this.Owner != null ? this.Owner.Menu : throw new NullReferenceException(Strings.ContainerNotAttached);
        public virtual Rectangle InnerBounds { get => this.OuterBounds; set => this.OuterBounds = value; }
        public abstract bool TabNext();
        public abstract bool TabBack();
        public abstract void TabAccess(TabType type);
        public abstract bool HasFocus(IComponent component);
    }
}
