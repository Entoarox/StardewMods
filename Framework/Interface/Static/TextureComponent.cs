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
                _Scaled = value;
                if (!_Scaled)
                    UpdateBounds();
                else
                    OuterBounds = _Subrect;
            }
        }
        private Rectangle _Subrect;
        public Rectangle Subrect
        {
            get => _Subrect;
            set
            {
                _Subrect = value;
                if(!_Scaled)
                    UpdateBounds();
            }
        }

        public TextureComponent(string name, Point position, Texture2D texture, Rectangle? subrect=null, Color? color=null, int layer=0) : base(name, new Rectangle(position.X,position.Y,0,0),layer)
        {
            Texture = texture;
            _Subrect = subrect ?? texture.Bounds;
            _Scaled = false;
            Color = color ?? Color.White;
            UpdateBounds();
        }
        public TextureComponent(string name, Rectangle bounds, Texture2D texture, Rectangle? subrect = null, Color? color = null, int layer = 0) : base(name, bounds, layer)
        {
            Texture = texture;
            _Subrect = subrect ?? texture.Bounds;
            Color = color ?? Color.White;
            _Scaled = true;
        }

        private void UpdateBounds()
        {
            OuterBounds = new Rectangle(OuterBounds.X, OuterBounds.Y, _Subrect.Width, _Subrect.Height);
        }

        public override void Draw(Point offset, SpriteBatch batch)
        {
            Rectangle rect = GetDrawRectangle(offset, OuterBounds);
            batch.Draw(Texture, rect, _Subrect, Color);
        }
    }
}
