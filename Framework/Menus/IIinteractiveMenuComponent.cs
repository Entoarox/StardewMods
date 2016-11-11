using Microsoft.Xna.Framework;

namespace Entoarox.Framework.Menus
{
    public interface IInteractiveMenuComponent : IMenuComponent
    {
        bool InBounds(Point position, Point offset);
        void LeftClick(Point position, Point offset, IComponentCollection collection, FrameworkMenu menu);
        void RightClick(Point position, Point offset, IComponentCollection collection, FrameworkMenu menu);
        void LeftHeld(Point position, Point offset, IComponentCollection collection, FrameworkMenu menu);
        void LeftUp(Point position, Point offset, IComponentCollection collection, FrameworkMenu menu);
        void HoverIn(Point position, Point offset, IComponentCollection collection, FrameworkMenu menu);
        void HoverOut(Point position, Point offset, IComponentCollection collection, FrameworkMenu menu);
        void HoverOver(Point position, Point offset, IComponentCollection collection, FrameworkMenu menu);
        void Scroll(int direction, Point position, Point offset, IComponentCollection collection, FrameworkMenu menu);
    }
}
