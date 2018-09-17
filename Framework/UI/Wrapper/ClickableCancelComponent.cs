using Microsoft.Xna.Framework;
using StardewValley;

namespace Entoarox.Framework.UI
{
    internal class ClickableCancelComponent : ClickableTextureComponent
    {
        /*********
        ** Public methods
        *********/
        public ClickableCancelComponent(Point position, ClickHandler handler = null)
            : base(new Rectangle(position.X, position.Y, 16, 16), Game1.mouseCursors, handler, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 47), true) { }
    }
}
