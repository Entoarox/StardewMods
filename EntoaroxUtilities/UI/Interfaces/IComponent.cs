using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Entoarox.Utilities.UI.Interfaces
{
    public interface IComponent
    {
        IComponentContainer Container { get; set; }
        string Id { get; set; }
        int Layer { get; set; }
        bool Visible { get; set; }
        Rectangle DisplayRegion { get; set; }

        void Draw(Rectangle drawRect, SpriteBatch batch);
    }
}
