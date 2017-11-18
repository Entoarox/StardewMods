using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewValley;

namespace Entoarox.Framework.UI
{
    public class TextComponent : BaseMenuComponent
    {
        protected string _Label;
        protected SpriteFont Font;
        protected float _Scale;
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
        public TextComponent(Point position, string label, bool shadow=true, float scale=1, Color? color=null, SpriteFont font=null)
        {
            if (color == null)
                this.Color = Game1.textColor;
            else
                this.Color = (Color)color;
            if (font == null)
                this.Font = Game1.smallFont;
            else
                this.Font = font;
            this.Shadow = shadow;
            this._Scale = scale;
            this._Label = label;
            Vector2 size = this.Font.MeasureString(label) / Game1.pixelZoom * scale;
            SetScaledArea(new Rectangle(position.X, position.Y,(int)Math.Ceiling(size.X),(int)Math.Ceiling(size.Y)));
        }
        public override void Draw(SpriteBatch b, Point o)
        {
            if (!this.Visible)
                return;
            Vector2 p = new Vector2(this.Area.X + o.X, this.Area.Y + o.Y);
            if (this.Shadow)
                Utility.drawTextWithShadow(b, this.Label, this.Font, p, this.Color, this.Scale);
            else
                b.DrawString(this.Font, this.Label, p, this.Color, 0, Vector2.Zero, Game1.pixelZoom * this.Scale, SpriteEffects.None, 1);
        }
    }
}
