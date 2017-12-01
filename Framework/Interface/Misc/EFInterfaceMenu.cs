using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using StardewValley;
using StardewValley.Menus;

namespace Entoarox.Framework.Interface
{
    public class InterfaceMenu : IClickableMenu, IComponentCollection
    {
        [Obsolete("The Interface framework has not yet been completed and should not be used!", true)]
        public InterfaceMenu()
        {

        }

        public bool AcceptsComponent<T>() where T : IComponent => true;
        public bool AcceptsComponent(IComponent component) => true;

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

        public bool RemoveComponent(IComponent component) => RemoveComponent(component.Name);
        public bool RemoveComponent(string name)
        {
            if (!this._Components.ContainsKey(name))
                throw new KeyNotFoundException(Strings.KeyNotFound);
            bool result = this._Components.Remove(name);
            UpdateSorting();
            return result;
        }
        public bool ContainsComponent(IComponent component) => ContainsComponent(component.Name);
        public bool ContainsComponent(string name) => this._Components.ContainsKey(name);

        public void RemoveComponents<T>() where T : IComponent => RemoveComponents(a => a is T);
        public void RemoveComponents(Predicate<IComponent> predicate)
        {
            foreach (var a in this._Components.Where(a => predicate(a.Value)))
                this._Components.Remove(a.Key);
            UpdateSorting();
        }

        public void ClearComponents()
        {
            this._Components.Clear();
            this._DrawComponents.Clear();
            this._EventComponents.Clear();
        }

        public InterfaceMenu Menu => this;
        public Rectangle OuterBounds { get; set; }
        public Rectangle InnerBounds { get; set; }

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
                    index = 0;
                this._FocusComponent = this._EventComponents[index];
            }
            return true;
        }
        public void TabAccess(TabType type)
        {
            //TODO: Implement proper controller mapping for the menu, since currently there is no mapping
            throw new NotImplementedException(Strings.InvalidClassMethod);
        }
        public bool HasFocus(IComponent component) => this._FocusComponent == component;
        public IComponent this[string name] { get => _Components.ContainsKey(name) ? _Components[name] : throw new KeyNotFoundException(Strings.KeyNotFound); }

        public IEnumerator<IComponent> GetEnumerator()
        {
            foreach (var component in this._Components.Values)
                yield return component;
        }
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        // Internals
        private Dictionary<string, IComponent> _Components = new Dictionary<string, IComponent>();
        private List<IComponent> _DrawComponents = new List<IComponent>();
        private List<IDynamicComponent> _EventComponents = new List<IDynamicComponent>();
        private IDynamicComponent _FocusComponent;
        private IDynamicComponent _HoverComponent;

        private void UpdateSorting()
        {
            var sorted = this._Components.Values.OrderByDescending(a => a is IDynamicComponent).OrderBy(a => a.Layer).OrderBy(a => a.OuterBounds.Y).OrderBy(a => a.OuterBounds.X);
            this._DrawComponents = sorted.ToList();
            this._DrawComponents.Reverse();
            this._EventComponents = sorted.Where(a => a is IDynamicComponent).Cast<IDynamicComponent>().ToList();
        }
        private void UpdateFocus(IDynamicComponent component)
        {
            if (this._FocusComponent != component)
            {
                this._FocusComponent?.FocusLost();
                if (this._FocusComponent is IInputComponent)
                    KeyboardResolver.CharReceived -= this.ReceiveInput;
                this._FocusComponent = component;
                component?.FocusGained();
                if (component is IInputComponent)
                    KeyboardResolver.CharReceived += this.ReceiveInput;
            }
        }
        private bool GetTarget(Point offset, Point position, out IDynamicComponent component, out bool floating)
        {
            component = null;
            floating = false;
            if ((this._FocusComponent as IFloatComponent)?.FloatComponent?.InBounds(offset, position)==true)
            {
                floating = true;
                component = (this._FocusComponent as IFloatComponent).FloatComponent;
                return true;
            }
            else
                foreach (IDynamicComponent comp in this._EventComponents)
                    if (comp.Visible && comp.Enabled && comp.InBounds(offset, position))
                    {
                        floating = false;
                        component = comp;
                        return true;
                    }
            return false;
        }

        private void ReceiveInput(char input)
        {
            if ((this._FocusComponent as IInputComponent).Selected)
                (this._FocusComponent as IInputComponent).ReceiveInput(input);
        }

        // IClickableMenu implementations
        public override bool areGamePadControlsImplemented()
        {
            return false;
        }

        public override void clickAway()
        {
            this._FocusComponent?.FocusLost();
            this._FocusComponent = null;
        }
        public override void receiveRightClick(int x, int y, bool playSound = true)
        {
            Point offset = new Point(this.InnerBounds.X, this.InnerBounds.Y), position = new Point(x / Game1.pixelZoom, y / Game1.pixelZoom);
            if (GetTarget(offset, position, out IDynamicComponent component, out bool floating))
            {
                if (!floating)
                    UpdateFocus(component);
                component.RightClick(offset, position);
            }
            else
                UpdateFocus(null);
        }
        public override bool isWithinBounds(int x, int y)
        {
            return true;
        }
        public override void leftClickHeld(int x, int y)
        {
            Point offset = new Point(this.InnerBounds.X, this.InnerBounds.Y), position = new Point(x / Game1.pixelZoom, y / Game1.pixelZoom);
            if (GetTarget(offset, position, out IDynamicComponent component, out bool floating))
                (floating? component : this._FocusComponent)?.LeftHeld(offset, position);
        }
        public override void performHoverAction(int x, int y)
        {
            Point offset = new Point(this.InnerBounds.X, this.InnerBounds.Y), position = new Point(x / Game1.pixelZoom, y / Game1.pixelZoom);
            if (GetTarget(offset, position, out IDynamicComponent component, out bool floating))
            {
                if (component == this._HoverComponent)
                    component.HoverOver(offset, position);
                else
                {
                    this._HoverComponent?.HoverOut(offset, position);
                    this._HoverComponent = component;
                    component.HoverIn(offset, position);
                }
            }
            else if(this._HoverComponent !=null)
            {
                this._HoverComponent?.HoverOut(offset, position);
                this._HoverComponent = null;
            }
        }
        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            Point offset = new Point(this.InnerBounds.X, this.InnerBounds.Y), position = new Point(x / Game1.pixelZoom, y / Game1.pixelZoom);
            if (GetTarget(offset, position, out IDynamicComponent component, out bool floating))
            {
                if (!floating)
                    UpdateFocus(component);
                component.LeftClick(offset, position);
            }
            else
                UpdateFocus(null);
        }
        public override void receiveKeyPress(Keys key)
        {
            if (this._FocusComponent is IInputComponent || (this._FocusComponent as IHotkeyComponent)?.ReceiveHotkey(key) == true)
                return;
            if (key == Keys.Escape && this._FocusComponent != null)
                UpdateFocus(null);
            else if (key == Keys.Tab)
                TabNext();
            else if (key == Keys.Enter)
            {
                if (this._FocusComponent == null)
                    return;
                (this._FocusComponent as IComponentContainer)?.TabAccess(TabType.LeftClick);
            }
            else
                base.receiveKeyPress(key);
        }
        public override void receiveScrollWheelAction(int direction)
        {
            var state = Mouse.GetState();
            Point offset = new Point(this.InnerBounds.X, this.InnerBounds.Y), position = new Point(state.X / Game1.pixelZoom, state.Y / Game1.pixelZoom);
            if (GetTarget(offset, position, out IDynamicComponent component, out bool floating))
                component.Scroll(offset, position, direction);
        }
        public override void releaseLeftClick(int x, int y)
        {
            Point offset = new Point(this.InnerBounds.X, this.InnerBounds.Y), position = new Point(x / Game1.pixelZoom, y / Game1.pixelZoom);
            if (GetTarget(offset, position, out IDynamicComponent component, out bool floating))
                (floating ? component : this._FocusComponent)?.LeftUp(offset, position);
        }
        public override void update(GameTime time)
        {
            foreach (IComponent component in this._DrawComponents)
                component.Update(time);
            (this._FocusComponent as IFloatComponent)?.FloatComponent?.Update(time);
        }
        public override void draw(SpriteBatch b)
        {
            Point offset = new Point(this.InnerBounds.X, this.InnerBounds.Y);
            foreach (IComponent component in this._DrawComponents)
                component.Draw(offset, b);
            (this._FocusComponent as IFloatComponent)?.FloatComponent?.Draw(new Point(0, 0), b);
        }
    }
}
