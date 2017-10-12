using System;
using System.Collections.Generic;

namespace Entoarox.Framework.Interface
{
    public interface IComponentCollection : IEnumerable<IComponent>, IComponentContainer
    {
        /// <summary>
        /// If a certain type of component is accepted by this collection
        /// This should be tried before adding a component, or there is a risk of a exception being thrown
        /// </summary>
        /// <typeparam name="T">The component type to check for acceptance</typeparam>
        /// <returns>If the component is accepted</returns>
        bool AcceptsComponent<T>() where T : IComponent;
        /// <summary>
        /// If a certain component is accepted by this collection
        /// This should be tried before adding a component, or there is a risk of a exception being thrown
        /// </summary>
        /// <tparam name="component">The component to check for acceptance</param>
        /// <returns>If the component is accepted</returns>
        bool AcceptsComponent(IComponent component);
        /// <summary>
        /// Tries to attach the given component to this collection
        /// </summary>
        /// <exception cref="InvalidOperationException">Component is already attached to a collection</exception>
        /// <exception cref="ArgumentException">Component is not accepted by this collection</exception>
        /// <exception cref="ArgumentException">Another component with the same name is already attached to this collection</exception>
        /// <param name="component">The component to attempt to attach</param>
        void AddComponent(IComponent component);
        /// <summary>
        /// Tries to remove the given component from this collection
        /// </summary>
        /// <remarks>
        /// This method should cause a Resort to happen for all components in the collection
        /// </remarks>
        /// <exception cref="ArgumentException">Component is not attached to a collection</exception>
        /// <exception cref="KeyNotFoundException">Component is not attached to this collection</exception>
        /// <param name="component">The component to remove</param>
        /// <returns>If removal was successfull</returns>
        bool RemoveComponent(IComponent component);
        /// <summary>
        /// Tries to remove the component with the given name from this collection
        /// </summary>
        /// <remarks>
        /// This method should cause a Resort to happen for all components in the collection
        /// </remarks>
        /// <exception cref="KeyNotFoundException">Component is not attached to this collection</exception>
        /// <param name="component">The component to remove</param>
        /// <returns>If removal was successfull</returns>
        bool RemoveComponent(string name);
        /// <summary>
        /// Checks if the given component is part of this collection
        /// </summary>
        /// <remarks>
        /// This method should cause a Resort to happen for all components in the collection
        /// </remarks>
        /// <param name="component">The component to check for</param>
        /// <returns>The result of the check</returns>
        bool ContainsComponent(IComponent component);
        /// <summary>
        /// Checks if a component with the given name is part of this collection
        /// </summary>
        /// <param name="name">The name to check for</param>
        /// <returns>The result of the check</returns>
        bool ContainsComponent(string name);
        /// <summary>
        /// Removes all components of the given type
        /// </summary>
        /// <remarks>
        /// This method should cause a Resort to happen for all components in the collection
        /// </remarks>
        /// <typeparam name="T">The type of components to remove from the collection</typeparam>
        void RemoveComponents<T>() where T : IComponent;
        /// <summary>
        /// Removes all components that match the given predicate
        /// </summary>
        /// <remarks>
        /// This method should cause a Resort to happen for all components in the collection
        /// </remarks>
        /// <param name="predicate">The predicate to use for removing components</param>
        void RemoveComponents(Predicate<IComponent> predicate);
        /// <summary>
        /// Removes all components from the collection
        /// </summary>
        /// <remarks>
        /// This method should cause a Resort to happen for all components in the collection
        /// </remarks>
        void ClearComponents();
        /// <summary>
        /// Enables access to a named component in the collection
        /// </summary>
        /// <param name="name">The name of the component to access</param>
        /// <exception cref="KeyNotFoundException">Component is not attached to this collection</exception>
        /// <returns>A reference to the accessed component</returns>
        IComponent this[string name] { get; }
    }
}
