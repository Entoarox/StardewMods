using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewValley;

namespace SundropCity
{
    class PaletteTexture
    {
        private readonly Dictionary<string, Texture2D> Layers = new Dictionary<string, Texture2D>();
        private readonly Texture2D Mask;
        internal readonly Texture2D Source;

        public static Dictionary<Color, Color> ParsePalette(Dictionary<string, string> raw)
        {
            return raw.Select(_ =>
            {
                string[] keys = _.Key.Split(',');
                string[] values = _.Value.Split(',');
                return new KeyValuePair<Color, Color>(new Color(Convert.ToInt32(keys[0]), Convert.ToInt32(keys[1]), Convert.ToInt32(keys[2])), new Color(Convert.ToInt32(values[0]), Convert.ToInt32(values[1]), Convert.ToInt32(values[2])));
            }).ToDictionary(_ => _.Key, _ => _.Value);
        }
        public PaletteTexture(Texture2D source, Dictionary<string, Color> sourceMap)
        {
            this.Source = source;
            var invertMap = sourceMap.ToDictionary(_ => _.Value, _ => _.Key);
            Color[] pixels = new Color[source.Width * source.Height];
            Color[] trans = new Color[source.Width * source.Height];
            for (int i = 0; i < trans.Length; i++)
                trans[i] = Color.Transparent;
            source.GetData(pixels);
            this.Mask = new Texture2D(Game1.graphics.GraphicsDevice, source.Width, source.Height);
            this.Mask.SetData(trans);
            for(int c=0;c<pixels.Length;c++)
            {
                int y = (int)Math.Floor((double)c / source.Width);
                int x = c - y * source.Width;
                Color pixel = pixels[c];
                if (pixel.A != 255 || !invertMap.ContainsKey(pixel))
                {
                    this.Mask.SetData(0, new Rectangle(x, y, 1, 1), new Color[] { pixel }, 0, 1);
                }
                else
                {
                    string key = invertMap[pixel];
                    if (!this.Layers.ContainsKey(key))
                    {
                        this.Layers.Add(key, new Texture2D(Game1.graphics.GraphicsDevice, source.Width, source.Height));
                        this.Layers[key].SetData(trans);
                    }
                    this.Layers[key].SetData(0, new Rectangle(x, y, 1, 1), new Color[] { Color.White }, 0, 1);
                }
            }
        }

        public void Draw(SpriteBatch batch, Vector2 position, Rectangle? sourceRectangle, Dictionary<string, Color> palette, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects, float layerDepth)
        {
            batch.Draw(this.Mask, position, sourceRectangle, Color.White, rotation, origin, scale, effects, layerDepth);
            foreach (var Layer in this.Layers)
                if (palette.ContainsKey(Layer.Key))
                    batch.Draw(Layer.Value, position, sourceRectangle, palette[Layer.Key], rotation, origin, scale, effects, layerDepth);
        }
    }
}
