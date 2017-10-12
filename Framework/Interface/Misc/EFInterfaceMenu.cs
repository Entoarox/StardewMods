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
            if (_Components.ContainsKey(component.Name))
                throw new ArgumentException(Strings.DuplicateKey);
            component.Attach(this);
            _Components.Add(component.Name, component);
            UpdateSorting();
        }

        public bool RemoveComponent(IComponent component) => RemoveComponent(component.Name);
        public bool RemoveComponent(string name)
        {
            if (!_Components.ContainsKey(name))
                throw new KeyNotFoundException(Strings.KeyNotFound);
            bool result = _Components.Remove(name);
            UpdateSorting();
            return result;
        }
        public bool ContainsComponent(IComponent component) => ContainsComponent(component.Name);
        public bool ContainsComponent(string name) => _Components.ContainsKey(name);

        public void RemoveComponents<T>() where T : IComponent => RemoveComponents(a => a is T);
        public void RemoveComponents(Predicate<IComponent> predicate)
        {
            foreach (var a in _Components.Where(a => predicate(a.Value)))
                _Components.Remove(a.Key);
            UpdateSorting();
        }

        public void ClearComponents()
        {
            _Components.Clear();
            _DrawComponents.Clear();
            _EventComponents.Clear();
        }

        public InterfaceMenu Menu => this;
        public Rectangle OuterBounds { get; set; }
        public Rectangle InnerBounds { get; set; }

        public bool TabNext()
        {
            if (_FocusComponent == null)
            {
                if (_EventComponents.Count == 0)
                    return false;
                _FocusComponent = _EventComponents[0];
            }
            else if (!(_FocusComponent is IComponentContainer) || !(_FocusComponent as IComponentContainer).TabNext())
            {
                var index = _EventComponents.IndexOf(_FocusComponent) + 1;
                if (index >= _EventComponents.Count)
                    index = 0;
                _FocusComponent = _EventComponents[index];
            }
            return true;
        }
        public void TabAccess(TabType type)
        {
            //TODO: Implement proper controller mapping for the menu, since currently there is no mapping
            throw new NotImplementedException(Strings.InvalidClassMethod);
        }
        public bool HasFocus(IComponent component) => _FocusComponent == component;
        public IComponent this[string name] { get => _Components.ContainsKey(name) ? _Components[name] : throw new KeyNotFoundException(Strings.KeyNotFound); }

        public IEnumerator<IComponent> GetEnumerator()
        {
            foreach (var component in _Components.Values)
                yield return component;
        }
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        // Internals
        private Dictionary<string, IComponent> _Components;
        private List<IComponent> _DrawComponents = new List<IComponent>();
        private List<IDynamicComponent> _EventComponents = new List<IDynamicComponent>();
        private IDynamicComponent _FocusComponent;
        private IDynamicComponent _HoverComponent;

        private void UpdateSorting()
        {
            var sorted = _Components.Values.OrderByDescending(a => a is IDynamicComponent).OrderBy(a => a.Layer).OrderBy(a => a.OuterBounds.Y).OrderBy(a => a.OuterBounds.X);
            _DrawComponents = sorted.ToList();
            _DrawComponents.Reverse();
            _EventComponents = sorted.Where(a => a is IDynamicComponent).Cast<IDynamicComponent>().ToList();
        }
        private void UpdateFocus(IDynamicComponent component)
        {
            if (_FocusComponent != component)
            {
                _FocusComponent?.FocusLost();
                if (_FocusComponent is IInputComponent)
                    KeyboardResolver.CharReceived -= ReceiveInput;
                _FocusComponent = component;
                component?.FocusGained();
                if (component is IInputComponent)
                    KeyboardResolver.CharReceived += ReceiveInput;
            }
        }
        private bool GetTarget(Point offset, Point position, out IDynamicComponent component, out bool floating)
        {
            component = null;
            floating = false;
            if (_FocusComponent is IFloatComponent && (_FocusComponent as IFloatComponent).FloatComponent != null && (_FocusComponent as IFloatComponent).FloatComponent.InBounds(offset, position))
            {
                floating = true;
                component = (_FocusComponent as IFloatComponent).FloatComponent;
                return true;
            }
            else
                foreach (IDynamicComponent comp in _EventComponents)
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
            if ((_FocusComponent as IInputComponent).Selected)
                (_FocusComponent as IInputComponent)?.ReceiveInput(input);
        }

        // IClickableMenu implementations
        public override bool areGamePadControlsImplemented() => false;
        public override void clickAway()
        {
            _FocusComponent?.FocusLost();
            _FocusComponent = null;
        }
        public override void receiveRightClick(int x, int y, bool playSound = true)
        {
            Point offset = new Point(InnerBounds.X, InnerBounds.Y), position = new Point(x / Game1.pixelZoom, y / Game1.pixelZoom);
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
            Point offset = new Point(InnerBounds.X, InnerBounds.Y), position = new Point(x / Game1.pixelZoom, y / Game1.pixelZoom);
            if (GetTarget(offset, position, out IDynamicComponent component, out bool floating))
                (floating? component : _FocusComponent)?.LeftHeld(offset, position);
        }
        public override void performHoverAction(int x, int y)
        {
            Point offset = new Point(InnerBounds.X, InnerBounds.Y), position = new Point(x / Game1.pixelZoom, y / Game1.pixelZoom);
            if (GetTarget(offset, position, out IDynamicComponent component, out bool floating))
            {
                if (component == _HoverComponent)
                    component.HoverOver(offset, position);
                else
                {
                    _HoverComponent?.HoverOut(offset, position);
                    _HoverComponent = component;
                    component.HoverIn(offset, position);
                }
            }
            else if(_HoverComponent!=null)
            {
                _HoverComponent?.HoverOut(offset, position);
                _HoverComponent = null;
            }
        }
        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            Point offset = new Point(InnerBounds.X, InnerBounds.Y), position = new Point(x / Game1.pixelZoom, y / Game1.pixelZoom);
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
            if (!(_FocusComponent is IInputComponent) && !(_FocusComponent is IHotkeyComponent && (_FocusComponent as IHotkeyComponent).ReceiveHotkey(key)))
                if (key == Keys.Escape && _FocusComponent != null)
                    UpdateFocus(null);
                else if (key == Keys.Tab)
                    TabNext();
                else if(key==Keys.Enter)
                {
                    if (_FocusComponent == null)
                        return;
                    if (_FocusComponent is IComponentContainer)
                        (_FocusComponent as IComponentContainer).TabAccess(TabType.LeftClick);
                }
                else
                    base.receiveKeyPress(key);
        }
        public override void receiveScrollWheelAction(int direction)
        {
            var state = Mouse.GetState();
            Point offset = new Point(InnerBounds.X, InnerBounds.Y), position = new Point(state.X / Game1.pixelZoom, state.Y / Game1.pixelZoom);
            if (GetTarget(offset, position, out IDynamicComponent component, out bool floating))
                component.Scroll(offset, position, direction);
        }
        public override void releaseLeftClick(int x, int y)
        {
            _FocusComponent?.LeftUp(new Point(InnerBounds.X, InnerBounds.Y), new Point(x / Game1.pixelZoom, y / Game1.pixelZoom));
        }
        public override void update(GameTime time)
        {
            foreach (IComponent component in _DrawComponents)
                component.Update(time);
            if (_FocusComponent is IFloatComponent)
                (_FocusComponent as IFloatComponent).FloatComponent.Update(time);
        }
        public override void draw(SpriteBatch b)
        {
            Point offset = new Point(InnerBounds.X, InnerBounds.Y);
            foreach (IComponent component in _DrawComponents)
                component.Draw(offset, b);
            if (_FocusComponent is IFloatComponent)
                (_FocusComponent as IFloatComponent).FloatComponent.Draw(new Point(0, 0), b);
        }
    }
}
