using System;
using System.Collections.Generic;

using StardewModdingAPI;
using StardewModdingAPI.Events;

using StardewValley;
using StardewValley.Menus;

namespace Entoarox.ExtendedMinecart
{
    public class ExtendedMinecart : Mod
    {
        public override void Entry(IModHelper helper)
        {
            GameEvents.UpdateTick += GameEvents_UpdateTick;
        }
        private static List<Response> Choices;
        private static Response Quarry;
        private static Response Desert;
        private static Response Cancel;
        private void GameEvents_UpdateTick(object s, EventArgs e)
        {
            if (!Game1.hasLoadedGame || Game1.CurrentEvent!=null)
                return;
            GameEvents.UpdateTick -= GameEvents_UpdateTick;
            MenuEvents.MenuChanged += MenuEvents_MenuChanged;
            Choices = new List<Response>()
            {
                new Response("Mine", Game1.content.LoadString("Strings\\Locations:MineCart_Destination_Mines")),
                new Response("Town", Game1.content.LoadString("Strings\\Locations:MineCart_Destination_Town")),
                new Response("BusStop", Game1.content.LoadString("Strings\\Locations:MineCart_Destination_BusStop")),
                new Response("Farm", "Farm")
            };
            Quarry = new Response("Mountain", Game1.content.LoadString("Strings\\Locations:MineCart_Destination_Quarry"));
            Desert = new Response("Desert", "Desert");
            Cancel = new Response("Cancel", Game1.content.LoadString("Strings\\Locations:MineCart_Destination_Cancel"));
            GameLocation farm=Game1.getFarm();
            // # Farm
            // Clear annoying flower
            farm.removeTile(79, 12, "Buildings");
            // Cut dark short
            farm.setMapTileIndex(77, 11, 375, "Back", 1);
            farm.setMapTileIndex(78, 11, 376, "Back", 1);
            farm.setMapTileIndex(79, 11, 376, "Back", 1);
            // Lay tracks
            farm.setMapTileIndex(78, 12, 729, "Back", 1);
            farm.setMapTileIndex(78, 13, 754, "Back", 1);
            farm.setMapTileIndex(78, 14, 755, "Back", 1);
            farm.setMapTileIndex(79, 12, 730, "Back", 1);
            // Trim grass
            farm.setMapTileIndex(77, 13, 175, "Back", 1);
            farm.setMapTileIndex(77, 14, 175, "Back", 1);
            farm.setMapTileIndex(77, 15, 175, "Back", 1);
            farm.setMapTileIndex(78, 15, 175, "Back", 1);
            farm.setMapTileIndex(79, 13, 175, "Back", 1);
            farm.setMapTileIndex(79, 14, 175, "Back", 1);
            farm.setMapTileIndex(79, 15, 175, "Back", 1);
            // Clean up fence
            farm.setMapTileIndex(78, 11, 436, "Buildings", 1);
            farm.removeTile(78, 14, "Buildings");
            // Plop down minecart
            farm.setMapTileIndex(78, 12, 933, "Buildings", 1);
            farm.setMapTile(78, 13, 958, "Buildings", "MinecartTransport", 1);
            // Keep exit clear
            farm.setTileProperty(78, 14, "Back", "NoFurniture", "T");
            GameLocation desert = Game1.getLocationFromName("Desert");
            // # Desert
            xTile.Tiles.TileSheet parent = Game1.getLocationFromName("Mountain").map.GetTileSheet("outdoors");
            desert.map.AddTileSheet(new xTile.Tiles.TileSheet("z_path_objects_custom_sheet",desert.map, parent.ImageSource, parent.SheetSize, parent.TileSize));
            desert.map.DisposeTileSheets(Game1.mapDisplayDevice);
            desert.map.LoadTileSheets(Game1.mapDisplayDevice);
            // Reset
            desert.removeTile(34, 42, "Buildings");
            // Backdrop
            desert.setMapTileIndex(33, 39, 221, "Front");
            desert.setMapTileIndex(34, 39, 222, "Front");
            desert.setMapTileIndex(35, 39, 223, "Front");

            desert.setMapTileIndex(33, 40, 237, "Front");
            desert.setMapTileIndex(34, 40, 254, "Buildings");
            desert.setMapTileIndex(34, 40, 238, "Front");
            desert.setMapTileIndex(35, 40, 239, "Front");

            desert.setMapTileIndex(33, 41, 253, "Buildings");
            desert.setMapTileIndex(34, 41, 254, "Buildings");
            desert.setMapTileIndex(35, 41, 255, "Buildings");

            desert.setMapTileIndex(33, 42, 269, "Buildings");
            desert.setMapTileIndex(34, 42, 270, "Back");
            desert.setMapTileIndex(35, 42, 271, "Buildings");
            // Cart
            desert.setMapTileIndex(34, 41, 933, "Front", 2);
            desert.setMapTile(34, 42, 958, "Buildings", "MinecartTransport", 2);

        }
        private void MenuEvents_MenuChanged(object s, EventArgs e)
        {
            if (Game1.activeClickableMenu == null || !(Game1.activeClickableMenu is DialogueBox) || Game1.currentLocation.lastQuestionKey != "Minecart")
                return;
                    Game1.currentLocation.lastQuestionKey = "CustomMinecart";
                    List<Response> answerChoices = new List<Response>();
                    foreach (Response r in Choices)
                        if (Game1.currentLocation.name != r.responseKey)
                            answerChoices.Add(r);
                    if (Game1.player.mailReceived.Contains("ccCraftsRoom") && Game1.currentLocation.Name != "Mountain")
                        answerChoices.Add(Quarry);
                    if (Game1.player.mailReceived.Contains("ccVault") && Game1.currentLocation.Name != "Desert")
                        answerChoices.Add(Desert);
                    answerChoices.Add(Cancel);
                    Game1.currentLocation.createQuestionDialogue(Game1.content.LoadString("Strings\\Locations:MineCart_ChooseDestination"), answerChoices.ToArray(), AnswerResolver);
        }
        private void AnswerResolver(Farmer who, string answer)
        {
            if (answer == "Cancel")
                return;
            Game1.player.Halt();
            Game1.player.freezePause = 700;
            switch (answer)
            {
                case "Mountain":
                    Game1.warpFarmer("Mountain", 124, 12, 2);
                    break;
                case "BusStop":
                    Game1.warpFarmer("BusStop", 4, 4, 2);
                    break;
                case "Mines":
                    Game1.warpFarmer("Mine", 13, 9, 1);
                    break;
                case "Town":
                    Game1.warpFarmer("Town", 105, 80, 1);
                    break;
                case "Farm":
                    Game1.warpFarmer("Farm", 78, 14, 1);
                    break;
                case "Desert":
                    Game1.warpFarmer("Desert", 34, 43, 1);
                    break;
            }
        }
    }
}
