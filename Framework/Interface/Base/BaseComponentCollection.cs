using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Entoarox.Framework.Interface
{
    public abstract class BaseComponentCollection : BaseComponentContainer, IHotkeyComponent, IInputComponent, IFloatComponent, IComponentCollection
    {
        protected BaseComponentCollection(string name, Rectangle bounds, int layer) : base(name, bounds, layer)
        {
        }
        protected Dictionary<string, IComponent> _Components;
        protected List<IComponent> _DrawComponents = new List<IComponent>();
        protected List<IDynamicComponent> _EventComponents = new List<IDynamicComponent>();
        protected IDynamicComponent _FocusComponent;
        protected IDynamicComponent _HoverComponent;

        protected Point GetMyOffset(Point offset) => new Point(offset.X + InnerBounds.X, offset.Y + InnerBounds.Y);
        protected void UpdateSorting()
        {
            var sorted = _Components.Values.OrderByDescending(a => a is IDynamicComponent).OrderBy(a => a.Layer).OrderBy(a => a.OuterBounds.Y).OrderBy(a => a.OuterBounds.X);
            _DrawComponents = sorted.ToList();
            _DrawComponents.Reverse();
            _EventComponents = sorted.Where(a => a is IDynamicComponent).Cast<IDynamicComponent>().ToList();
        }
        protected void UpdateFocus(IDynamicComponent component)
        {
            if (_FocusComponent != component)
            {
                _FocusComponent?.FocusLost();
                _FocusComponent = component;
                component?.FocusGained();
            }
        }
        protected bool GetTarget(Point offset, Point position, out IDynamicComponent component)
        {
            component = null;
            foreach (IDynamicComponent comp in _EventComponents)
                if (comp.Visible && comp.Enabled && comp.InBounds(offset, position))
                {
                    component = comp;
                    return true;
                }
            return false;
        }

        public override void FocusLost() => UpdateFocus(null);
        public override void HoverIn(Point offset, Point position)
        {
            HoverOver(offset, position);
        }
        public override void HoverOut(Point offset, Point position)
        {
            if (_HoverComponent != null)
            {
                _HoverComponent?.HoverOut(GetMyOffset(offset), position);
                _HoverComponent = null;
            }
        }
        public override void HoverOver(Point offset, Point position)
        {
            var myOffset = GetMyOffset(offset);
            if (GetTarget(myOffset, position, out IDynamicComponent component))
            {
                if (component == _HoverComponent)
                    component.HoverOver(myOffset, position);
                else
                {
                    _HoverComponent?.HoverOut(myOffset, position);
                    _HoverComponent = component;
                    component.HoverIn(myOffset, position);
                }
            }
            else if (_HoverComponent != null)
            {
                _HoverComponent?.HoverOut(myOffset, position);
                _HoverComponent = null;
            }
        }
        public override void LeftClick(Point offset, Point position)
        {
            var myOffset = GetMyOffset(offset);
            GetTarget(myOffset, position, out IDynamicComponent component);
            UpdateFocus(component);
            component?.LeftClick(myOffset, position);
        }
        public override void LeftHeld(Point offset, Point position) => _FocusComponent?.LeftHeld(GetMyOffset(offset), position);
        public override void LeftUp(Point offset, Point position) => _FocusComponent?.LeftUp(GetMyOffset(offset), position);
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
            foreach (IComponent component in _DrawComponents)
                component.Update(time);
        }
        public override void Draw(Point offset, SpriteBatch batch)
        {
            Point myOffset = GetMyOffset(offset);
            foreach (IComponent component in _DrawComponents)
                if (component.Visible)
                    component.Draw(myOffset, batch);
        }
        public void ReceiveInput(char input) => (_FocusComponent as IInputComponent)?.ReceiveInput(input);
        public bool ReceiveHotkey(Microsoft.Xna.Framework.Input.Keys key) => (_FocusComponent as IHotkeyComponent)?.ReceiveHotkey(key) ?? false;

        public override string Tooltip => _HoverComponent?.Tooltip;

        public IDynamicComponent FloatComponent => (_FocusComponent as IFloatComponent)?.FloatComponent;

        public virtual bool AcceptsComponent<T>() where T : IComponent => true;
        public virtual bool AcceptsComponent(IComponent component) => true;

        public void AddComponent(IComponent component)
        {
            if (component.IsAttached)
                throw new InvalidOperationException(Strings.ComponentAttached);
            if (_Components.ContainsKey(component.Name))
                throw new ArgumentException(Strings.DuplicateKey);
            if (!AcceptsComponent(component))
                throw new ArgumentException(Strings.NotAccepted);
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
        public bool Selected
        {
            get => _FocusComponent != null && _FocusComponent is IInputComponent && (_FocusComponent as IInputComponent).Selected;
            set
            {
                if (_FocusComponent != null && _FocusComponent is IInputComponent)
                    (_FocusComponent as IInputComponent).Selected = value;
            }
        }
        public override bool TabNext()
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
        public override void TabAccess(TabType type)
        {
            //TODO: Implement controller mapping for collections, since that can be done
        }
        public override bool HasFocus(IComponent component) => _FocusComponent == component;

        public IComponent this[string name] { get => _Components.ContainsKey(name) ? _Components[name] : throw new KeyNotFoundException(Strings.KeyNotFound); }

        public IEnumerator<IComponent> GetEnumerator()
        {
            foreach (var component in _Components.Values)
                yield return component;
        }
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}