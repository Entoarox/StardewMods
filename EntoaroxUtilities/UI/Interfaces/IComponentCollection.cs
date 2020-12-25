using Microsoft.Xna.Framework;

namespace Entoarox.Utilities.UI.Interfaces
{
    public interface IComponentCollection : IComponentContainer, IInteractiveComponent, IUpdatingComponent
    {
        Rectangle ComponentRegion { get; }
    }
}
