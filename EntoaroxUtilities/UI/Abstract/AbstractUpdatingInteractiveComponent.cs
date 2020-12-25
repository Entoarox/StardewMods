using Entoarox.Utilities.UI.Interfaces;

using Microsoft.Xna.Framework;

namespace Entoarox.Utilities.UI.Abstract
{
    public abstract class AbstractUpdatingInteractiveComponent : AbstractInteractiveComponent, IUpdatingComponent
    {
        public abstract void Update(GameTime time);
    }
}
