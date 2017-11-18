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
            this.Chrome = true;
            SetScaledArea(area);
        }
        public override void Draw(SpriteBatch b, Point o)
        {
            if (!this.Visible)
                return;
            if (this.Chrome)
                FrameworkMenu.DrawMenuRect(b, this.Area.X + o.X, this.Area.Y + o.Y, this.Area.Width, this.Area.Height);
            else
                IClickableMenu.drawTextureBox(b, this.Texture, this.Crop, this.Area.X + o.X, this.Area.Y + o.Y, this.Area.Width, this.Area.Height, Color.White, Game1.pixelZoom, false);
        }
    }
}
