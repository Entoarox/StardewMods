using System;
using System.IO;
using System.Collections.Generic;

using StardewModdingAPI;
using StardewModdingAPI.Events;

using StardewValley;

using Entoarox.Framework;
using Entoarox.Framework.Events;

using Entoarox.AdvancedLocationLoader.Configs;

namespace Entoarox.AdvancedLocationLoader
{
    internal class ModEntry : Mod
    {
        public static Dictionary<string, string> Strings = new Dictionary<string, string>()
        {
            {"sparkle","Despite the sparkle you saw, there doesnt seem to be anything here." },
            {"gold","Gold" },
            {"yesCost","Yes, costs {0} {1}"},
            {"no","No" },
            {"notEnough","You do not have enough {0} to do this." },
            {"cancel","Cancel" },
            {"teleporter","Choose a destination:" }
        };
        internal static IMonitor Logger;
        internal static string ModPath;
        internal static IFrameworkHelper FHelper;
        public override void Entry(IModHelper helper)
        {
            Logger = Monitor;
            FHelper = FrameworkHelper.Get(this);
            ModPath = helper.DirectoryPath;
            FHelper.CheckForUpdates("https://raw.githubusercontent.com/Entoarox/StardewMods/master/AdvancedLocationLoader/About/version.json");

            Events.GameEvents_LoadContent(null,null);
            MoreEvents.ActionTriggered += Events.MoreEvents_ActionTriggered;
            GameEvents.UpdateTick+=Events.MoreEvents_WorldReady;
            LocationEvents.CurrentLocationChanged += Events.LocationEvents_CurrentLocationChanged;

            FHelper.AddTypeToSerializer<Locations.Greenhouse>();
            FHelper.AddTypeToSerializer<Locations.Sewer>();
            FHelper.AddTypeToSerializer<Locations.Desert>();
            FHelper.AddTypeToSerializer<Locations.DecoratableLocation>();
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
