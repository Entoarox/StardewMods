using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using StardewValley;
using StardewValley.Menus;

namespace Entoarox.Framework.Interface
{
    [Obsolete("The Interface framework has not yet been completed and should not be used!")]
    public class InterfaceMenu : IClickableMenu, IComponentContainer
    {
        private bool ShowCloseButton;
        private bool DrawChrome;
        public InterfaceMenu(Rectangle bounds, bool showCloseButton = true, bool drawChrome = true)
        {
            Rectangle realRect = Utilities.GetRealRectangle(bounds);
            this.DrawChrome = drawChrome;
            this.OuterBounds = bounds;
            initialize(realRect.X, realRect.Y, realRect.Width, realRect.Height, showCloseButton);
        }
        public InterfaceMenu(Point size, bool showCloseButton=true, bool drawChrome=true) : this(size.X,size.Y,showCloseButton,drawChrome)
        {

        }
        public InterfaceMenu(int width, int height, bool showCloseButton = true, bool drawChrome = true)
        {
            Vector2 vector = Utility.getTopLeftPositionForCenteringOnScreen(width, height);
            Rectangle realRect = new Rectangle((int)vector.X, (int)vector.Y, width * Game1.pixelZoom, height * Game1.pixelZoom);
            this.DrawChrome = drawChrome;
            this.OuterBounds = Utilities.GetZoomRectangle(realRect);
            initialize(realRect.X, realRect.Y, realRect.Width, realRect.Height, showCloseButton);
        }

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
        public bool ContainsComponent(IComponent component)
        {
            return ContainsComponent(component.Name) && this._Components[component.Name] == component;
        }

        public bool ContainsComponent(string name)
        {
            return this._Components.ContainsKey(name);
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

        public void ClearComponents()
        {
            this._Components.Clear();
            this._DrawComponents.Clear();
            this._EventComponents.Clear();
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
                    index = 0;
                this._FocusComponent = this._EventComponents[index];
            }
            return true;
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
                    index = this._EventComponents.Count-1;
                this._FocusComponent = this._EventComponents[index];
            }
            return true;
        }

        // Internals
        private Dictionary<string, IComponent> _Components = new Dictionary<string, IComponent>();
        private List<IComponent> _DrawComponents = new List<IComponent>();
        private List<IDynamicComponent> _EventComponents = new List<IDynamicComponent>();
        private IDynamicComponent _FocusComponent;
        private IDynamicComponent _HoverComponent;
        private int RepeatCounter = 0;

        public InterfaceMenu Menu => this;

        public Rectangle InnerBounds => this.OuterBounds;

        public Rectangle OuterBounds { get; private set; }

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
            if ((this._FocusComponent as IInputComponent)?.Selected==true)
                (this._FocusComponent as IInputComponent).ReceiveInput(input);
        }
        private void ReceiveKeypress(Keys key)
        {
            if (key != Keys.Escape && (this._FocusComponent is IInputComponent || (this._FocusComponent as IHotkeyComponent)?.ReceiveHotkey(key) == true))
                return;
            else if (key == Keys.Escape && this._FocusComponent != null)
                UpdateFocus(null);
            else if (key == Keys.Tab)
                TabNext();
            else
                base.receiveKeyPress(key);
        }

        // IClickableMenu implementations
        public override bool areGamePadControlsImplemented()
        {
            return false;
        }
        public override void gamePadButtonHeld(Buttons b)
        {
            if (b == Buttons.Start)
                this.receiveKeyPress(Keys.Escape);
            else if ((this._FocusComponent as IControllerComponent)?.ReceiveController(b) != true)
            {
                switch (b)
                {
                    case Buttons.A:
                        this.ReceiveKeypress(Keys.Enter);
                        break;
                    case Buttons.B:
                        this.ReceiveKeypress(Keys.Escape);
                        break;
                    case Buttons.LeftShoulder:
                    case Buttons.LeftTrigger:
                        this.TabNext();
                        break;
                    case Buttons.RightShoulder:
                    case Buttons.RightTrigger:
                        this.TabBack();
                        break;
                    case Buttons.LeftThumbstickUp:
                    case Buttons.DPadUp:
                        this.ReceiveKeypress(Keys.Up);
                        break;
                    case Buttons.LeftThumbstickDown:
                    case Buttons.DPadDown:
                        this.ReceiveKeypress(Keys.Down);
                        break;
                    case Buttons.LeftThumbstickLeft:
                    case Buttons.DPadLeft:
                        this.ReceiveKeypress(Keys.Left);
                        break;
                    case Buttons.LeftThumbstickRight:
                    case Buttons.DPadRight:
                        this.ReceiveKeypress(Keys.Right);
                        break;
                }
                int X = this.InnerBounds.X;
                int Y = this.InnerBounds.Y;
                IComponent target = this._FocusComponent;
                X += target.OuterBounds.X;
                Y += target.OuterBounds.Y;
                while (target is IComponentContainer parent && parent.FocusComponent != null)
                {
                    target = parent.FocusComponent;
                    X += target.OuterBounds.X;
                    Y += target.OuterBounds.Y;
                }
                X += target.OuterBounds.Width - 1;
                Y += target.OuterBounds.Height - 1;
                Mouse.SetPosition(X*Game1.pixelZoom, Y*Game1.pixelZoom);

            }
        }
        public override void receiveGamePadButton(Buttons b)
        {
            this.gamePadButtonHeld(b);
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
            this.RepeatCounter++;
            if(this.RepeatCounter>45)
            {
                this.RepeatCounter = 0;
                Point offset = new Point(this.InnerBounds.X, this.InnerBounds.Y), position = new Point(x / Game1.pixelZoom, y / Game1.pixelZoom);
                if (GetTarget(offset, position, out IDynamicComponent component, out bool floating))
                    (floating ? component : this._FocusComponent)?.LeftHeld(offset, position);
            }
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
            this.RepeatCounter = 0;
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
            base.update(time);
            foreach (IComponent component in this._DrawComponents)
                if (component.Visible && (!(component is IDynamicComponent) || (component as IDynamicComponent).Enabled))
                    component.Update(time);
            (this._FocusComponent as IFloatComponent)?.FloatComponent?.Update(time);
        }
        public override void draw(SpriteBatch b)
        {
            if (this.DrawChrome)
            {
                Rectangle drawRect = Utilities.GetRealRectangle(this.OuterBounds);
                Utilities.DrawMenuRect(b, drawRect.X, drawRect.Y, drawRect.Width, drawRect.Height);
            }
            Point offset = new Point(this.InnerBounds.X, this.InnerBounds.Y);
            foreach (IComponent component in this._DrawComponents)
                if (component.Visible)
                    component.Draw(offset, b);
            (this._FocusComponent as IFloatComponent)?.FloatComponent?.Draw(new Point(0, 0), b);
            base.draw(b);
            this.drawMouse(b);
        }

        public bool HasFocus(IDynamicComponent component)
        {
            return this._FocusComponent == component || (this._FocusComponent as IComponentContainer)?.HasFocus(component) == true;
        }

        public IDynamicComponent FocusComponent => this._FocusComponent;
    }
}
