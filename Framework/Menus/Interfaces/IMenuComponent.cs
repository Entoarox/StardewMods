using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Entoarox.Framework.Menus
{
    public interface IMenuComponent
    {
        void Update(GameTime t, IComponentContainer container, FrameworkMenu menu);
        void Draw(SpriteBatch b, Point offset);
        void Attach(IComponentContainer collection);
        void Detach(IComponentContainer collection);
        Point GetPosition();
        Rectangle GetRegion();
        bool Visible { get; set; }
        IComponentContainer Parent { get; }
    }
}
