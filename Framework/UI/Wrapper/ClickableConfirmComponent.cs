using Microsoft.Xna.Framework;
using StardewValley;

namespace Entoarox.Framework.UI
{
    internal class ClickableConfirmComponent : ClickableTextureComponent
    {
        /*********
        ** Public methods
        *********/
        public ClickableConfirmComponent(Point position, ClickHandler handler = null)
            : base(new Rectangle(position.X, position.Y, 16, 16), Game1.mouseCursors, handler, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 46), true) { }
    }
}
