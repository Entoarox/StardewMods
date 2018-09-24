using Microsoft.Xna.Framework;

namespace Entoarox.Framework.UI
{
    public interface IComponentContainer
    {
        /*********
        ** Accessors
        *********/
        Rectangle EventRegion { get; }
        Rectangle ZoomEventRegion { get; }


        /*********
        ** Methods
        *********/
        void ResetFocus();
        void GiveFocus(IInteractiveMenuComponent component);
        FrameworkMenu GetAttachedMenu();
    }
}
