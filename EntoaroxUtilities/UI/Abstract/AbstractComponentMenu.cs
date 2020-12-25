using System;
using System.Collections;
using System.Collections.Generic;

using Entoarox.Utilities.UI.Interfaces;

namespace Entoarox.Utilities.UI.Abstract
{
    public abstract class AbstractComponentMenu : IComponentMenu
    {
        protected bool Dirty = false;
        protected Dictionary<string, IComponent> ComponentMap = new Dictionary<string, IComponent>();
        public IComponent this[string componentId] => this.ComponentMap[componentId];

        protected IInteractiveComponent _FocusComponent;
        public IInteractiveComponent FocusComponent
        {
            get
            {
                return this._FocusComponent;
            }

            set
            {
                if (this._FocusComponent != value)
                {
                    this._FocusComponent?.FocusLost();
                    this._FocusComponent = value;
                    this._FocusComponent.FocusGained();
                }
            }
        }

        public IComponentMenu Menu => this;

        public IComponentContainer Components => this;

        public void MarkDirty()
        {
            this.Dirty = true;
        }

        public void Add(IComponent component)
        {
            this.ComponentMap.Add(component.Id, component);
            component.Container = this;
            this.MarkDirty();
        }

        public bool Contains(IComponent component)
        {
            return this.ComponentMap.ContainsKey(component.Id) && this.ComponentMap[component.Id] == component;
        }

        public bool Contains(string componentId)
        {
            return this.ComponentMap.ContainsKey(componentId);
        }

        public IEnumerator<IComponent> GetEnumerator()
        {
            return this.ComponentMap.Values.GetEnumerator();
        }

        public void Remove(IComponent component)
        {
            if(this.Contains(component))
            {
                this.ComponentMap.Remove(component.Id);
                this.MarkDirty();
            }
        }

        public void Remove(string componentId)
        {
            if (this.ComponentMap.ContainsKey(componentId))
            {
                this.ComponentMap.Remove(componentId);
                this.MarkDirty();
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.ComponentMap.Values.GetEnumerator();
        }
    }
}
