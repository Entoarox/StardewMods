using System;

namespace Entoarox.Framework.Menus
{
    public interface IComponentCollection
    {
        bool AcceptsComponent(IMenuComponent component);
        void AddComponent(IMenuComponent component);
        void RemoveComponent(IMenuComponent component);
        void RemoveComponents<T>() where T : IMenuComponent;
        void RemoveComponents(Predicate<IMenuComponent> filter);
        void ClearComponents();
    }
}
