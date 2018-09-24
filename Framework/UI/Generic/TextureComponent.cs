using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Entoarox.Framework.UI
{
    public class TextureComponent : BaseMenuComponent
    {
        /*********
        ** Public methods
        *********/
        public TextureComponent(Rectangle area, Texture2D texture, Rectangle? crop = null)
            : base(area, texture, crop) { }
    }
}
