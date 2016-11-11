using Microsoft.Xna.Framework;

namespace Entoarox.Framework.Menus
{
    public interface IClickHandler
    {
        void LeftClick(Point position, Point offset, IComponentCollection collection, FrameworkMenu menu);
        void RightClick(Point position, Point offset, IComponentCollection collection, FrameworkMenu menu);
    }
}
