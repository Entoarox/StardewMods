using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewValley;

namespace Entoarox.Framework.UI
{
    public class ButtonFormComponent : BaseFormComponent
    {
        protected readonly static Rectangle ButtonNormal = new Rectangle(256, 256, 10, 10);
        protected readonly static Rectangle ButtonHover = new Rectangle(267, 256, 10, 10);
        public event ClickHandler Handler;
        protected string _Label;
        protected int _Width;
        public string Label
        {
            get
            {
                return this._Label;
            }
            set
            {
                this._Label = value;
                int labelWidth = GetStringWidth(value, Game1.smallFont);
                int width = Math.Max(this._Width, labelWidth + 4);
                this.LabelOffset = (int)Math.Round((width - labelWidth) / 2D);
                this.Area.Width = width * Game1.pixelZoom;
            }
        }
        protected int LabelOffset;
        protected bool Hovered = false;
        protected bool Pressed = false;
        public ButtonFormComponent(Point position, string label, ClickHandler handler=null) : this(position,50,label,handler)
        {

        }
        public ButtonFormComponent(Point position, int width, string label, ClickHandler handler =null)
        {
            this._Width = width;
            int labelWidth = GetStringWidth(label, Game1.smallFont);
            width = Math.Max(width,labelWidth+4);
            this.LabelOffset = (int)Math.Round((width - labelWidth) / 2D);
            SetScaledArea(new Rectangle(position.X, position.Y, width, 10));
            this._Label = label;
            if (handler!=null)
                Handler += handler;
        }
        public override void LeftHeld(Point p, Point o)
        {
            if(!this.Disabled)
                this.Pressed = true;
        }
        public override void LeftUp(Point p, Point o)
        {
            if(!this.Disabled)
                this.Pressed = false;
        }
        public override void LeftClick(Point p, Point o)
        {
            if (this.Disabled)
                return;
            Game1.playSound("bigDeSelect");
            Handler?.Invoke(this, this.Parent, this.Parent.GetAttachedMenu());
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
            Rectangle r = this.Hovered && !this.Pressed ? ButtonHover : ButtonNormal;
            // Begin
            b.Draw(Game1.mouseCursors, new Rectangle(this.Area.X + o.X, this.Area.Y + o.Y, zoom2, this.Area.Height), new Rectangle(r.X, r.Y, 2, r.Height), Color.White * (this.Disabled ? 0.33f : 1), 0, Vector2.Zero, SpriteEffects.None, 1f);
            // End
            b.Draw(Game1.mouseCursors, new Rectangle(this.Area.X + o.X + this.Area.Width-zoom2, this.Area.Y + o.Y, zoom2, this.Area.Height), new Rectangle(r.X+r.Width-2, r.Y, 2, r.Height), Color.White * (this.Disabled ? 0.33f : 1), 0, Vector2.Zero, SpriteEffects.None, 1f);
            // Center
            b.Draw(Game1.mouseCursors, new Rectangle(this.Area.X + o.X + zoom2, this.Area.Y + o.Y, this.Area.Width - zoom4, this.Area.Height), new Rectangle(r.X+2, r.Y, r.Width - 4, r.Height), Color.White *(this.Disabled ? 0.33f : 1), 0, Vector2.Zero, SpriteEffects.None, 1f);
            // Text
            Utility.drawTextWithShadow(b, this.Label, Game1.smallFont, new Vector2(o.X + this.Area.X + this.LabelOffset *Game1.pixelZoom, o.Y + this.Area.Y + zoom2), Game1.textColor * (this.Disabled ?0.33f:1));
        }
    }
}
