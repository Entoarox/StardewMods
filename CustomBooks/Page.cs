using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Entoarox.CustomBooks
{
    public abstract class Page
    {
        public abstract void Draw(SpriteBatch batch, Rectangle region);
        public abstract Bookshelf.Book.Page Serialize();
    }
}
