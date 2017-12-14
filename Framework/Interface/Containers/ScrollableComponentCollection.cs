using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewValley;

namespace Entoarox.Framework.Interface
{
    class ScrollableComponentCollection : GenericComponentCollection, IVisibilityObserver
    {
        public ScrollableComponentCollection(string name, Rectangle bounds, int layer = 0) : base(name, bounds, layer)
        {

        }
        // IComponentCollection
        public override Rectangle OuterBounds
        {
            get => base.OuterBounds;
            set
            {
                base.OuterBounds = value;
                UpdateSorting();
            }
        }
        public override Rectangle InnerBounds => this.ScrollLimit > 0 ? new Rectangle(0, 0, this.OuterBounds.Width - UpButton.Width, this.OuterBounds.Height) : this.OuterBounds;
        // IDynamicComponent
        public override void HoverOver(Point offset, Point position)
        {
            if (this.ScrollLimit > 0)
            {
                if (this.ScrollOffset > 0 && new Rectangle(this.MyUp.X + offset.X, this.MyUp.Y + offset.Y, UpButton.Width, UpButton.Height).Contains(position))
                {
                    this.HoverDown = false;
                    this.HoverUp = true;
                }
                else if (this.ScrollOffset < this.ScrollLimit && new Rectangle(this.MyUp.X + offset.X, this.MyUp.Y + offset.Y, UpButton.Width, UpButton.Height).Contains(position))
                {
                    this.HoverDown = true;
                    this.HoverUp = false;
                }
                else
                {
                    this.HoverDown = false;
                    this.HoverUp = true;
                }
            }
            base.HoverOver(offset, position);
        }
        public override void LeftClick(Point offset, Point position)
        {
            if (this.ScrollLimit>0)
            {
                if (this.ScrollOffset > 0 && this.HoverUp)
                {
                    this.ScrollOffset--;
                    Game1.playSound("drumkit6");
                }
                else if(this.ScrollOffset < this.ScrollLimit && this.HoverDown)
                {
                    this.ScrollOffset++;
                    Game1.playSound("drumkit6");
                }
            }
            base.LeftClick(offset, position);
        }
        public override bool Scroll(Point offset, Point position, int amount)
        {
            if (base.Scroll(offset, position, amount))
                return true;
            if (this.ScrollLimit > 0)
            {
                int diff = Math.Min(this.ScrollLimit, Math.Max(0, this.ScrollOffset - amount / 120));
                if (diff != this.ScrollOffset)
                {
                    Game1.playSound("drumkit6");
                    this.ScrollOffset = diff;
                    return true;
                }
            }
            return false;
        }
        public override void Draw(Point offset, SpriteBatch batch)
        {
            batch.End();
            var drawRect = GetDrawRectangle(offset, new Rectangle(this.OuterBounds.X, this.OuterBounds.Y, this.InnerBounds.Width, this.InnerBounds.Height));
            batch.GraphicsDevice.ScissorRectangle = drawRect;
            batch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, new RasterizerState() { ScissorTestEnable = true });
            Point offset2 = new Point(this.OuterBounds.X + this.InnerBounds.X + offset.X, this.OuterBounds.Y + this.InnerBounds.Y + offset.Y - this.ScrollOffset);
            foreach (IComponent component in this._DrawComponents)
                if(component.Visible)
                    component.Draw(offset2, batch);
            batch.End();
            batch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null);

            // Scrollbar
            if (this.ScrollLimit == 0)
                return;
            // Up
            var uprect = GetDrawRectangle(offset, new Rectangle(this.MyUp.X, this.MyUp.Y, UpButton.Width, UpButton.Height));
            if (this.HoverUp)
                uprect = ScaleRectangle(uprect, 1.2);
            batch.Draw(Game1.mouseCursors, uprect, UpButton, Color.White * (this.ScrollOffset > 0 ? 1 : 0.5f));
            // down
            var downrect = GetDrawRectangle(offset, new Rectangle(this.MyDown.X + offset.X, this.MyDown.Y + offset.Y, UpButton.Width, UpButton.Height));
            if (this.HoverDown)
                downrect = ScaleRectangle(downrect, 1.2);
            batch.Draw(Game1.mouseCursors, downrect, DownButton, Color.White * (this.ScrollOffset < this.ScrollLimit ? 1 : 0.5f));
        }
        // IVisibilityObserver
        public void VisibilityChanged(IComponent component)
        {
            UpdateSorting();
        }
        // Internal
        protected override void UpdateSorting()
        {
            base.UpdateSorting();
            this.ScrollOffset = 0;
            this.ScrollLimit = this.OuterBounds.Height;
            foreach (IComponent component in this._DrawComponents)
                if (component.OuterBounds.X + component.OuterBounds.Height > this.ScrollLimit)
                    this.ScrollLimit = component.OuterBounds.X + component.OuterBounds.Height;
            this.ScrollLimit -= this.OuterBounds.Height;
            this.MyUp = new Point(this.OuterBounds.X + this.OuterBounds.Width - UpButton.Width, this.OuterBounds.Y);
            this.MyDown = new Point(this.OuterBounds.X + this.OuterBounds.Width - DownButton.Width, this.OuterBounds.Y + this.OuterBounds.Height - DownButton.Height);
        }

        protected bool HoverUp = false;
        protected bool HoverDown = false;

        protected Point MyUp;
        protected Point MyDown;

        protected static Rectangle UpButton = new Rectangle(421, 459, 11, 12);
        protected static Rectangle DownButton = new Rectangle(421, 472, 11, 12);

        protected int ScrollOffset = 0;
        protected int ScrollLimit;
    }
}
