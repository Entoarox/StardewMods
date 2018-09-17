using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Entoarox.Framework.Interface
{
    public abstract class BaseComponentCollection : BaseDynamicComponent, IComponentCollection
    {
        /*********
        ** Fields
        *********/
        protected Dictionary<string, IComponent> Components = new Dictionary<string, IComponent>();
        protected List<IComponent> DrawComponents = new List<IComponent>();
        protected List<IDynamicComponent> EventComponents = new List<IDynamicComponent>();
        protected IDynamicComponent _FocusComponent;
        protected IDynamicComponent HoverComponent;


        /*********
        ** Accessors
        *********/
        public override string Tooltip => this.HoverComponent?.Tooltip;
        public virtual IDynamicComponent FloatComponent => (this._FocusComponent as IFloatComponent)?.FloatComponent;
        public InterfaceMenu Menu => this.Owner.Menu;
        public virtual Rectangle InnerBounds => this.OuterBounds;

        public bool Selected
        {
            get => this._FocusComponent is IInputComponent component && component.Selected;
            set
            {
                if (this._FocusComponent is IInputComponent component)
                    component.Selected = value;
            }
        }

        public virtual IDynamicComponent FocusComponent => this._FocusComponent;
        public IComponent this[string name] => this.Components.ContainsKey(name) ? this.Components[name] : throw new KeyNotFoundException(Strings.KeyNotFound);


        /*********
        ** Public methods
        *********/
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public override void FocusLost()
        {
            this.UpdateFocus(null);
        }

        public override void HoverIn(Point offset, Point position)
        {
            this.HoverOver(offset, position);
        }

        public override void HoverOut(Point offset, Point position)
        {
            if (this.HoverComponent != null)
            {
                this.HoverComponent?.HoverOut(this.GetMyOffset(offset), position);
                this.HoverComponent = null;
            }
        }

        public override void HoverOver(Point offset, Point position)
        {
            Point myOffset = this.GetMyOffset(offset);
            if (this.GetTarget(myOffset, position, out IDynamicComponent component))
            {
                if (component == this.HoverComponent)
                    component.HoverOver(myOffset, position);
                else
                {
                    this.HoverComponent?.HoverOut(myOffset, position);
                    this.HoverComponent = component;
                    component.HoverIn(myOffset, position);
                }
            }
            else if (this.HoverComponent != null)
            {
                this.HoverComponent?.HoverOut(myOffset, position);
                this.HoverComponent = null;
            }
        }

        public override void LeftClick(Point offset, Point position)
        {
            Point myOffset = this.GetMyOffset(offset);
            this.GetTarget(myOffset, position, out IDynamicComponent component);
            this.UpdateFocus(component);
            component?.LeftClick(myOffset, position);
        }

        public override void LeftHeld(Point offset, Point position)
        {
            this._FocusComponent?.LeftHeld(this.GetMyOffset(offset), position);
        }

        public override void LeftUp(Point offset, Point position)
        {
            this._FocusComponent?.LeftUp(this.GetMyOffset(offset), position);
        }

        public override void RightClick(Point offset, Point position)
        {
            Point myOffset = this.GetMyOffset(offset);
            this.GetTarget(myOffset, position, out IDynamicComponent component);
            this.UpdateFocus(component);
            component?.RightClick(myOffset, position);
        }

        public override bool Scroll(Point offset, Point position, int amount)
        {
            Point myOffset = this.GetMyOffset(offset);
            if (this.GetTarget(myOffset, position, out IDynamicComponent component))
                return component.Scroll(myOffset, position, amount);
            return false;
        }

        public override void Update(GameTime time)
        {
            foreach (IComponent component in this.DrawComponents)
                component.Update(time);
        }

        public override void Draw(Point offset, SpriteBatch batch)
        {
            Point myOffset = this.GetMyOffset(offset);
            foreach (IComponent component in this.DrawComponents)
                if (component.Visible)
                    component.Draw(myOffset, batch);
        }

        public virtual void ReceiveInput(char input)
        {
            (this._FocusComponent as IInputComponent)?.ReceiveInput(input);
        }

        public virtual bool ReceiveHotkey(Keys key)
        {
            return (this._FocusComponent as IHotkeyComponent)?.ReceiveHotkey(key) ?? false;
        }

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
            if (this.Components.ContainsKey(component.Name))
                throw new ArgumentException(Strings.DuplicateKey);
            if (!this.AcceptsComponent(component))
                throw new ArgumentException(Strings.NotAccepted);
            component.Attach(this);
            this.Components.Add(component.Name, component);
            this.UpdateSorting();
        }

        public virtual bool RemoveComponent(IComponent component)
        {
            return this.RemoveComponent(component.Name);
        }

        public virtual bool RemoveComponent(string name)
        {
            if (!this.Components.ContainsKey(name))
                throw new KeyNotFoundException(Strings.KeyNotFound);
            bool result = this.Components.Remove(name);
            this.UpdateSorting();
            return result;
        }

        public virtual bool ContainsComponent(IComponent component)
        {
            return this.ContainsComponent(component.Name);
        }

        public virtual bool ContainsComponent(string name)
        {
            return this.Components.ContainsKey(name);
        }

        public virtual void RemoveComponents<T>() where T : IComponent
        {
            this.RemoveComponents(a => a is T);
        }

        public virtual void RemoveComponents(Predicate<IComponent> predicate)
        {
            foreach (KeyValuePair<string, IComponent> a in this.Components.Where(a => predicate(a.Value)))
                this.Components.Remove(a.Key);
            this.UpdateSorting();
        }

        public virtual void ClearComponents()
        {
            this.Components.Clear();
            this.DrawComponents.Clear();
            this.EventComponents.Clear();
        }

        public virtual bool TabNext()
        {
            if (this._FocusComponent == null)
            {
                if (this.EventComponents.Count == 0)
                    return false;
                this._FocusComponent = this.EventComponents[0];
            }
            else if (!(this._FocusComponent is IComponentContainer) || !(this._FocusComponent as IComponentContainer).TabNext())
            {
                int index = this.EventComponents.IndexOf(this._FocusComponent) + 1;
                if (index >= this.EventComponents.Count)
                    return false;
                this.UpdateFocus(this.EventComponents[index]);
            }

            return true;
        }

        public virtual bool TabBack()
        {
            if (this._FocusComponent == null)
            {
                if (this.EventComponents.Count == 0)
                    return false;
                this._FocusComponent = this.EventComponents[this.EventComponents.Count - 1];
            }
            else if (!(this._FocusComponent is IComponentContainer) || !(this._FocusComponent as IComponentContainer).TabNext())
            {
                int index = this.EventComponents.IndexOf(this._FocusComponent) - 1;
                if (index < 0)
                    return false;
                this.UpdateFocus(this.EventComponents[index]);
            }

            return true;
        }

        public virtual bool HasFocus(IDynamicComponent component)
        {
            return this._FocusComponent == component;
        }

        public IEnumerator<IComponent> GetEnumerator()
        {
            foreach (IComponent component in this.Components.Values)
                yield return component;
        }


        /*********
        ** Protected methods
        *********/
        protected BaseComponentCollection(string name, Rectangle bounds, int layer)
            : base(name, bounds, layer) { }

        protected Point GetMyOffset(Point offset)
        {
            return new Point(offset.X + this.InnerBounds.X, offset.Y + this.InnerBounds.Y);
        }

        protected virtual void UpdateSorting()
        {
            IOrderedEnumerable<IComponent> sorted = this.Components.Values.OrderByDescending(a => a is IDynamicComponent).OrderBy(a => a.Layer).OrderBy(a => a.OuterBounds.Y).OrderBy(a => a.OuterBounds.X);
            this.DrawComponents = sorted.ToList();
            this.DrawComponents.Reverse();
            this.EventComponents = sorted.Where(a => a is IDynamicComponent).Cast<IDynamicComponent>().ToList();
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
            foreach (IDynamicComponent comp in this.EventComponents)
            {
                if (comp.Visible && comp.Enabled && comp.InBounds(offset, position))
                {
                    component = comp;
                    return true;
                }
            }

            return false;
        }
    }
}
