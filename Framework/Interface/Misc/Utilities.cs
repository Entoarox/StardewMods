using System;
using System.Linq;
using System.Diagnostics;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using StardewValley;
using StardewValley.Menus;

namespace Entoarox.Framework.Interface
{
    public static class Utilities
    {
        private static Color[] GradientData = new Color[11]{
            new Color(1,1,1,0.0f),
            new Color(1,1,1,0.1f),
            new Color(1,1,1,0.2f),
            new Color(1,1,1,0.3f),
            new Color(1,1,1,0.4f),
            new Color(1,1,1,0.5f),
            new Color(1,1,1,0.6f),
            new Color(1,1,1,0.7f),
            new Color(1,1,1,0.8f),
            new Color(1,1,1,0.9f),
            new Color(1,1,1,1.0f)
        };
        private static Texture2D _GradientTextureHorizontal;
        public static Texture2D GradientTextureHorizontal
        {
            get
            {
                if(_GradientTextureHorizontal==null)
                {
                    _GradientTextureHorizontal = new Texture2D(StardewValley.Game1.graphics.GraphicsDevice, 1, 11);
                    _GradientTextureHorizontal.SetData(GradientData);
                }
                return _GradientTextureHorizontal;
            }
        }
        private static Texture2D _GradientTextureVertical;
        public static Texture2D GradientTextureVertical
        {
            get
            {
                if (_GradientTextureVertical == null)
                {
                    _GradientTextureVertical = new Texture2D(StardewValley.Game1.graphics.GraphicsDevice, 11, 1);
                    Color[] gradient = new Color[10];
                    for (int i = 0; i < 10; i++)
                        gradient[i] = new Color(0, 0, 0, i * 0.1f);
                    _GradientTextureVertical.SetData(GradientData);
                }
                return _GradientTextureVertical;
            }
        }
        public static Rectangle GetDrawRectangle(Point offset, Rectangle rect)
        {
            return new Rectangle((offset.X + rect.X) * Game1.pixelZoom, (offset.Y + rect.Y) * Game1.pixelZoom, rect.Width * Game1.pixelZoom, rect.Height * Game1.pixelZoom);
        }

        public static Vector2 GetDrawVector(Point offset, Rectangle rect)
        {
            return new Vector2((offset.X + rect.X) * Game1.pixelZoom, (offset.Y + rect.Y) * Game1.pixelZoom);
        }

        public static Rectangle GetRealRectangle(Rectangle rect)
        {
            return new Rectangle(rect.X * Game1.pixelZoom, rect.Y * Game1.pixelZoom, rect.Width * Game1.pixelZoom, rect.Height * Game1.pixelZoom);
        }

        public static Rectangle GetZoomRectangle(Rectangle rect)
        {
            return new Rectangle((int)Math.Floor(rect.X / Game1.pixelZoom + 0f), (int)Math.Floor(rect.Y / Game1.pixelZoom + 0f), (int)Math.Ceiling(rect.Width / Game1.pixelZoom + 0f), (int)Math.Ceiling(rect.Height / Game1.pixelZoom + 0f));
        }

        public static Rectangle ScaleRectangle(Rectangle rect, double scale)
        {
            int width = (int)Math.Round(rect.Width * scale);
            int height = (int)Math.Round(rect.Height * scale);
            int xdiff = (int)Math.Round((rect.Width - width) / 2D);
            int ydiff = (int)Math.Round((rect.Height - height) / 2D);
            return new Rectangle(rect.X + xdiff, rect.Y + ydiff, width, height);
        }
        private readonly static Rectangle tl = new Rectangle(0, 0, 64, 64);
        private readonly static Rectangle tc = new Rectangle(128, 0, 64, 64);
        private readonly static Rectangle tr = new Rectangle(192, 0, 64, 64);
        private readonly static Rectangle ml = new Rectangle(0, 128, 64, 64);
        private readonly static Rectangle mr = new Rectangle(192, 128, 64, 64);
        private readonly static Rectangle br = new Rectangle(192, 192, 64, 64);
        private readonly static Rectangle bl = new Rectangle(0, 192, 64, 64);
        private readonly static Rectangle bc = new Rectangle(128, 192, 64, 64);
        private readonly static Rectangle bg = new Rectangle(64, 128, 64, 64);
        public static void DrawMenuRect(SpriteBatch b, int x, int y, int width, int height)
        {
            Rectangle o = new Rectangle(x + 2, y + 2, width - 4, height - 4);
            b.Draw(Game1.menuTexture, new Rectangle(o.X, o.Y, o.Width, o.Height), bg, Color.White);
            o = new Rectangle(x - 3, y - 3, width + 6, height + 6);
            b.Draw(Game1.menuTexture, new Rectangle(o.X, o.Y, 64, 64), tl, Color.White);
            b.Draw(Game1.menuTexture, new Rectangle(o.X + o.Width - 64, o.Y, 64, 64), tr, Color.White);
            b.Draw(Game1.menuTexture, new Rectangle(o.X + 64, o.Y, o.Width - 128, 64), tc, Color.White);
            b.Draw(Game1.menuTexture, new Rectangle(o.X, o.Y + o.Height - 64, 64, 64), bl, Color.White);
            b.Draw(Game1.menuTexture, new Rectangle(o.X + o.Width - 64, o.Y + o.Height - 64, 64, 64), br, Color.White);
            b.Draw(Game1.menuTexture, new Rectangle(o.X + 64, o.Y + o.Height - 64, o.Width - 128, 64), bc, Color.White);
            b.Draw(Game1.menuTexture, new Rectangle(o.X, o.Y + 64, 64, o.Height - 128), ml, Color.White);
            b.Draw(Game1.menuTexture, new Rectangle(o.X + o.Width - 64, o.Y + 64, 64, o.Height - 128), mr, Color.White);
        }
    }
}
