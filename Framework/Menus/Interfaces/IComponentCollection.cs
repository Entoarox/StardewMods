using System;
using System.Collections.Generic;

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
        void ResetFocus();
        void GiveFocus(IInteractiveMenuComponent component);
        List<IInteractiveMenuComponent> InteractiveComponents { get; }
        List<IMenuComponent> StaticComponents { get; }
    }
}
