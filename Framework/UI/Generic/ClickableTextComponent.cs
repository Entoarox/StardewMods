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
            HoverEffect = hoverEffect;
            Font = font;
            Color = (Color)color;
            Shadow = shadow;
            _Scale = scale;
            _Label = label;
            Vector2 size = Font.MeasureString(label) / Game1.pixelZoom * scale;
            SetScaledArea(new Rectangle(position.X, position.Y, (int)Math.Ceiling(size.X), (int)Math.Ceiling(size.Y)));
        }
        public override void HoverIn(Point p, Point o)
        {
            Game1.playSound("Cowboy_Footstep");
            Hovered = true;
        }
        public override void HoverOut(Point p, Point o)
        {
            Hovered = false;
        }
        public override void LeftClick(Point p, Point o)
        {
            Game1.playSound("bigDeSelect");
            Handler?.Invoke(this, Parent, Parent.GetAttachedMenu());
        }
        public override void Draw(SpriteBatch b, Point o)
        {
            if (!Visible)
                return;
            Vector2 p = new Vector2(Area.X + o.X, Area.Y + o.Y);
            if (Shadow)
                Utility.drawTextWithShadow(b, Label, Font, p, Color * (HoverEffect && !Hovered ? 0.8f : 1), Scale);
            else
                b.DrawString(Font, Label, p, Color * (HoverEffect && !Hovered ? 0.8f : 1), 0, Vector2.Zero, Game1.pixelZoom * Scale, SpriteEffects.None, 1);
        }
    }
}