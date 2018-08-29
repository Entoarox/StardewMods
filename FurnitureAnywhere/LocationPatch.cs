using StardewValley;

using Microsoft.Xna.Framework;

using Harmony;

using StardewValley.Locations;
using StardewValley.Network;
using StardewValley.Objects;

namespace Entoarox.FurnitureAnywhere
{

    [HarmonyPatch(typeof(GameLocation), "isCollidingPosition", new[] { typeof(Rectangle), typeof(xTile.Dimensions.Rectangle), typeof(bool), typeof(int), typeof(bool), typeof(Character) })]
    public class LocationPatch
    {
        internal static void Prefix(GameLocation __instance, Rectangle position)
        {
            OverlaidDictionary<Vector2, Object> objects = __instance.objects;
            Vector2 key = new Vector2((position.Left / Game1.tileSize), (position.Top / Game1.tileSize));

            if (__instance is DecoratableLocation || objects.ContainsKey(key))
                return;

            foreach (Vector2 k in objects.Keys)
                if (objects[k] is Furniture)
                    if (objects[k].boundingBox.Value.Intersects(position))
                    {
                        Chest chest = new Chest(true)
                        {
                            name = "collider"
                        };
                        objects.Add(key, chest);
                        return;
                    }
        }

        internal static void Postfix(GameLocation __instance, Rectangle position)
        {
            OverlaidDictionary<Vector2, Object> objects = __instance.objects;
            Vector2 key = new Vector2((position.Left / Game1.tileSize), (position.Top / Game1.tileSize));

            if (objects.ContainsKey(key) && objects[key] is Chest chest && chest.name.Equals("collider"))
                objects.Remove(key);
        }
    }
}
