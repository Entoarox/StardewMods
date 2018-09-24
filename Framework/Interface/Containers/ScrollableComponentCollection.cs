using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace Entoarox.Framework.Interface
{
    internal class ScrollableComponentCollection : GenericComponentCollection, IVisibilityObserver
    {
        /*********
        ** Fields
        *********/
        protected static Rectangle DownButton = new Rectangle(421, 472, 11, 12);
        protected bool HoverDown;
        protected bool HoverUp;
        protected Point MyDown;
        protected Point MyUp;
        protected int ScrollLimit;
        protected int ScrollOffset;
        protected static Rectangle UpButton = new Rectangle(421, 459, 11, 12);


        /*********
        ** Accessors
        *********/
        public override Rectangle OuterBounds
        {
            get => base.OuterBounds;
            set
            {
                base.OuterBounds = value;
                this.UpdateSorting();
            }
        }

        public override Rectangle InnerBounds => this.ScrollLimit > 0 ? new Rectangle(0, 0, this.OuterBounds.Width - ScrollableComponentCollection.UpButton.Width, this.OuterBounds.Height) : this.OuterBounds;


        /*********
        ** Public methods
        *********/
        public ScrollableComponentCollection(string name, Rectangle bounds, int layer = 0)
            : base(name, bounds, layer) { }

        public override void HoverOver(Point offset, Point position)
        {
            if (this.ScrollLimit > 0)
            {
                if (this.ScrollOffset > 0 && new Rectangle(this.MyUp.X + offset.X, this.MyUp.Y + offset.Y, ScrollableComponentCollection.UpButton.Width, ScrollableComponentCollection.UpButton.Height).Contains(position))
                {
                    this.HoverDown = false;
                    this.HoverUp = true;
                }
                else if (this.ScrollOffset < this.ScrollLimit && new Rectangle(this.MyUp.X + offset.X, this.MyUp.Y + offset.Y, ScrollableComponentCollection.UpButton.Width, ScrollableComponentCollection.UpButton.Height).Contains(position))
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
            if (this.ScrollLimit > 0)
            {
                if (this.ScrollOffset > 0 && this.HoverUp)
                {
                    this.ScrollOffset--;
                    Game1.playSound("drumkit6");
                }
                else if (this.ScrollOffset < this.ScrollLimit && this.HoverDown)
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
            Rectangle drawRect = Utilities.GetDrawRectangle(offset, new Rectangle(this.OuterBounds.X, this.OuterBounds.Y, this.InnerBounds.Width, this.InnerBounds.Height));
            batch.GraphicsDevice.ScissorRectangle = drawRect;
            batch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, new RasterizerState { ScissorTestEnable = true });
            Point offset2 = new Point(this.OuterBounds.X + this.InnerBounds.X + offset.X, this.OuterBounds.Y + this.InnerBounds.Y + offset.Y - this.ScrollOffset);
            foreach (IComponent component in this.DrawComponents)
            {
                if (component.Visible)
                    component.Draw(offset2, batch);
            }
            batch.End();
            batch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null);

            // Scrollbar
            if (this.ScrollLimit == 0)
                return;
            // Up
            Rectangle uprect = Utilities.GetDrawRectangle(offset, new Rectangle(this.MyUp.X, this.MyUp.Y, ScrollableComponentCollection.UpButton.Width, ScrollableComponentCollection.UpButton.Height));
            if (this.HoverUp)
                uprect = Utilities.ScaleRectangle(uprect, 1.2);
            batch.Draw(Game1.mouseCursors, uprect, ScrollableComponentCollection.UpButton, Color.White * (this.ScrollOffset > 0 ? 1 : 0.5f));
            // down
            Rectangle downrect = Utilities.GetDrawRectangle(offset, new Rectangle(this.MyDown.X + offset.X, this.MyDown.Y + offset.Y, ScrollableComponentCollection.UpButton.Width, ScrollableComponentCollection.UpButton.Height));
            if (this.HoverDown)
                downrect = Utilities.ScaleRectangle(downrect, 1.2);
            batch.Draw(Game1.mouseCursors, downrect, ScrollableComponentCollection.DownButton, Color.White * (this.ScrollOffset < this.ScrollLimit ? 1 : 0.5f));
        }

        // IVisibilityObserver
        public void VisibilityChanged(IComponent component)
        {
            this.UpdateSorting();
        }


        /*********
        ** Protected methods
        *********/
        protected override void UpdateSorting()
        {
            base.UpdateSorting();
            this.ScrollOffset = 0;
            this.ScrollLimit = this.OuterBounds.Height;
            foreach (IComponent component in this.DrawComponents)
            {
                if (component.OuterBounds.X + component.OuterBounds.Height > this.ScrollLimit)
                    this.ScrollLimit = component.OuterBounds.X + component.OuterBounds.Height;
            }
            this.ScrollLimit -= this.OuterBounds.Height;
            this.MyUp = new Point(this.OuterBounds.X + this.OuterBounds.Width - ScrollableComponentCollection.UpButton.Width, this.OuterBounds.Y);
            this.MyDown = new Point(this.OuterBounds.X + this.OuterBounds.Width - ScrollableComponentCollection.DownButton.Width, this.OuterBounds.Y + this.OuterBounds.Height - ScrollableComponentCollection.DownButton.Height);
        }
    }
}
