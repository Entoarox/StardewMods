using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

using StardewValley;

namespace Entoarox.Utilities.Internals.Api
{
    internal static class Data
    {
        internal static Dictionary<string, Func<string, Item>> HandlerRegistry = new Dictionary<string, Func<string, Item>>();
        internal static List<EUGlobals.TypeIdResolverDelegate> ResolverRegistry = new List<EUGlobals.TypeIdResolverDelegate>();
        #region Mappings
        internal static Dictionary<string, int> StardewObjectMapping;
        internal static Dictionary<string, int> StardewCraftableMapping;
        internal static Dictionary<string, int> StardewBootsMapping;
        internal static Dictionary<string, int> StardewFurnitureMapping;
        internal static Dictionary<string, int> StardewHatMapping;
        internal static Dictionary<string, int> StardewRingMapping;
        internal static Dictionary<string, int> StardewFloorMapping;
        internal static Dictionary<string, int> StardewWallpaperMapping;
        internal static Dictionary<string, int> StardewWeaponMapping;
        internal static Dictionary<string, int> StardewClothingMapping;
        #endregion
        internal static IJsonAssetsApi JaApi;
        internal static ICustomFarmingApi CfApi;
        internal static IPrismaticToolsApi PtApi;

        internal static Dictionary<int, string> StardewObjectMappingReverse;
        internal static Dictionary<int, string> StardewCraftableMappingReverse;
        internal static Dictionary<int, string> StardewBootsMappingReverse;
        internal static Dictionary<int, string> StardewFurnitureMappingReverse;
        internal static Dictionary<int, string> StardewHatMappingReverse;
        internal static Dictionary<int, string> StardewRingMappingReverse;
        internal static Dictionary<int, string> StardewFloorMappingReverse;
        internal static Dictionary<int, string> StardewWallpaperMappingReverse;
        internal static Dictionary<int, string> StardewWeaponMappingReverse;
        internal static Dictionary<int, string> StardewClothingMappingReverse;

        internal static Dictionary<int, string> JaObjectMapping;
        internal static Dictionary<int, string> JaCraftableMapping;
        internal static Dictionary<int, string> JaHatMapping;
        internal static Dictionary<int, string> JaWeaponMapping;
        internal static Dictionary<int, string> JaClothingMapping;

        internal static void Init()
        {
            // Load Name => Id mappings from their respective JSON files
            StardewBootsMapping = EntoUtilsMod.Instance.Helper.Data.ReadJsonFile<Dictionary<string, int>>("Assets/Boots.json");
            StardewCraftableMapping = EntoUtilsMod.Instance.Helper.Data.ReadJsonFile<Dictionary<string, int>>("Assets/Craftable.json");
            StardewFloorMapping = EntoUtilsMod.Instance.Helper.Data.ReadJsonFile<Dictionary<string, int>>("Assets/Floor.json");
            StardewFurnitureMapping = EntoUtilsMod.Instance.Helper.Data.ReadJsonFile<Dictionary<string, int>>("Assets/Furniture.json");
            StardewHatMapping = EntoUtilsMod.Instance.Helper.Data.ReadJsonFile<Dictionary<string, int>>("Assets/Hat.json");
            StardewObjectMapping = EntoUtilsMod.Instance.Helper.Data.ReadJsonFile<Dictionary<string, int>>("Assets/Object.json");
            StardewRingMapping = EntoUtilsMod.Instance.Helper.Data.ReadJsonFile<Dictionary<string, int>>("Assets/Ring.json");
            StardewWallpaperMapping = EntoUtilsMod.Instance.Helper.Data.ReadJsonFile<Dictionary<string, int>>("Assets/Wallpaper.json");
            StardewWeaponMapping = EntoUtilsMod.Instance.Helper.Data.ReadJsonFile<Dictionary<string, int>>("Assets/Weapon.json");
            StardewClothingMapping = EntoUtilsMod.Instance.Helper.Data.ReadJsonFile<Dictionary<string, int>>("Assets/Clothing.json");
            // Create reverse mappings to make reverse-lookups faster (Each mapping gets its own task, as this makes things faster)
            Task.WaitAll(new Task[]
            {
                Task.Run(() => Map(StardewBootsMapping, ref StardewBootsMappingReverse)),
                Task.Run(() => Map(StardewCraftableMapping, ref StardewCraftableMappingReverse)),
                Task.Run(() => Map(StardewFloorMapping, ref StardewFloorMappingReverse)),
                Task.Run(() => Map(StardewFurnitureMapping, ref StardewFurnitureMappingReverse)),
                Task.Run(() => Map(StardewHatMapping, ref StardewHatMappingReverse)),
                Task.Run(() => Map(StardewObjectMapping, ref StardewObjectMappingReverse)),
                Task.Run(() => Map(StardewRingMapping, ref StardewRingMappingReverse)),
                Task.Run(() => Map(StardewWallpaperMapping, ref StardewWallpaperMappingReverse)),
                Task.Run(() => Map(StardewWeaponMapping, ref StardewWeaponMappingReverse)),
                Task.Run(() => Map(StardewClothingMapping, ref StardewClothingMappingReverse)),
            });
        }
        internal static void Map(Dictionary<string, int> source, ref Dictionary<int, string> target)
        {
            target = new Dictionary<int, string>();
            foreach (var itm in source)
                if (target.ContainsKey(itm.Value))
                    EntoUtilsMod.Instance.Monitor.Log($"Duplicate mapping: {itm.Value} already mapped as `{target[itm.Value]}` but also being mapped as `{itm.Key} now.", StardewModdingAPI.LogLevel.Warn);
                else
                    target.Add(itm.Value, itm.Key);
        }
    }
}
