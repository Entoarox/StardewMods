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
                this._Label = value;
                CalculateBounds();
            }
        }
        private Color _Color;
        public Color Color
        {
            get => _Color;
            set
            {
                this._Color = value;
                CalculateBounds();
            }
        }
        private SpriteFont _Font;
        public SpriteFont Font
        {
            get => _Font;
            set
            {
                this._Font = value;
                CalculateBounds();
            }
        }
        private float _Scale;
        public float Scale
        {
            get => _Scale;
            set
            {
                this._Scale = value;
                CalculateBounds();
            }
        }
        public bool Shadow;
        public TextComponent(string name, Point position, string label, Color color, SpriteFont font=null, float scale=1, bool shadow=true, int layer=0) : base(name, new Rectangle(position.X,position.Y,0,0), layer)
        {
            this._Font = font ?? Game1.dialogueFont;
            this._Label = label;
            this._Color = color;
            this._Scale = scale;
            this.Shadow = shadow;
        }
        protected void CalculateBounds()
        {
            Vector2 bounds = this.Font.MeasureString(this._Label) * this._Scale;
            Rectangle rect = GetRealRectangle(this.OuterBounds);
            this.OuterBounds = GetZoomRectangle(new Rectangle(rect.X, rect.Y, (int)Math.Floor(bounds.X), (int)Math.Floor(bounds.Y)));
        }

        public override void Draw(Point offset, SpriteBatch batch)
        {
            Rectangle rect = GetDrawRectangle(offset, this.OuterBounds);
            if (this.Shadow)
                batch.DrawString(this._Font, this._Label, new Vector2(rect.X + Game1.pixelZoom, rect.Y + Game1.pixelZoom), new Color(0, 0, 0, 0.33f), 0f, Vector2.Zero, this._Scale, SpriteEffects.None, 0);
            batch.DrawString(this._Font, this._Label, new Vector2(rect.X, rect.Y), this._Color, 0f, Vector2.Zero, this._Scale, SpriteEffects.None, 0);
        }
    }
}
