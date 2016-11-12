using System;
using System.Linq;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewValley;
using StardewValley.Menus;
using Microsoft.Xna.Framework.Input;

namespace Entoarox.Framework.Menus
{
    public class FrameworkMenu : IClickableMenu, IComponentCollection
    {
        protected List<IMenuComponent> StaticComponents = new List<IMenuComponent>();
        protected List<IInteractiveMenuComponent> InteractiveComponents = new List<IInteractiveMenuComponent>();
        protected Rectangle Area;
        protected IInteractiveMenuComponent HoverInElement=null;
        protected IInteractiveMenuComponent FocusElement=null;
        protected bool DrawChrome;
        protected Vector2 Center;
        protected bool Hold;
        protected Dictionary<string, int> StaticDrawMap = new Dictionary<string, int>();
        protected Dictionary<string, int> InteractiveDrawMap = new Dictionary<string, int>();
        protected List<string> StaticDrawIndex = new List<string>();
        protected List<string> InteractiveDrawIndex = new List<string>();
        protected List<string> StaticDrawIndexInverted = new List<string>();
        protected List<string> InteractiveDrawIndexInverted = new List<string>();
        public bool TextboxActive = false;
        protected string PointConverter(Point p)
        {
            return p.X.ToString() + ',' + p.Y.ToString();
        }
        protected void UpdateDrawOrder()
        {
            StaticDrawMap.Clear();
            InteractiveDrawMap.Clear();
            StaticDrawIndex.Clear();
            InteractiveDrawIndex.Clear();
            StaticDrawIndexInverted.Clear();
            InteractiveDrawIndexInverted.Clear();
            List<Point> Points = new List<Point>();
            List<string> SPoints = new List<string>();
            for(int c=0;c<StaticComponents.Count;c++)
            {
                IMenuComponent imc = StaticComponents[c];
                Point p = imc.GetPosition();
                Points.Add(p);
                StaticDrawMap.Add(PointConverter(p), c);
            }
            SPoints = Points.OrderBy(p => p.Y).ThenBy(p => p.X).ToList().ConvertAll(PointConverter);
            StaticDrawIndex.AddRange(SPoints);
            SPoints.Reverse();
            StaticDrawIndexInverted.AddRange(SPoints);
            Points.Clear();
            SPoints.Clear();
            for (int c = 0; c < InteractiveComponents.Count; c++)
            {
                IInteractiveMenuComponent imc = InteractiveComponents[c];
                Point p = imc.GetPosition();
                Points.Add(p);
                InteractiveDrawMap.Add(p.X.ToString() + ',' + p.Y.ToString(), c);
            }
            SPoints = Points.OrderBy(p => p.Y).ThenBy(p => p.X).ToList().ConvertAll(PointConverter);
            InteractiveDrawIndex.AddRange(SPoints);
            SPoints.Reverse();
            InteractiveDrawIndexInverted.AddRange(SPoints);
        }
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
        public void ResetFocus()
        {
            if (FocusElement == null)
                return;
            FocusElement.FocusLost(this, this);
            FocusElement = null;
        }
        public void AddComponent(IMenuComponent component)
        {
            if (component is IInteractiveMenuComponent)
                InteractiveComponents.Add(component as IInteractiveMenuComponent);
            else
                StaticComponents.Add(component);
            UpdateDrawOrder();
        }
        public void RemoveComponent(IMenuComponent component)
        {
            if (component is IInteractiveMenuComponent)
                InteractiveComponents.Remove(component as IInteractiveMenuComponent);
            else
                StaticComponents.Remove(component);
            UpdateDrawOrder();
        }
        public void RemoveComponents<T>() where T : IMenuComponent
        {
            InteractiveComponents.RemoveAll((a)=> a.GetType()==typeof(T));
            StaticComponents.RemoveAll((a) => a.GetType() == typeof(T));
            UpdateDrawOrder();
        }
        public void RemoveComponents(Predicate<IMenuComponent> filter)
        {
            InteractiveComponents.RemoveAll(filter);
            StaticComponents.RemoveAll(filter);
            UpdateDrawOrder();
        }
        public void ClearComponents()
        {
            InteractiveComponents.Clear();
            StaticComponents.Clear();
            UpdateDrawOrder();
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
            foreach(string pos in InteractiveDrawIndex)
            {
                IInteractiveMenuComponent el = InteractiveComponents[InteractiveDrawMap[pos]];
                if (el.InBounds(p, o))
                {
                    if (FocusElement != null && FocusElement != el)
                        FocusElement.FocusLost(this,this);
                    FocusElement = el;
                    el.LeftClick(p, o, this, this);
                    return;
                }
            }
            if (FocusElement == null)
                return;
            FocusElement.FocusLost(this,this);
            FocusElement = null;
        }
        public override void receiveRightClick(int x, int y, bool playSound = true)
        {
            Point p = new Point(x, y);
            Point o = new Point(Area.X + 5 * Game1.pixelZoom, Area.Y + 5 * Game1.pixelZoom);
            foreach (string pos in InteractiveDrawIndex)
            {
                IInteractiveMenuComponent el = InteractiveComponents[InteractiveDrawMap[pos]];
                if (el.InBounds(p, o))
                {
                    if (FocusElement != null && FocusElement!=el)
                        FocusElement.FocusLost(this,this);
                    FocusElement = el;
                    el.RightClick(p, o, this, this);
                    return;
                }
            }
            if (FocusElement == null)
                return;
            FocusElement.FocusLost(this,this);
            FocusElement = null;
        }
        public override void performHoverAction(int x, int y)
        {
            base.performHoverAction(x, y);
            if (!Area.Contains(x, y) || Hold)
                return;
            Point p = new Point(x, y);
            Point o = new Point(Area.X + 5 * Game1.pixelZoom, Area.Y + 5 * Game1.pixelZoom);
            if (HoverInElement != null && !HoverInElement.InBounds(p, o))
            {
                HoverInElement.HoverOut(p, o, this, this);
                HoverInElement = null;
            }
            foreach (string pos in InteractiveDrawIndex)
            {
                IInteractiveMenuComponent el = InteractiveComponents[InteractiveDrawMap[pos]];
                if (el.InBounds(p, o))
                {
                    if (HoverInElement == null)
                    {
                        HoverInElement = el;
                        el.HoverIn(p, o, this, this);
                    }
                    el.HoverOver(p, o, this, this);
                    break;
                }
            }
        }
        public override void receiveScrollWheelAction(int direction)
        {
            base.receiveScrollWheelAction(direction);
            Point p = Game1.getMousePosition();
            Point o = new Point(Area.X + 5 * Game1.pixelZoom, Area.Y + 5 * Game1.pixelZoom);
            foreach (string pos in InteractiveDrawIndex)
                InteractiveComponents[InteractiveDrawMap[pos]].Scroll(direction, p, o, this, this);
        }
        public override void receiveKeyPress(Keys key)
        {
            if(!TextboxActive)
                base.receiveKeyPress(key);
        }
        public override void update(GameTime time)
        {
            base.update(time);
            foreach (string pos in InteractiveDrawIndex)
                InteractiveComponents[InteractiveDrawMap[pos]].Update(time, this, this);
        }
        public override void draw(SpriteBatch b)
        {
            if (DrawChrome)
                //Game1.drawDialogueBox(Area.X,Area.Y,Area.Width,Area.Height, false, true);
                drawTextureBox(b, Area.X, Area.Y, Area.Width, Area.Height, Color.White);
            Point o = new Point(Area.X + 5 * Game1.pixelZoom, Area.Y + 5 * Game1.pixelZoom);
            foreach (string pos in StaticDrawIndexInverted)
                StaticComponents[StaticDrawMap[pos]].Draw(b, o);
            foreach (string pos in InteractiveDrawIndexInverted)
                InteractiveComponents[InteractiveDrawMap[pos]].Draw(b, o);
            base.draw(b);
            drawMouse(b);
        }
    }
}
