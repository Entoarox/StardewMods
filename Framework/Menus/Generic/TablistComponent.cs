using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewValley;
using StardewValley.Menus;

namespace Entoarox.Framework.Menus
{
    public class TablistComponent : BaseInteractiveMenuComponent, IComponentContainer
    {
        protected class TabInfo
        {
            public IInteractiveMenuComponent Component;
            public int Icon;
            public TabInfo(IInteractiveMenuComponent component, int icon)
            {
                Component = component;
                Icon = icon;
            }
        }
        protected static Rectangle Tab = new Rectangle(16, 368, 16, 16);

        protected IInteractiveMenuComponent HoverElement;
        protected IInteractiveMenuComponent FocusElement;
        protected bool Hold = false;

        protected FrameworkMenu AttachedMenu;

        protected List<TabInfo> Tabs=new List<TabInfo>();
        protected int Current=-1;
        protected IInteractiveMenuComponent CurrentTab=null;

        public int Index
        {
            get
            {
                return Current;
            }
            set
            {
                if(value >= 0 && value < Tabs.Count)
                {
                    Current = value;
                    CurrentTab = Tabs[value].Component;
                }
            }
        }

        public TablistComponent(Rectangle area)
        {
            SetScaledArea(area);
        }
        public void AddTab<T>(int icon, T collection) where T : IComponentCollection, IInteractiveMenuComponent
        {
            Tabs.Add(new TabInfo(collection, icon));
            collection.Attach(this);
            if(CurrentTab==null)
            {
                Current = 0;
                CurrentTab = collection;
            }
        }
        // IInteractiveMenuComponent
        public override void FocusLost(IComponentContainer c, FrameworkMenu m)
        {
            ResetFocus();
        }
        public override void LeftUp(Point p, Point o, IComponentContainer c, FrameworkMenu m)
        {
            CurrentTab?.LeftUp(p, new Point(o.X + Area.X + zoom5, o.Y + Area.Y + zoom22), c, m);
        }
        public override void LeftHeld(Point p, Point o, IComponentContainer c, FrameworkMenu m)
        {
            CurrentTab?.LeftHeld(p, new Point(o.X + Area.X + zoom5, o.Y + Area.Y + zoom22), c, m);
        }
        public override void LeftClick(Point p, Point o, IComponentContainer c, FrameworkMenu m)
        {
            if (p.Y - o.Y - Area.Y < zoom16)
            {
                int pos = (p.X - o.X - Area.X - zoom4) / zoom16;
                if (pos < 0 || pos >= Tabs.Count || Current == pos)
                    return;
                Current = pos;
                CurrentTab = Tabs[pos].Component;
                Game1.playSound("smallSelect");
            }
            else
                CurrentTab?.LeftClick(p, new Point(o.X + Area.X + zoom5, o.Y + Area.Y + zoom22), c, m);
        }
        public override void RightClick(Point p, Point o, IComponentContainer c, FrameworkMenu m)
        {
            CurrentTab?.RightClick(p, new Point(o.X + Area.X + zoom5, o.Y + Area.Y + zoom22), c, m);
        }
        public override void HoverOver(Point p, Point o, IComponentContainer c, FrameworkMenu m)
        {
            CurrentTab?.HoverOver(p, new Point(o.X + Area.X + zoom5, o.Y + Area.Y + zoom22), c, m);
        }
        public override void Scroll(int d, Point p, Point o, IComponentContainer c, FrameworkMenu m)
        {
            CurrentTab?.Scroll(d, p, new Point(o.X + Area.X + zoom5, o.Y + Area.Y + zoom22), c, m);
        }
        public override void Update(GameTime t, IComponentContainer c, FrameworkMenu m)
        {
            CurrentTab?.Update(t, c, m);
        }
        public override void Draw(SpriteBatch b, Point o)
        {
            // Draw chrome
            FrameworkMenu.DrawMenuRect(b, o.X + Area.X - zoom2, o.Y + Area.Y + zoom15, Area.Width, Area.Height - zoom15);
            // Draw tabs
            for(var c=0;c<Tabs.Count;c++)
            {
                b.Draw(Game1.mouseCursors, new Rectangle(o.X + Area.X + zoom4 + c * zoom16, o.Y + Area.Y + (c == Current ? zoom2 : 0), zoom16, zoom16), Tab, Color.White);
                b.Draw(Game1.objectSpriteSheet, new Rectangle(o.X + Area.X + zoom8 + c * zoom16, o.Y + Area.Y + zoom5 + (c == Current ? zoom2 : 0), zoom8, zoom8), Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, Tabs[c].Icon, 16, 16), Color.White);
            }
            // Draw body
            CurrentTab?.Draw(b, new Point(o.X + Area.X + zoom5, o.Y + Area.Y + zoom22));
        }
        // IComponentCollection proxy
        public Rectangle EventRegion
        {
            get
            {
                return new Rectangle(Area.X+zoom5, Area.Y + zoom22, Area.Width-zoom10, Area.Height - zoom28);
            }
        }
        public Rectangle ZoomEventRegion
        {
            get
            {
                return new Rectangle((Area.X + zoom5)/Game1.pixelZoom, (Area.Y + zoom22)/Game1.pixelZoom, (Area.Width - zoom14)/Game1.pixelZoom, (Area.Height - zoom28)/Game1.pixelZoom);
            }
        }
        public FrameworkMenu GetAttachedMenu()
        {
            return Parent.GetAttachedMenu();
        }
        public void ResetFocus()
        {
            if (FocusElement == null)
                return;
            FocusElement.FocusLost(this, Parent.GetAttachedMenu());
            FocusElement = null;
        }
        public void GiveFocus(IInteractiveMenuComponent component)
        {
            if (!Tabs.Exists(x => x.Component==component) || component == FocusElement)
                return;
            Parent.GiveFocus(this);
            ResetFocus();
            FocusElement = component;
            component.FocusGained(this, Parent.GetAttachedMenu());
        }
    }
}
