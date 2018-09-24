using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace Entoarox.Framework.UI
{
    public class ScrollableCollectionComponent : GenericCollectionComponent
    {
        /*********
        ** Fields
        *********/
        protected static Rectangle UpButton = new Rectangle(421, 459, 11, 12);
        protected static Rectangle DownButton = new Rectangle(421, 472, 11, 12);
        protected int ScrollOffset;
        protected int InnerHeight;
        protected int BarOffset;
        protected bool UpActive;
        protected bool DownActive;
        protected int Counter;
        protected int Limiter = 20;


        /*********
        ** Public methods
        *********/
        public ScrollableCollectionComponent(Point size, List<IMenuComponent> components = null)
            : base(size, components) { }

        public ScrollableCollectionComponent(Rectangle area, List<IMenuComponent> components = null)
            : base(area, components) { }


        /*********
        ** Public methods
        *********/
        public override bool Scroll(int d, Point p, Point o)
        {
            if (!this.Visible)
                return false;
            if (base.Scroll(d, p, new Point(o.X, o.Y - this.ScrollOffset * BaseMenuComponent.Zoom10)))
                return true;
            if (this.HoverElement != null)
                return false;
            int change = d / 120;
            int oldOffset = this.ScrollOffset;
            this.ScrollOffset = Math.Max(0, Math.Min(this.ScrollOffset - change, this.InnerHeight));
            if (oldOffset != this.ScrollOffset)
            {
                Game1.playSound("drumkit6");
                this.UpdateDrawOrder();
                return true;
            }

            return false;
        }

        public override void HoverOver(Point p, Point o)
        {
            Rectangle up = new Rectangle(this.Area.X + o.X + this.Area.Width - (this.UpActive ? BaseMenuComponent.Zoom12 : BaseMenuComponent.Zoom11 + BaseMenuComponent.Zoom05), this.Area.Y + o.Y + (this.UpActive ? 0 : BaseMenuComponent.Zoom05), this.UpActive ? BaseMenuComponent.Zoom12 : BaseMenuComponent.Zoom11, this.UpActive ? BaseMenuComponent.Zoom13 : BaseMenuComponent.Zoom12);
            Rectangle down = new Rectangle(this.Area.X + o.X + this.Area.Width - (this.DownActive ? BaseMenuComponent.Zoom12 : BaseMenuComponent.Zoom11 + BaseMenuComponent.Zoom05), this.Area.Y + o.Y + this.Area.Height - BaseMenuComponent.Zoom12 - (this.DownActive ? BaseMenuComponent.Zoom05 : 0), this.DownActive ? BaseMenuComponent.Zoom12 : BaseMenuComponent.Zoom11, this.DownActive ? BaseMenuComponent.Zoom13 : BaseMenuComponent.Zoom12);
            this.UpActive = this.ScrollOffset > 0 && up.Contains(p);
            this.DownActive = this.ScrollOffset < this.InnerHeight && down.Contains(p);
            base.HoverOver(p, new Point(o.X, o.Y - this.ScrollOffset * BaseMenuComponent.Zoom10));
        }

        public override void LeftHeld(Point p, Point o)
        {
            base.LeftHeld(p, new Point(o.X, o.Y - this.ScrollOffset * BaseMenuComponent.Zoom10));
            if (!this.UpActive && !this.DownActive)
                return;
            this.Counter++;
            if (this.Counter % this.Limiter != 0)
                return;
            this.Limiter = Math.Max(1, this.Limiter - 1);
            this.Counter = 0;
            this.ArrowClick(p, o);
        }

        public override void LeftUp(Point p, Point o)
        {
            this.Limiter = 10;
            this.Counter = 0;
            base.LeftUp(p, new Point(o.X, o.Y - this.ScrollOffset * BaseMenuComponent.Zoom10));
        }

        public override void LeftClick(Point p, Point o)
        {
            base.LeftClick(p, new Point(o.X, o.Y - this.ScrollOffset * BaseMenuComponent.Zoom10));
            this.Counter = 0;
            this.Limiter = 10;
            this.ArrowClick(p, o);
        }

        public override void Draw(SpriteBatch b, Point o)
        {
            if (!this.Visible)
                return;
            b.End();
            Rectangle reg = this.EventRegion;
            b.GraphicsDevice.ScissorRectangle = new Rectangle(reg.X + o.X, reg.Y + o.Y, reg.Width, reg.Height);
            b.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, new RasterizerState { ScissorTestEnable = true });
            Point o2 = new Point(o.X + reg.X, o.Y + reg.Y - this.ScrollOffset * BaseMenuComponent.Zoom10);
            foreach (IMenuComponent el in this.DrawOrder)
                el.Draw(b, o2);
            b.End();
            b.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null);

            // Scrollbar
            if (this.BarOffset == 0)
                return;
            // Up
            b.Draw(Game1.mouseCursors, new Rectangle(this.Area.X + o.X + this.Area.Width - (this.UpActive ? BaseMenuComponent.Zoom12 : BaseMenuComponent.Zoom11 + BaseMenuComponent.Zoom05), this.Area.Y + o.Y + (this.UpActive ? 0 : BaseMenuComponent.Zoom05), this.UpActive ? BaseMenuComponent.Zoom12 : BaseMenuComponent.Zoom11, this.UpActive ? BaseMenuComponent.Zoom13 : BaseMenuComponent.Zoom12), ScrollableCollectionComponent.UpButton, Color.White * (this.ScrollOffset > 0 ? 1 : 0.5f));
            // down
            b.Draw(Game1.mouseCursors, new Rectangle(this.Area.X + o.X + this.Area.Width - (this.DownActive ? BaseMenuComponent.Zoom12 : BaseMenuComponent.Zoom11 + BaseMenuComponent.Zoom05), this.Area.Y + o.Y + this.Area.Height - BaseMenuComponent.Zoom12 - (this.DownActive ? BaseMenuComponent.Zoom05 : 0), this.DownActive ? BaseMenuComponent.Zoom12 : BaseMenuComponent.Zoom11, this.DownActive ? BaseMenuComponent.Zoom13 : BaseMenuComponent.Zoom12), ScrollableCollectionComponent.DownButton, Color.White * (this.ScrollOffset < this.InnerHeight ? 1 : 0.5f));
        }


        /*********
        ** Protected methods
        *********/
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

            this.BarOffset = height > this.Area.Height
                ? BaseMenuComponent.Zoom12
                : 0;
            this.InnerHeight = (int)Math.Ceiling((height - this.Area.Height) / (double)BaseMenuComponent.Zoom10);
            // Remove components that do not intersect the collection viewport from both the draw and event order
            Rectangle self = new Rectangle(0, -(this.ScrollOffset * BaseMenuComponent.Zoom10), this.Area.Width, this.Area.Height);
            this.DrawOrder = this.DrawOrder.FindAll(c => c.GetRegion().Intersects(self));
            this.EventOrder = this.EventOrder.FindAll(c => c.GetRegion().Intersects(self));
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
    }
}
