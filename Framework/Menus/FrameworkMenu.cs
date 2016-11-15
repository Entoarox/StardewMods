using System;
using System.Linq;
using System.Diagnostics;
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
        protected List<IMenuComponent> _StaticComponents = new List<IMenuComponent>();
        protected List<IInteractiveMenuComponent> _InteractiveComponents = new List<IInteractiveMenuComponent>();
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
        public List<IMenuComponent> StaticComponents { get { return new List<IMenuComponent>(_StaticComponents); } }
        public List<IInteractiveMenuComponent> InteractiveComponents { get { return new List<IInteractiveMenuComponent>(_InteractiveComponents); } }
        protected readonly static Rectangle tl = new Rectangle(0, 0, 64, 64);
        protected readonly static Rectangle tc = new Rectangle(128, 0, 64, 64);
        protected readonly static Rectangle tr = new Rectangle(192, 0, 64, 64);
        protected readonly static Rectangle ml = new Rectangle(0, 128, 64, 64);
        protected readonly static Rectangle mr = new Rectangle(192, 128, 64, 64);
        protected readonly static Rectangle br = new Rectangle(192, 192, 64, 64);
        protected readonly static Rectangle bl = new Rectangle(0, 192, 64, 64);
        protected readonly static Rectangle bc = new Rectangle(128, 192, 64, 64);
        protected readonly static Rectangle bg = new Rectangle(64, 128, 64, 64);
        protected readonly static int zoom2 = Game1.pixelZoom * 2;
        protected readonly static int zoom3 = Game1.pixelZoom * 3;
        protected readonly static int zoom4 = Game1.pixelZoom * 4;
        protected readonly static int zoom6 = Game1.pixelZoom * 6;
        protected readonly static int zoom10 = Game1.pixelZoom * 10;
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
            for(int c=0;c<_StaticComponents.Count;c++)
            {
                IMenuComponent imc = _StaticComponents[c];
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
            for (int c = 0; c < _InteractiveComponents.Count; c++)
            {
                IInteractiveMenuComponent imc = _InteractiveComponents[c];
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
        public void GiveFocus(IInteractiveMenuComponent component)
        {
            if (!_InteractiveComponents.Contains(component) || component == FocusElement)
                return;
            ResetFocus();
            FocusElement = component;
            component.FocusGained(this, this);
        }
        public void AddComponent(IMenuComponent component)
        {
            if (component is IInteractiveMenuComponent)
                _InteractiveComponents.Add(component as IInteractiveMenuComponent);
            else
                _StaticComponents.Add(component);
            UpdateDrawOrder();
        }
        public void RemoveComponent(IMenuComponent component)
        {
            if (component is IInteractiveMenuComponent)
                _InteractiveComponents.Remove(component as IInteractiveMenuComponent);
            else
                _StaticComponents.Remove(component);
            UpdateDrawOrder();
        }
        public void RemoveComponents<T>() where T : IMenuComponent
        {
            _InteractiveComponents.RemoveAll((a)=> a.GetType()==typeof(T));
            _StaticComponents.RemoveAll((a) => a.GetType() == typeof(T));
            UpdateDrawOrder();
        }
        public void RemoveComponents(Predicate<IMenuComponent> filter)
        {
            _InteractiveComponents.RemoveAll(filter);
            _StaticComponents.RemoveAll(filter);
            UpdateDrawOrder();
        }
        public void ClearComponents()
        {
            _InteractiveComponents.Clear();
            _StaticComponents.Clear();
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
            Point o = new Point(Area.X + zoom10, Area.Y + zoom10);
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
            HoverInElement.LeftHeld(new Point(x, y), new Point(Area.X + zoom10, Area.Y + zoom10), this, this);
        }
        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            base.receiveLeftClick(x, y, playSound);
            Point p = new Point(x, y);
            Point o = new Point(Area.X + zoom10, Area.Y + zoom10);
            foreach (string pos in InteractiveDrawIndex)
            {
                IInteractiveMenuComponent el = _InteractiveComponents[InteractiveDrawMap[pos]];
                if (el.InBounds(p, o))
                {
                    GiveFocus(el);
                    el.LeftClick(p, o, this, this);
                    return;
                }
            }
            ResetFocus();
        }
        public override void receiveRightClick(int x, int y, bool playSound = true)
        {
            Point p = new Point(x, y);
            Point o = new Point(Area.X + zoom10, Area.Y + zoom10);
            foreach (string pos in InteractiveDrawIndex)
            {
                IInteractiveMenuComponent el = _InteractiveComponents[InteractiveDrawMap[pos]];
                if (el.InBounds(p, o))
                {
                    GiveFocus(el);
                    FocusElement = el;
                    return;
                }
            }
            ResetFocus();
        }
        public override void performHoverAction(int x, int y)
        {
            base.performHoverAction(x, y);
            if (!Area.Contains(x, y) || Hold)
                return;
            Point p = new Point(x, y);
            Point o = new Point(Area.X + zoom10, Area.Y + zoom10);
            if (HoverInElement != null && !HoverInElement.InBounds(p, o))
            {
                HoverInElement.HoverOut(p, o, this, this);
                HoverInElement = null;
            }
            foreach (string pos in InteractiveDrawIndex)
            {
                IInteractiveMenuComponent el = _InteractiveComponents[InteractiveDrawMap[pos]];
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
            Point o = new Point(Area.X + zoom10, Area.Y + zoom10);
            foreach (string pos in InteractiveDrawIndex)
                _InteractiveComponents[InteractiveDrawMap[pos]].Scroll(direction, p, o, this, this);
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
                _InteractiveComponents[InteractiveDrawMap[pos]].Update(time, this, this);
        }
        //protected Stopwatch Watch=new Stopwatch();
        //protected int count = 0;
        public override void draw(SpriteBatch b)
        {
            //Watch.Start();
            if (DrawChrome)
                DrawMenuRect(b, Area.X, Area.Y, Area.Width, Area.Height);
            Point o = new Point(Area.X + zoom10, Area.Y + zoom10);
            foreach (string pos in StaticDrawIndexInverted)
                _StaticComponents[StaticDrawMap[pos]].Draw(b, o);
            foreach (string pos in InteractiveDrawIndexInverted)
                _InteractiveComponents[InteractiveDrawMap[pos]].Draw(b, o);
            base.draw(b);
            drawMouse(b);
            /*
            Watch.Stop();
            if (count == 60)
            {
                Console.WriteLine("Total draw time over last 60 frames: " + Watch.ElapsedMilliseconds.ToString() + "ms");
                Watch.Reset();
                count=0;
            }
            else
                count++;
            */
        }
    }
}
