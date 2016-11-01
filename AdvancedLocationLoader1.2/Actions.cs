using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using StardewValley;

using Entoarox.Framework;

using StardewModdingAPI.Events;

using StardewValley.Menus;

namespace Entoarox.AdvancedLocationLoader
{
    internal static class Actions
    {
        internal static Random _Random = new Random();
        internal static void Shift(Farmer who, string[] arguments, Vector2 tile)
        {
            Game1.warpFarmer(who.currentLocation, Convert.ToInt32(arguments[0]), Convert.ToInt32(arguments[1]),who.facingDirection,who.currentLocation.isStructure);
        }
        internal static void RandomMessage(Farmer who, string[] arguments, Vector2 tile)
        {
            string[] opts = string.Join(" ", arguments).Split('|');
            Game1.drawObjectDialogue(opts[_Random.Next(opts.Length)]);
        }
        internal static void React(Farmer who, string[] arguments, Vector2 tile)
        {
            int interval = Convert.ToInt32(arguments[0]);
            int[] indexes = arguments[1].Split(',').Select(e => Convert.ToInt32(e)).ToArray();
            var layer = who.currentLocation.Map.GetLayer("Buildings");
            var source = layer.Tiles[(int)tile.X, (int)tile.Y];
            xTile.ObjectModel.PropertyValue property = EntoFramework.GetLocationHelper().GetTileProperty(who.currentLocation, "Buildings", (int)tile.X, (int)tile.Y, "Action");
            int delay = interval * indexes.Length;
            EntoFramework.GetLocationHelper().SetAnimatedTile(who.currentLocation, "Buildings", (int)tile.X, (int)tile.Y, indexes, interval, source.TileSheet.Id);
            System.Threading.Timer timer = null;
            timer = new System.Threading.Timer((obj) =>
            {
                EntoFramework.GetLocationHelper().SetStaticTile(who.currentLocation, "Buildings", (int)tile.X, (int)tile.Y, source.TileIndex, source.TileSheet.Id);
                EntoFramework.GetLocationHelper().SetTileProperty(who.currentLocation, "Buildings", (int)tile.X, (int)tile.Y, "Action", property);
                timer.Dispose();
            },
            null, delay, System.Threading.Timeout.Infinite);
        }
        internal static void Teleporter(Farmer who, string[] arguments, Vector2 tile)
        {
            if(Configs.Compound.Teleporters.Exists(e => e.ListName==arguments[0]))
                TeleportationResolver.Request(arguments[0]).Init();
            else
            {
                AdvancedLocationLoaderMod.Logger.Error("Teleporter does not exist: "+arguments[0]);
                Game1.drawObjectDialogue(AdvancedLocationLoaderMod.Localizer.Localize("sparkle"));
            }
        }
        internal static void Conditional(Farmer who, string[] arguments, Vector2 tile)
        {
            if(!who.mailReceived.Contains("ALLCondition_"+arguments[0]) && Configs.Compound.Conditionals.Exists(e => e.Name==arguments[0]))
                ConditionalResolver.Request(arguments[0]).Init();
            else
            {
                if(who.mailReceived.Contains("ALLCondition_"+arguments[0]))
                    AdvancedLocationLoaderMod.Logger.Error("Conditional has already been completed: " + arguments[0]);
                else
                    AdvancedLocationLoaderMod.Logger.Error("Conditional does not exist: " + arguments[0]);
                Game1.drawObjectDialogue(AdvancedLocationLoaderMod.Localizer.Localize("sparkle"));
            }
        }
        internal static void Shop(Farmer who, string[] arguments, Vector2 tile)
        {
            ControlEvents.MouseChanged += (s, e) =>
            {
                if (e.NewState.RightButton != ButtonState.Pressed && e.PriorState.RightButton == ButtonState.Pressed)
                    RealShop(who, arguments, tile);
            };
        }
        internal static void RealShop(Farmer who, string[] arguments, Vector2 tile)
        {
            if (!Configs.Compound.Shops.ContainsKey(arguments[0]))
            {
                Game1.activeClickableMenu = new ShopMenu(new List<Item>(), 0, null);
                AdvancedLocationLoaderMod.Logger.Error("Unable to open shop, shop not found: "+arguments[0]);
            }
            else
            {
                Configs.ShopConfig shop = Configs.Compound.Shops[arguments[0]];
                List<Item> stock = new List<Item>();
                NPC portrait = new NPC();
                EntoFramework.GetContentRegistry().RegisterXnb(shop.Portrait, shop.Portrait);
                portrait.Portrait = Game1.content.Load<Texture2D>(shop.Portrait);
                portrait.name = shop.Owner;
                foreach (Configs.ShopItem item in shop.Items)
                {
                    if (!string.IsNullOrEmpty(item.Conditions) || !Conditions.CheckConditionList(item.Conditions))
                        continue;
                    StardewValley.Object result;
                    if (item.Price!=null)
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
                ShopMenu menu = new ShopMenu(stock);
                menu.portraitPerson = portrait;
                menu.potraitPersonDialogue = shop.Messages[_Random.Next(shop.Messages.Count)];
                Game1.activeClickableMenu = menu;
            }
        }
    }
    internal class TeleportationResolver
    {
        private static Dictionary<string, TeleportationResolver> Cache=new Dictionary<string, TeleportationResolver>();
        internal static TeleportationResolver Request(string list)
        {
            if (!Cache.ContainsKey(list))
                Cache.Add(list, new TeleportationResolver(list));
            return Cache[list];
        }
        private Configs.TeleporterList List;
        private Dictionary<string,Response> Destinations;
        private TeleportationResolver(string list)
        {
            List = Configs.Compound.Teleporters.Find(e => e.ListName == list);
            Destinations = new Dictionary<string, Response>() { { "", new Response("cancel", AdvancedLocationLoaderMod.Localizer.Localize("cancel")) } };
            for (int c = 0; c < List.Destinations.Count; c++)
            {
                if (List.Destinations[c].MapName != Game1.currentLocation.name)
                    Destinations.Add(List.Destinations[c].MapName,new Response(c.ToString(), List.Destinations[c].ItemText));
            }
        }
        internal void Init()
        {
            List<Response> destinations = new List<Response>();
            foreach (KeyValuePair<string, Response> entry in Destinations)
                if (entry.Key != Game1.currentLocation.name)
                    destinations.Add(entry.Value);
            Game1.currentLocation.lastQuestionKey = "SelectTeleportDestination";
            Game1.currentLocation.createQuestionDialogue(AdvancedLocationLoaderMod.Localizer.Localize("teleporter"), destinations.ToArray(), Resolver, null);
        }
        internal void Resolver(Farmer who, string answer)
        {
            if (answer == "cancel")
                return;
            int i = Convert.ToInt32(answer);
            Configs.TeleporterDestination destination = List.Destinations[i];
            Game1.warpFarmer(destination.MapName, destination.TileX, destination.TileY, false);
        }
    }
    internal class ConditionalResolver
    {
        private static Dictionary<string, ConditionalResolver> Cache=new Dictionary<string, ConditionalResolver>();
        internal static ConditionalResolver Request(string list)
        {
            if (!Cache.ContainsKey(list))
                Cache.Add(list, new ConditionalResolver(list));
            return Cache[list];
        }
        private Configs.Conditional Conditional;
        private ConditionalResolver(string name)
        {
            Conditional = Configs.Compound.Conditionals.Find(e => e.Name == name);
        }
        private string GetItemName()
        {
            if (Conditional.Item == -1)
                return AdvancedLocationLoaderMod.Localizer.Localize("gold");
            else
                return new StardewValley.Object(Conditional.Item, 1).Name;
        }
        internal void Init()
        {
            Response[] answers = new Response[2];
            answers[0] = new Response("y", AdvancedLocationLoaderMod.Localizer.Localize("yesCost",Conditional.Amount.ToString(),GetItemName()));
            answers[1] = new Response("n", AdvancedLocationLoaderMod.Localizer.Localize("no"));
            Game1.currentLocation.lastQuestionKey = "CompleteConditionalQuestion";
            Game1.currentLocation.createQuestionDialogue(Conditional.Question, answers, Resolver, null);
        }
        internal void Resolver(Farmer who, string answer)
        {
            if (answer == "n")
                return;
            if ((Conditional.Item == -1 && who.money >= Conditional.Amount) || who.hasItemInInventory(Conditional.Item, Conditional.Amount))
            {
                if (Conditional.Item == -1)
                    who.money -= Conditional.Amount;
                else
                    who.removeItemsFromInventory(Conditional.Item, Conditional.Amount);
                who.mailReceived.Add("ALLCondition_" + Conditional.Name);
                AdvancedLocationLoaderMod.UpdateConditionalEdits();
            }
            else
                Game1.drawObjectDialogue(AdvancedLocationLoaderMod.Localizer.Localize("notEnough", GetItemName()));

        }
    }
}
