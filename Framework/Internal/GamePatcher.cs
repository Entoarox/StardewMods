using System;

using Microsoft.Xna.Framework;

using StardewModdingAPI.Events;

using StardewValley;

namespace Entoarox.Framework
{
    internal static class GamePatcher
    {
        private static string ReportedLocation="";
        private static int WarpStage = 0;
        private static Point Greenhouse;
        internal static void Patch()
        {
            EntoFramework.Logger.Log("GamePatcher: Patching greenhouse...", StardewModdingAPI.LogLevel.Trace);
            EntoFramework.GetLocationHelper().SetTileProperty(Game1.getLocationFromName("Farm"), "Buildings", 28, 15, "Action", "NewGreenhouse");
            if (EntoFramework.GetLocationHelper().GetTileProperty(Game1.getLocationFromName("Farm"), "Buildings", 28, 15, "Action") != "NewGreenhouse")
                EntoFramework.Logger.Log("Setting the greenhouse tile action failed!",StardewModdingAPI.LogLevel.Error);
            foreach (Warp warp in Game1.getLocationFromName("Greenhouse").warps)
            {
                if (warp.TargetName != "Farm")
                    continue;
                Greenhouse = new Point(warp.X, warp.Y - 1);
                break;
            }
            if (Greenhouse == null)
                EntoFramework.Logger.Log("Unable to find the greenhouse exit warp to use as a reference for entering the greenhouse!",StardewModdingAPI.LogLevel.Error);
            else
                EntoFramework.Logger.Log($"GamePatcher: Greenhouse exit warp found at [{Greenhouse.X},{Greenhouse.Y+1}], player will enter greenhouse at [{Greenhouse.X},{Greenhouse.Y}]", StardewModdingAPI.LogLevel.Trace);
        }
        internal static void Update(object s, EventArgs e)
        {
            if (Game1.fadeToBlack && !Game1.messagePause && !Game1.nonWarpFade && Game1.currentLocation != null && Game1.locationAfterWarp != null && !Game1.menuUp && WarpStage == 0)
            {
                ReportedLocation = Game1.currentLocation.Name;
                WarpStage++;
            }
            if (Game1.fadeToBlack && !Game1.messagePause && WarpStage == 1 && !Game1.nonWarpFade && Game1.currentLocation != null && Game1.locationAfterWarp != null && !Game1.menuUp && !Game1.eventUp && Game1.locationAfterWarp.Name != "UndergroundMine" && Game1.locationAfterWarp.Name == ReportedLocation)
            {
                EntoFramework.Logger.Log("GamePatcher: Fixing lighting...", StardewModdingAPI.LogLevel.Trace);
                Game1.currentLocation.cleanupBeforePlayerExit();
                Game1.currentLocation.resetForPlayerEntry();
                WarpStage++;
            }
            if (WarpStage > 0 && !Game1.fadeToBlack)
            {
                WarpStage = 0;
            }
        }
        internal static void MoreEvents_ActionTriggered(object s, Events.EventArgsActionTriggered e)
        {
            switch(e.Action)
            {
                case "NewGreenhouse":
                case "ALLGreenhouse":
                    EntoFramework.Logger.Log("GamePatcher: Detected `NewGreenhouse` being triggered", StardewModdingAPI.LogLevel.Trace);
                    if (Game1.player.mailReceived.Contains("ccPantry"))
                    {
                        Game1.warpFarmer("Greenhouse", Greenhouse.X, Greenhouse.Y, false);
                        Game1.playSound("doorClose");
                    }
                    else
                        Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:Farm_GreenhouseRuins"));
                    break;
            }
        }
        internal static void TimeEvents_DayOfMonthChanged(object s, EventArgsIntChanged e)
        {
            switch(Game1.currentSeason)
            {
                case "spring":
                    if (Game1.year == 1)
                        switch (e.NewInt)
                        {
                            case 1:
                            case 3:
                                EntoFramework.Logger.Log("GamePatcher: Fixing weather report...", StardewModdingAPI.LogLevel.Trace);
                                Game1.weatherForTomorrow = Game1.weather_sunny;
                                break;
                        }
                    break;
                case "summer":
                    switch (e.NewInt)
                    {
                        case 12:
                        case 25:
                            EntoFramework.Logger.Log("GamePatcher: Fixing weather report...", StardewModdingAPI.LogLevel.Trace);
                            Game1.weatherForTomorrow = Game1.weather_lightning;
                            break;
                    }
                    break;
            }
        }
    }
}
