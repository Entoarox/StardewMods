using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;

namespace Entoarox.Framework.Core.Utilities
{
    public static class TextureHelper
    {
        /*********
        ** Fields
        *********/
        private static readonly BlendState BlendAlphaBlendState = new BlendState
        {
            ColorWriteChannels = ColorWriteChannels.Alpha,
            AlphaDestinationBlend = Blend.Zero,
            ColorDestinationBlend = Blend.Zero,
            AlphaSourceBlend = Blend.One,
            ColorSourceBlend = Blend.One
        };

        private static readonly BlendState BlendColorBlendState = new BlendState
        {
            ColorDestinationBlend = Blend.Zero,
            ColorWriteChannels = ColorWriteChannels.Red | ColorWriteChannels.Green | ColorWriteChannels.Blue,
            AlphaDestinationBlend = Blend.Zero,
            AlphaSourceBlend = Blend.SourceAlpha,
            ColorSourceBlend = Blend.SourceAlpha
        };

        private static readonly Dictionary<string, Texture2D> Cache = new Dictionary<string, Texture2D>();
        private static readonly PropertyInfo DrawLoop = typeof(Context).GetProperty("IsInDrawLoop", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);


        /*********
        ** Public methods
        *********/
        public static Texture2D GetTexture(string file, IMonitor monitor = null)
        {
            if (!TextureHelper.Cache.ContainsKey(file))
            {
                Texture2D texture = Texture2D.FromStream(Game1.graphics.GraphicsDevice, new FileStream(file, FileMode.Open));
                if ((bool)TextureHelper.DrawLoop.GetValue(null))
                {
                    (monitor ?? EntoaroxFrameworkMod.Logger).Log("It is not recommended to load a texture during the draw loop!" + Environment.NewLine + file, LogLevel.Warn);
                    TextureHelper.Cache.Add(file, TextureHelper.PremultiplyCPU(texture));
                }
                else
                    TextureHelper.Cache.Add(file, TextureHelper.PremultiplyGPU(texture));
            }

            return TextureHelper.Cache[file];
        }

        public static Texture2D Premultiply(Texture2D texture, IMonitor monitor = null)
        {
            if ((bool)TextureHelper.DrawLoop.GetValue(null))
            {
                (monitor ?? EntoaroxFrameworkMod.Logger).Log("It is not recommended to load a texture during the draw loop!", LogLevel.Warn);
                return TextureHelper.PremultiplyCPU(texture);
            }

            return TextureHelper.PremultiplyGPU(texture);
        }


        /*********
        ** Protected methods
        *********/
        private static Texture2D PremultiplyCPU(Texture2D texture)
        {
            Color[] data = new Color[texture.Width * texture.Height];
            texture.GetData(data);
            Parallel.For(0, data.Length, i => { data[i] = Color.FromNonPremultiplied(data[i].ToVector4()); });

            texture.SetData(data);
            return texture;
        }

        private static Texture2D PremultiplyGPU(Texture2D texture)
        {
            using (RenderTarget2D renderTarget = new RenderTarget2D(Game1.graphics.GraphicsDevice, texture.Width, texture.Height))
            {
                SpriteBatch spriteBatch = new SpriteBatch(Game1.graphics.GraphicsDevice);

                Viewport viewportBackup = Game1.graphics.GraphicsDevice.Viewport;
                Game1.graphics.GraphicsDevice.SetRenderTarget(renderTarget);
                Game1.graphics.GraphicsDevice.Clear(Color.Black);

                spriteBatch.Begin(SpriteSortMode.Immediate, TextureHelper.BlendColorBlendState);
                spriteBatch.Draw(texture, texture.Bounds, Color.White);
                spriteBatch.End();

                spriteBatch.Begin(SpriteSortMode.Immediate, TextureHelper.BlendAlphaBlendState);
                spriteBatch.Draw(texture, texture.Bounds, Color.White);
                spriteBatch.End();

                Game1.graphics.GraphicsDevice.SetRenderTarget(null);
                Game1.graphics.GraphicsDevice.Viewport = viewportBackup;

                Color[] data = new Color[texture.Width * texture.Height];
                renderTarget.GetData(data);

                Game1.graphics.GraphicsDevice.Textures[0] = null;
                texture.SetData(data);
            }

            return texture;
        }
    }
}
