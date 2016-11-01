using System;
using System.IO;
using System.Collections.Generic;
using Version = System.Version;

using StardewModdingAPI;
using StardewModdingAPI.Events;

using StardewValley;

using Entoarox.Framework;
using Entoarox.Framework.Events;

using Entoarox.AdvancedLocationLoader.Configs;

namespace Entoarox.AdvancedLocationLoader
{
    internal class AdvancedLocationLoaderMod : Mod
    {
        internal static DataLogger Logger;
        internal static LocalizationHelper Localizer;
        internal static string ModPath;
        public override void Entry(params object[] objects)
        {
            ModPath = PathOnDisk;
            if (EntoFramework.Version < new Version(1, 3, 0))
                throw new DllNotFoundException("A newer version of EntoaroxFramework.dll is required as the currently installed one is to old for AdvancedLocationLoader to use.");
            Logger = new DataLogger("AdvancedLocationLoader", 4);
            Localizer = new LocalizationHelper(Path.Combine(PathOnDisk,"localization"));
            VersionChecker.AddCheck("AdvancedLocationLoader",GetType().Assembly.GetName().Version, "https://raw.githubusercontent.com/Entoarox/StardewMods/master/VersionChecker/AdvancedLocationLoader.json");
            GameEvents.LoadContent += Events.GameEvents_LoadContent;
            MoreEvents.ActionTriggered += Events.MoreEvents_ActionTriggered;
            MoreEvents.WorldReady+=Events.MoreEvents_WorldReady;
#if DEBUG
            Logger.Warn("Warning, this is a BETA version, features may be buggy or not work as intended!");
            GameEvents.UpdateTick += DebugNotification;
            GameEvents.UpdateTick += EntoFramework.CreditsTick;
        }
        internal static void DebugNotification(object s, EventArgs e)
        {
            if (Game1.activeClickableMenu is StardewValley.Menus.TitleMenu && Game1.activeClickableMenu != null)
            {
                Framework.Reflection.FieldHelper.SetField(Game1.activeClickableMenu, "subMenu", new TitleMenuDialogue(Localizer.Localize("betaNotice","BETA")));
                GameEvents.UpdateTick -= DebugNotification;
            }
#endif
        }
        internal static void UpdateConditionalEdits()
        {
            foreach(Tile t in Compound.DynamicTiles)
                Processors.ApplyTile(t);
            foreach (Configs.Warp t in Compound.DynamicWarps)
                Processors.ApplyWarp(t);
            foreach (Property t in Compound.DynamicProperties)
                Processors.ApplyProperty(t);
        }
        internal static void UpdateTilesheets()
        {
            List<string> locations=new List<string>();
            foreach (Tilesheet t in Compound.SeasonalTilesheets)
            {
                Processors.ApplyTilesheet(t);
                if (!locations.Contains(t.MapName))
                    locations.Add(t.MapName);
            }
            foreach(string map in locations)
            {
                xTile.Map location = Game1.getLocationFromName(map).map;
                location.DisposeTileSheets(Game1.mapDisplayDevice);
                location.LoadTileSheets(Game1.mapDisplayDevice);
            }
        }
    }
}
