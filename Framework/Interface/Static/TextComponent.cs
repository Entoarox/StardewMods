using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewValley;

namespace Entoarox.Framework.Interface
{
    public class TextComponent : BaseComponent
    {
        private string _Label;
        public string Label
        {
            get => _Label;
            set
            {
                _Label = value;
                CalculateBounds();
            }
        }
        private Color _Color;
        public Color Color
        {
            get => _Color;
            set
            {
                _Color = value;
                CalculateBounds();
            }
        }
        private SpriteFont _Font;
        public SpriteFont Font
        {
            get => _Font;
            set
            {
                _Font = value;
                CalculateBounds();
            }
        }
        private float _Scale;
        public float Scale
        {
            get => _Scale;
            set
            {
                _Scale = value;
                CalculateBounds();
            }
        }
        public bool Shadow;
        public TextComponent(string name, Point position, string label, Color color, SpriteFont font=null, float scale=1, bool shadow=true, int layer=0) : base(name, new Rectangle(position.X,position.Y,0,0), layer)
        {
            _Font = font ?? Game1.dialogueFont;
            _Label = label;
            _Color = color;
            _Scale = scale;
            Shadow = shadow;
        }
        protected void CalculateBounds()
        {
            Vector2 bounds = Font.MeasureString(_Label) * _Scale;
            Rectangle rect = GetRealRectangle(OuterBounds);
            OuterBounds = GetZoomRectangle(new Rectangle(rect.X, rect.Y, (int)Math.Floor(bounds.X), (int)Math.Floor(bounds.Y)));
        }

        public override void Draw(Point offset, SpriteBatch batch)
        {
            Rectangle rect = GetDrawRectangle(offset, OuterBounds);
            if (Shadow)
                batch.DrawString(_Font, _Label, new Vector2(rect.X + Game1.pixelZoom, rect.Y + Game1.pixelZoom), new Color(0, 0, 0, 0.33f), 0f, Vector2.Zero, _Scale, SpriteEffects.None, 0);
            batch.DrawString(_Font, _Label, new Vector2(rect.X, rect.Y), _Color, 0f, Vector2.Zero, _Scale, SpriteEffects.None, 0);
        }
    }
}
