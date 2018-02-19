using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Entoarox.Framework.UI.Containers
{
    class BookComponent : IComponentContainer
    {
        public Rectangle EventRegion => throw new NotImplementedException();

        public Rectangle ZoomEventRegion => throw new NotImplementedException();

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
