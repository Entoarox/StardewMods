using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewValley;

namespace Entoarox.Framework.UI
{
    public class ScrollableCollectionComponent : GenericCollectionComponent
    {
        protected static Rectangle UpButton = new Rectangle(421, 459, 11, 12);
        protected static Rectangle DownButton = new Rectangle(421, 472, 11, 12);

        protected int ScrollOffset=0;
        protected int InnerHeight;
        protected int BarOffset;

        protected bool UpActive=false;
        protected bool DownActive=false;

        protected int Counter = 0;
        protected int Limiter = 20;
        public ScrollableCollectionComponent(Point size, List<IMenuComponent> components = null) : base(size, components)
        {

        }
        public ScrollableCollectionComponent(Rectangle area, List<IMenuComponent> components = null):base(area,components)
        {
            
        }
        protected override void UpdateDrawOrder()
        {
            base.UpdateDrawOrder();
            int height = this.Area.Height;
            foreach (IMenuComponent c in this.DrawOrder)
            {
                if (!c.Visible)
                    return;
                Rectangle r = c.GetRegion();
                int b = r.Y + r.Height;
                if (b > height)
                    height = b;
            }
            if (height > this.Area.Height)
                this.BarOffset = zoom12;
            else
                this.BarOffset = 0;
            this.InnerHeight = (int)Math.Ceiling((height - this.Area.Height) / (double)zoom10);
            // Remove components that do not intersect the collection viewport from both the draw and event order
            Rectangle self = new Rectangle(0, -(this.ScrollOffset * zoom10), this.Area.Width, this.Area.Height);
            this.DrawOrder = this.DrawOrder.FindAll(c => c.GetRegion().Intersects(self));
            this.EventOrder = this.EventOrder.FindAll(c => c.GetRegion().Intersects(self));
        }
        public override bool Scroll(int d, Point p, Point o)
        {
            if (!this.Visible)
                return false;
            if (base.Scroll(d, p, new Point(o.X, o.Y - this.ScrollOffset * zoom10)))
                return true;
            if (this.HoverElement != null)
                return false;
            int change = d / 120;
            int oldOffset = this.ScrollOffset;
            this.ScrollOffset = Math.Max(0, Math.Min(this.ScrollOffset - change, this.InnerHeight));
            if (oldOffset != this.ScrollOffset)
            {
                Game1.playSound("drumkit6");
                UpdateDrawOrder();
                return true;
            }
            return false;
        }
        public override void HoverOver(Point p, Point o)
        {
            Rectangle up = new Rectangle(this.Area.X + o.X + this.Area.Width - (this.UpActive ? zoom12 : zoom11 + zoom05), this.Area.Y + o.Y + (this.UpActive ? 0 : zoom05), this.UpActive ? zoom12 : zoom11, this.UpActive ? zoom13 : zoom12);
            Rectangle down = new Rectangle(this.Area.X + o.X + this.Area.Width - (this.DownActive ? zoom12 : zoom11 + zoom05), this.Area.Y + o.Y + this.Area.Height - zoom12 - (this.DownActive ? zoom05 : 0), this.DownActive ? zoom12 : zoom11, this.DownActive ? zoom13 : zoom12);
            this.UpActive = this.ScrollOffset > 0 && up.Contains(p);
            this.DownActive = this.ScrollOffset < this.InnerHeight && down.Contains(p);
            base.HoverOver(p, new Point(o.X, o.Y - this.ScrollOffset * zoom10));
        }
        protected void ArrowClick(Point p, Point o)
        {
            if (this.UpActive && this.ScrollOffset > 0)
            {
                this.ScrollOffset--;
                Game1.playSound("drumkit6");
            }
            else if (this.DownActive && this.ScrollOffset < this.InnerHeight)
            {
                this.ScrollOffset++;
                Game1.playSound("drumkit6");
            }
        }
        public override void LeftHeld(Point p, Point o)
        {
            base.LeftHeld(p, new Point(o.X,o.Y- this.ScrollOffset *zoom10));
            if (!this.UpActive && !this.DownActive)
                return;
            this.Counter++;
            if (this.Counter % this.Limiter != 0)
                return;
            this.Limiter = Math.Max(1, this.Limiter - 1);
            this.Counter = 0;
            ArrowClick(p,o);
        }
        public override void LeftUp(Point p, Point o)
        {
            this.Limiter = 10;
            this.Counter = 0;
            base.LeftUp(p, new Point(o.X, o.Y - this.ScrollOffset * zoom10));
        }
        public override void LeftClick(Point p, Point o)
        {
            base.LeftClick(p, new Point(o.X, o.Y - this.ScrollOffset * zoom10));
            this.Counter = 0;
            this.Limiter = 10;
            ArrowClick(p,o);
        }
        public override void Draw(SpriteBatch b, Point o)
        {
            if (!this.Visible)
                return;
            b.End();
            Rectangle reg = this.EventRegion;
            b.GraphicsDevice.ScissorRectangle = new Rectangle(reg.X+o.X,reg.Y+o.Y,reg.Width,reg.Height);
            b.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, new RasterizerState() { ScissorTestEnable = true });
            Point o2=new Point(o.X+reg.X, o.Y+reg.Y-(this.ScrollOffset * zoom10));
            foreach(IMenuComponent el in this.DrawOrder)
                el.Draw(b,o2);
            b.End();
            b.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null);

            // Scrollbar
            if (this.BarOffset == 0)
                return;
            // Up
            b.Draw(Game1.mouseCursors, new Rectangle(this.Area.X + o.X + this.Area.Width - (this.UpActive ? zoom12 : zoom11 + zoom05), this.Area.Y + o.Y + (this.UpActive ? 0 : zoom05), this.UpActive ? zoom12 : zoom11, this.UpActive ? zoom13 : zoom12), UpButton, Color.White * (this.ScrollOffset > 0 ? 1 : 0.5f));
            // down
            b.Draw(Game1.mouseCursors, new Rectangle(this.Area.X + o.X + this.Area.Width - (this.DownActive ? zoom12 : zoom11 + zoom05), this.Area.Y + o.Y + this.Area.Height - zoom12 - (this.DownActive ? zoom05 : 0), this.DownActive ? zoom12 : zoom11, this.DownActive ? zoom13 : zoom12), DownButton, Color.White * (this.ScrollOffset < this.InnerHeight ? 1 : 0.5f));
        }
    }
}
