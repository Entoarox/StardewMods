using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Entoarox.DynamicDungeons
{
    internal class ImagePage : Page
    {
        /*********
        ** Fields
        *********/
        private readonly Color Color;
        private readonly Texture2D Image;
        private readonly bool Shadow;


        /*********
        ** Public methods
        *********/
        public ImagePage(Texture2D image, Color? color = null, bool shadow = false)
        {
            this.Image = image;
            this.Color = color ?? Color.White;
            this.Shadow = shadow;
        }

        public override void Draw(SpriteBatch batch, Rectangle region)
        {
            int w = Math.Min(region.Width, this.Image.Width * 2);
            int h = Math.Min(region.Height, this.Image.Height * 2);
            int x = Math.Max(0, (region.Width - w) / 2);
            int y = Math.Max(0, (region.Height - h) / 2);
            if (this.Shadow)
            {
                Color c = new Color(221, 148, 84);
                batch.Draw(this.Image, new Rectangle(region.X + x + 2, region.Y + y, w, h), c);
                batch.Draw(this.Image, new Rectangle(region.X + x, region.Y + y + 2, w, h), c);
                batch.Draw(this.Image, new Rectangle(region.X + x + 2, region.Y + y + 2, w, h), c);
            }

            batch.Draw(this.Image, new Rectangle(region.X + x, region.Y + y, w, h), this.Color);
        }
    }
}
