using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley;
using StardewValley.Menus;

namespace Entoarox.Framework.UI
{
    public class FrameworkMenu : IClickableMenu, IComponentCollection
    {
        /*********
        ** Fields
        *********/
        private static readonly Rectangle BottomCenter = new Rectangle(128, 192, 64, 64);
        private static readonly Rectangle Background = new Rectangle(64, 128, 64, 64);
        private static readonly Rectangle BottomLeft = new Rectangle(0, 192, 64, 64);
        private static readonly Rectangle BottomRight = new Rectangle(192, 192, 64, 64);
        private static readonly Rectangle MiddleLeft = new Rectangle(0, 128, 64, 64);
        private static readonly Rectangle MiddleRight = new Rectangle(192, 128, 64, 64);
        private static readonly Rectangle TopCenter = new Rectangle(128, 0, 64, 64);
        private static readonly Rectangle TopLeft = new Rectangle(0, 0, 64, 64);
        private static readonly Rectangle TopRight = new Rectangle(192, 0, 64, 64);
        private static readonly int Zoom10 = Game1.pixelZoom * 10;
        private static readonly int Zoom2 = Game1.pixelZoom * 2;
        private static readonly int Zoom20 = Game1.pixelZoom * 20;
        private static readonly int Zoom3 = Game1.pixelZoom * 3;
        private static readonly int Zoom4 = Game1.pixelZoom * 4;
        private static readonly int Zoom6 = Game1.pixelZoom * 6;
        private readonly List<IInteractiveMenuComponent> _InteractiveComponents = new List<IInteractiveMenuComponent>();
        private readonly List<IMenuComponent> _StaticComponents = new List<IMenuComponent>();
        private readonly bool Centered;
        private readonly bool DrawChrome;
        private List<IMenuComponent> DrawOrder = new List<IMenuComponent>();
        private List<IInteractiveMenuComponent> EventOrder = new List<IInteractiveMenuComponent>();
        private IInteractiveMenuComponent FloatingComponent;
        private IInteractiveMenuComponent FocusElement;
        private bool Hold;
        private IInteractiveMenuComponent HoverInElement;


        /*********
        ** Accessors
        *********/
        public Rectangle Area;
        public List<IMenuComponent> StaticComponents => new List<IMenuComponent>(this._StaticComponents);
        public List<IInteractiveMenuComponent> InteractiveComponents => new List<IInteractiveMenuComponent>(this._InteractiveComponents);

        public virtual Rectangle EventRegion => new Rectangle(this.Area.X + FrameworkMenu.Zoom10, this.Area.Y + FrameworkMenu.Zoom10, this.Area.Width - FrameworkMenu.Zoom20, this.Area.Height - FrameworkMenu.Zoom20);

        public virtual Rectangle ZoomEventRegion => new Rectangle((this.Area.X + FrameworkMenu.Zoom10) / Game1.pixelZoom, (this.Area.Y + FrameworkMenu.Zoom10) / Game1.pixelZoom, (this.Area.Width - FrameworkMenu.Zoom20) / Game1.pixelZoom, (this.Area.Height - FrameworkMenu.Zoom20) / Game1.pixelZoom);


        /*********
        ** Public methods
        *********/
        public FrameworkMenu(Rectangle area, bool showCloseButton = true, bool drawChrome = true)
        {
            this.DrawChrome = drawChrome;
            this.Area = new Rectangle(area.X * Game1.pixelZoom, area.Y * Game1.pixelZoom, area.Width * Game1.pixelZoom, area.Height * Game1.pixelZoom);
            this.initialize(this.Area.X, this.Area.Y, this.Area.Width, this.Area.Height, showCloseButton);
        }

        public FrameworkMenu(Point size, bool showCloseButton = true, bool drawChrome = true)
        {
            this.DrawChrome = drawChrome;
            this.Centered = true;
            Vector2 pos = Utility.getTopLeftPositionForCenteringOnScreen(size.X * Game1.pixelZoom, size.Y * Game1.pixelZoom);
            this.Area = new Rectangle((int)pos.X, (int)pos.Y, size.X * Game1.pixelZoom, size.Y * Game1.pixelZoom);
            this.initialize(this.Area.X, this.Area.Y, this.Area.Width, this.Area.Height, showCloseButton);
        }

        public static void DrawMenuRect(SpriteBatch b, int x, int y, int width, int height)
        {
            Rectangle o = new Rectangle(x + FrameworkMenu.Zoom2, y + FrameworkMenu.Zoom2, width - FrameworkMenu.Zoom4, height - FrameworkMenu.Zoom4);
            b.Draw(Game1.menuTexture, new Rectangle(o.X, o.Y, o.Width, o.Height), FrameworkMenu.Background, Color.White);
            o = new Rectangle(x - FrameworkMenu.Zoom3, y - FrameworkMenu.Zoom3, width + FrameworkMenu.Zoom6, height + FrameworkMenu.Zoom6);
            b.Draw(Game1.menuTexture, new Rectangle(o.X, o.Y, 64, 64), FrameworkMenu.TopLeft, Color.White);
            b.Draw(Game1.menuTexture, new Rectangle(o.X + o.Width - 64, o.Y, 64, 64), FrameworkMenu.TopRight, Color.White);
            b.Draw(Game1.menuTexture, new Rectangle(o.X + 64, o.Y, o.Width - 128, 64), FrameworkMenu.TopCenter, Color.White);
            b.Draw(Game1.menuTexture, new Rectangle(o.X, o.Y + o.Height - 64, 64, 64), FrameworkMenu.BottomLeft, Color.White);
            b.Draw(Game1.menuTexture, new Rectangle(o.X + o.Width - 64, o.Y + o.Height - 64, 64, 64), FrameworkMenu.BottomRight, Color.White);
            b.Draw(Game1.menuTexture, new Rectangle(o.X + 64, o.Y + o.Height - 64, o.Width - 128, 64), FrameworkMenu.BottomCenter, Color.White);
            b.Draw(Game1.menuTexture, new Rectangle(o.X, o.Y + 64, 64, o.Height - 128), FrameworkMenu.MiddleLeft, Color.White);
            b.Draw(Game1.menuTexture, new Rectangle(o.X + o.Width - 64, o.Y + 64, 64, o.Height - 128), FrameworkMenu.MiddleRight, Color.White);
        }

        public static KeyValuePair<List<IInteractiveMenuComponent>, List<IMenuComponent>> GetOrderedLists(List<IMenuComponent> statics, List<IInteractiveMenuComponent> interactives)
        {
            List<IMenuComponent> drawOrder = new List<IMenuComponent>(statics);
            drawOrder.AddRange(interactives);
            drawOrder = drawOrder.OrderBy(x => x.Layer).ThenByDescending(x => x.GetPosition().Y).ThenByDescending(x => x.GetPosition().X).ToList();
            List<IInteractiveMenuComponent> eventOrder = interactives.OrderByDescending(x => x.Layer).ThenBy(x => x.GetPosition().Y).ThenBy(x => x.GetPosition().X).ToList();
            return new KeyValuePair<List<IInteractiveMenuComponent>, List<IMenuComponent>>(eventOrder, drawOrder);
        }

        public virtual FrameworkMenu GetAttachedMenu()
        {
            return this;
        }

        public virtual void ResetFocus()
        {
            if (this.FocusElement == null)
                return;
            this.FocusElement.FocusLost();
            if (this.FocusElement is IKeyboardComponent && Game1.keyboardDispatcher.Subscriber != null)
            {
                Game1.keyboardDispatcher.Subscriber.Selected = false;
                Game1.keyboardDispatcher.Subscriber = null;
            }

            this.FocusElement = null;
            if (this.FloatingComponent != null)
            {
                this.FloatingComponent.Detach(this);
                this.FloatingComponent = null;
            }
        }

        public virtual void GiveFocus(IInteractiveMenuComponent component)
        {
            if (component == this.FocusElement)
                return;
            this.ResetFocus();
            this.FocusElement = component;
            if (this.FocusElement is IKeyboardComponent keyboardComponent)
                Game1.keyboardDispatcher.Subscriber = new KeyboardSubscriberProxy(keyboardComponent);
            if (!this._InteractiveComponents.Contains(component))
            {
                this.FloatingComponent = component;
                component.Attach(this);
            }

            component.FocusGained();
        }

        public virtual void AddComponent(IMenuComponent component)
        {
            if (component is IInteractiveMenuComponent menuComponent)
                this._InteractiveComponents.Add(menuComponent);
            else
                this._StaticComponents.Add(component);
            component.Attach(this);
            this.UpdateDrawOrder();
        }

        public virtual void RemoveComponent(IMenuComponent component)
        {
            this.RemoveComponents(a => a == component);
        }

        public virtual void RemoveComponents<T>() where T : IMenuComponent
        {
            this.RemoveComponents(a => a.GetType() == typeof(T));
        }

        public virtual void RemoveComponents(Predicate<IMenuComponent> filter)
        {
            this._InteractiveComponents.RemoveAll(a =>
            {
                bool b = filter(a);
                if (b)
                    a.Detach(this);
                return b;
            });
            this._StaticComponents.RemoveAll(a =>
            {
                bool b = filter(a);
                if (b)
                    a.Detach(this);
                return b;
            });
            this.UpdateDrawOrder();
        }

        public virtual void ClearComponents()
        {
            this._InteractiveComponents.TrueForAll(a =>
            {
                a.Detach(this);
                return true;
            });
            this._StaticComponents.TrueForAll(a =>
            {
                a.Detach(this);
                return true;
            });
            this._InteractiveComponents.Clear();
            this._StaticComponents.Clear();
            this.UpdateDrawOrder();
        }

        public virtual bool AcceptsComponent(IMenuComponent component)
        {
            return true;
        }

        public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
        {
            if (!this.Centered)
                return;
            Vector2 pos = Utility.getTopLeftPositionForCenteringOnScreen(this.Area.Width, this.Area.Height, 0, 0);
            this.Area = new Rectangle((int)pos.X, (int)pos.Y, this.Area.Width, this.Area.Height);
        }

        public override void releaseLeftClick(int x, int y)
        {
            if (this.HoverInElement == null)
                return;
            Point p = new Point(x, y);
            Point o = new Point(this.Area.X + FrameworkMenu.Zoom10, this.Area.Y + FrameworkMenu.Zoom10);
            this.HoverInElement.LeftUp(p, o);
            this.Hold = false;
            if (this.HoverInElement.InBounds(p, o))
                return;
            this.HoverInElement.HoverOut(p, o);
            this.HoverInElement = null;
        }

        public override void leftClickHeld(int x, int y)
        {
            if (this.HoverInElement == null)
                return;
            this.Hold = true;
            this.HoverInElement.LeftHeld(new Point(x, y), new Point(this.Area.X + FrameworkMenu.Zoom10, this.Area.Y + FrameworkMenu.Zoom10));
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            base.receiveLeftClick(x, y, playSound);
            Point p = new Point(x, y);
            Point o = new Point(this.Area.X + FrameworkMenu.Zoom10, this.Area.Y + FrameworkMenu.Zoom10);
            if (this.FloatingComponent != null && this.FloatingComponent.InBounds(p, o))
            {
                this.FloatingComponent.LeftClick(p, o);
                return;
            }

            foreach (IInteractiveMenuComponent el in this.EventOrder)
            {
                if (el.InBounds(p, o))
                {
                    this.GiveFocus(el);
                    el.LeftClick(p, o);
                    return;
                }
            }

            this.ResetFocus();
        }

        public void ExitMenu()
        {
            this.exitThisMenu(true);
        }

        public override void receiveRightClick(int x, int y, bool playSound = true)
        {
            Point p = new Point(x, y);
            Point o = new Point(this.Area.X + FrameworkMenu.Zoom10, this.Area.Y + FrameworkMenu.Zoom10);
            if (this.FloatingComponent != null && this.FloatingComponent.InBounds(p, o))
            {
                this.FloatingComponent.RightClick(p, o);
                return;
            }

            foreach (IInteractiveMenuComponent el in this.EventOrder)
            {
                if (el.InBounds(p, o))
                {
                    this.GiveFocus(el);
                    this.FocusElement = el;
                    el.RightClick(p, o);
                    return;
                }
            }

            this.ResetFocus();
        }

        public override void performHoverAction(int x, int y)
        {
            base.performHoverAction(x, y);
            if (!this.Area.Contains(x, y) || this.Hold)
                return;
            Point p = new Point(x, y);
            Point o = new Point(this.Area.X + FrameworkMenu.Zoom10, this.Area.Y + FrameworkMenu.Zoom10);
            if (this.HoverInElement != null && !this.HoverInElement.InBounds(p, o))
            {
                this.HoverInElement.HoverOut(p, o);
                this.HoverInElement = null;
            }

            if (this.FloatingComponent != null && this.FloatingComponent.InBounds(p, o))
            {
                this.FloatingComponent.HoverOver(p, o);
                return;
            }

            foreach (IInteractiveMenuComponent el in this.EventOrder)
            {
                if (el.InBounds(p, o))
                {
                    if (this.HoverInElement == null)
                    {
                        this.HoverInElement = el;
                        el.HoverIn(p, o);
                    }

                    el.HoverOver(p, o);
                    break;
                }
            }
        }

        public override void receiveScrollWheelAction(int direction)
        {
            base.receiveScrollWheelAction(direction);
            Point p = Game1.getMousePosition();
            Point o = new Point(this.Area.X + FrameworkMenu.Zoom10, this.Area.Y + FrameworkMenu.Zoom10);
            if (this.FloatingComponent != null)
                this.FloatingComponent.Scroll(direction, p, o);
            else
                foreach (IInteractiveMenuComponent el in this.EventOrder)
                    if (el.InBounds(p, o) && el.Scroll(direction, p, o))
                        return;
        }

        public override void receiveKeyPress(Keys key)
        {
            if (Game1.keyboardDispatcher.Subscriber == null || !Game1.keyboardDispatcher.Subscriber.Selected)
                base.receiveKeyPress(key);
        }

        public override void update(GameTime time)
        {
            base.update(time);
            this.FloatingComponent?.Update(time);
            foreach (IMenuComponent el in this.DrawOrder)
                el.Update(time);
        }

        public override void draw(SpriteBatch b)
        {
            if (this.DrawChrome)
                FrameworkMenu.DrawMenuRect(b, this.Area.X, this.Area.Y, this.Area.Width, this.Area.Height);
            Point o = new Point(this.Area.X + FrameworkMenu.Zoom10, this.Area.Y + FrameworkMenu.Zoom10);
            foreach (IMenuComponent el in this.DrawOrder)
                el.Draw(b, o);
            this.FloatingComponent?.Draw(b, o);
            base.draw(b);
            this.drawMouse(b);
        }


        /*********
        ** Protected methods
        *********/
        protected virtual void UpdateDrawOrder()
        {
            KeyValuePair<List<IInteractiveMenuComponent>, List<IMenuComponent>> sorted = FrameworkMenu.GetOrderedLists(this._StaticComponents, this._InteractiveComponents);
            this.DrawOrder = sorted.Value;
            this.EventOrder = sorted.Key;
        }
    }
}
