using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Entoarox.Framework.Interface
{
    public class GenericComponentCollection : BaseComponentCollection
    {
        /*********
        ** Public methods
        *********/
        public GenericComponentCollection(string name, Rectangle bounds, int layer = 0)
            : base(name, bounds, layer) { }

        public GenericComponentCollection(string name, Rectangle bounds, IEnumerable<IComponent> components, int layer = 0)
            : base(name, bounds, layer)
        {
            foreach (IComponent component in components)
                this.AddComponent(component);
        }
    }
}
