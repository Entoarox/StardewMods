using System.Collections.Generic;

namespace Entoarox.Utilities.UI.Interfaces
{
    public interface IComponentContainer : IEnumerable<IComponent>
    {
        IComponentMenu Menu { get; }
        IComponentContainer Components { get; }

        IComponent this[string componentId] { get; }

        void Add(IComponent component);
        void Remove(IComponent component);
        bool Contains(IComponent component);
        void Remove(string componentId);
        bool Contains(string componentId);
        void MarkDirty();
    }
}
