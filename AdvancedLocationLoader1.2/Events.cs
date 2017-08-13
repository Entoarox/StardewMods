using System;
using System.IO;
using System.Linq;
using Entoarox.Framework;
using Entoarox.Framework.Events;
using Newtonsoft.Json.Linq;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace Entoarox.AdvancedLocationLoader
{
    internal static class Events
    {
        internal static void GameEvents_FirstUpdateTick()
        {
            try
            {
                AdvancedLocationLoaderMod.Logger.Log("Loading location mods into memory...",LogLevel.Debug);
                string baseDir = Path.Combine(AdvancedLocationLoaderMod.ModPath, "locations");
                int count = 0;
                Directory.CreateDirectory(baseDir);
                foreach (string dir in Directory.EnumerateDirectories(baseDir))
                {
                    string file = Path.Combine(dir, "manifest.json");
                    if (File.Exists(file))
                    {
                        JObject o;
                        try
                        {
                            o = JObject.Parse(File.ReadAllText(file));
                        }
                        catch (Exception err)
                        {
                            AdvancedLocationLoaderMod.Logger.Log(LogLevel.Error,"Unable to load manifest, json is invalid:" + file, err);
                            return;
                        }
                        string loaderVersion = (string)o["LoaderVersion"];
                        if (loaderVersion == null)
                            loaderVersion = (string)o["loaderVersion"];
                        if (loaderVersion == null)
                            loaderVersion = "1.0";
                        AdvancedLocationLoaderMod.Logger.Log("Attempting to load manifest version `" + loaderVersion + "` at: " + file,LogLevel.Trace);
                        string parserVersion = loaderVersion.Substring(0, 3);
                        switch (parserVersion)
                        {
                            case "1.0":
                                AdvancedLocationLoaderMod.Logger.Log("Unable to load manifest, version `1.0` is no longer supported: " + file,LogLevel.Error);
                                break;
                            case "1.1":
                                Loaders.Loader1_1.Load(file);
                                count++;
                                break;
                            case "1.2":
                                Loaders.Loader1_2.Load(file);
                                count++;
                                break;
                            default:
                                AdvancedLocationLoaderMod.Logger.Log("Unable to load manifest, version `" + loaderVersion + "` is unknown: " + file,LogLevel.Error);
                                break;
                        }
                    }
                    else
                        AdvancedLocationLoaderMod.Logger.Log("Could not find a manifest.json in the "+dir+" directory, if this is intentional you can ignore this message", LogLevel.Warn);
                }
                if(count>0)
                    AdvancedLocationLoaderMod.Logger.Log("Found and loaded [" + count + "] location mods into memory",LogLevel.Info);
                else
                    AdvancedLocationLoaderMod.Logger.Log("Was unable to load any location mods, if you do not have any installed yet you can ignore this message", LogLevel.Warn);
            }
            catch(Exception err)
            {
                AdvancedLocationLoaderMod.Logger.ExitGameImmediately("A unexpected error occured while loading location mod manifests", err);
            }
        }
        internal static void TimeEvents_AfterDayStarted(object s, EventArgs e)
        {
            // on new day
            if(Configs.Compound.DynamicTiles.Any() || Configs.Compound.DynamicProperties.Any() || Configs.Compound.DynamicWarps.Any())
                AdvancedLocationLoaderMod.UpdateConditionalEdits();

            // on new season
            if(Game1.dayOfMonth == 1 && Configs.Compound.SeasonalTilesheets.Any())
                AdvancedLocationLoaderMod.UpdateTilesheets();
        }
        internal static void MoreEvents_WorldReady(object s, EventArgs e)
        {
            Loaders.Loader1_2.ApplyPatches();
            TimeEvents.AfterDayStarted += TimeEvents_AfterDayStarted;
        }
        internal static void MoreEvents_ActionTriggered(object s, EventArgsActionTriggered e)
        {
            try
            {
                switch (e.Action)
                {
                    case "ALLShift":
                        Actions.Shift(e.Who, e.Arguments, e.Position);
                        break;
                    case "ALLReact":
                        Actions.React(e.Who, e.Arguments, e.Position);
                        break;
                    case "ALLRandomMessage":
                        Actions.RandomMessage(e.Who, e.Arguments, e.Position);
                        break;
                    case "ALLMinecart":
                    case "ALLTeleporter":
                        Actions.Teleporter(e.Who, e.Arguments, e.Position);
                        break;
                    case "ALLCondition":
                    case "ALLConditional":
                        Actions.Conditional(e.Who, e.Arguments, e.Position);
                        break;
                    case "ALLShop":
                        Actions.Shop(e.Who, e.Arguments, e.Position);
                        break;
                    default:
                        return;
                }
                AdvancedLocationLoaderMod.Logger.Log("ActionTriggered(" + e.Action + ")", LogLevel.Trace);
            }
            catch(Exception err)
            {
                AdvancedLocationLoaderMod.Logger.ExitGameImmediately("Could not fire appropriate action response, a unexpected error happened",err);
            }
        }
        internal static void LocationEvents_CurrentLocationChanged(object s, EventArgs e)
        {
            LocationEvents.CurrentLocationChanged -= LocationEvents_CurrentLocationChanged;
            AdvancedLocationLoaderMod.UpdateConditionalEdits();
        }
    }
}
