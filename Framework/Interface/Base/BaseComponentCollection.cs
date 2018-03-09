using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Entoarox.Framework.Interface
{
    public abstract class BaseComponentCollection : BaseDynamicComponent, IComponentCollection
    {
        protected BaseComponentCollection(string name, Rectangle bounds, int layer) : base(name, bounds, layer)
        {
        }
        protected Dictionary<string, IComponent> _Components = new Dictionary<string, IComponent>();
        protected List<IComponent> _DrawComponents = new List<IComponent>();
        protected List<IDynamicComponent> _EventComponents = new List<IDynamicComponent>();
        protected IDynamicComponent _FocusComponent;
        protected IDynamicComponent _HoverComponent;

        protected Point GetMyOffset(Point offset)
        {
            return new Point(offset.X + this.InnerBounds.X, offset.Y + this.InnerBounds.Y);
        }

        protected virtual void UpdateSorting()
        {
            var sorted = this._Components.Values.OrderByDescending(a => a is IDynamicComponent).OrderBy(a => a.Layer).OrderBy(a => a.OuterBounds.Y).OrderBy(a => a.OuterBounds.X);
            this._DrawComponents = sorted.ToList();
            this._DrawComponents.Reverse();
            this._EventComponents = sorted.Where(a => a is IDynamicComponent).Cast<IDynamicComponent>().ToList();
        }
        protected virtual void UpdateFocus(IDynamicComponent component)
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

        public override void FocusLost()
        {
            UpdateFocus(null);
        }

        public override void HoverIn(Point offset, Point position)
        {
            HoverOver(offset, position);
        }
        public override void HoverOut(Point offset, Point position)
        {
            if (this._HoverComponent != null)
            {
                this._HoverComponent?.HoverOut(GetMyOffset(offset), position);
                this._HoverComponent = null;
            }
        }
        public override void HoverOver(Point offset, Point position)
        {
            var myOffset = GetMyOffset(offset);
            if (GetTarget(myOffset, position, out IDynamicComponent component))
            {
                if (component == this._HoverComponent)
                    component.HoverOver(myOffset, position);
                else
                {
                    this._HoverComponent?.HoverOut(myOffset, position);
                    this._HoverComponent = component;
                    component.HoverIn(myOffset, position);
                }
            }
            else if (this._HoverComponent != null)
            {
                this._HoverComponent?.HoverOut(myOffset, position);
                this._HoverComponent = null;
            }
        }
        public override void LeftClick(Point offset, Point position)
        {
            var myOffset = GetMyOffset(offset);
            GetTarget(myOffset, position, out IDynamicComponent component);
            UpdateFocus(component);
            component?.LeftClick(myOffset, position);
        }
        public override void LeftHeld(Point offset, Point position)
        {
            this._FocusComponent?.LeftHeld(GetMyOffset(offset), position);
        }

        public override void LeftUp(Point offset, Point position)
        {
            this._FocusComponent?.LeftUp(GetMyOffset(offset), position);
        }

        public override void RightClick(Point offset, Point position)
        {
            var myOffset = GetMyOffset(offset);
            GetTarget(myOffset, position, out IDynamicComponent component);
            UpdateFocus(component);
            component?.RightClick(myOffset, position);
        }
        public override bool Scroll(Point offset, Point position, int amount)
        {
            var myOffset = GetMyOffset(offset);
            if (GetTarget(myOffset, position, out IDynamicComponent component))
                return component.Scroll(myOffset, position, amount);
            return false;
        }

        public override void Update(GameTime time)
        {
            foreach (IComponent component in this._DrawComponents)
                component.Update(time);
        }
        public override void Draw(Point offset, SpriteBatch batch)
        {
            Point myOffset = GetMyOffset(offset);
            foreach (IComponent component in this._DrawComponents)
                if (component.Visible)
                    component.Draw(myOffset, batch);
        }
        public virtual void ReceiveInput(char input)
        {
            (this._FocusComponent as IInputComponent)?.ReceiveInput(input);
        }

        public virtual bool ReceiveHotkey(Microsoft.Xna.Framework.Input.Keys key)
        {
            return (this._FocusComponent as IHotkeyComponent)?.ReceiveHotkey(key) ?? false;
        }

        public override string Tooltip => this._HoverComponent?.Tooltip;

        public virtual IDynamicComponent FloatComponent => (this._FocusComponent as IFloatComponent)?.FloatComponent;


        public InterfaceMenu Menu => this.Owner.Menu;

        public virtual Rectangle InnerBounds => this.OuterBounds;

        public virtual bool AcceptsComponent<T>() where T : IComponent
        {
            return true;
        }

        public virtual bool AcceptsComponent(IComponent component)
        {
            return true;
        }

        public virtual void AddComponent(IComponent component)
        {
            if (component.IsAttached)
                throw new InvalidOperationException(Strings.ComponentAttached);
            if (this._Components.ContainsKey(component.Name))
                throw new ArgumentException(Strings.DuplicateKey);
            if (!AcceptsComponent(component))
                throw new ArgumentException(Strings.NotAccepted);
            component.Attach(this);
            this._Components.Add(component.Name, component);
            UpdateSorting();
        }

        public virtual bool RemoveComponent(IComponent component)
        {
            return RemoveComponent(component.Name);
        }

        public virtual bool RemoveComponent(string name)
        {
            if (!this._Components.ContainsKey(name))
                throw new KeyNotFoundException(Strings.KeyNotFound);
            bool result = this._Components.Remove(name);
            UpdateSorting();
            return result;
        }
        public virtual bool ContainsComponent(IComponent component)
        {
            return ContainsComponent(component.Name);
        }

        public virtual bool ContainsComponent(string name)
        {
            return this._Components.ContainsKey(name);
        }

        public virtual void RemoveComponents<T>() where T : IComponent
        {
            RemoveComponents(a => a is T);
        }

        public virtual void RemoveComponents(Predicate<IComponent> predicate)
        {
            foreach (var a in this._Components.Where(a => predicate(a.Value)))
                this._Components.Remove(a.Key);
            UpdateSorting();
        }

        public virtual void ClearComponents()
        {
            this._Components.Clear();
            this._DrawComponents.Clear();
            this._EventComponents.Clear();
        }
        public bool Selected
        {
            get => this._FocusComponent != null && this._FocusComponent is IInputComponent && (this._FocusComponent as IInputComponent).Selected;
            set
            {
                if (this._FocusComponent != null && this._FocusComponent is IInputComponent)
                    (this._FocusComponent as IInputComponent).Selected = value;
            }
        }
        public virtual bool TabNext()
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
                    return false;
                this.UpdateFocus(this._EventComponents[index]);
            }
            return true;
        }
        public virtual bool TabBack()
        {
            if (this._FocusComponent == null)
            {
                if (this._EventComponents.Count == 0)
                    return false;
                this._FocusComponent = this._EventComponents[this._EventComponents.Count - 1];
            }
            else if (!(this._FocusComponent is IComponentContainer) || !(this._FocusComponent as IComponentContainer).TabNext())
            {
                int index = this._EventComponents.IndexOf(this._FocusComponent) - 1;
                if (index < 0)
                    return false;
                this.UpdateFocus(this._EventComponents[index]);
            }
            return true;
        }
        public virtual bool HasFocus(IDynamicComponent component)
        {
            return this._FocusComponent == component;
        }
        public virtual IDynamicComponent FocusComponent => this._FocusComponent;

        public IComponent this[string name] { get => this._Components.ContainsKey(name) ? this._Components[name] : throw new KeyNotFoundException(Strings.KeyNotFound); }

        public IEnumerator<IComponent> GetEnumerator()
        {
            foreach (var component in this._Components.Values)
                yield return component;
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
