using System;
using System.IO;

using Newtonsoft.Json.Linq;

using StardewModdingAPI;
using StardewModdingAPI.Events;

using Entoarox.Framework;
using Entoarox.Framework.Events;

using StardewValley;

namespace Entoarox.AdvancedLocationLoader
{
    internal static class Events
    {
        internal static void GameEvents_LoadContent(object s, EventArgs e)
        {
            try
            {
                ModEntry.Logger.Log("Loading location mods into memory...",LogLevel.Debug);
                string baseDir = Path.Combine(ModEntry.ModPath, "locations");
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
                            ModEntry.Logger.Log("Unable to load manifest, json is invalid:" + file,LogLevel.Error, err);
                            return;
                        }
                        string loaderVersion = (string)o["LoaderVersion"];
                        if (loaderVersion == null)
                            loaderVersion = (string)o["loaderVersion"];
                        if (loaderVersion == null)
                            loaderVersion = "1.0";
                        ModEntry.Logger.Log("Attempting to load manifest version `" + loaderVersion + "` at: " + file,LogLevel.Trace);
                        string parserVersion = loaderVersion.Substring(0, 3);
                        switch (parserVersion)
                        {
                            case "1.0":
                                ModEntry.Logger.Log("Unable to load manifest, version `1.0` is no longer supported: " + file,LogLevel.Error);
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
                                ModEntry.Logger.Log("Unable to load manifest, version `" + loaderVersion + "` is unknown: " + file,LogLevel.Error);
                                break;
                        }
                    }
                    else
                        ModEntry.Logger.Log("Could not find a manifest.json in the "+dir+" directory, if this is intentional you can ignore this message", LogLevel.Warn);
                }
                if(count>0)
                    ModEntry.Logger.Log("Found and loaded [" + count + "] location mods into memory",LogLevel.Info);
                else
                    ModEntry.Logger.Log("Was unable to load any location mods, if you do not have any installed yet you can ignore this message", LogLevel.Warn);
            }
            catch(Exception err)
            {
                ModEntry.Logger.ExitGameImmediately("A unexpected error occured while loading location mod manifests", err);
            }
        }
        internal static void TimeEvents_AfterDayStarted(object s, EventArgs e)
        {
            ModEntry.UpdateConditionalEdits();
            if(Game1.dayOfMonth==1)
                ModEntry.UpdateTilesheets();
        }
        internal static void GameEvents_UpdateTick(object s, EventArgs e)
        {
            if (!Context.IsWorldReady)
                return;
            GameEvents.UpdateTick -= GameEvents_UpdateTick;
            Loaders.Loader1_2.ApplyPatches();
            if(Configs.Compound.DynamicTiles.Count>0 || Configs.Compound.DynamicProperties.Count>0 || Configs.Compound.DynamicWarps.Count>0)
                TimeEvents.AfterDayStarted += TimeEvents_AfterDayStarted;
        }
        internal static void MoreEvents_ActionTriggered(object s, EventArgsActionTriggered e)
        {
            try
            {
                switch (e.Action)
                {
                    case "ALLMessage":
                        Actions.Message(e.Who, e.Arguments, e.Position);
                        break;
                    case "ALLRawMessage":
                        Actions.RawMessage(e.Who, e.Arguments, e.Position);
                        break;
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
                ModEntry.Logger.Log("ActionTriggered(" + e.Action + ")", LogLevel.Trace);
            }
            catch(Exception err)
            {
                ModEntry.Logger.ExitGameImmediately("Could not fire appropriate action response, a unexpected error happened",err);
            }
        }
        internal static void LocationEvents_CurrentLocationChanged(object s, EventArgs e)
        {
            LocationEvents.CurrentLocationChanged -= LocationEvents_CurrentLocationChanged;
            ModEntry.UpdateConditionalEdits();
        }
    }
}
