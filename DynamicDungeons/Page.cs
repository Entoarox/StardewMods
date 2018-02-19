using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Entoarox.DynamicDungeons
{
    abstract class Page
    {
        public abstract void Draw(SpriteBatch batch, Rectangle region);
    }
}
