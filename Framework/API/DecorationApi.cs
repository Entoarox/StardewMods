using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using SObject = StardewValley.Object;

namespace Entoarox.Framework.API
{
    class DecorationApi
    {
        /// <summary>
        /// Enables registration of custom wallpapers
        /// </summary>
        /// <param name="id">A unique ID for this wallpaper, recommended to prefix with mod ID</param>
        /// <param name="texture">The texture from which to retrieve this wallpaper</param>
        /// <param name="region">If this is a spritesheet, what region of the texture is the wallpaper</param>
        public void RegisterCustomWallpaper(string id, Texture2D texture, Rectangle? region=null)
        {
            region = region ?? texture.Bounds;
        }
        /// <summary>
        /// Enables registration of custom floors
        /// </summary>
        /// <param name="id">A unique ID for this wallpaper, recommended to prefix with mod ID</param>
        /// <param name="texture">The texture from which to retrieve this floor</param>
        /// <param name="region">If this is a spritesheet, what region of the texture is the floor</param>
        public void RegisterCustomFloor(string id, Texture2D texture, Rectangle? region = null)
        {
            region = region ?? texture.Bounds;
        }
        /// <summary>
        /// Provides a StardewValley.Object that the user can use to place their custom wallpaper
        /// </summary>
        /// <param name="id">The unique ID of the wallpaper</param>
        /// <returns></returns>
        public SObject GetCustomWallpaperObject(string id)
        {
            return default;
        }
        /// <summary>
        /// Provides a StardewValley.Object that the user can use to place their custom floor
        /// </summary>
        /// <param name="id">The unique ID of the floor</param>
        /// <returns></returns>
        public SObject GetCustomFloorObject(string id)
        {
            return default;
        }
    }
}
