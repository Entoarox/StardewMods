using System;
using System.Collections.Generic;

namespace Entoarox.Framework.UI
{
    public interface IComponentCollection : IComponentContainer
    {
        /*********
        ** Accessors
        *********/
        List<IInteractiveMenuComponent> InteractiveComponents { get; }
        List<IMenuComponent> StaticComponents { get; }


        /*********
        ** Methods
        *********/
        bool AcceptsComponent(IMenuComponent component);
        void AddComponent(IMenuComponent component);
        void RemoveComponent(IMenuComponent component);
        void RemoveComponents<T>() where T : IMenuComponent;
        void RemoveComponents(Predicate<IMenuComponent> filter);
        void ClearComponents();
    }
}
