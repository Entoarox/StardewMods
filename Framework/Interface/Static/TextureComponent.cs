using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Entoarox.Framework.Interface
{
    class TextureComponent : BaseComponent
    {
        public Texture2D Texture;
        public Color Color;
        private bool _Scaled;
        public bool Scaled
        {
            get => _Scaled;
            set
            {
                this._Scaled = value;
                if (!this._Scaled)
                    UpdateBounds();
                else
                    this.OuterBounds = this._Subrect;
            }
        }
        private Rectangle _Subrect;
        public Rectangle Subrect
        {
            get => _Subrect;
            set
            {
                this._Subrect = value;
                if(!this._Scaled)
                    UpdateBounds();
            }
        }

        public TextureComponent(string name, Point position, Texture2D texture, Rectangle? subrect=null, Color? color=null, int layer=0) : base(name, new Rectangle(position.X,position.Y,0,0),layer)
        {
            this.Texture = texture;
            this._Subrect = subrect ?? texture.Bounds;
            this._Scaled = false;
            this.Color = color ?? Color.White;
            UpdateBounds();
        }
        public TextureComponent(string name, Rectangle bounds, Texture2D texture, Rectangle? subrect = null, Color? color = null, int layer = 0) : base(name, bounds, layer)
        {
            this.Texture = texture;
            this._Subrect = subrect ?? texture.Bounds;
            this.Color = color ?? Color.White;
            this._Scaled = true;
        }

        private void UpdateBounds()
        {
            this.OuterBounds = new Rectangle(this.OuterBounds.X, this.OuterBounds.Y, this._Subrect.Width, this._Subrect.Height);
        }

        public override void Draw(Point offset, SpriteBatch batch)
        {
            Rectangle rect = GetDrawRectangle(offset, this.OuterBounds);
            batch.Draw(this.Texture, rect, this._Subrect, this.Color);
        }
    }
}
