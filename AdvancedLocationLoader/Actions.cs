using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewValley;

using Entoarox.Framework;

using StardewValley.Menus;

namespace Entoarox.AdvancedLocationLoader
{
    internal static class Actions
    {
        internal static Random _Random = new Random();
        internal static void Shift(StardewValley.Farmer who, string[] arguments, Vector2 tile)
        {
            Game1.warpFarmer(who.currentLocation.Name, Convert.ToInt32(arguments[0]), Convert.ToInt32(arguments[1]),who.facingDirection,who.currentLocation.isStructure);
        }
        internal static void Message(StardewValley.Farmer who, string[] arguments, Vector2 tile)
        {
            Game1.drawDialogueNoTyping(ModEntry.Strings.Get(arguments[0] + ":" + arguments[1]));
        }
        internal static void RawMessage(StardewValley.Farmer who, string[] arguments, Vector2 tile)
        {
            Game1.drawDialogueNoTyping(string.Join(" ", arguments));
        }
        internal static void RandomMessage(StardewValley.Farmer who, string[] arguments, Vector2 tile)
        {
            string[] opts = string.Join(" ", arguments).Split('|');
            Game1.drawObjectDialogue(opts[_Random.Next(opts.Length)]);
        }
        internal static void React(StardewValley.Farmer who, string[] arguments, Vector2 tile)
        {
            int interval = Convert.ToInt32(arguments[0]);
            int[] indexes = arguments[1].Split(',').Select(e => Convert.ToInt32(e)).ToArray();
            var layer = who.currentLocation.Map.GetLayer("Buildings");
            var source = layer.Tiles[(int)tile.X, (int)tile.Y];
            xTile.ObjectModel.PropertyValue property = who.currentLocation.GetTileProperty((int)tile.X, (int)tile.Y, "Buildings", "Action");
            int delay = interval * indexes.Length;
            who.currentLocation.SetTile((int)tile.X, (int)tile.Y, "Buildings", indexes, interval, source.TileSheet.Id);
            System.Threading.Timer timer = null;
            timer = new System.Threading.Timer((obj) =>
            {
                who.currentLocation.SetTile((int)tile.X, (int)tile.Y, "Buildings", source.TileIndex, source.TileSheet.Id);
                who.currentLocation.SetTileProperty((int)tile.X, (int)tile.Y, "Buildings", "Action",property);
                timer.Dispose();
            },
            null, delay, System.Threading.Timeout.Infinite);
        }
        internal static void Teleporter(StardewValley.Farmer who, string[] arguments, Vector2 tile)
        {
            if(ModEntry.PatchData.Teleporters.Any(e => e.ListName.Equals(arguments[0].Trim())))
                TeleportationResolver.Request(arguments[0]).Init();
            else
            {
                ModEntry.Logger.Log("Teleporter does not exist: "+arguments[0],StardewModdingAPI.LogLevel.Error);
                List<string> lists = new List<string>();
                foreach (var list in ModEntry.PatchData.Teleporters)
                    lists.Add(list.ListName);
                ModEntry.Logger.Log("Known lists: " + string.Join(",", lists), StardewModdingAPI.LogLevel.Trace);
                Game1.drawObjectDialogue(ModEntry.Strings.Get("sparkle"));
            }
        }
        internal static void Conditional(StardewValley.Farmer who, string[] arguments, Vector2 tile)
        {
            if(!who.mailReceived.Contains("ALLCondition_"+arguments[0]) && ModEntry.PatchData.Conditionals.Any(e => e.Name==arguments[0]))
                ConditionalResolver.Request(arguments[0]).Init();
            else
            {
                if(who.mailReceived.Contains("ALLCondition_"+arguments[0]))
                    ModEntry.Logger.Log("Conditional has already been completed: " + arguments[0], StardewModdingAPI.LogLevel.Error);
                else
                    ModEntry.Logger.Log("Conditional does not exist: " + arguments[0], StardewModdingAPI.LogLevel.Error);
                Game1.drawObjectDialogue(ModEntry.Strings.Get("sparkle"));
            }
        }
        internal static void Shop(StardewValley.Farmer who, string[] arguments, Vector2 tile)
        {
            try
            {
                if (!ModEntry.PatchData.Shops.Any(p => p.Name == arguments[0]))
                {
                    Game1.activeClickableMenu = new ShopMenu(new List<Item>(), 0, null);
                    ModEntry.Logger.Log("Unable to open shop, shop not found: " + arguments[0], StardewModdingAPI.LogLevel.Error);
                }
                else
                {
                    Configs.ShopConfig shop = ModEntry.PatchData.Shops.First(p => p.Name == arguments[0]);
                    List<Item> stock = new List<Item>();
                    NPC portrait = new NPC();
                    portrait.Portrait = ModEntry.SHelper.Content.Load<Texture2D>(shop.Portrait);
                    portrait.Name = shop.Owner;
                    foreach (Configs.ShopItem item in shop.Items)
                    {
                        if (!string.IsNullOrEmpty(item.Conditions) && !ModEntry.SHelper.Conditions().ValidateConditions(item.Conditions))
                            continue;
                        StardewValley.Object result;
                        if (item.Price != null)
                            result = new StardewValley.Object(item.Id, item.Stock, false, (int)item.Price);
                        else
                            result = new StardewValley.Object(item.Id, item.Stock, false, -1);
                        if (item.Stack > 1)
                        {
                            stock.Add(new StackableShopObject(result, item.Stack));
                        }
                        else
                            stock.Add(result);
                    }
                    if (stock.Count == 0)
                        ModEntry.Logger.Log("No stock: " + arguments[0] + ", if this is intended this message can be ignored.", StardewModdingAPI.LogLevel.Warn);
                    ShopMenu menu = new ShopMenu(stock);
                    menu.portraitPerson = portrait;
                    menu.potraitPersonDialogue = shop.Messages[_Random.Next(shop.Messages.Count)];
                    Game1.activeClickableMenu = menu;
                }
            }
            catch (Exception err)
            {
                ModEntry.Logger.Log("Unable to open shop due to unexpected error: ", StardewModdingAPI.LogLevel.Error, err);
            }
        }
    }
}
