using System;
using System.Linq;
using System.Diagnostics;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using StardewValley;
using StardewValley.Menus;

namespace Entoarox.Framework.UI
{
    public class FrameworkMenu : IClickableMenu, IComponentCollection
    {
        protected List<IMenuComponent> DrawOrder=new List<IMenuComponent>();
        protected List<IInteractiveMenuComponent> EventOrder=new List<IInteractiveMenuComponent>();

        protected List<IMenuComponent> _StaticComponents = new List<IMenuComponent>();
        protected List<IInteractiveMenuComponent> _InteractiveComponents = new List<IInteractiveMenuComponent>();

        protected IInteractiveMenuComponent HoverInElement=null;
        protected IInteractiveMenuComponent FocusElement=null;
        protected IInteractiveMenuComponent FloatingComponent = null;

        protected bool Hold;

        protected bool DrawChrome;
        protected bool Centered = false;

        public Rectangle Area;

        public List<IMenuComponent> StaticComponents => new List<IMenuComponent>(this._StaticComponents);
        public List<IInteractiveMenuComponent> InteractiveComponents => new List<IInteractiveMenuComponent>(this._InteractiveComponents);
        protected static readonly Rectangle tl = new Rectangle(0, 0, 64, 64);
        protected static readonly Rectangle tc = new Rectangle(128, 0, 64, 64);
        protected static readonly Rectangle tr = new Rectangle(192, 0, 64, 64);
        protected static readonly Rectangle ml = new Rectangle(0, 128, 64, 64);
        protected static readonly Rectangle mr = new Rectangle(192, 128, 64, 64);
        protected static readonly Rectangle br = new Rectangle(192, 192, 64, 64);
        protected static readonly Rectangle bl = new Rectangle(0, 192, 64, 64);
        protected static readonly Rectangle bc = new Rectangle(128, 192, 64, 64);
        protected static readonly Rectangle bg = new Rectangle(64, 128, 64, 64);
        protected static readonly int zoom2 = Game1.pixelZoom * 2;
        protected static readonly int zoom3 = Game1.pixelZoom * 3;
        protected static readonly int zoom4 = Game1.pixelZoom * 4;
        protected static readonly int zoom6 = Game1.pixelZoom * 6;
        protected static readonly int zoom10 = Game1.pixelZoom * 10;
        protected static readonly int zoom20 = Game1.pixelZoom * 20;
        public static void DrawMenuRect(SpriteBatch b, int x, int y, int width, int height)
        {
            Rectangle o = new Rectangle(x + zoom2, y + zoom2, width - zoom4, height - zoom4);
            b.Draw(Game1.menuTexture, new Rectangle(o.X, o.Y, o.Width, o.Height), bg, Color.White);
            o = new Rectangle(x - zoom3, y - zoom3, width + zoom6, height + zoom6);
            b.Draw(Game1.menuTexture, new Rectangle(o.X, o.Y, 64, 64), tl, Color.White);
            b.Draw(Game1.menuTexture, new Rectangle(o.X + o.Width - 64, o.Y, 64, 64), tr, Color.White);
            b.Draw(Game1.menuTexture, new Rectangle(o.X + 64, o.Y, o.Width - 128, 64), tc, Color.White);
            b.Draw(Game1.menuTexture, new Rectangle(o.X, o.Y + o.Height - 64, 64, 64), bl, Color.White);
            b.Draw(Game1.menuTexture, new Rectangle(o.X + o.Width - 64, o.Y + o.Height - 64, 64, 64), br, Color.White);
            b.Draw(Game1.menuTexture, new Rectangle(o.X + 64, o.Y + o.Height - 64, o.Width - 128, 64), bc, Color.White);
            b.Draw(Game1.menuTexture, new Rectangle(o.X, o.Y + 64, 64, o.Height - 128), ml, Color.White);
            b.Draw(Game1.menuTexture, new Rectangle(o.X + o.Width - 64, o.Y + 64, 64, o.Height - 128), mr, Color.White);
        }
        public static KeyValuePair<List<IInteractiveMenuComponent>,List<IMenuComponent>> GetOrderedLists(List<IMenuComponent> statics, List<IInteractiveMenuComponent> interactives)
        {
            List<IMenuComponent> drawOrder = new List<IMenuComponent>(statics);
            drawOrder.AddRange(interactives);
            drawOrder = drawOrder.OrderBy(x => x.Layer).ThenByDescending(x => x.GetPosition().Y).ThenByDescending(x => x.GetPosition().X).ToList();
            List<IInteractiveMenuComponent>  eventOrder = interactives.OrderByDescending(x => x.Layer).ThenBy(x => x.GetPosition().Y).ThenBy(x => x.GetPosition().X).ToList();
            return new KeyValuePair<List<IInteractiveMenuComponent>, List<IMenuComponent>>(eventOrder, drawOrder);
        }
        public FrameworkMenu(Rectangle area, bool showCloseButton = true, bool drawChrome = true)
        {
            this.DrawChrome = drawChrome;
            this.Area = new Rectangle(area.X * Game1.pixelZoom, area.Y * Game1.pixelZoom, area.Width * Game1.pixelZoom, area.Height * Game1.pixelZoom);
            initialize(this.Area.X, this.Area.Y, this.Area.Width, this.Area.Height, showCloseButton);
        }
        public FrameworkMenu(Point size, bool showCloseButton = true, bool drawChrome = true)
        {
            this.DrawChrome = drawChrome;
            this.Centered = true;
            Vector2 pos = Utility.getTopLeftPositionForCenteringOnScreen(size.X * Game1.pixelZoom, size.Y * Game1.pixelZoom, 0, 0);
            this.Area = new Rectangle((int)pos.X, (int)pos.Y, size.X * Game1.pixelZoom, size.Y * Game1.pixelZoom);
            initialize(this.Area.X, this.Area.Y, this.Area.Width, this.Area.Height, showCloseButton);
        }
        protected virtual void UpdateDrawOrder()
        {
            KeyValuePair<List<IInteractiveMenuComponent>, List<IMenuComponent>> sorted = GetOrderedLists(this._StaticComponents, this._InteractiveComponents);
            this.DrawOrder = sorted.Value;
            this.EventOrder = sorted.Key;
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
            if(this.FocusElement is IKeyboardComponent && Game1.keyboardDispatcher.Subscriber!=null)
            {
                Game1.keyboardDispatcher.Subscriber.Selected = false;
                Game1.keyboardDispatcher.Subscriber = null;
            }
            this.FocusElement = null;
            if (this.FloatingComponent == null) return;
            this.FloatingComponent.Detach(this);
            this.FloatingComponent = null;
        }
        public virtual void GiveFocus(IInteractiveMenuComponent component)
        {
            if (component == this.FocusElement)
                return;
            ResetFocus();
            this.FocusElement = component;
            if(this.FocusElement is IKeyboardComponent)
                Game1.keyboardDispatcher.Subscriber = new KeyboardSubscriberProxy((IKeyboardComponent)this.FocusElement);
            if (!this._InteractiveComponents.Contains(component))
            {
                this.FloatingComponent = component;
                component.Attach(this);
            }
            component.FocusGained();
        }
        public virtual void AddComponent(IMenuComponent component)
        {
            if (component is IInteractiveMenuComponent)
                this._InteractiveComponents.Add(component as IInteractiveMenuComponent);
            else
                this._StaticComponents.Add(component);
            component.Attach(this);
            UpdateDrawOrder();
        }
        public virtual void RemoveComponent(IMenuComponent component)
        {
            bool Removed = false;
            RemoveComponents(a => a == component && !Removed);
        }
        public virtual void RemoveComponents<T>() where T : IMenuComponent
        {
            RemoveComponents(a => a.GetType() == typeof(T));
        }
        public virtual void RemoveComponents(Predicate<IMenuComponent> filter)
        {
            this._InteractiveComponents.RemoveAll(a => { bool b = filter(a); if (b) a.Detach(this); return b; });
            this._StaticComponents.RemoveAll(a => { bool b = filter(a); if (b) a.Detach(this); return b; });
            UpdateDrawOrder();
        }
        public virtual void ClearComponents()
        {
            this._InteractiveComponents.TrueForAll(a => { a.Detach(this); return true; });
            this._StaticComponents.TrueForAll(a => { a.Detach(this); return true; });
            this._InteractiveComponents.Clear();
            this._StaticComponents.Clear();
            UpdateDrawOrder();
        }
        public virtual bool AcceptsComponent(IMenuComponent component)
        {
            return true;
        }
        public virtual Rectangle EventRegion
        {
            get { return new Rectangle(this.Area.X + zoom10, this.Area.Y + zoom10, this.Area.Width - zoom20, this.Area.Height - zoom20); }
        }
        public virtual Rectangle ZoomEventRegion
        {
            get { return new Rectangle((this.Area.X + zoom10)/Game1.pixelZoom,(this.Area.Y + zoom10)/Game1.pixelZoom,(this.Area.Width - zoom20)/Game1.pixelZoom,(this.Area.Height - zoom20)/Game1.pixelZoom); }
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
            Point o = new Point(this.Area.X + zoom10, this.Area.Y + zoom10);
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
            this.HoverInElement.LeftHeld(new Point(x, y), new Point(this.Area.X + zoom10, this.Area.Y + zoom10));
        }
        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            base.receiveLeftClick(x, y, playSound);
            Point p = new Point(x, y);
            Point o = new Point(this.Area.X + zoom10, this.Area.Y + zoom10);
            if(this.FloatingComponent != null && this.FloatingComponent.InBounds(p,o))
            {
                this.FloatingComponent.LeftClick(p, o);
                return;
            }
            foreach (IInteractiveMenuComponent el in this.EventOrder)
            {
                if (el.InBounds(p, o))
                {
                    GiveFocus(el);
                    el.LeftClick(p, o);
                    return;
                }
            }
            ResetFocus();
        }
        public void ExitMenu()
        {
            exitThisMenu(true);
        }
        public override void receiveRightClick(int x, int y, bool playSound = true)
        {
            Point p = new Point(x, y);
            Point o = new Point(this.Area.X + zoom10, this.Area.Y + zoom10);
            if (this.FloatingComponent != null && this.FloatingComponent.InBounds(p, o))
            {
                this.FloatingComponent.RightClick(p, o);
                return;
            }
            foreach (IInteractiveMenuComponent el in this.EventOrder)
            {
                if (el.InBounds(p, o))
                {
                    GiveFocus(el);
                    this.FocusElement = el;
                    el.RightClick(p, o);
                    return;
                }
            }
            ResetFocus();
        }
        public override void performHoverAction(int x, int y)
        {
            base.performHoverAction(x, y);
            if (!this.Area.Contains(x, y) || this.Hold)
                return;
            Point p = new Point(x, y);
            Point o = new Point(this.Area.X + zoom10, this.Area.Y + zoom10);
            if (this.HoverInElement != null && !this.HoverInElement.InBounds(p, o))
            {
                this.HoverInElement.HoverOut(p, o);
                this.HoverInElement = null;
            }
            if (this.FloatingComponent !=null && this.FloatingComponent.InBounds(p, o))
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
            Point o = new Point(this.Area.X + zoom10, this.Area.Y + zoom10);
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
            if (this.FloatingComponent != null)
                this.FloatingComponent.Update(time);
            foreach (IMenuComponent el in this.DrawOrder)
                el.Update(time);
        }
        public override void draw(SpriteBatch b)
        {
            if (this.DrawChrome)
                DrawMenuRect(b, this.Area.X, this.Area.Y, this.Area.Width, this.Area.Height);
            Point o = new Point(this.Area.X + zoom10, this.Area.Y + zoom10);
            foreach (IMenuComponent el in this.DrawOrder)
                el.Draw(b, o);
            if (this.FloatingComponent != null)
                this.FloatingComponent.Draw(b, o);
            base.draw(b);
            drawMouse(b);
        }
    }
}
