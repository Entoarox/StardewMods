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
        internal static ITranslationHelper Strings;
        internal static IMonitor Logger;
        internal static string ModPath;
        internal static IModHelper SHelper;
        public override void Entry(IModHelper helper)
        {
            Logger = this.Monitor;
            ModPath = helper.DirectoryPath;
            SHelper = helper;
            Strings = helper.Translation;
            this.Helper.RequestUpdateCheck("https://raw.githubusercontent.com/Entoarox/StardewMods/master/AdvancedLocationLoader/About/update.json");

            Events.GameEvents_LoadContent(null,null);
            MoreEvents.ActionTriggered += Events.MoreEvents_ActionTriggered;
            GameEvents.UpdateTick+=Events.GameEvents_UpdateTick;
            LocationEvents.CurrentLocationChanged += Events.LocationEvents_CurrentLocationChanged;

            this.Helper.Content.RegisterSerializerType<Locations.Greenhouse>();
            this.Helper.Content.RegisterSerializerType<Locations.Sewer>();
            this.Helper.Content.RegisterSerializerType<Locations.Desert>();
            this.Helper.Content.RegisterSerializerType<Locations.DecoratableLocation>();
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
