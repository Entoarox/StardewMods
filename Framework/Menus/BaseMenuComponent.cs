using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewValley;

namespace Entoarox.Framework.Menus
{
    abstract public class BaseMenuComponent : IMenuComponent
    {
        protected Rectangle Area;
        protected Texture2D Texture;
        protected Rectangle Crop;
        public bool Visible;
        protected void SetScaledArea(Rectangle area)
        {
            Area = new Rectangle(area.X * Game1.pixelZoom, area.Y * Game1.pixelZoom, area.Width * Game1.pixelZoom, area.Height * Game1.pixelZoom);
        }
        protected int GetStringWidth(string text, SpriteFont font, float scale = 1f)
        {
            return (int)Math.Ceiling(font.MeasureString(text).X / Game1.pixelZoom * scale);
        }
        protected BaseMenuComponent()
        {

        }
        public BaseMenuComponent(Rectangle area, Texture2D texture, Rectangle? crop = null)
        {
            if (crop == null)
                crop = new Rectangle(0, 0, texture.Width, texture.Height);
            Texture = texture;
            Crop = (Rectangle)crop;
            SetScaledArea(area);
        }
        public virtual void Update(GameTime t, IComponentCollection c, FrameworkMenu m)
        {

        }
        public virtual void Draw(SpriteBatch b, Point offset)
        {
            if (Visible)
                b.Draw(Texture, new Vector2(Area.X + offset.X, Area.Y + offset.Y), Crop, Color.White, 0, Vector2.Zero, Game1.pixelZoom, SpriteEffects.None, 1f);
        }
    }
}
