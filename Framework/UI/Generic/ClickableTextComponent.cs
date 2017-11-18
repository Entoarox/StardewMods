using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewValley;

namespace Entoarox.Framework.UI
{
    public class ClickableTextComponent : BaseInteractiveMenuComponent
    {
        protected string _Label;
        protected float _Scale;
        protected SpriteFont Font;
        protected bool Hovered = false;
        public float Scale
        {
            get
            {
                return this._Scale;
            }
            set
            {

                this._Scale = value;
                Vector2 size = this.Font.MeasureString(this.Label) * value;
                this.Area.Width = (int)Math.Ceiling(size.X);
                this.Area.Height = (int)Math.Ceiling(size.Y);
            }
        }
        public string Label
        {
            get
            {
                return this._Label;
            }
            set
            {
                this._Label = value;
                Vector2 size = this.Font.MeasureString(value) * this.Scale;
                this.Area.Width = (int)Math.Ceiling(size.X);
                this.Area.Height = (int)Math.Ceiling(size.Y);
            }
        }
        public Color Color;
        public bool Shadow;
        public bool HoverEffect;
        public event ClickHandler Handler;
        public ClickableTextComponent(Point position, string label, ClickHandler handler = null, bool hoverEffect = true, bool shadow = true, float scale = 1, Color? color = null, SpriteFont font = null)
        {
            if (color == null)
                color = Game1.textColor;
            if (font == null)
                font = Game1.smallFont;
            if (handler != null)
                Handler += handler;
            this.HoverEffect = hoverEffect;
            this.Font = font;
            this.Color = (Color)color;
            this.Shadow = shadow;
            this._Scale = scale;
            this._Label = label;
            Vector2 size = this.Font.MeasureString(label) / Game1.pixelZoom * scale;
            SetScaledArea(new Rectangle(position.X, position.Y, (int)Math.Ceiling(size.X), (int)Math.Ceiling(size.Y)));
        }
        public override void HoverIn(Point p, Point o)
        {
            Game1.playSound("Cowboy_Footstep");
            this.Hovered = true;
        }
        public override void HoverOut(Point p, Point o)
        {
            this.Hovered = false;
        }
        public override void LeftClick(Point p, Point o)
        {
            Game1.playSound("bigDeSelect");
            Handler?.Invoke(this, this.Parent, this.Parent.GetAttachedMenu());
        }
        public override void Draw(SpriteBatch b, Point o)
        {
            if (!this.Visible)
                return;
            Vector2 p = new Vector2(this.Area.X + o.X, this.Area.Y + o.Y);
            if (this.Shadow)
                Utility.drawTextWithShadow(b, this.Label, this.Font, p, this.Color * (this.HoverEffect && !this.Hovered ? 0.8f : 1), this.Scale);
            else
                b.DrawString(this.Font, this.Label, p, this.Color * (this.HoverEffect && !this.Hovered ? 0.8f : 1), 0, Vector2.Zero, Game1.pixelZoom * this.Scale, SpriteEffects.None, 1);
        }
    }
}