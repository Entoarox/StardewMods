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
            EntoFramework.Logger.Debug("Overriding greenhouse teleportation...");
            EntoFramework.GetLocationHelper().SetTileProperty(Game1.getLocationFromName("Farm"), "Buildings", 28, 15, "Action", "NewGreenhouse");
            if (EntoFramework.GetLocationHelper().GetTileProperty(Game1.getLocationFromName("Farm"), "Buildings", 28, 15, "Action") != "NewGreenhouse")
                EntoFramework.Logger.Error("Setting the greenhouse tile action failed!");
            EntoFramework.Logger.Trace("Attempting to find the greenhouse exit...");
            foreach (Warp warp in Game1.getLocationFromName("Greenhouse").warps)
            {
                if (warp.TargetName != "Farm")
                    continue;
                Greenhouse = new Point(warp.X, warp.Y - 1);
                EntoFramework.Logger.Trace("Greenhouse exit found and hooked into");
                break;
            }
            if (Greenhouse == null)
                EntoFramework.Logger.Error("Unable to find the greenhouse exit warp to use as a reference for entering the greenhouse!");
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
                    EntoFramework.Logger.Trace("Greenhouse clicked, performing logic");
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
                                Game1.weatherForTomorrow = Game1.weather_sunny;
                                EntoFramework.Logger.Trace("Fixed weatherForTomorrow");
                                break;
                        }
                    break;
                case "summer":
                    switch (e.NewInt)
                    {
                        case 12:
                        case 25:
                            Game1.weatherForTomorrow = Game1.weather_lightning;
                            EntoFramework.Logger.Trace("Fixed weatherForTomorrow");
                            break;
                    }
                    break;
            }
        }
    }
}
