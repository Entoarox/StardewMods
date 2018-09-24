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
        /*********
        ** Fields
        *********/
        private readonly Dictionary<string, IComponent> Components = new Dictionary<string, IComponent>();
        private List<IComponent> DrawComponents = new List<IComponent>();
        private List<IDynamicComponent> EventComponents = new List<IDynamicComponent>();
        private IDynamicComponent HoverComponent;
        private readonly bool DrawChrome;
        private int RepeatCounter;


        /*********
        ** Accessors
        *********/
        public InterfaceMenu Menu => this;
        public Rectangle InnerBounds => this.OuterBounds;
        public Rectangle OuterBounds { get; }
        public IDynamicComponent FocusComponent { get; private set; }


        /*********
        ** Public methods
        *********/
        public InterfaceMenu(Rectangle bounds, bool showCloseButton = true, bool drawChrome = true)
        {
            Rectangle realRect = Utilities.GetRealRectangle(bounds);
            this.DrawChrome = drawChrome;
            this.OuterBounds = bounds;
            this.initialize(realRect.X, realRect.Y, realRect.Width, realRect.Height, showCloseButton);
        }

        public InterfaceMenu(Point size, bool showCloseButton = true, bool drawChrome = true)
            : this(size.X, size.Y, showCloseButton, drawChrome) { }

        public InterfaceMenu(int width, int height, bool showCloseButton = true, bool drawChrome = true)
        {
            Vector2 vector = Utility.getTopLeftPositionForCenteringOnScreen(width, height);
            Rectangle realRect = new Rectangle((int)vector.X, (int)vector.Y, width * Game1.pixelZoom, height * Game1.pixelZoom);
            this.DrawChrome = drawChrome;
            this.OuterBounds = Utilities.GetZoomRectangle(realRect);
            this.initialize(realRect.X, realRect.Y, realRect.Width, realRect.Height, showCloseButton);
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
            if (this.Components.ContainsKey(component.Name))
                throw new ArgumentException(Strings.DuplicateKey);
            component.Attach(this);
            this.Components.Add(component.Name, component);
            this.UpdateSorting();
        }

        public bool RemoveComponent(IComponent component)
        {
            if (!this.ContainsComponent(component))
                throw new KeyNotFoundException(Strings.KeyNotFound);
            return this.RemoveComponent(component.Name);
        }

        public bool RemoveComponent(string name)
        {
            if (!this.ContainsComponent(name))
                throw new KeyNotFoundException(Strings.KeyNotFound);
            bool result = this.Components.Remove(name);
            this.UpdateSorting();
            return result;
        }

        public bool ContainsComponent(IComponent component)
        {
            return this.ContainsComponent(component.Name) && this.Components[component.Name] == component;
        }

        public bool ContainsComponent(string name)
        {
            return this.Components.ContainsKey(name);
        }

        public void RemoveComponents<T>() where T : IComponent
        {
            this.RemoveComponents(a => a is T);
        }

        public void RemoveComponents(Predicate<IComponent> predicate)
        {
            foreach (KeyValuePair<string, IComponent> a in this.Components.Where(a => predicate(a.Value)))
                this.Components.Remove(a.Key);
            this.UpdateSorting();
        }

        public void ClearComponents()
        {
            this.Components.Clear();
            this.DrawComponents.Clear();
            this.EventComponents.Clear();
        }

        public bool TabNext()
        {
            if (this.FocusComponent == null)
            {
                if (this.EventComponents.Count == 0)
                    return false;
                this.FocusComponent = this.EventComponents[0];
            }
            else if (!(this.FocusComponent is IComponentContainer) || !(this.FocusComponent as IComponentContainer).TabNext())
            {
                int index = this.EventComponents.IndexOf(this.FocusComponent) + 1;
                if (index >= this.EventComponents.Count)
                    index = 0;
                this.FocusComponent = this.EventComponents[index];
            }

            return true;
        }

        public bool TabBack()
        {
            if (this.FocusComponent == null)
            {
                if (this.EventComponents.Count == 0)
                    return false;
                this.FocusComponent = this.EventComponents[0];
            }
            else if (!(this.FocusComponent is IComponentContainer) || !((IComponentContainer)this.FocusComponent).TabBack())
            {
                int index = this.EventComponents.IndexOf(this.FocusComponent) - 1;
                if (index < 0)
                    index = this.EventComponents.Count - 1;
                this.FocusComponent = this.EventComponents[index];
            }

            return true;
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
            else if ((this.FocusComponent as IControllerComponent)?.ReceiveController(b) != true)
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

                int x = this.InnerBounds.X;
                int y = this.InnerBounds.Y;
                IComponent target = this.FocusComponent;
                x += target.OuterBounds.X;
                y += target.OuterBounds.Y;
                while (target is IComponentContainer parent && parent.FocusComponent != null)
                {
                    target = parent.FocusComponent;
                    x += target.OuterBounds.X;
                    y += target.OuterBounds.Y;
                }

                x += target.OuterBounds.Width - 1;
                y += target.OuterBounds.Height - 1;
                Mouse.SetPosition(x * Game1.pixelZoom, y * Game1.pixelZoom);
            }
        }

        public override void receiveGamePadButton(Buttons b)
        {
            this.gamePadButtonHeld(b);
        }

        public override void clickAway()
        {
            this.FocusComponent?.FocusLost();
            this.FocusComponent = null;
        }

        public override void receiveRightClick(int x, int y, bool playSound = true)
        {
            Point offset = new Point(this.InnerBounds.X, this.InnerBounds.Y), position = new Point(x / Game1.pixelZoom, y / Game1.pixelZoom);
            if (this.GetTarget(offset, position, out IDynamicComponent component, out bool floating))
            {
                if (!floating)
                    this.UpdateFocus(component);
                component.RightClick(offset, position);
            }
            else
                this.UpdateFocus(null);
        }

        public override bool isWithinBounds(int x, int y)
        {
            return true;
        }

        public override void leftClickHeld(int x, int y)
        {
            this.RepeatCounter++;
            if (this.RepeatCounter > 45)
            {
                this.RepeatCounter = 0;
                Point offset = new Point(this.InnerBounds.X, this.InnerBounds.Y), position = new Point(x / Game1.pixelZoom, y / Game1.pixelZoom);
                if (this.GetTarget(offset, position, out IDynamicComponent component, out bool floating))
                    (floating ? component : this.FocusComponent)?.LeftHeld(offset, position);
            }
        }

        public override void performHoverAction(int x, int y)
        {
            Point offset = new Point(this.InnerBounds.X, this.InnerBounds.Y), position = new Point(x / Game1.pixelZoom, y / Game1.pixelZoom);
            if (this.GetTarget(offset, position, out IDynamicComponent component, out _))
            {
                if (component == this.HoverComponent)
                    component.HoverOver(offset, position);
                else
                {
                    this.HoverComponent?.HoverOut(offset, position);
                    this.HoverComponent = component;
                    component.HoverIn(offset, position);
                }
            }
            else if (this.HoverComponent != null)
            {
                this.HoverComponent?.HoverOut(offset, position);
                this.HoverComponent = null;
            }
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            this.RepeatCounter = 0;
            Point offset = new Point(this.InnerBounds.X, this.InnerBounds.Y), position = new Point(x / Game1.pixelZoom, y / Game1.pixelZoom);
            if (this.GetTarget(offset, position, out IDynamicComponent component, out bool floating))
            {
                if (!floating)
                    this.UpdateFocus(component);
                component.LeftClick(offset, position);
            }
            else
                this.UpdateFocus(null);
        }

        public override void receiveKeyPress(Keys key)
        {
        }

        public override void receiveScrollWheelAction(int direction)
        {
            MouseState state = Mouse.GetState();
            Point offset = new Point(this.InnerBounds.X, this.InnerBounds.Y), position = new Point(state.X / Game1.pixelZoom, state.Y / Game1.pixelZoom);
            if (this.GetTarget(offset, position, out IDynamicComponent component, out _))
                component.Scroll(offset, position, direction);
        }

        public override void releaseLeftClick(int x, int y)
        {
            Point offset = new Point(this.InnerBounds.X, this.InnerBounds.Y), position = new Point(x / Game1.pixelZoom, y / Game1.pixelZoom);
            if (this.GetTarget(offset, position, out IDynamicComponent component, out bool floating))
                (floating ? component : this.FocusComponent)?.LeftUp(offset, position);
        }

        public override void update(GameTime time)
        {
            base.update(time);
            foreach (IComponent component in this.DrawComponents)
            {
                if (component.Visible && (!(component is IDynamicComponent) || (component as IDynamicComponent).Enabled))
                    component.Update(time);
            }
            (this.FocusComponent as IFloatComponent)?.FloatComponent?.Update(time);
        }

        public override void draw(SpriteBatch b)
        {
            if (this.DrawChrome)
            {
                Rectangle drawRect = Utilities.GetRealRectangle(this.OuterBounds);
                Utilities.DrawMenuRect(b, drawRect.X, drawRect.Y, drawRect.Width, drawRect.Height);
            }

            Point offset = new Point(this.InnerBounds.X, this.InnerBounds.Y);
            foreach (IComponent component in this.DrawComponents)
            {
                if (component.Visible)
                    component.Draw(offset, b);
            }
            (this.FocusComponent as IFloatComponent)?.FloatComponent?.Draw(new Point(0, 0), b);
            base.draw(b);
            this.drawMouse(b);
        }

        public bool HasFocus(IDynamicComponent component)
        {
            return this.FocusComponent == component || (this.FocusComponent as IComponentContainer)?.HasFocus(component) == true;
        }


        /*********
        ** Protected methods
        *********/
        private void UpdateSorting()
        {
            IOrderedEnumerable<IComponent> sorted = this.Components.Values.OrderByDescending(a => a is IDynamicComponent).OrderBy(a => a.Layer).OrderBy(a => a.OuterBounds.Y).OrderBy(a => a.OuterBounds.X);
            this.DrawComponents = sorted.ToList();
            this.DrawComponents.Reverse();
            this.EventComponents = sorted.Where(a => a is IDynamicComponent).Cast<IDynamicComponent>().ToList();
        }

        private void UpdateFocus(IDynamicComponent component)
        {
            if (this.FocusComponent != component)
            {
                this.FocusComponent?.FocusLost();
                if (this.FocusComponent is IInputComponent)
                    KeyboardResolver.CharReceived -= this.ReceiveInput;
                this.FocusComponent = component;
                component?.FocusGained();
                if (component is IInputComponent)
                    KeyboardResolver.CharReceived += this.ReceiveInput;
            }
        }

        private bool GetTarget(Point offset, Point position, out IDynamicComponent component, out bool floating)
        {
            component = null;
            floating = false;
            if ((this.FocusComponent as IFloatComponent)?.FloatComponent?.InBounds(offset, position) == true)
            {
                floating = true;
                component = ((IFloatComponent)this.FocusComponent).FloatComponent;
                return true;
            }

            foreach (IDynamicComponent comp in this.EventComponents)
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
            if ((this.FocusComponent as IInputComponent)?.Selected == true)
                ((IInputComponent)this.FocusComponent).ReceiveInput(input);
        }

        private void ReceiveKeypress(Keys key)
        {
            if (key != Keys.Escape && (this.FocusComponent is IInputComponent || (this.FocusComponent as IHotkeyComponent)?.ReceiveHotkey(key) == true))
                return;
            if (key == Keys.Escape && this.FocusComponent != null)
                this.UpdateFocus(null);
            else if (key == Keys.Tab)
                this.TabNext();
            else
                base.receiveKeyPress(key);
        }
    }
}
