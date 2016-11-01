using System;
using System.IO;

using Newtonsoft.Json.Linq;

using StardewModdingAPI.Events;

using Entoarox.Framework.Events;

namespace Entoarox.AdvancedLocationLoader
{
    internal static class Events
    {
        internal static void GameEvents_LoadContent(object s, EventArgs e)
        {
            AdvancedLocationLoaderMod.Logger.Info("Loading location mods into memory...");
            int count = 0;
            foreach(string dir in Directory.EnumerateDirectories(Path.Combine(AdvancedLocationLoaderMod.ModPath,"locations")))
            {
                string file = Path.Combine(dir, "manifest.json");
                if(File.Exists(file))
                {
                    JObject o = JObject.Parse(File.ReadAllText(file));
                    string loaderVersion = (string)o["LoaderVersion"];
                    if (loaderVersion == null)
                        loaderVersion = (string)o["loaderVersion"];
                    if (loaderVersion == null)
                        loaderVersion = "1.0";
                    AdvancedLocationLoaderMod.Logger.Trace("Attempting to load manifest version `" + loaderVersion + "` at: " + file);
                    string parserVersion = loaderVersion.Substring(0, 3);
                    switch (parserVersion)
                    {
                        case "1.0":
                            AdvancedLocationLoaderMod.Logger.Error("Unable to load manifest, version `1.0` is no longer supported: " + file);
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
                            AdvancedLocationLoaderMod.Logger.Error("Unable to load manifest, version `" + loaderVersion + "` is unknown: " + file);
                            break;
                    }
                }
            }
            AdvancedLocationLoaderMod.Logger.Info("Found and loaded ["+count+"] location mods into memory");
        }
        internal static void TimeEvents_SeasonOfYearChanged(object s, EventArgs e)
        {
            AdvancedLocationLoaderMod.UpdateTilesheets();
        }
        internal static void TimeEvents_DayOfMonthChanged(object s, EventArgs e)
        {
            AdvancedLocationLoaderMod.UpdateConditionalEdits();
        }
        internal static void MoreEvents_WorldReady(object s, EventArgs e)
        {
            Loaders.Loader1_2.ApplyPatches();
            if(Configs.Compound.DynamicTiles.Count>0 || Configs.Compound.DynamicProperties.Count>0 || Configs.Compound.DynamicWarps.Count>0)
                TimeEvents.DayOfMonthChanged += TimeEvents_DayOfMonthChanged;
            if (Configs.Compound.SeasonalTilesheets.Count > 0)
                TimeEvents.SeasonOfYearChanged += TimeEvents_SeasonOfYearChanged;
        }
        internal static void MoreEvents_ActionTriggered(object s, EventArgsActionTriggered e)
        {
            AdvancedLocationLoaderMod.Logger.Trace("EventFired.MoreEvents_ActionTriggered("+e.Action+")");
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
                }
            }
            catch(Exception err)
            {
                AdvancedLocationLoaderMod.Logger.Error("Could not fire appropriate action response, a unexpected error happened",err);
            }
        }
    }
}
