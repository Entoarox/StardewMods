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
    public class ExtendedMinecartMod : Mod
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
            GameEvents.UpdateTick += this.GameEvents_UpdateTick;
            this.Config = helper.ReadConfig<Config>();
        }
        private void GameEvents_UpdateTick(object s, EventArgs e)
        {
            if (!Game1.hasLoadedGame || Game1.CurrentEvent!=null)
                return;
            GameEvents.UpdateTick -= this.GameEvents_UpdateTick;
            MenuEvents.MenuChanged += this.MenuEvents_MenuChanged;
            Destinations = new Dictionary<string, ButtonFormComponent>();
            foreach(KeyValuePair<string, string> item in DestinationData)
            {
                switch(item.Key)
                {
                    case "Farm":
                        if (!this.Config.FarmDestinationEnabled)
                            continue;
                        break;
                    case "Desert":
                        if (!this.Config.DesertDestinationEnabled)
                            continue;
                        break;
                    case "Woods":
                        if (!this.Config.WoodsDestinationEnabled)
                            continue;
                        break;
                    case "Beach":
                        if (!this.Config.BeachDestinationEnabled)
                            continue;
                        break;
                    case "Forest":
                        if (!this.Config.WizardDestinationEnabled)
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
            if (this.Config.FarmDestinationEnabled && !this.Config.UseCustomFarmDestination)
            {
                try
                {
                    GameLocation farm = Game1.getFarm();
                    if (this.Config.AlternateFarmMinecart)
                    {
                        farm.SetTile(18, 5, "Front", 483, "untitled tile sheet");
                        farm.SetTile(19, 5, "Front", 484, "untitled tile sheet");
                        farm.SetTile(19, 5, "Buildings", 217, "untitled tile sheet");
                        farm.SetTile(20, 5, "Front", 485, "untitled tile sheet");

                        farm.SetTile(18, 6, "Buildings", 508, "untitled tile sheet");
                        farm.SetTile(19, 6, "Back", 509, "untitled tile sheet");
                        farm.SetTile(20, 6, "Buildings", 510, "untitled tile sheet");

                        farm.SetTile(18, 7, "Buildings", 533, "untitled tile sheet");
                        farm.SetTile(19, 7, "Back", 534, "untitled tile sheet");
                        farm.SetTile(20, 7, "Buildings", 535, "untitled tile sheet");

                        farm.SetTile(19, 6, "Buildings", 933, "untitled tile sheet");
                        farm.SetTile(19, 7, "Buildings", 958, "untitled tile sheet");
                        farm.SetTileProperty(19, 7, "Buildings", "Action", "MinecartTransport");
                    }
                    else
                    {
                        // Clear annoying flower
                        farm.removeTile(79, 12, "Buildings");
                        // Cut dark short
                        farm.SetTile(77, 11, "Back", 375, "untitled tile sheet");
                        farm.SetTile(78, 11, "Back", 376, "untitled tile sheet");
                        farm.SetTile(79, 11, "Back", 376, "untitled tile sheet");
                        // Lay tracks
                        farm.SetTile(78, 12, "Back", 729, "untitled tile sheet");
                        farm.SetTile(78, 13, "Back", 754, "untitled tile sheet");
                        farm.SetTile(78, 14, "Back", 755, "untitled tile sheet");
                        farm.SetTile(79, 12, "Back", 730, "untitled tile sheet");
                        // Trim grass
                        farm.SetTile(77, 13, "Back", 175, "untitled tile sheet");
                        farm.SetTile(77, 14, "Back", 175, "untitled tile sheet");
                        farm.SetTile(77, 15, "Back", 175, "untitled tile sheet");
                        farm.SetTile(78, 15, "Back", 175, "untitled tile sheet");
                        farm.SetTile(79, 13, "Back", 175, "untitled tile sheet");
                        farm.SetTile(79, 14, "Back", 175, "untitled tile sheet");
                        farm.SetTile(79, 15, "Back", 175, "untitled tile sheet");
                        // Clean up fence
                        farm.SetTile(78, 11, "Buildings",436,  "untitled tile sheet");
                        farm.removeTile(78, 14, "Buildings");
                        // Plop down minecart
                        farm.SetTile(78, 12, "Buildings", 933, "untitled tile sheet");
                        farm.SetTile(78, 13, "Buildings",958,  "untitled tile sheet");
                        farm.SetTileProperty(78, 13, "Buildings", "Action", "MinecartTransport");
                        // Keep exit clear
                        farm.setTileProperty(78, 14, "Back", "NoFurniture", "T");
                    }
                }
                catch(Exception err)
                {
                    this.Monitor.Log("Could not patch the Farm due to a unknown error", LogLevel.Error, err);
                }
            }
            if (this.Config.DesertDestinationEnabled)
            {
                try
                {
                    // # Desert
                    GameLocation desert = Game1.getLocationFromName("Desert");
                    xTile.Tiles.TileSheet parent = Game1.getLocationFromName("Mountain").map.GetTileSheet("outdoors");
                    desert.map.AddTileSheet(new xTile.Tiles.TileSheet("z_path_objects_custom_sheet", desert.map, parent.ImageSource, parent.SheetSize, parent.TileSize));
                    desert.map.DisposeTileSheets(Game1.mapDisplayDevice);
                    desert.map.LoadTileSheets(Game1.mapDisplayDevice);
                    if (this.Config.AlternateDesertMinecart)
                    {
                        // Backdrop
                        desert.SetTile(33, 1, "Front", 221, "desert-new");
                        desert.SetTile(34, 1, "Front", 222, "desert-new");
                        desert.SetTile(35, 1, "Front", 223, "desert-new");

                        desert.SetTile(33, 2, "Front", 237, "desert-new");
                        desert.SetTile(34, 2, "Buildings", 254, "desert-new");
                        desert.SetTile(34, 2, "Front", 238, "desert-new");
                        desert.SetTile(35, 2, "Front", 239, "desert-new");

                        desert.SetTile(33, 3, "Buildings", 253, "desert-new");
                        desert.SetTile(34, 3, "Buildings", 254, "desert-new");
                        desert.SetTile(35, 3, "Buildings", 255, "desert-new");

                        desert.SetTile(33, 4, "Buildings", 269, "desert-new");
                        desert.SetTile(34, 4, "Back", 270, "desert-new");
                        desert.SetTile(35, 4, "Buildings", 271, "desert-new");
                        // Cart
                        desert.SetTile(34, 3, "Front", 933, "z_path_objects_custom_sheet");
                        desert.SetTile(34, 4, "Buildings", 958, "z_path_objects_custom_sheet");
                        desert.SetTileProperty(34, 4, "Buildings", "Action", "MinecartTransport");
                    }
                    else
                    {
                        // Backdrop
                        desert.SetTile(33, 39, "Front", 221, "desert-new");
                        desert.SetTile(34, 39, "Front", 222, "desert-new");
                        desert.SetTile(35, 39, "Front", 223, "desert-new");

                        desert.SetTile(33, 40, "Front", 237, "desert-new");
                        desert.SetTile(34, 40, "Buildings", 254, "desert-new");
                        desert.SetTile(34, 40, "Front", 238, "desert-new");
                        desert.SetTile(35, 40, "Front", 239, "desert-new");

                        desert.SetTile(33, 41, "Buildings", 253, "desert-new");
                        desert.SetTile(34, 41, "Buildings", 254, "desert-new");
                        desert.SetTile(35, 41, "Buildings", 255, "desert-new");

                        desert.SetTile(33, 42, "Buildings", 269, "desert-new");
                        desert.SetTile(34, 42, "Back", 270, "desert-new");
                        desert.SetTile(35, 42, "Buildings", 271, "desert-new");
                        // Cart
                        desert.SetTile(34, 41, "Front", 933, "z_path_objects_custom_sheet");
                        desert.SetTile(34, 42, "Buildings", 958, "z_path_objects_custom_sheet");
                        desert.SetTileProperty(34, 42, "Buildings", "Action", "MinecartTransport");
                    }
                }
                catch (Exception err)
                {
                    this.Monitor.Log("Could not patch the Desert due to a unknown error",LogLevel.Error,  err);
                }
            }
            if (this.Config.WoodsDestinationEnabled)
            {
                try
                {
                    // # Woods
                    GameLocation woods = Game1.getLocationFromName("Woods");
                    woods.SetTile(46, 3, "Front", 933, "untitled tile sheet");
                    woods.SetTile(46, 4, "Buildings", 958, "untitled tile sheet");
                    woods.SetTileProperty(46, 4, "Buildings", "Action", "MinecartTransport");
                }
                catch (Exception err)
                {
                    this.Monitor.Log("Could not patch the Woods due to a unknown error",LogLevel.Error,  err);
                }
            }
            if(this.Config.WizardDestinationEnabled)
            {
                try
                {
                    // # Wizard
                    GameLocation forest = Game1.getLocationFromName("Forest");
                    forest.SetTile(13, 37, "Front", 483, "outdoors");
                    forest.SetTile(14, 37, "Front", 484, "outdoors");
                    forest.SetTile(14, 37, "Buildings", 217, "outdoors");
                    forest.SetTile(15, 37, "Front", 485, "outdoors");

                    forest.SetTile(13, 38, "Buildings", 508, "outdoors");
                    forest.SetTile(14, 38, "Back", 509, "outdoors");
                    forest.SetTile(15, 38, "Buildings", 510, "outdoors");

                    forest.SetTile(13, 39, "Buildings", 533, "outdoors");
                    forest.SetTile(15, 39, "Buildings", 535, "outdoors");

                    forest.SetTile(14, 38, "Buildings", 933, "outdoors");
                    forest.SetTile(14, 39, "Buildings", 958, "outdoors");
                    forest.SetTileProperty(14, 39, "Buildings", "Action", "MinecartTransport");
                }
                catch (Exception err)
                {
                    this.Monitor.Log("Could not patch the Forest due to a unknown error",LogLevel.Error,  err);
                }
            }
            if(this.Config.BeachDestinationEnabled)
            {
                try
                {
                    // # Beach
                    GameLocation beach = Game1.getLocationFromName("Beach");
                    xTile.Tiles.TileSheet parent = Game1.getLocationFromName("Mountain").map.GetTileSheet("outdoors");
                    beach.map.AddTileSheet(new xTile.Tiles.TileSheet("z_path_objects_custom_sheet", beach.map, parent.ImageSource, parent.SheetSize, parent.TileSize));
                    beach.map.DisposeTileSheets(Game1.mapDisplayDevice);
                    beach.map.LoadTileSheets(Game1.mapDisplayDevice);
                    beach.RemoveTile(67, 2, "Buildings");
                    beach.RemoveTile(67, 5, "Buildings");
                    beach.RemoveTile(67, 4, "Buildings");
                    beach.SetTile(67, 2,"Buildings", 933, "z_path_objects_custom_sheet");
                    beach.SetTile(67, 3, "Buildings", 958, "z_path_objects_custom_sheet");
                    beach.SetTileProperty(67, 3, "Buildings", "Action", "MinecartTransport");
                }
                catch (Exception err)
                {
                    this.Monitor.Log("Could not patch the Beach due to a unknown error", LogLevel.Error, err);
                }
        }
        }
        private void MenuEvents_MenuChanged(object s, EventArgs e)
        {
            if (Game1.activeClickableMenu == null || !(Game1.activeClickableMenu is DialogueBox) || !Context.IsWorldReady || Game1.player==null || Game1.currentLocation==null || Game1.currentLocation.lastQuestionKey==null || !Game1.currentLocation.lastQuestionKey.Equals("Minecart"))
                return;
            (Game1.activeClickableMenu as DialogueBox)?.closeDialogue();
            Game1.currentLocation.lastQuestionKey = null;
            Game1.dialogueUp = false;
            Game1.player.CanMove = true;
            if (this.Config.RefuelingEnabled)
            {
                if (this.CheckRefuel && !Game1.player.mailReceived.Contains("MinecartNeedsRefuel") && Rand.NextDouble() < 0.05)
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
                    return;
                }
            }
            foreach (KeyValuePair<string, ButtonFormComponent> item in Destinations)
                item.Value.Disabled = (item.Key.Equals(Game1.currentLocation.Name) || (Game1.isFestival() && item.Key.Equals(Game1.whereIsTodaysFest)));
            if (!Game1.player.mailReceived.Contains("ccCraftsRoom"))
                Destinations["Mountain"].Disabled = true;
            if (this.Config.DesertDestinationEnabled && !Game1.player.mailReceived.Contains("ccVault"))
                Destinations["Desert"].Disabled = true;
            if (this.Config.WoodsDestinationEnabled && !Game1.player.mailReceived.Contains("beenToWoods"))
                Destinations["Woods"].Disabled = true;
            if (this.Config.BeachDestinationEnabled && !(Game1.getLocationFromName("Beach") as StardewValley.Locations.Beach).bridgeFixed.Value)
                Destinations["Beach"].Disabled = true;
            this.CheckRefuel = false;
            Game1.activeClickableMenu = Menu;
        }
        private void AnswerResolver(string answer)
        {
            Menu?.ExitMenu();
            this.CheckRefuel = true;
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
                    if (this.Config.UseCustomFarmDestination)
                        Game1.warpFarmer("Farm", this.Config.CustomFarmDestinationPoint.X, this.Config.CustomFarmDestinationPoint.Y, 1);
                    else if (this.Config.AlternateFarmMinecart)
                        Game1.warpFarmer("Farm", 19, 8, 1);
                    else
                        Game1.warpFarmer("Farm", 78, 14, 1);
                    break;
                case "Desert":
                    if(Game1.currentLocation.Name.Equals("BusStop"))
                        Game1.warpFarmer("Woods", 46, 5, 1);
                    if (this.Config.AlternateDesertMinecart)
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
