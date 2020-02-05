using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using StardewValley;
using StardewValley.Objects;
using StardewValley.Tools;
using SObject = StardewValley.Object;

namespace Entoarox.Utilities.Internals.Api
{
    internal static class TypeResolvers
    {
        internal static bool StardewResolver(Item item, ref string typeId)
        {
            switch(item)
            {
                case Tool tool:
                    string Obj;
                    switch(tool)
                    {
                        case Axe axe:
                            Obj = "axe";
                            break;
                        case Pickaxe pickaxe:
                            Obj = "pickaxe";
                            break;
                        case WateringCan can:
                            Obj = "wateringcan";
                            break;
                        case Hoe hoe:
                            Obj = "hoe";
                            break;
                        case FishingRod rod:
                            Obj = "fishing_rod";
                            if (tool.UpgradeLevel > 3)
                                return false;
                            break;
                        case Pan pan:
                            typeId = "sdv.tool:pan";
                            return true;
                        case Shears shears:
                            typeId = "sdv.tool:shears";
                            return true;
                        case Wand wand:
                            typeId = "sdv.tool:wand";
                            return true;
                        case MilkPail pail:
                            typeId = "sdv.tool:milk_pail";
                            return true;
                        case MeleeWeapon weapon:
                            if (Data.StardewWeaponMappingReverse.ContainsKey(weapon.ParentSheetIndex))
                            {
                                typeId = "sdv.weapon:" + Data.StardewWeaponMappingReverse[weapon.ParentSheetIndex];
                                return true;
                            }
                            typeId = "sdv.weapon:" + weapon.ParentSheetIndex;
                            return false;
                        case Slingshot shot:
                            if (Data.StardewWeaponMappingReverse.ContainsKey(shot.ParentSheetIndex))
                            {
                                typeId = "sdv.weapon:" + Data.StardewWeaponMappingReverse[shot.ParentSheetIndex];
                                return true;
                            }
                            typeId = "sdv.weapon:" + shot.ParentSheetIndex;
                            return false;
                        default:
                            return false;
                    }
                    switch(tool.UpgradeLevel)
                    {
                        case 0:
                            typeId = "sdv.tool:starter_" + Obj;
                            return true;
                        case 1:
                            typeId = "sdv.tool:copper_" + Obj;
                            return true;
                        case 2:
                            typeId = "sdv.tool:steel_" + Obj;
                            return true;
                        case 3:
                            typeId = "sdv.tool:gold_" + Obj;
                            return true;
                        case 4:
                            typeId = "sdv.tool:iridium_" + Obj;
                            return true;
                    }
                    break;
                case Ring ring:
                    if (Data.StardewRingMappingReverse.ContainsKey(ring.ParentSheetIndex))
                    {
                        typeId = "sdv.ring:" + Data.StardewRingMappingReverse[ring.ParentSheetIndex];
                        return true;
                    }
                    typeId = "sdv.ring:" + ring.ParentSheetIndex;
                    break;
                case Hat hat:
                    if (Data.StardewHatMappingReverse.ContainsKey(hat.ParentSheetIndex))
                    {
                        typeId = "sdv.hat:" + Data.StardewHatMappingReverse[hat.ParentSheetIndex];
                        return true;
                    }
                    typeId = "sdv.hat:" + hat.ParentSheetIndex;
                    break;
                case Boots boots:
                    if (Data.StardewBootsMappingReverse.ContainsKey(boots.ParentSheetIndex))
                    {
                        typeId = "sdv.boots:" + Data.StardewBootsMappingReverse[boots.ParentSheetIndex];
                        return true;
                    }
                    typeId = "sdv.boots:" + boots.ParentSheetIndex;
                    break;
                case Wallpaper wallpaper:
                    if(wallpaper.isFloor.Value)
                    {
                        if (Data.StardewFloorMappingReverse.ContainsKey(wallpaper.ParentSheetIndex))
                        {
                            typeId = "sdv.floor:" + Data.StardewFloorMappingReverse[wallpaper.ParentSheetIndex];
                            return true;
                        }
                        typeId = "sdv.floor:" + wallpaper.ParentSheetIndex;
                    }
                    else
                    {
                        if (Data.StardewFloorMappingReverse.ContainsKey(wallpaper.ParentSheetIndex))
                        {
                            typeId = "sdv.wallpaper:" + Data.StardewFloorMappingReverse[wallpaper.ParentSheetIndex];
                            return true;
                        }
                        typeId = "sdv.wallpaper:" + wallpaper.ParentSheetIndex;
                    }
                    break;
                case SObject obj:
                    if(obj.bigCraftable.Value)
                    {
                        if (Data.StardewCraftableMappingReverse.ContainsKey(obj.ParentSheetIndex))
                        {
                            typeId = "sdv.craftable:" + Data.StardewCraftableMappingReverse[obj.ParentSheetIndex];
                            return true;
                        }
                        typeId = "sdv.craftable:" + obj.ParentSheetIndex;
                    }
                    else
                    {
                        //TODO: Implement handling for Artisan items
                        if (Data.StardewObjectMappingReverse.ContainsKey(obj.ParentSheetIndex))
                        {
                            typeId = "sdv.craftable:" + Data.StardewObjectMappingReverse[obj.ParentSheetIndex];
                            return true;
                        }
                        typeId = "sdv.craftable:" + obj.ParentSheetIndex;
                    }
                    break;
            }
            return false;
        }

        internal static bool JsonAssetsResolver(Item item, ref string typeId)
        {
            if(Data.JaObjectMapping == null)
            {
                Data.JaObjectMapping = Data.JaApi.GetAllObjectIds().ToDictionary(_ => _.Value, _ => _.Key);
                Data.JaCraftableMapping = Data.JaApi.GetAllBigCraftableIds().ToDictionary(_ => _.Value, _ => _.Key);
                Data.JaHatMapping = Data.JaApi.GetAllHatIds().ToDictionary(_ => _.Value, _ => _.Key);
                Data.JaWeaponMapping = Data.JaApi.GetAllWeaponIds().ToDictionary(_ => _.Value, _ => _.Key);
                Data.JaClothingMapping = Data.JaApi.GetAllClothingIds().ToDictionary(_ => _.Value, _ => _.Key);

            }
            switch(item)
            {
                case Clothing clothing:
                    if(Data.JaClothingMapping.ContainsKey(clothing.ParentSheetIndex))
                    {
                        typeId = "ja.clothing:" + Data.JaClothingMapping[clothing.ParentSheetIndex];
                        return true;
                    }
                    break;
                case Hat hat:
                    if (Data.JaHatMapping.ContainsKey(hat.ParentSheetIndex))
                    {
                        typeId = "ja.hat:" + Data.JaClothingMapping[hat.ParentSheetIndex];
                        return true;
                    }
                    break;
                case MeleeWeapon weapon:
                    if (Data.JaWeaponMapping.ContainsKey(weapon.ParentSheetIndex))
                    {
                        typeId = "ja.weapon:" + Data.JaWeaponMapping[weapon.ParentSheetIndex];
                        return true;
                    }
                    break;
                case SObject obj:
                    if(obj.bigCraftable.Value)
                    {
                        if (Data.JaCraftableMapping.ContainsKey(obj.ParentSheetIndex))
                        {
                            typeId = "ja.craftable:" + Data.JaCraftableMapping[obj.ParentSheetIndex];
                            return true;
                        }
                    }
                    else if (Data.JaObjectMapping.ContainsKey(obj.ParentSheetIndex))
                    {
                        typeId = "ja.object:" + Data.JaObjectMapping[obj.ParentSheetIndex];
                        return true;
                    }
                    break;
            }
            return false;
        }

        internal static bool PrismaticToolsResolver(Item item, ref string typeId)
        {
            if(item is Tool tool && tool.UpgradeLevel == 5)
            {
                if(item is Axe)
                {
                    typeId = "pt.item:axe";
                    return true;
                }
                if(item is Pickaxe)
                {
                    typeId = "pt.item:pickaxe";
                    return true;
                }
                if(item is Hoe)
                {
                    typeId = "pt.item:hoe";
                    return true;
                }
                if(item is WateringCan)
                {
                    typeId = "pt.item:wateringCan";
                    return true;
                }
            }
            else if(item is SObject obj && !obj.bigCraftable)
            {
                if(obj.ParentSheetIndex == Data.PtApi.BarIndex)
                {
                    typeId = "pt.item:bar";
                    return true;
                }
                if (obj.ParentSheetIndex == Data.PtApi.SprinklerIndex)
                {
                    typeId = "pt.item:sprinkler";
                    return true;
                }
            }
            return false;
        }
    }
}
