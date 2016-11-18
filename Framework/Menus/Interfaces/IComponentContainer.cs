using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;

namespace Entoarox.Framework.Menus
{
    public interface IComponentContainer
    {
        void ResetFocus();
        void GiveFocus(IInteractiveMenuComponent component);
        Rectangle EventRegion { get; }
        Rectangle ZoomEventRegion { get; }
        FrameworkMenu GetAttachedMenu();
    }
}
