using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Entoarox.AdvancedLocationLoader.Configs;
using Entoarox.Framework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Locations;
using xTile.Layers;
using xTile.ObjectModel;
using SObject = StardewValley.Object;
using Tile = xTile.Tiles.Tile;

namespace Entoarox.AdvancedLocationLoader
{
    internal static class Actions
    {
        /*********
        ** Fields
        *********/
        internal static Random Random = new Random();


        /*********
        ** Public methods
        *********/
        public static void Shift(Farmer who, string[] arguments, Vector2 tile)
        {
            Game1.warpFarmer(who.currentLocation.Name, Convert.ToInt32(arguments[0]), Convert.ToInt32(arguments[1]), who.facingDirection, who.currentLocation.isStructure.Value);
        }

        public static void Message(Farmer who, string[] arguments, Vector2 tile)
        {
            Game1.drawDialogueNoTyping(ModEntry.Strings.Get(arguments[0] + ":" + arguments[1]));
        }

        public static void RawMessage(Farmer who, string[] arguments, Vector2 tile)
        {
            Game1.drawDialogueNoTyping(string.Join(" ", arguments));
        }

        public static void RandomMessage(Farmer who, string[] arguments, Vector2 tile)
        {
            string[] opts = string.Join(" ", arguments).Split('|');
            Game1.drawObjectDialogue(opts[Actions.Random.Next(opts.Length)]);
        }

        public static void React(Farmer who, string[] arguments, Vector2 tile)
        {
            int interval = Convert.ToInt32(arguments[0]);
            int[] indexes = arguments[1].Split(',').Select(e => Convert.ToInt32(e)).ToArray();
            Layer layer = who.currentLocation.Map.GetLayer("Buildings");
            Tile source = layer.Tiles[(int)tile.X, (int)tile.Y];
            PropertyValue property = who.currentLocation.GetTileProperty((int)tile.X, (int)tile.Y, "Buildings", "Action");
            int delay = interval * indexes.Length;
            who.currentLocation.SetTile((int)tile.X, (int)tile.Y, "Buildings", indexes, interval, source.TileSheet.Id);
            Timer timer = null;
            timer = new Timer(obj =>
                {
                    who.currentLocation.SetTile((int)tile.X, (int)tile.Y, "Buildings", source.TileIndex, source.TileSheet.Id);
                    who.currentLocation.SetTileProperty((int)tile.X, (int)tile.Y, "Buildings", "Action", property);
                    timer.Dispose();
                },
                null, delay, Timeout.Infinite
            );
        }

        public static void Teleporter(Farmer who, string[] arguments, Vector2 tile)
        {
            if (ModEntry.PatchData.Teleporters.Any(e => e.ListName.Equals(arguments[0].Trim())))
                TeleportationResolver.Request(arguments[0]).Init();
            else
            {
                ModEntry.Logger.Log("Teleporter does not exist: " + arguments[0], LogLevel.Error);
                IList<string> lists = new List<string>();
                foreach (TeleporterList list in ModEntry.PatchData.Teleporters)
                    lists.Add(list.ListName);
                ModEntry.Logger.Log("Known lists: " + string.Join(",", lists), LogLevel.Trace);
                Game1.drawObjectDialogue(ModEntry.Strings.Get("sparkle"));
            }
        }

        public static void Conditional(Farmer who, string[] arguments, Vector2 tile)
        {
            if (!who.mailReceived.Contains("ALLCondition_" + arguments[0]) && ModEntry.PatchData.Conditionals.Any(e => e.Name == arguments[0]))
                ConditionalResolver.Request(arguments[0]).Init();
            else
            {
                if (who.mailReceived.Contains("ALLCondition_" + arguments[0]))
                    ModEntry.Logger.Log("Conditional has already been completed: " + arguments[0], LogLevel.Error);
                else
                    ModEntry.Logger.Log("Conditional does not exist: " + arguments[0], LogLevel.Error);
                Game1.drawObjectDialogue(ModEntry.Strings.Get("sparkle"));
            }
        }

        public static void Shop(Farmer who, string[] arguments, Vector2 tile)
        {
            try
            {
                if (!ModEntry.PatchData.Shops.Any(p => p.Name == arguments[0]))
                {
                    Game1.activeClickableMenu = new ShopMenu(new List<Item>());
                    ModEntry.Logger.Log("Unable to open shop, shop not found: " + arguments[0], LogLevel.Error);
                }
                else
                {
                    ShopConfig shop = ModEntry.PatchData.Shops.First(p => p.Name == arguments[0]);
                    List<Item> stock = new List<Item>();
                    NPC portrait = new NPC
                    {
                        Portrait = shop.PortraitTexture,
                        Name = shop.Owner
                    };
                    foreach (ShopItem item in shop.Items)
                    {
                        if (!string.IsNullOrEmpty(item.Conditions) && !ModEntry.SHelper.Conditions().ValidateConditions(item.Conditions))
                            continue;
                        SObject result = item.Price != null
                            ? new SObject(item.Id, item.Stock, false, (int)item.Price)
                            : new SObject(item.Id, item.Stock);
                        stock.Add(item.Stack > 1
                            ? new StackableShopObject(result, item.Stack)
                            : result
                        );
                    }

                    if (stock.Count == 0)
                        ModEntry.Logger.Log("No stock: " + arguments[0] + ", if this is intended this message can be ignored.", LogLevel.Warn);
                    Game1.activeClickableMenu = new ShopMenu(stock)
                    {
                        portraitPerson = portrait,
                        potraitPersonDialogue = shop.Messages[Actions.Random.Next(shop.Messages.Count)]
                    };
                }
            }
            catch (Exception err)
            {
                ModEntry.Logger.Log("Unable to open shop due to unexpected error: ", LogLevel.Error, err);
            }
        }
        public static void Trigger(Farmer who, string[] arguments, Vector2 tile)
        {
            switch(arguments[0])
            {
                case "Fridge":
                    ((FarmHouse)Game1.getLocationFromName("FarmHouse")).fridge.Value.checkForAction(who, false);
                    break;
                default:
                    ModEntry.Logger.Log("Unknown ALLTrigger argument: " + arguments[0], LogLevel.Error);
                    break;
            }
        }
    }
}
