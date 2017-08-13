using System;
using System.IO;
using System.Collections.Generic;
using Version = System.Version;

using StardewModdingAPI;
using StardewModdingAPI.Events;

using StardewValley;
using StardewValley.Menus;

using Entoarox.Framework;
using Entoarox.Framework.Events;

using Entoarox.AdvancedLocationLoader.Configs;

namespace Entoarox.AdvancedLocationLoader
{
    internal class AdvancedLocationLoaderMod : Mod
    {
        internal static IMonitor Logger;
        internal static LocalizationHelper Localizer;
        internal static string ModPath;
        public override void Entry(IModHelper helper)
        {
            ModPath = helper.DirectoryPath;
            if (EntoFramework.Version < new Version(1, 6, 5))
                throw new DllNotFoundException("A newer version of EntoaroxFramework.dll is required as the currently installed one is to old for AdvancedLocationLoader to use.");
            EntoFramework.VersionRequired("AdvancedLocationLoader", new Version(1, 6, 6));
            Logger = Monitor;
            Localizer = new LocalizationHelper(Path.Combine(ModPath,"localization"));
            VersionChecker.AddCheck("AdvancedLocationLoader",GetType().Assembly.GetName().Version, "https://raw.githubusercontent.com/Entoarox/StardewMods/master/VersionChecker/AdvancedLocationLoader.json");

            GameEvents.LoadContent += Events.GameEvents_LoadContent;
            MoreEvents.ActionTriggered += Events.MoreEvents_ActionTriggered;
            MoreEvents.WorldReady+=Events.MoreEvents_WorldReady;
            LocationEvents.CurrentLocationChanged += Events.LocationEvents_CurrentLocationChanged;

            ITypeRegistry registry = EntoFramework.GetTypeRegistry();
            registry.RegisterType<Locations.Greenhouse>();
            registry.RegisterType<Locations.Sewer>();
            registry.RegisterType<Locations.Desert>();
            registry.RegisterType<Locations.DecoratableLocation>();
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
        internal static bool? ConditionResolver(string condition)
        {
            if (condition.Substring(0, 13) != "ALLCondition:")
                return null;
            return Game1.player.mailReceived.Contains("ALLCondition_" + condition.Substring(13));
        }
    }
}
