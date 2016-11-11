using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewValley;
using StardewValley.Menus;

namespace Entoarox.Framework.Menus
{
    public class FrameworkMenu : IClickableMenu, IComponentCollection
    {
        protected List<IMenuComponent> StaticComponents = new List<IMenuComponent>();
        protected List<IInteractiveMenuComponent> InteractiveComponents = new List<IInteractiveMenuComponent>();
        protected Rectangle Area;
        protected IInteractiveMenuComponent HoverInElement;
        protected bool DrawChrome;
        protected Vector2 Center;
        protected bool Hold;
        public FrameworkMenu(Rectangle area, bool showCloseButton = true, bool drawChrome = true)
        {
            DrawChrome = drawChrome;
            Area = new Rectangle(area.X * Game1.pixelZoom, area.Y * Game1.pixelZoom, area.Width * Game1.pixelZoom, area.Height * Game1.pixelZoom);
            initialize(Area.X, Area.Y, Area.Width, Area.Height, showCloseButton);
        }
        public FrameworkMenu(Point size, bool showCloseButton = true, bool drawChrome = true)
        {
            DrawChrome = drawChrome;
            Center = Utility.getTopLeftPositionForCenteringOnScreen(size.X * Game1.pixelZoom, size.Y * Game1.pixelZoom, 0, 0);
            Area = new Rectangle((int)Center.X, (int)Center.Y, size.X * Game1.pixelZoom, size.Y * Game1.pixelZoom);
            initialize(Area.X, Area.Y, Area.Width, Area.Height, showCloseButton);
        }
        public void AddComponent(IMenuComponent component)
        {
            if (component is IInteractiveMenuComponent)
                InteractiveComponents.Add(component as IInteractiveMenuComponent);
            else
                StaticComponents.Add(component);
        }
        public void RemoveComponent(IMenuComponent component)
        {
            if (component is IInteractiveMenuComponent)
                InteractiveComponents.Remove(component as IInteractiveMenuComponent);
            else
                StaticComponents.Remove(component);
        }
        public void RemoveComponents<T>() where T : IMenuComponent
        {
            InteractiveComponents.RemoveAll((a)=> a.GetType()==typeof(T));
            StaticComponents.RemoveAll((a) => a.GetType() == typeof(T));
        }
        public void RemoveComponents(Predicate<IMenuComponent> filter)
        {
            InteractiveComponents.RemoveAll(filter);
            StaticComponents.RemoveAll(filter);
        }
        public void ClearComponents()
        {
            InteractiveComponents.Clear();
            StaticComponents.Clear();
        }
        public bool AcceptsComponent(IMenuComponent component)
        {
            return true;
        }
        public override void releaseLeftClick(int x, int y)
        {
            if (HoverInElement == null)
                return;
            Point p = new Point(x, y);
            Point o = new Point(Area.X + 5 * Game1.pixelZoom, Area.Y + 5 * Game1.pixelZoom);
            HoverInElement.LeftUp(p, o, this, this);
            Hold = false;
            if (HoverInElement.InBounds(p, o))
                return;
            HoverInElement.HoverOut(p, o, this, this);
            HoverInElement = null;
        }
        public override void leftClickHeld(int x, int y)
        {
            if (HoverInElement == null)
                return;
            Hold = true;
            HoverInElement.LeftHeld(new Point(x, y), new Point(Area.X + 5 * Game1.pixelZoom, Area.Y + 5 * Game1.pixelZoom), this, this);
        }
        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            base.receiveLeftClick(x, y, playSound);
            Point p = new Point(x, y);
            Point o = new Point(Area.X + 5 * Game1.pixelZoom, Area.Y + 5 * Game1.pixelZoom);
            foreach (IInteractiveMenuComponent el in InteractiveComponents)
                if (el.InBounds(p,o))
                    el.LeftClick(p,o, this, this);
        }
        public override void receiveRightClick(int x, int y, bool playSound = true)
        {
            Point p = new Point(x, y);
            Point o = new Point(Area.X + 5 * Game1.pixelZoom, Area.Y + 5 * Game1.pixelZoom);
            foreach (IInteractiveMenuComponent el in InteractiveComponents)
                if (el.InBounds(p,o))
                    el.RightClick(p, o, this, this);
        }
        public override void performHoverAction(int x, int y)
        {
            base.performHoverAction(x, y);
            if (!Area.Contains(x, y))
                return;
            Point p = new Point(x, y);
            Point o = new Point(Area.X + 5 * Game1.pixelZoom, Area.Y + 5 * Game1.pixelZoom);
            if (!Hold && HoverInElement != null && !HoverInElement.InBounds(p,o))
            {
                HoverInElement.HoverOut(p,o, this, this);
                HoverInElement = null;
            }
            foreach (IInteractiveMenuComponent el in InteractiveComponents)
                if (el.InBounds(p,o))
                {
                    if(HoverInElement==null)
                    {
                        HoverInElement = el;
                        el.HoverIn(p,o, this, this);
                    }
                    el.HoverOver(p,o, this, this);
                }
        }
        public override void receiveScrollWheelAction(int direction)
        {
            base.receiveScrollWheelAction(direction);
            Point p = Game1.getMousePosition();
            Point o = new Point(Area.X + 5 * Game1.pixelZoom, Area.Y + 5 * Game1.pixelZoom);
            foreach (IInteractiveMenuComponent el in InteractiveComponents)
                el.Scroll(direction, p,o, this, this);
        }
        public override void update(GameTime time)
        {
            base.update(time);
            foreach (IInteractiveMenuComponent el in InteractiveComponents)
                el.Update(time, this, this);
        }
        public override void draw(SpriteBatch b)
        {
            if (DrawChrome)
                //Game1.drawDialogueBox(Area.X,Area.Y,Area.Width,Area.Height, false, true);
                drawTextureBox(b, Area.X, Area.Y, Area.Width, Area.Height, Color.White);
            Point o = new Point(Area.X + 5 * Game1.pixelZoom, Area.Y + 5 * Game1.pixelZoom);
            foreach (IMenuComponent el in StaticComponents)
                el.Draw(b, o);
            foreach (IInteractiveMenuComponent el in InteractiveComponents)
                el.Draw(b, o);
            base.draw(b);
            drawMouse(b);
        }
    }
}
