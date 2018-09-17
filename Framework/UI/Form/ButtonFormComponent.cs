using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace Entoarox.Framework.UI
{
    public class ButtonFormComponent : BaseFormComponent
    {
        /*********
        ** Fields
        *********/
        protected static readonly Rectangle ButtonNormal = new Rectangle(256, 256, 10, 10);
        protected static readonly Rectangle ButtonHover = new Rectangle(267, 256, 10, 10);
        protected int Width;
        protected string _Label;
        protected int LabelOffset;
        protected bool Hovered;
        protected bool Pressed;


        /*********
        ** Accessors
        *********/
        public event ClickHandler Handler;
        public string Label
        {
            get => this._Label;
            set
            {
                this._Label = value;
                int labelWidth = this.GetStringWidth(value, Game1.smallFont);
                int width = Math.Max(this.Width, labelWidth + 4);
                this.LabelOffset = (int)Math.Round((width - labelWidth) / 2D);
                this.Area.Width = width * Game1.pixelZoom;
            }
        }


        /*********
        ** Public methods
        *********/
        public ButtonFormComponent(Point position, string label, ClickHandler handler = null)
            : this(position, 50, label, handler) { }

        public ButtonFormComponent(Point position, int width, string label, ClickHandler handler = null)
        {
            this.Width = width;
            int labelWidth = this.GetStringWidth(label, Game1.smallFont);
            width = Math.Max(width, labelWidth + 4);
            this.LabelOffset = (int)Math.Round((width - labelWidth) / 2D);
            this.SetScaledArea(new Rectangle(position.X, position.Y, width, 10));
            this._Label = label;
            if (handler != null)
                this.Handler += handler;
        }

        public override void LeftHeld(Point p, Point o)
        {
            if (!this.Disabled)
                this.Pressed = true;
        }

        public override void LeftUp(Point p, Point o)
        {
            if (!this.Disabled)
                this.Pressed = false;
        }

        public override void LeftClick(Point p, Point o)
        {
            if (this.Disabled)
                return;
            Game1.playSound("bigDeSelect");
            this.Handler?.Invoke(this, this.Parent, this.Parent.GetAttachedMenu());
        }

        public override void HoverIn(Point p, Point o)
        {
            if (this.Disabled)
                return;
            Game1.playSound("Cowboy_Footstep");
            this.Hovered = true;
        }

        public override void HoverOut(Point p, Point o)
        {
            if (this.Disabled)
                return;
            this.Hovered = false;
        }

        public override void Draw(SpriteBatch b, Point o)
        {
            if (!this.Visible)
                return;
            if (this.Pressed)
                o.Y += Game1.pixelZoom / 2;
            Rectangle r = this.Hovered && !this.Pressed ? ButtonFormComponent.ButtonHover : ButtonFormComponent.ButtonNormal;
            // Begin
            b.Draw(Game1.mouseCursors, new Rectangle(this.Area.X + o.X, this.Area.Y + o.Y, BaseMenuComponent.Zoom2, this.Area.Height), new Rectangle(r.X, r.Y, 2, r.Height), Color.White * (this.Disabled ? 0.33f : 1), 0, Vector2.Zero, SpriteEffects.None, 1f);
            // End
            b.Draw(Game1.mouseCursors, new Rectangle(this.Area.X + o.X + this.Area.Width - BaseMenuComponent.Zoom2, this.Area.Y + o.Y, BaseMenuComponent.Zoom2, this.Area.Height), new Rectangle(r.X + r.Width - 2, r.Y, 2, r.Height), Color.White * (this.Disabled ? 0.33f : 1), 0, Vector2.Zero, SpriteEffects.None, 1f);
            // Center
            b.Draw(Game1.mouseCursors, new Rectangle(this.Area.X + o.X + BaseMenuComponent.Zoom2, this.Area.Y + o.Y, this.Area.Width - BaseMenuComponent.Zoom4, this.Area.Height), new Rectangle(r.X + 2, r.Y, r.Width - 4, r.Height), Color.White * (this.Disabled ? 0.33f : 1), 0, Vector2.Zero, SpriteEffects.None, 1f);
            // Text
            Utility.drawTextWithShadow(b, this.Label, Game1.smallFont, new Vector2(o.X + this.Area.X + this.LabelOffset * Game1.pixelZoom, o.Y + this.Area.Y + BaseMenuComponent.Zoom2), Game1.textColor * (this.Disabled ? 0.33f : 1));
        }
    }
}
