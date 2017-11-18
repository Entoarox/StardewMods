using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewValley;

namespace Entoarox.Framework.UI
{
    public class GenericCollectionComponent : BaseInteractiveMenuComponent, IComponentCollection
    {
        protected List<IMenuComponent> DrawOrder;
        protected List<IInteractiveMenuComponent> EventOrder;

        protected List<IMenuComponent> _StaticComponents = new List<IMenuComponent>();
        protected List<IInteractiveMenuComponent> _InteractiveComponents = new List<IInteractiveMenuComponent>();

        protected IInteractiveMenuComponent HoverElement;
        protected IInteractiveMenuComponent FocusElement;
        protected bool Hold = false;

        public List<IMenuComponent> StaticComponents { get { return new List<IMenuComponent>(this._StaticComponents); } }
        public List<IInteractiveMenuComponent> InteractiveComponents { get { return new List<IInteractiveMenuComponent>(this._InteractiveComponents); } }

        protected bool Center = false;

        protected GenericCollectionComponent()
        {

        }
        protected GenericCollectionComponent(List<IMenuComponent> components = null)
        {
            if (components != null)
                foreach (IMenuComponent c in components)
                    AddComponent(c);
        }
        public GenericCollectionComponent(Point size, List<IMenuComponent> components = null) : this(components)
        {
            this.Center = true;
            SetScaledArea(new Rectangle(0, 0, size.X, size.Y));
        }
        public GenericCollectionComponent(Rectangle area, List<IMenuComponent> components=null) : this(components)
        {
            SetScaledArea(area);
        }
        public override void OnAttach(IComponentContainer parent)
        {
            if (!this.Center)
                return;
            this.Area.X = (parent.EventRegion.Width - this.Area.Width) / 2;
            this.Area.Y = (parent.EventRegion.Height - this.Area.Height) / 2;
        }
        // IComponentCollection
        protected virtual void UpdateDrawOrder()
        {
            KeyValuePair<List<IInteractiveMenuComponent>, List<IMenuComponent>> sorted = FrameworkMenu.GetOrderedLists(this._StaticComponents, this._InteractiveComponents);
            this.DrawOrder = sorted.Value;
            this.EventOrder = sorted.Key;
        }
        public FrameworkMenu GetAttachedMenu()
        {
            return this.Parent.GetAttachedMenu();
        }
        public void ResetFocus()
        {
            if (this.FocusElement == null)
                return;
            this.FocusElement.FocusLost();
            if (this.FocusElement is IKeyboardComponent)
            {
                Game1.keyboardDispatcher.Subscriber.Selected = false;
                Game1.keyboardDispatcher.Subscriber = null;
            }
            this.FocusElement = null;
        }
        public void GiveFocus(IInteractiveMenuComponent component)
        {
            if (!this._InteractiveComponents.Contains(component) || component == this.FocusElement)
                return;
            this.Parent.GiveFocus(this);
            ResetFocus();
            this.FocusElement = component;
            if (this.FocusElement is IKeyboardComponent)
                Game1.keyboardDispatcher.Subscriber = new KeyboardSubscriberProxy((IKeyboardComponent)this.FocusElement);
            component.FocusGained();
        }
        public void AddComponent(IMenuComponent component)
        {
            if (component is IInteractiveMenuComponent)
                this._InteractiveComponents.Add(component as IInteractiveMenuComponent);
            else
                this._StaticComponents.Add(component);
            component.Attach(this);
            UpdateDrawOrder();
        }
        public void RemoveComponent(IMenuComponent component)
        {
            bool Removed = false;
            RemoveComponents(a => { bool b = a == component && !Removed; if (b) { Removed = true; a.Detach(this); } return b; });
        }
        public void RemoveComponents<T>() where T : IMenuComponent
        {
            RemoveComponents(a => a.GetType() == typeof(T));
        }
        public void RemoveComponents(Predicate<IMenuComponent> filter)
        {
            this._InteractiveComponents.RemoveAll(a => { bool b = filter(a);if (b)a.Detach(this); return b; });
            this._StaticComponents.RemoveAll(a => { bool b = filter(a); if (b) a.Detach(this); return b; });
            UpdateDrawOrder();
        }
        public void ClearComponents()
        {
            this._InteractiveComponents.TrueForAll(a => { a.Detach(this); return true; });
            this._StaticComponents.TrueForAll(a => { a.Detach(this); return true; });
            this._InteractiveComponents.Clear();
            this._StaticComponents.Clear();
            UpdateDrawOrder();
        }
        public bool AcceptsComponent(IMenuComponent component)
        {
            return true;
        }
        public Rectangle EventRegion
        {
            get { return this.Area; }
        }
        public Rectangle ZoomEventRegion
        {
            get { return new Rectangle(this.Area.X/Game1.pixelZoom, this.Area.Y/Game1.pixelZoom, this.Area.Width/Game1.pixelZoom, this.Area.Height/Game1.pixelZoom); }
        }
        // IInteractiveMenuComponent
        public override void FocusLost()
        {
            ResetFocus();
        }
        public override void LeftUp(Point p, Point o)
        {
            if (!this.Visible)
                return;
            if (this.HoverElement == null)
                return;
            Point o2 = new Point(this.Area.X + o.X, this.Area.Y+o.Y);
            this.HoverElement.LeftUp(p, o2);
            this.Hold = false;
            if (this.HoverElement.InBounds(p, o2))
                return;
            this.HoverElement.HoverOut(p, o2);
            this.HoverElement = null;
        }
        public override void LeftHeld(Point p, Point o)
        {
            if (!this.Visible)
                return;
            if (this.HoverElement == null)
                return;
            this.Hold = true;
            this.HoverElement.LeftHeld(p, new Point(this.Area.X + o.X, this.Area.Y + o.Y));
        }
        public override void LeftClick(Point p, Point o)
        {
            if (!this.Visible)
                return;
            Point o2 = new Point(this.Area.X + o.X, this.Area.Y + o.Y);
            foreach (IInteractiveMenuComponent el in this.EventOrder)
            {
                if (el.InBounds(p, o2))
                {
                    GiveFocus(el);
                    el.LeftClick(p, o2);
                    return;
                }
            }
            ResetFocus();
        }
        public override void RightClick(Point p, Point o)
        {
            if (!this.Visible)
                return;
            Point o2 = new Point(this.Area.X + o.X, this.Area.Y + o.Y);
            foreach (IInteractiveMenuComponent el in this.EventOrder)
            {
                if (el.InBounds(p, o2))
                {
                    GiveFocus(el);
                    this.FocusElement = el;
                    el.RightClick(p, o2);
                    return;
                }
            }
            ResetFocus();
        }
        public override void HoverOver(Point p, Point o)
        {
            if (!this.Visible || this.Hold)
                return;
            Point o2 = new Point(this.Area.X + o.X, this.Area.Y + o.Y);
            if (this.HoverElement != null && !this.HoverElement.InBounds(p, o2))
            {
                this.HoverElement.HoverOut(p, o2);
                this.HoverElement = null;
            }
            foreach (IInteractiveMenuComponent el in this.EventOrder)
            {
                if (el.InBounds(p, o2))
                {
                    if (this.HoverElement == null)
                    {
                        this.HoverElement = el;
                        el.HoverIn(p, o2);
                    }
                    el.HoverOver(p, o2);
                    break;
                }
            }
        }
        public override bool Scroll(int d, Point p, Point o)
        {
            if (!this.Visible)
                return false;
            Point o2 = new Point(this.Area.X + o.X, this.Area.Y + o.Y);
            foreach (IInteractiveMenuComponent el in this.EventOrder)
                if (el.InBounds(p, o) && el.Scroll(d, p, o))
                    return true;
            return false;
        }
        public override void Update(GameTime t)
        {
            if (!this.Visible)
                return;
            foreach(IMenuComponent el in this.DrawOrder)
                el.Update(t);
        }
        public override void Draw(SpriteBatch b, Point o)
        {
            if (!this.Visible)
                return;
            Point o2 = new Point(this.Area.X + o.X, this.Area.Y + o.Y);
            foreach(IMenuComponent el in this.DrawOrder)
                el.Draw(b, o2);
        }
    }
}
