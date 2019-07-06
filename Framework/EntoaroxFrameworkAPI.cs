using System;
using System.Collections.Generic;
using System.Linq;

using StardewValley;
using SObject = StardewValley.Object;
using StardewValley.Objects;
using StardewValley.Tools;

using Microsoft.Xna.Framework;

using Entoarox.Framework.Core;
using Entoarox.Framework.ApiWrappers;

namespace Entoarox.Framework
{
    public class EntoaroxFrameworkAPI
    {
        /*********
        ** Fields
        *********/
        internal static EntoaroxFrameworkAPI Singleton;
        private static PlayerModifier Modifier = new PlayerModifier();
        private static readonly Dictionary<string, int> RunBoosts = new Dictionary<string, int>();
        private static readonly Dictionary<string, int> WalkBoosts = new Dictionary<string, int>();
        private static readonly Dictionary<string, Func<string, Item>> IdResolvers = new Dictionary<string, Func<string, Item>>();


        public EntoaroxFrameworkAPI()
        {
            Singleton = this;
        }

        internal static void Ready()
        {
            Singleton.RegisterIdResolver("sdv.object", id => new SObject(Convert.ToInt32(id), 1));
            Singleton.RegisterIdResolver("sdv.craftable", id => new SObject(Vector2.Zero, Convert.ToInt32(id), 1));
            Singleton.RegisterIdResolver("sdv.hat", id => new Hat(Convert.ToInt32(id)));
            Singleton.RegisterIdResolver("sdv.furniture", id => new Furniture(Convert.ToInt32(id), Vector2.Zero));
            Singleton.RegisterIdResolver("sdv.ring", id => new Ring(Convert.ToInt32(id)));
            Singleton.RegisterIdResolver("sdv.flooring", id => new Wallpaper(Convert.ToInt32(id), true));
            Singleton.RegisterIdResolver("sdv.wallpaper", id => new Wallpaper(Convert.ToInt32(id)));
            Singleton.RegisterIdResolver("sdv.sign", id => new Sign(Vector2.Zero, Convert.ToInt32(id)));
            Singleton.RegisterIdResolver("sdv.weapon", id => new MeleeWeapon(Convert.ToInt32(id)));
            Singleton.RegisterIdResolver("sdv.wine", id =>
            {
                SObject item = new SObject(Convert.ToInt32(id), 1);
                SObject wine = new SObject(348, 1)
                {
                    Name = $"{item.Name} Wine",
                    Price = item.Price * 3
                };
                wine.preserve.Value = SObject.PreserveType.Wine;
                wine.preservedParentSheetIndex.Value = item.ParentSheetIndex;
                return wine;
            });
            Singleton.RegisterIdResolver("sdv.jelly", id =>
            {
                SObject item = new SObject(Convert.ToInt32(id), 1);
                SObject jelly = new SObject(344, 1)
                {
                    Name = $"{item.Name} Jelly",
                    Price = 50 + item.Price * 2
                };
                jelly.preserve.Value = SObject.PreserveType.Jelly;
                jelly.preservedParentSheetIndex.Value = item.ParentSheetIndex;
                return jelly;
            });
            Singleton.RegisterIdResolver("sdv.juice", id =>
            {
                SObject item = new SObject(Convert.ToInt32(id), 1);
                SObject juice = new SObject(350, 1)
                {
                    Name = $"{item.Name} Juice",
                    Price = (int)(item.Price * 2.25d)
                };
                juice.preserve.Value = SObject.PreserveType.Juice;
                juice.preservedParentSheetIndex.Value = item.ParentSheetIndex;
                return juice;
            });
            Singleton.RegisterIdResolver("sdv.pickled", id =>
            {
                SObject item = new SObject(Convert.ToInt32(id), 1);
                SObject pickled = new SObject(342, 1)
                {
                    Name = $"Pickled {item.Name}",
                    Price = 50 + item.Price * 2
                };
                pickled.preserve.Value = SObject.PreserveType.Pickle;
                pickled.preservedParentSheetIndex.Value = item.ParentSheetIndex;
                return pickled;
            });
            Singleton.RegisterIdResolver("sdv.honey", id =>
            {
                SObject item = new SObject(Convert.ToInt32(id), 1);
                SObject honey = new SObject(Vector2.Zero, 340, "Honey", false, true, false, false)
                {
                    Name = "Wild Honey",
                };
                var type = SObject.HoneyType.Wild;
                switch (item.ParentSheetIndex)
                {
                    case 376:
                        type = SObject.HoneyType.Poppy;
                        break;
                    case 591:
                        type = SObject.HoneyType.Tulip;
                        break;
                    case 593:
                        type = SObject.HoneyType.SummerSpangle;
                        break;
                    case 595:
                        type = SObject.HoneyType.FairyRose;
                        break;
                    case 597:
                        type = SObject.HoneyType.BlueJazz;
                        break;
                }
                honey.honeyType.Value = type;
                if (type != SObject.HoneyType.Wild)
                {
                    honey.Name = $"{item.Name} Honey";
                    honey.Price += item.Price * 2;
                }
                return honey;
            });
            Singleton.RegisterIdResolver("sdv.other", id => {
                switch(id)
                {
                    case "chest":
                        return new Chest(Vector2.Zero);
                    case "crabpot":
                        return new CrabPot(Vector2.Zero);
                    case "indoorpot":
                        return new IndoorPot(Vector2.Zero);
                    case "milkpail":
                        return new MilkPail();
                }
                return null;
            });
            var registry = EntoaroxFrameworkMod.SHelper.ModRegistry;
            // Compatibility: Support for items added by JsonAssets
            if (registry.IsLoaded("spacechase0.JsonAssets"))
            {
                IJsonAssetsAPI jaApi = registry.GetApi<IJsonAssetsAPI>("spacechase0.JsonAssets");
                Singleton.RegisterIdResolver("ja.object", id => new SObject(jaApi.GetObjectId(id), 1));
                Singleton.RegisterIdResolver("ja.craftable", id => new SObject(Vector2.Zero, jaApi.GetBigCraftableId(id), 1));
                Singleton.RegisterIdResolver("ja.hat", id => new Hat(jaApi.GetHatId(id)));
                Singleton.RegisterIdResolver("ja.weapon", id => new MeleeWeapon(jaApi.GetWeaponId(id)));
                Singleton.RegisterIdResolver("ja.ring", id => new Ring(jaApi.GetObjectId(id)));
            }
        }

        /*********
        ** Public methods
        *********/
        public void AddBoost(string id, int amount, bool forWalking = true, bool forRunning = true)
        {
            if (forWalking)
                EntoaroxFrameworkAPI.WalkBoosts[id] = amount;
            if (forRunning)
                EntoaroxFrameworkAPI.RunBoosts[id] = amount;
            EntoaroxFrameworkAPI.Recalculate();
        }

        public void RemoveBoost(string id)
        {
            EntoaroxFrameworkAPI.WalkBoosts.Remove(id);
            EntoaroxFrameworkAPI.RunBoosts.Remove(id);
            EntoaroxFrameworkAPI.Recalculate();
        }

        public void RegisterIdResolver(string nspace, Func<string, Item> resolver)
        {
            EntoaroxFrameworkAPI.IdResolvers.Add(nspace, resolver);
        }
        public Item ResolveId(string id)
        {
            string[] parts = id.Split(':');
            if (!EntoaroxFrameworkAPI.IdResolvers.ContainsKey(parts[0]))
                return null;
            return EntoaroxFrameworkAPI.IdResolvers[parts[0]](parts[1]);
        }


        /*********
        ** Protected methods
        *********/
        private static void Recalculate()
        {
            EntoaroxFrameworkMod.SHelper.Player().Modifiers.Remove(EntoaroxFrameworkAPI.Modifier);
            EntoaroxFrameworkAPI.Modifier.WalkSpeedModifier = EntoaroxFrameworkAPI.WalkBoosts.Values.Aggregate((total, next) => total + next);
            EntoaroxFrameworkAPI.Modifier.RunSpeedModifier = EntoaroxFrameworkAPI.RunBoosts.Values.Aggregate((total, next) => total + next);
            EntoaroxFrameworkMod.SHelper.Player().Modifiers.Add(EntoaroxFrameworkAPI.Modifier);
        }
    }
}
