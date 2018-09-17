using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace Entoarox.Framework.Interface
{
    public class TextComponent : BaseComponent
    {
        /*********
        ** Fields
        *********/
        private SpriteFont _Font;
        private string _Label;
        private float _Scale;


        /*********
        ** Accessors
        *********/
        public string Label
        {
            get => this._Label;
            set
            {
                this._Label = value;
                this.CalculateBounds();
            }
        }

        public Color Color;

        public SpriteFont Font
        {
            get => this._Font;
            set
            {
                this._Font = value;
                this.CalculateBounds();
            }
        }

        public float Scale
        {
            get => this._Scale;
            set
            {
                this._Scale = value;
                this.CalculateBounds();
            }
        }

        public bool Shadow;


        /*********
        ** Public methods
        *********/
        public TextComponent(string name, Point position, string label, Color color, SpriteFont font = null, float scale = 1, bool shadow = true, int layer = 0)
            : base(name, new Rectangle(position.X, position.Y, 0, 0), layer)
        {
            this._Font = font ?? Game1.dialogueFont;
            this._Label = label;
            this.Color = color;
            this._Scale = scale;
            this.Shadow = shadow;
        }

        public override void Draw(Point offset, SpriteBatch batch)
        {
            Rectangle rect = Utilities.GetDrawRectangle(offset, this.OuterBounds);
            if (this.Shadow)
                batch.DrawString(this._Font, this._Label, new Vector2(rect.X + Game1.pixelZoom, rect.Y + Game1.pixelZoom), new Color(0, 0, 0, 0.33f), 0f, Vector2.Zero, this._Scale, SpriteEffects.None, 0);
            batch.DrawString(this._Font, this._Label, new Vector2(rect.X, rect.Y), this.Color, 0f, Vector2.Zero, this._Scale, SpriteEffects.None, 0);
        }


        /*********
        ** Protected methods
        *********/
        protected void CalculateBounds()
        {
            Vector2 bounds = this.Font.MeasureString(this._Label) * this._Scale;
            Rectangle rect = Utilities.GetRealRectangle(this.OuterBounds);
            this.OuterBounds = Utilities.GetZoomRectangle(new Rectangle(rect.X, rect.Y, (int)Math.Floor(bounds.X), (int)Math.Floor(bounds.Y)));
        }
    }
}
