using Entoarox.Framework;
using StardewValley;

namespace Entoarox.ExtendedMinecart
{
    internal static class GLExtension
    {
        public static void SetTile(this GameLocation self, int x, int y, int index, string layer, string sheet)
        {
            EntoFramework.GetLocationHelper().SetStaticTile(self, layer, x, y, index, sheet);
        }
        public static void SetTile(this GameLocation self, int x, int y, int index, string layer, string value, string sheet)
        {
            EntoFramework.GetLocationHelper().SetStaticTile(self, layer, x, y, index, sheet);
            EntoFramework.GetLocationHelper().SetTileProperty(self, layer, x, y, "Action", value);
        }
    }
}
