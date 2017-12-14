using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Entoarox.Framework.Interface
{
    public class GenericComponentCollection : BaseDynamicComponent, IComponentCollection
    {
        public GenericComponentCollection(string name, Rectangle bounds, int layer=0) : base(name, bounds, layer)
        {

        }
        // IDynamicComponent
        public override void Draw(Point offset, SpriteBatch batch)
        {
            Point offset2 = new Point(this.OuterBounds.X + this.InnerBounds.X + offset.X, this.OuterBounds.Y + this.InnerBounds.Y + offset.Y);
            foreach (IComponent component in this._DrawComponents)
                if (component.Visible)
                    component.Draw(offset2, batch);
        }
        public override void FocusLost()
        {
            UpdateFocus(null);
        }
        public override void FocusGained()
        {
            UpdateFocus(this._Components.Where(a => a.Value is IDynamicComponent).Select(a => a.Value as IDynamicComponent).First());
        }
        public override void HoverIn(Point offset, Point position)
        {
            this.HoverOver(offset, position);
        }
        public override void HoverOver(Point offset, Point position)
        {
            Point offset2 = new Point(this.OuterBounds.X + this.InnerBounds.X + offset.X, this.OuterBounds.Y + this.InnerBounds.Y + offset.Y);
            if (GetTarget(offset2, position, out IDynamicComponent component))
            {
                if (component == this._HoverComponent)
                    component.HoverOver(offset2, position);
                else
                {
                    this._HoverComponent?.HoverOut(offset2, position);
                    this._HoverComponent = component;
                    component.HoverIn(offset2, position);
                }
            }
            else if (this._HoverComponent != null)
            {
                this._HoverComponent?.HoverOut(offset2, position);
                this._HoverComponent = null;
            }
        }
        public override void HoverOut(Point offset, Point position)
        {
            Point offset2 = new Point(this.OuterBounds.X + this.InnerBounds.X + offset.X, this.OuterBounds.Y + this.InnerBounds.Y + offset.Y);
            this._HoverComponent?.HoverOut(offset2, position);
            this._HoverComponent = null;
        }
        public override void LeftClick(Point offset, Point position)
        {
            Point offset2 = new Point(this.OuterBounds.X + this.InnerBounds.X + offset.X, this.OuterBounds.Y + this.InnerBounds.Y + offset.Y);
            if (GetTarget(offset2, position, out IDynamicComponent component))
            {
                UpdateFocus(component);
                component.LeftClick(offset2, position);
            }
            else
                UpdateFocus(null);
        }
        public override void LeftHeld(Point offset, Point position)
        {
            this._FocusComponent?.LeftHeld(new Point(this.OuterBounds.X + this.InnerBounds.X + offset.X, this.OuterBounds.Y + this.InnerBounds.Y + offset.Y), position);
        }
        public override void LeftUp(Point offset, Point position)
        {
            this._FocusComponent?.LeftUp(new Point(this.OuterBounds.X + this.InnerBounds.X + offset.X, this.OuterBounds.Y + this.InnerBounds.Y + offset.Y), position);
        }
        public override void RightClick(Point offset, Point position)
        {
            Point offset2 = new Point(this.OuterBounds.X + this.InnerBounds.X + offset.X, this.OuterBounds.Y + this.InnerBounds.Y + offset.Y);
            if (GetTarget(offset2, position, out IDynamicComponent component))
            {
                UpdateFocus(component);
                component.RightClick(offset2, position);
            }
            else
                UpdateFocus(null);
        }
        public override bool Scroll(Point offset, Point position, int amount)
        {
            Point offset2 = new Point(this.OuterBounds.X + this.InnerBounds.X + offset.X, this.OuterBounds.Y + this.InnerBounds.Y + offset.Y);
            if (GetTarget(offset2, position, out IDynamicComponent component))
                return component.Scroll(offset2, position, amount);
            return false;
        }
        public override void Update(GameTime time)
        {
            foreach (IComponent component in this._DrawComponents)
                if (component.Visible && (!(component is IDynamicComponent) || (component as IDynamicComponent).Enabled))
                    component.Update(time);
        }
        // IComponentCollection
        public IComponent this[string name] => this._Components.ContainsKey(name) ? this._Components[name] : throw new KeyNotFoundException(Strings.KeyNotFound);

        public InterfaceMenu Menu => this.Owner.Menu;

        public virtual Rectangle InnerBounds => this.OuterBounds;

        public bool AcceptsComponent<T>() where T : IComponent
        {
            return true;
        }

        public bool AcceptsComponent(IComponent component)
        {
            return true;
        }

        public void AddComponent(IComponent component)
        {
            if (component.IsAttached)
                throw new InvalidOperationException(Strings.ComponentAttached);
            if (this._Components.ContainsKey(component.Name))
                throw new ArgumentException(Strings.DuplicateKey);
            component.Attach(this);
            this._Components.Add(component.Name, component);
            UpdateSorting();
        }

        public void ClearComponents()
        {
            this._Components.Clear();
            this._DrawComponents.Clear();
            this._EventComponents.Clear();
        }

        public bool ContainsComponent(IComponent component)
        {
            return ContainsComponent(component.Name) && this._Components[component.Name] == component;
        }

        public bool ContainsComponent(string name)
        {
            return this._Components.ContainsKey(name);
        }

        public IEnumerator<IComponent> GetEnumerator()
        {
            foreach (var component in this._Components.Values)
                yield return component;
        }

        public bool HasFocus(IComponent component)
        {
            return this._FocusComponent == component;
        }

        public bool RemoveComponent(IComponent component)
        {
            if (!ContainsComponent(component))
                throw new KeyNotFoundException(Strings.KeyNotFound);
            return RemoveComponent(component.Name);
        }

        public bool RemoveComponent(string name)
        {
            if (!ContainsComponent(name))
                throw new KeyNotFoundException(Strings.KeyNotFound);
            bool result = this._Components.Remove(name);
            UpdateSorting();
            return result;
        }

        public void RemoveComponents<T>() where T : IComponent
        {
            RemoveComponents(a => a is T);
        }

        public void RemoveComponents(Predicate<IComponent> predicate)
        {
            foreach (var a in this._Components.Where(a => predicate(a.Value)))
                this._Components.Remove(a.Key);
            UpdateSorting();
        }
        public bool TabBack()
        {
            if (this._FocusComponent == null)
            {
                if (this._EventComponents.Count == 0)
                    return false;
                this._FocusComponent = this._EventComponents[0];
            }
            else if (!(this._FocusComponent is IComponentContainer) || !(this._FocusComponent as IComponentContainer).TabBack())
            {
                int index = this._EventComponents.IndexOf(this._FocusComponent) - 1;
                if (index < 0)
                {
                    this.UpdateFocus(null);
                    return false;
                }
                this._FocusComponent = this._EventComponents[index];
            }
            return true;
        }

        public bool TabNext()
        {
            if (this._FocusComponent == null)
            {
                if (this._EventComponents.Count == 0)
                    return false;
                this._FocusComponent = this._EventComponents[0];
            }
            else if (!(this._FocusComponent is IComponentContainer) || !(this._FocusComponent as IComponentContainer).TabNext())
            {
                int index = this._EventComponents.IndexOf(this._FocusComponent) + 1;
                if (index >= this._EventComponents.Count)
                {
                    this.UpdateFocus(null);
                    return false;
                }
                this._FocusComponent = this._EventComponents[index];
            }
            return true;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        // Internals
        protected Dictionary<string, IComponent> _Components = new Dictionary<string, IComponent>();
        protected List<IComponent> _DrawComponents = new List<IComponent>();
        protected List<IDynamicComponent> _EventComponents = new List<IDynamicComponent>();
        protected IDynamicComponent _FocusComponent;
        protected IDynamicComponent _HoverComponent;

        protected virtual void UpdateSorting()
        {
            var sorted = this._Components.Values.OrderByDescending(a => a is IDynamicComponent).OrderBy(a => a.Layer).OrderBy(a => a.OuterBounds.Y).OrderBy(a => a.OuterBounds.X);
            this._DrawComponents = sorted.ToList();
            this._DrawComponents.Reverse();
            this._EventComponents = sorted.Where(a => a is IDynamicComponent).Cast<IDynamicComponent>().ToList();
        }
        protected void UpdateFocus(IDynamicComponent component)
        {
            if (this._FocusComponent != component)
            {
                this._FocusComponent?.FocusLost();
                this._FocusComponent = component;
                component?.FocusGained();
            }
        }
        protected bool GetTarget(Point offset, Point position, out IDynamicComponent component)
        {
            component = null;
            foreach (IDynamicComponent comp in this._EventComponents)
                if (comp.Visible && comp.Enabled && comp.InBounds(offset, position))
                {
                    component = comp;
                    return true;
                }
            return false;
        }
    }
}
