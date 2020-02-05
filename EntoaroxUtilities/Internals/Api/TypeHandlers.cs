using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using StardewValley;
using StardewValley.Objects;
using StardewValley.Tools;
using SObject = StardewValley.Object;

using Microsoft.Xna.Framework;

namespace Entoarox.Utilities.Internals.Api
{
    internal static class TypeHandlers
    {

        public static Item StardewObject(string sid)
        {
            if (Data.StardewObjectMapping.ContainsKey(sid))
                return new SObject(Data.StardewObjectMapping[sid], 1);
            if (int.TryParse(sid, out int iid))
                return new SObject(iid, 1);
            return null;
        }
        public static Item StardewCraftable(string sid)
        {
            if (Data.StardewCraftableMapping.ContainsKey(sid))
                return new SObject(Vector2.Zero, Data.StardewCraftableMapping[sid], 1);
            if (int.TryParse(sid, out int iid))
                return new SObject(Vector2.Zero, iid, 1);
            return null;
        }
        public static Item StardewBoots(string sid)
        {
            if (Data.StardewBootsMapping.ContainsKey(sid))
                return new Boots(Data.StardewBootsMapping[sid]);
            if (int.TryParse(sid, out int iid))
                return new Boots(iid);
            return null;
        }
        public static Item StardewFurniture(string sid)
        {
            int riid = -1;
            if (Data.StardewFurnitureMapping.ContainsKey(sid))
                riid = Data.StardewFurnitureMapping[sid];
            else if (int.TryParse(sid, out int iid))
                riid = iid;
            if(riid == -1)
                return null;
            if (riid == 1466 || riid == 1468)
                return new TV(riid, Vector2.Zero);
            return new Furniture(riid, Vector2.Zero);
        }
        public static Item StardewHat(string sid)
        {
            if (Data.StardewHatMapping.ContainsKey(sid))
                return new Hat(Data.StardewHatMapping[sid]);
            if (int.TryParse(sid, out int iid))
                return new Hat(iid);
            return null;
        }
        public static Item StardewRing(string sid)
        {
            if (Data.StardewRingMapping.ContainsKey(sid))
                return new Ring(Data.StardewRingMapping[sid]);
            if (int.TryParse(sid, out int iid))
                return new Ring(iid);
            return null;
        }
        public static Item StardewFloor(string sid)
        {
            if (Data.StardewFloorMapping.ContainsKey(sid))
                return new Wallpaper(Data.StardewFloorMapping[sid], true);
            if (int.TryParse(sid, out int iid))
                return new Wallpaper(iid, true);
            return null;
        }
        public static Item StardewWallpaper(string sid)
        {
            if (Data.StardewWallpaperMapping.ContainsKey(sid))
                return new Wallpaper(Data.StardewWallpaperMapping[sid]);
            if (int.TryParse(sid, out int iid))
                return new Wallpaper(iid);
            return null;
        }
        public static Item StardewWeapon(string sid)
        {
            int riid = -1;
            if (Data.StardewWeaponMapping.ContainsKey(sid))
                riid = Data.StardewWeaponMapping[sid];
            else if (int.TryParse(sid, out int iid))
                riid = iid;
            if (riid == -1)
                return null;
            if (riid >= 32 && riid <= 34)
                return new Slingshot(riid);
            return new MeleeWeapon(riid);
        }
        public static Item StardewClothing(string sid)
        {
            int riid = -1;
            if (Data.StardewClothingMapping.ContainsKey(sid))
                riid = Data.StardewClothingMapping[sid];
            else if (int.TryParse(sid, out int iid))
                riid = iid;
            if (riid == -1)
                return null;
            return new Clothing(riid);
        }
        public static Item StardewTool(string sid)
        {
            switch (sid)
            {
                case "milk_pail":
                    return new MilkPail();
                case "shears":
                    return new Shears();
                case "pan":
                    return new Pan();
                case "wand":
                    return new Wand();
                case "iridium_fishing_rod": // Exception, fishing rod does not have a iridium upgrade level unlike the other upgrade-supporting tools
                    return null; // So when asked for, we return null
                default:
                    string[] split = sid.Split('_');
                    if (split.Length != 2)
                        return null;
                    Tool tool;
                    switch (split[1])
                    {
                        case "axe":
                            tool = new Axe();
                            break;
                        case "hoe":
                            tool = new Hoe();
                            break;
                        case "pickaxe":
                            tool = new Pickaxe();
                            break;
                        case "wateringcan":
                            tool = new WateringCan();
                            break;
                        case "fishing_rod":
                            tool = new FishingRod();
                            break;
                        default:
                            return null;
                    }
                    switch (split[0])
                    {
                        case "starter":
                            tool.UpgradeLevel = 0;
                            break;
                        case "copper":
                            tool.UpgradeLevel = 1;
                            break;
                        case "steel":
                            tool.UpgradeLevel = 2;
                            break;
                        case "gold":
                            tool.UpgradeLevel = 3;
                            break;
                        case "iridium":
                            tool.UpgradeLevel = 4;
                            break;
                        default:
                            return null;
                    }
                    return tool;
            }
        }
        public static Item StardewArtisan(string sid)
        {
            if (sid.Equals("wild_honey"))
                return new SObject(340, 1) { name = "Wild Honey" };
            string[] split = sid.Split('_');
            if (split.Length < 2)
                return null;
            string oid = split[split.Length - 1];
            string cid = EntoUtilsApi.Instance.ResolveFirstMatchingTypeId<SObject>(string.Join("_", split.Take(split.Length - 1)), new List<string>() { "sdv.artisan" });
            if (cid == null)
                return null;
            SObject item = (SObject)EntoUtilsApi.Instance.ResolveItemTypeId(cid);
            SObject result;
            switch(oid)
            {
                case "honey":
                    result = new SObject(340, 1);
                    result.Price += item.Price;
                    break;
                case "pickles":
                    result = new SObject(Vector2.Zero, 342, "Pickled " + item.Name, false, true, false, false)
                    {
                        Price = 50 + item.Price * 2
                    };
                    break;
                case "jelly":
                    result = new SObject(Vector2.Zero, 344, item.Name + " Jelly", false, true, false, false)
                    {
                        Price = 50 + item.Price * 2
                    };
                    break;
                case "wine":
                    result = new SObject(Vector2.Zero, 348, item.Name + " Wine", false, true, false, false)
                    {
                        Price = item.Price * 3
                    };
                    break;
                case "juice":
                    result = new SObject(Vector2.Zero, 350, item.Name + " Juice", false, true, false, false)
                    {
                        Price = (int)(item.Price * 2.25)
                    };
                    break;
                default:
                    return null;
            }
            result.preservedParentSheetIndex.Value = item.ParentSheetIndex;
            return result;
        }

        public static Item JsonAssetsObject(string sid)
        {
            int iid = Data.JaApi.GetObjectId(sid);
            if (iid == -1)
                return null;
            return new SObject(iid, 1);
        }
        public static Item JsonAssetsCraftable(string sid)
        {
            int iid = Data.JaApi.GetBigCraftableId(sid);
            if (iid == -1)
                return null;
            return new SObject(Vector2.Zero, iid, 1);
        }
        public static Item JsonAssetsHat(string sid)
        {
            int iid = Data.JaApi.GetHatId(sid);
            if (iid == -1)
                return null;
            return new Hat(iid);
        }
        public static Item JsonAssetsWeapon(string sid)
        {
            int iid = Data.JaApi.GetWeaponId(sid);
            if (iid == -1)
                return null;
            return new MeleeWeapon(iid);
        }
        public static Item JsonAssetsClothing(string sid)
        {
            int iid = Data.JaApi.GetClothingId(sid);
            if (iid == -1)
                return null;
            return new Clothing(iid);
        }

        public static Item CustomFarmingMachine(string sid)
        {
            return Data.CfApi.getCustomObject(sid);
        }

        public static Item PrismaticToolsItem(string sid)
        {
            switch(sid)
            {
                case "axe":
                    return new Axe { UpgradeLevel = 5 };
                case "pickaxe":
                    return new Pickaxe { UpgradeLevel = 5 };
                case "hoe":
                    return new Hoe { UpgradeLevel = 5 };
                case "wateringcan":
                    return new WateringCan { UpgradeLevel = 5 };
                case "sprinkler":
                    return new SObject(Data.PtApi.SprinklerIndex, 1);
                case "bar":
                    return new SObject(Data.PtApi.BarIndex, 1);
                default:
                    return null;
            }
        }
    }
}
