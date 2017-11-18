using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewValley;

namespace Entoarox.Framework.UI
{
    public class ClickableTextureComponent : BaseInteractiveMenuComponent
    {
        protected bool ScaleOnHover;
        public event ClickHandler Handler;
        public ClickableTextureComponent(Rectangle area, Texture2D texture, ClickHandler handler = null, Rectangle? crop = null, bool scaleOnHover = true) : base(area, texture, crop)
        {
            if (handler != null)
                Handler += handler;
            this.ScaleOnHover = scaleOnHover;
        }
        public override void HoverIn(Point p, Point o)
        {
            Game1.playSound("Cowboy_Footstep");
            if (!this.ScaleOnHover)
                return;
            this.Area.X -= 2;
            this.Area.Y -= 2;
            this.Area.Width += 4;
            this.Area.Height += 4;
        }
        public override void HoverOut(Point p, Point o)
        {
            if (!this.ScaleOnHover)
                return;
            this.Area.X += 2;
            this.Area.Y += 2;
            this.Area.Width -= 4;
            this.Area.Height -= 4;
        }
        public override void LeftClick(Point p, Point o)
        {
            Game1.playSound("bigDeSelect");
            Handler?.Invoke(this, this.Parent, this.Parent.GetAttachedMenu());
        }
    }
}
