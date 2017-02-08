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
                return _Scale;
            }
            set
            {

                _Scale = value;
                Vector2 size = Font.MeasureString(Label) * value;
                Area.Width = (int)Math.Ceiling(size.X);
                Area.Height = (int)Math.Ceiling(size.Y);
            }
        }
        public string Label
        {
            get
            {
                return _Label;
            }
            set
            {
                _Label = value;
                Vector2 size = Font.MeasureString(value) * Scale;
                Area.Width = (int)Math.Ceiling(size.X);
                Area.Height = (int)Math.Ceiling(size.Y);
            }
        }
        public Color Color;
        public bool Shadow;
        public TextComponent(Point position, string label, bool shadow=true, float scale=1, Color? color=null, SpriteFont font=null)
        {
            if (color == null)
                Color = Game1.textColor;
            else
                Color = (Color)color;
            if (font == null)
                Font = Game1.smallFont;
            else
                Font = font;
            Shadow = shadow;
            _Scale = scale;
            _Label = label;
            Vector2 size = Font.MeasureString(label) / Game1.pixelZoom * scale;
            SetScaledArea(new Rectangle(position.X, position.Y,(int)Math.Ceiling(size.X),(int)Math.Ceiling(size.Y)));
        }
        public override void Draw(SpriteBatch b, Point o)
        {
            if (!Visible)
                return;
            Vector2 p = new Vector2(Area.X + o.X, Area.Y + o.Y);
            if (Shadow)
                Utility.drawTextWithShadow(b, Label, Font, p, Color, Scale);
            else
                b.DrawString(Font, Label, p, Color, 0, Vector2.Zero, Game1.pixelZoom * Scale, SpriteEffects.None, 1);
        }
    }
}
