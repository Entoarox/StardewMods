using Entoarox.Utilities.UI.Interfaces;

using Microsoft.Xna.Framework;

namespace Entoarox.Utilities.UI.Abstract
{
    public abstract class AbstractUpdatingComponent : AbstractComponent, IUpdatingComponent
    {
        public abstract void Update(GameTime time);
    }
}
