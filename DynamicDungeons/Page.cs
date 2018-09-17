using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Entoarox.DynamicDungeons
{
    internal abstract class Page
    {
        /*********
        ** Public methods
        *********/
        public abstract void Draw(SpriteBatch batch, Rectangle region);
    }
}
