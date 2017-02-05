using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewValley;
using StardewValley.Menus;

namespace Entoarox.Framework.UI
{
    public class FrameComponent : BaseMenuComponent
    {
        protected bool Chrome = false;
        public FrameComponent(Rectangle area, Texture2D texture, Rectangle? crop=null) : base(area,texture,crop)
        {

        }
        public FrameComponent(Rectangle area)
        {
            Chrome = true;
            SetScaledArea(area);
        }
        public override void Draw(SpriteBatch b, Point o)
        {
            if (!Visible)
                return;
            if (Chrome)
                FrameworkMenu.DrawMenuRect(b, Area.X + o.X, Area.Y + o.Y, Area.Width, Area.Height);
            else
                IClickableMenu.drawTextureBox(b, Texture, Crop, Area.X + o.X, Area.Y + o.Y, Area.Width, Area.Height, Color.White, Game1.pixelZoom, false);
        }
    }
}
