using System;
using System.Collections.Generic;
using StardewModdingAPI;
using StardewModdingAPI.Events;

using StardewValley;
using StardewValley.Menus;

using Entoarox.Framework;
using Entoarox.Framework.UI;

/*
 - Disable travel to a location that is subject of a Festival on the current day
*/
namespace Entoarox.ExtendedMinecart
{
    public class ExtendedMinecart : Mod
    {
        private static List<KeyValuePair<string, string>> DestinationData = new List<KeyValuePair<string, string>>()
        {
            new KeyValuePair<string, string>("Farm","Farm"),
            new KeyValuePair<string, string>("Town","Town"),
            new KeyValuePair<string, string>("Mine","Mine"),
            new KeyValuePair<string, string>("BusStop","Bus"),
            new KeyValuePair<string, string>("Mountain","Quarry"),
            new KeyValuePair<string, string>("Desert","Desert"),
            new KeyValuePair<string, string>("Woods","Woods"),
            new KeyValuePair<string, string>("Beach","Beach"),
            new KeyValuePair<string, string>("Forest","Wizard")
        };
        private static FrameworkMenu Menu;
        private static Dictionary<string, ButtonFormComponent> Destinations;
        private static Random Rand = new Random();
        private Config Config;
        private bool CheckRefuel = true;
        public override void Entry(IModHelper helper)
        {
            GameEvents.UpdateTick += GameEvents_UpdateTick;
            Config = helper.ReadConfig<Config>();
        }
        private void GameEvents_UpdateTick(object s, EventArgs e)
        {
            if (!Game1.hasLoadedGame || Game1.CurrentEvent!=null)
                return;
            GameEvents.UpdateTick -= GameEvents_UpdateTick;
            MenuEvents.MenuChanged += MenuEvents_MenuChanged;
            Destinations = new Dictionary<string, ButtonFormComponent>();
            foreach(KeyValuePair<string, string> item in DestinationData)
            {
                switch(item.Key)
                {
                    case "Farm":
                        if (!Config.FarmDestinationEnabled)
                            continue;
                        break;
                    case "Desert":
                        if (!Config.DesertDestinationEnabled)
                            continue;
                        break;
                    case "Woods":
                        if (!Config.WoodsDestinationEnabled)
                            continue;
                        break;
                    case "Beach":
                        if (!Config.BeachDestinationEnabled)
                            continue;
                        break;
                    case "Forest":
                        if (!Config.WizardDestinationEnabled)
                            continue;
                        break;
                }
                Destinations.Add(item.Key, new ButtonFormComponent(new Microsoft.Xna.Framework.Point(-1, 3 + 11 * Destinations.Count),65, item.Value, (t, p, m) => AnswerResolver(item.Key)));
            }
            Menu = new FrameworkMenu(new Microsoft.Xna.Framework.Point(85, Destinations.Count * 11 + 22));
            Menu.AddComponent(new LabelComponent(new Microsoft.Xna.Framework.Point(-3, -16), "Choose destination"));
            foreach (ButtonFormComponent c in Destinations.Values)
                Menu.AddComponent(c);
            // # Farm
            if (Config.FarmDestinationEnabled && !Config.UseCustomFarmDestination)
            {
                try
                {
                    GameLocation farm = Game1.getFarm();
                    if (Config.AlternateFarmMinecart)
                    {
                        farm.SetTile(18, 5, 483, "Front", "untitled tile sheet");
                        farm.SetTile(19, 5, 484, "Front", "untitled tile sheet");
                        farm.SetTile(19, 5, 217, "Buildings", "untitled tile sheet");
                        farm.SetTile(20, 5, 485, "Front", "untitled tile sheet");

                        farm.SetTile(18, 6, 508, "Buildings", "untitled tile sheet");
                        farm.SetTile(19, 6, 509, "Back", "untitled tile sheet");
                        farm.SetTile(20, 6, 510, "Buildings", "untitled tile sheet");

                        farm.SetTile(18, 7, 533, "Buildings", "untitled tile sheet");
                        farm.SetTile(19, 7, 534, "Back", "untitled tile sheet");
                        farm.SetTile(20, 7, 535, "Buildings", "untitled tile sheet");

                        farm.SetTile(19, 6, 933, "Buildings", "untitled tile sheet");
                        farm.SetTile(19, 7, 958, "Buildings", "MinecartTransport", "untitled tile sheet");
                    }
                    else
                    {
                        // Clear annoying flower
                        farm.removeTile(79, 12, "Buildings");
                        // Cut dark short
                        farm.SetTile(77, 11, 375, "Back", "untitled tile sheet");
                        farm.SetTile(78, 11, 376, "Back", "untitled tile sheet");
                        farm.SetTile(79, 11, 376, "Back", "untitled tile sheet");
                        // Lay tracks
                        farm.SetTile(78, 12, 729, "Back", "untitled tile sheet");
                        farm.SetTile(78, 13, 754, "Back", "untitled tile sheet");
                        farm.SetTile(78, 14, 755, "Back", "untitled tile sheet");
                        farm.SetTile(79, 12, 730, "Back", "untitled tile sheet");
                        // Trim grass
                        farm.SetTile(77, 13, 175, "Back", "untitled tile sheet");
                        farm.SetTile(77, 14, 175, "Back", "untitled tile sheet");
                        farm.SetTile(77, 15, 175, "Back", "untitled tile sheet");
                        farm.SetTile(78, 15, 175, "Back", "untitled tile sheet");
                        farm.SetTile(79, 13, 175, "Back", "untitled tile sheet");
                        farm.SetTile(79, 14, 175, "Back", "untitled tile sheet");
                        farm.SetTile(79, 15, 175, "Back", "untitled tile sheet");
                        // Clean up fence
                        farm.SetTile(78, 11, 436, "Buildings", "untitled tile sheet");
                        farm.removeTile(78, 14, "Buildings");
                        // Plop down minecart
                        farm.SetTile(78, 12, 933, "Buildings", "untitled tile sheet");
                        farm.SetTile(78, 13, 958, "Buildings", "MinecartTransport", "untitled tile sheet");
                        // Keep exit clear
                        farm.setTileProperty(78, 14, "Back", "NoFurniture", "T");
                    }
                }
                catch(Exception err)
                {
                    Monitor.Log(LogLevel.Error, "Could not patch the Farm due to a unknown error", err);
                }
            }
            if (Config.DesertDestinationEnabled)
            {
                try
                {
                    // # Desert
                    GameLocation desert = Game1.getLocationFromName("Desert");
                    xTile.Tiles.TileSheet parent = Game1.getLocationFromName("Mountain").map.GetTileSheet("outdoors");
                    desert.map.AddTileSheet(new xTile.Tiles.TileSheet("z_path_objects_custom_sheet", desert.map, parent.ImageSource, parent.SheetSize, parent.TileSize));
                    desert.map.DisposeTileSheets(Game1.mapDisplayDevice);
                    desert.map.LoadTileSheets(Game1.mapDisplayDevice);
                    if (Config.AlternateDesertMinecart)
                    {
                        // Backdrop
                        desert.SetTile(33, 1, 221, "Front", "desert-new");
                        desert.SetTile(34, 1, 222, "Front", "desert-new");
                        desert.SetTile(35, 1, 223, "Front", "desert-new");

                        desert.SetTile(33, 2, 237, "Front", "desert-new");
                        desert.SetTile(34, 2, 254, "Buildings", "desert-new");
                        desert.SetTile(34, 2, 238, "Front", "desert-new");
                        desert.SetTile(35, 2, 239, "Front", "desert-new");

                        desert.SetTile(33, 3, 253, "Buildings", "desert-new");
                        desert.SetTile(34, 3, 254, "Buildings", "desert-new");
                        desert.SetTile(35, 3, 255, "Buildings", "desert-new");

                        desert.SetTile(33, 4, 269, "Buildings", "desert-new");
                        desert.SetTile(34, 4, 270, "Back", "desert-new");
                        desert.SetTile(35, 4, 271, "Buildings", "desert-new");
                        // Cart
                        desert.SetTile(34, 3, 933, "Front", "z_path_objects_custom_sheet");
                        desert.SetTile(34, 4, 958, "Buildings", "MinecartTransport", "z_path_objects_custom_sheet");
                    }
                    else
                    {
                        // Backdrop
                        desert.SetTile(33, 39, 221, "Front", "desert-new");
                        desert.SetTile(34, 39, 222, "Front", "desert-new");
                        desert.SetTile(35, 39, 223, "Front", "desert-new");

                        desert.SetTile(33, 40, 237, "Front", "desert-new");
                        desert.SetTile(34, 40, 254, "Buildings", "desert-new");
                        desert.SetTile(34, 40, 238, "Front", "desert-new");
                        desert.SetTile(35, 40, 239, "Front", "desert-new");

                        desert.SetTile(33, 41, 253, "Buildings", "desert-new");
                        desert.SetTile(34, 41, 254, "Buildings", "desert-new");
                        desert.SetTile(35, 41, 255, "Buildings", "desert-new");

                        desert.SetTile(33, 42, 269, "Buildings", "desert-new");
                        desert.SetTile(34, 42, 270, "Back", "desert-new");
                        desert.SetTile(35, 42, 271, "Buildings", "desert-new");
                        // Cart
                        desert.SetTile(34, 41, 933, "Front", "z_path_objects_custom_sheet");
                        desert.SetTile(34, 42, 958, "Buildings", "MinecartTransport", "z_path_objects_custom_sheet");
                    }
                }
                catch (Exception err)
                {
                    Monitor.Log(LogLevel.Error, "Could not patch the Desert due to a unknown error", err);
                }
            }
            if (Config.WoodsDestinationEnabled)
            {
                try
                {
                    // # Woods
                    GameLocation woods = Game1.getLocationFromName("Woods");
                    woods.SetTile(46, 3, 933, "Front", "untitled tile sheet");
                    woods.SetTile(46, 4, 958, "Buildings", "MinecartTransport", "untitled tile sheet");
                }
                catch (Exception err)
                {
                    Monitor.Log(LogLevel.Error, "Could not patch the Woods due to a unknown error", err);
                }
            }
            if(Config.WizardDestinationEnabled)
            {
                try
                {
                    // # Wizard
                    GameLocation forest = Game1.getLocationFromName("Forest");
                    forest.SetTile(13, 37, 483, "Front", "outdoors");
                    forest.SetTile(14, 37, 484, "Front", "outdoors");
                    forest.SetTile(14, 37, 217, "Buildings", "outdoors");
                    forest.SetTile(15, 37, 485, "Front", "outdoors");

                    forest.SetTile(13, 38, 508, "Buildings", "outdoors");
                    forest.SetTile(14, 38, 509, "Back", "outdoors");
                    forest.SetTile(15, 38, 510, "Buildings", "outdoors");

                    forest.SetTile(13, 39, 533, "Buildings", "outdoors");
                    forest.SetTile(15, 39, 535, "Buildings", "outdoors");

                    forest.SetTile(14, 38, 933, "Buildings", "outdoors");
                    forest.SetTile(14, 39, 958, "Buildings", "MinecartTransport", "outdoors");
                }
                catch (Exception err)
                {
                    Monitor.Log(LogLevel.Error, "Could not patch the Forest due to a unknown error", err);
                }
            }
            if(Config.BeachDestinationEnabled)
            {
                try
                {
                    // # Beach
                    GameLocation beach = Game1.getLocationFromName("Beach");
                    xTile.Tiles.TileSheet parent = Game1.getLocationFromName("Mountain").map.GetTileSheet("outdoors");
                    beach.map.AddTileSheet(new xTile.Tiles.TileSheet("z_path_objects_custom_sheet", beach.map, parent.ImageSource, parent.SheetSize, parent.TileSize));
                    beach.map.DisposeTileSheets(Game1.mapDisplayDevice);
                    beach.map.LoadTileSheets(Game1.mapDisplayDevice);
                    beach.removeTile(67, 2, "Buildings");
                    beach.removeTile(67, 5, "Buildings");
                    beach.removeTile(67, 4, "Buildings");
                    beach.SetTile(67, 2, 933, "Buildings", "z_path_objects_custom_sheet");
                    beach.SetTile(67, 3, 958, "Buildings", "MinecartTransport", "z_path_objects_custom_sheet");
                }
                catch (Exception err)
                {
                    Monitor.Log(LogLevel.Error, "Could not patch the Beach due to a unknown error", err);
                }
        }
        }
        private void MenuEvents_MenuChanged(object s, EventArgs e)
        {
            if (Game1.activeClickableMenu == null || !(Game1.activeClickableMenu is DialogueBox) || !Game1.currentLocation.lastQuestionKey.Equals("Minecart"))
                return;
            (Game1.activeClickableMenu as DialogueBox)?.closeDialogue();
            if (Game1.currentLocation != null)
                Game1.currentLocation.lastQuestionKey = null;
            Game1.dialogueUp = false;
            if (Game1.player != null)
                Game1.player.CanMove = true;
            if (Config.RefuelingEnabled)
            {
                if (CheckRefuel && !Game1.player.mailReceived.Contains("MinecartNeedsRefuel") && Rand.NextDouble() < 0.05)
                    Game1.player.mailReceived.Add("MinecartNeedsRefuel");
                if (Game1.player.mailReceived.Contains("MinecartNeedsRefuel"))
                {
                    if (Game1.player.hasItemInInventory(382, 5))
                        Game1.currentLocation.createQuestionDialogue("The mincart has run out of fuel, use 5 coal to refuel it?", new Response[2] { new Response("Yes", "Yes"), new Response("No", "No") }, (a, b) =>
                        {
                            if (b == "Yes")
                            {
                                a.removeItemsFromInventory(382, 5);
                                a.mailReceived.Remove("MinecartNeedsRefuel");
                            }
                        });
                    else
                        Game1.drawObjectDialogue("The minecart is out of fuel and requires 5 coal to be refueled.");
                }
            }
            foreach (KeyValuePair<string, ButtonFormComponent> item in Destinations)
                item.Value.Disabled = (item.Key.Equals(Game1.currentLocation.Name) || (Game1.isFestival() && item.Key.Equals(Game1.whereIsTodaysFest)));
            if (!Game1.player.mailReceived.Contains("ccCraftsRoom"))
                Destinations["Mountain"].Disabled = true;
            if (Config.DesertDestinationEnabled && !Game1.player.mailReceived.Contains("ccVault"))
                Destinations["Desert"].Disabled = true;
            if (Config.WoodsDestinationEnabled && !Game1.player.mailReceived.Contains("beenToWoods"))
                Destinations["Woods"].Disabled = true;
            if (Config.BeachDestinationEnabled && !(Game1.getLocationFromName("Beach") as StardewValley.Locations.Beach).bridgeFixed)
                Destinations["Beach"].Disabled = true;
            CheckRefuel = false;
            Game1.activeClickableMenu = Menu;
        }
        private void AnswerResolver(string answer)
        {
            Menu.ExitMenu();
            CheckRefuel = true;
            switch (answer)
            {
                case "Mountain":
                    Game1.warpFarmer("Mountain", 124, 12, 2);
                    break;
                case "BusStop":
                    if(Game1.currentLocation.Name.Equals("Desert"))
                        Game1.warpFarmer("Woods", 46, 5, 1);
                    Game1.warpFarmer("BusStop", 4, 4, 2);
                    break;
                case "Mine":
                    Game1.warpFarmer("Mine", 13, 9, 1);
                    break;
                case "Town":
                    Game1.warpFarmer("Town", 105, 80, 1);
                    break;
                case "Farm":
                    if (Config.UseCustomFarmDestination)
                        Game1.warpFarmer("Farm", Config.CustomFarmDestinationPoint.X, Config.CustomFarmDestinationPoint.Y, 1);
                    else if (Config.AlternateFarmMinecart)
                        Game1.warpFarmer("Farm", 19, 8, 1);
                    else
                        Game1.warpFarmer("Farm", 78, 14, 1);
                    break;
                case "Desert":
                    if(Game1.currentLocation.Name.Equals("BusStop"))
                        Game1.warpFarmer("Woods", 46, 5, 1);
                    if (Config.AlternateDesertMinecart)
                        Game1.warpFarmer("Desert", 34, 5, 1);
                    else
                        Game1.warpFarmer("Desert", 34, 43, 1);
                    break;
                case "Woods":
                    Game1.warpFarmer("Woods", 46, 5, 1);
                    break;
                case "Forest":
                    Game1.warpFarmer("Forest", 14, 40, 1);
                    break;
                case "Beach":
                    Game1.warpFarmer("Beach", 67, 4, 1);
                    break;
            }
        }
    }
}
