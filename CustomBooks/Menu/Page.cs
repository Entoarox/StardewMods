using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Entoarox.CustomBooks.Menu
{
    internal abstract class Page
    {
        public bool Editable = false;
        public abstract void Draw(SpriteBatch batch, Rectangle region);
        public abstract Bookshelf.Book.Page Serialize();
        public abstract void Click(Rectangle region, int x, int y);
        public abstract void Release(Rectangle region, int x, int y);
        public abstract void Hover(Rectangle region, int x, int y);
    }
}
