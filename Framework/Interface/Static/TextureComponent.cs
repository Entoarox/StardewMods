using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Entoarox.Framework.Interface
{
    internal class TextureComponent : BaseComponent
    {
        /*********
        ** Fields
        *********/
        private bool _Scaled;
        private Rectangle _Subrect;


        /*********
        ** Accessors
        *********/
        public Texture2D Texture;
        public Color Color;

        public bool Scaled
        {
            get => this._Scaled;
            set
            {
                this._Scaled = value;
                if (!this._Scaled)
                    this.UpdateBounds();
                else
                    this.OuterBounds = this._Subrect;
            }
        }

        public Rectangle Subrect
        {
            get => this._Subrect;
            set
            {
                this._Subrect = value;
                if (!this._Scaled)
                    this.UpdateBounds();
            }
        }


        /*********
        ** Public methods
        *********/
        public TextureComponent(string name, Point position, Texture2D texture, Rectangle? subrect = null, Color? color = null, int layer = 0)
            : base(name, new Rectangle(position.X, position.Y, 0, 0), layer)
        {
            this.Texture = texture;
            this._Subrect = subrect ?? texture.Bounds;
            this._Scaled = false;
            this.Color = color ?? Color.White;
            this.UpdateBounds();
        }

        public TextureComponent(string name, Rectangle bounds, Texture2D texture, Rectangle? subrect = null, Color? color = null, int layer = 0)
            : base(name, bounds, layer)
        {
            this.Texture = texture;
            this._Subrect = subrect ?? texture.Bounds;
            this.Color = color ?? Color.White;
            this._Scaled = true;
        }

        public override void Draw(Point offset, SpriteBatch batch)
        {
            Rectangle rect = Utilities.GetDrawRectangle(offset, this.OuterBounds);
            batch.Draw(this.Texture, rect, this._Subrect, this.Color);
        }


        /*********
        ** Protected methods
        *********/
        private void UpdateBounds()
        {
            this.OuterBounds = new Rectangle(this.OuterBounds.X, this.OuterBounds.Y, this._Subrect.Width, this._Subrect.Height);
        }
    }
}
