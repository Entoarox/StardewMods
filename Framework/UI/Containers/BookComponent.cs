using System;
using Microsoft.Xna.Framework;

namespace Entoarox.Framework.UI
{
    internal class BookComponent : IComponentContainer
    {
        /*********
        ** Accessors
        *********/
        public Rectangle EventRegion => throw new NotImplementedException();
        public Rectangle ZoomEventRegion => throw new NotImplementedException();


        /*********
        ** Public methods
        *********/
        public FrameworkMenu GetAttachedMenu()
        {
            throw new NotImplementedException();
        }

        public void GiveFocus(IInteractiveMenuComponent component)
        {
            throw new NotImplementedException();
        }

        public void ResetFocus()
        {
            throw new NotImplementedException();
        }
    }
}
