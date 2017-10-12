using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Entoarox.Framework.Interface
{
    public static class Cache
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
                    for (var i = 0; i < 10; i++)
                        gradient[i] = new Color(0, 0, 0, i * 0.1f);
                    _GradientTextureVertical.SetData(GradientData);
                }
                return _GradientTextureVertical;
            }
        }
    }
}
