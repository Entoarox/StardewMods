using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace Entoarox.Framework.UI
{
    public class TablistComponent : BaseInteractiveMenuComponent, IComponentContainer
    {
        /*********
        ** Fields
        *********/
        protected static Rectangle Tab = new Rectangle(16, 368, 16, 16);
        protected IInteractiveMenuComponent HoverElement;
        protected IInteractiveMenuComponent FocusElement;
        protected bool Hold = false;
        protected FrameworkMenu AttachedMenu;
        protected List<TabInfo> Tabs = new List<TabInfo>();
        protected int Current = -1;
        protected IInteractiveMenuComponent CurrentTab;


        /*********
        ** Accessors
        *********/
        public int Index
        {
            get => this.Current;
            set
            {
                if (value >= 0 && value < this.Tabs.Count)
                {
                    this.Current = value;
                    this.CurrentTab = this.Tabs[value].Component;
                }
            }
        }

        // IComponentCollection proxy
        public Rectangle EventRegion => new Rectangle(this.Area.X + BaseMenuComponent.Zoom5, this.Area.Y + BaseMenuComponent.Zoom22, this.Area.Width - BaseMenuComponent.Zoom10, this.Area.Height - BaseMenuComponent.Zoom28);

        public Rectangle ZoomEventRegion => new Rectangle((this.Area.X + BaseMenuComponent.Zoom5) / Game1.pixelZoom, (this.Area.Y + BaseMenuComponent.Zoom22) / Game1.pixelZoom, (this.Area.Width - BaseMenuComponent.Zoom14) / Game1.pixelZoom, (this.Area.Height - BaseMenuComponent.Zoom28) / Game1.pixelZoom);


        /*********
        ** Public methods
        *********/
        public TablistComponent(Rectangle area)
        {
            this.SetScaledArea(area);
        }

        public void AddTab<T>(int icon, T collection) where T : IComponentCollection, IInteractiveMenuComponent
        {
            this.AddTab(Game1.objectSpriteSheet, icon, collection);
        }

        public void AddTab<T>(Texture2D texture, int icon, T collection) where T : IComponentCollection, IInteractiveMenuComponent
        {
            this.AddTab(texture, Game1.getSourceRectForStandardTileSheet(texture, icon, 16, 16), collection);
        }

        public void AddTab<T>(Texture2D texture, Rectangle crop, T collection) where T : IComponentCollection, IInteractiveMenuComponent
        {
            this.Tabs.Add(new TabInfo(collection, texture, crop));
            collection.Attach(this);
            if (this.CurrentTab == null)
            {
                this.Current = 0;
                this.CurrentTab = collection;
            }
        }

        // IInteractiveMenuComponent
        public override void FocusLost()
        {
            this.ResetFocus();
        }

        public override void LeftUp(Point p, Point o)
        {
            this.CurrentTab?.LeftUp(p, new Point(o.X + this.Area.X + BaseMenuComponent.Zoom5, o.Y + this.Area.Y + BaseMenuComponent.Zoom22));
        }

        public override void LeftHeld(Point p, Point o)
        {
            this.CurrentTab?.LeftHeld(p, new Point(o.X + this.Area.X + BaseMenuComponent.Zoom5, o.Y + this.Area.Y + BaseMenuComponent.Zoom22));
        }

        public override void LeftClick(Point p, Point o)
        {
            if (p.Y - o.Y - this.Area.Y < BaseMenuComponent.Zoom16)
            {
                int pos = (p.X - o.X - this.Area.X - BaseMenuComponent.Zoom4) / BaseMenuComponent.Zoom16;
                if (pos < 0 || pos >= this.Tabs.Count || this.Current == pos)
                    return;
                this.Current = pos;
                this.CurrentTab = this.Tabs[pos].Component;
                Game1.playSound("smallSelect");
            }
            else
                this.CurrentTab?.LeftClick(p, new Point(o.X + this.Area.X + BaseMenuComponent.Zoom5, o.Y + this.Area.Y + BaseMenuComponent.Zoom22));
        }

        public override void RightClick(Point p, Point o)
        {
            this.CurrentTab?.RightClick(p, new Point(o.X + this.Area.X + BaseMenuComponent.Zoom5, o.Y + this.Area.Y + BaseMenuComponent.Zoom22));
        }

        public override void HoverOver(Point p, Point o)
        {
            this.CurrentTab?.HoverOver(p, new Point(o.X + this.Area.X + BaseMenuComponent.Zoom5, o.Y + this.Area.Y + BaseMenuComponent.Zoom22));
        }

        public override bool Scroll(int d, Point p, Point o)
        {
            return this.CurrentTab?.Scroll(d, p, new Point(o.X + this.Area.X + BaseMenuComponent.Zoom5, o.Y + this.Area.Y + BaseMenuComponent.Zoom22)) ?? false;
        }

        public override void Update(GameTime t)
        {
            this.CurrentTab?.Update(t);
        }

        public override void Draw(SpriteBatch b, Point o)
        {
            if (!this.Visible)
                return;
            // Draw chrome
            FrameworkMenu.DrawMenuRect(b, o.X + this.Area.X - BaseMenuComponent.Zoom2, o.Y + this.Area.Y + BaseMenuComponent.Zoom15, this.Area.Width, this.Area.Height - BaseMenuComponent.Zoom15);
            // Draw tabs
            for (int c = 0; c < this.Tabs.Count; c++)
            {
                b.Draw(Game1.mouseCursors, new Rectangle(o.X + this.Area.X + BaseMenuComponent.Zoom4 + c * BaseMenuComponent.Zoom16, o.Y + this.Area.Y + (c == this.Current ? BaseMenuComponent.Zoom2 : 0), BaseMenuComponent.Zoom16, BaseMenuComponent.Zoom16), TablistComponent.Tab, Color.White);
                b.Draw(this.Tabs[c].Texture, new Rectangle(o.X + this.Area.X + BaseMenuComponent.Zoom8 + c * BaseMenuComponent.Zoom16, o.Y + this.Area.Y + BaseMenuComponent.Zoom5 + (c == this.Current ? BaseMenuComponent.Zoom2 : 0), BaseMenuComponent.Zoom8, BaseMenuComponent.Zoom8), this.Tabs[c].Crop, Color.White);
            }

            // Draw body
            this.CurrentTab?.Draw(b, new Point(o.X + this.Area.X + BaseMenuComponent.Zoom5, o.Y + this.Area.Y + BaseMenuComponent.Zoom22));
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
            if (!this.Tabs.Exists(x => x.Component == component) || component == this.FocusElement)
                return;
            this.Parent.GiveFocus(this);
            this.ResetFocus();
            this.FocusElement = component;
            if (this.FocusElement is IKeyboardComponent)
                Game1.keyboardDispatcher.Subscriber = new KeyboardSubscriberProxy((IKeyboardComponent) this.FocusElement);
            component.FocusGained();
        }

        protected class TabInfo
        {
            public IInteractiveMenuComponent Component;
            public Texture2D Texture;
            public Rectangle Crop;

            public TabInfo(IInteractiveMenuComponent component, Texture2D texture, Rectangle crop)
            {
                this.Component = component;
                this.Texture = texture;
                this.Crop = crop;
            }
        }
    }
}
