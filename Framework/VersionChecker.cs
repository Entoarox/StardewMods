using System;
using System.Threading;
using System.IO;
using System.Net;
using System.Collections.Generic;
using Version = System.Version;

using StardewModdingAPI;

using Newtonsoft.Json;

namespace Entoarox.Framework
{
    public static class VersionChecker
    {
        internal static WebClient Client;
        internal static VersionInfo Get(string uri)
        {
            try
            {
                if (Client == null)
                    Client = new WebClient();
                return JsonConvert.DeserializeObject<VersionInfo>(new StreamReader(Client.OpenRead(uri)).ReadToEnd(), new Newtonsoft.Json.Converters.VersionConverter());
            }
            catch
            {
                return null;
            }
        }
        internal static List<VersionCheck> cache = new List<VersionCheck>();
        internal static void DoChecks()
        {
            new Thread(() =>
            {
                try
                {
                    ModEntry.Logger.Log("Checking for updates...", LogLevel.Info);
                    foreach (VersionCheck check in cache)
                    {
                        VersionInfo info = Get(check.Url);
                        if (info == null)
                            ModEntry.Logger.Log("Could not check for updates to " + check.Mod + ", if you are not connected to the internet you can ignore this error.", LogLevel.Error);
                        else if (info.Minimum > check.Version)
                        {
                            if(ModEntry.Config.IngameUpdateNotices)
                                ModEntry.GetMessageBox().receiveMessage("Critical " + check.Mod + " update", "VersionChecker", new Microsoft.Xna.Framework.Color(255, 0, 0));
                            ModEntry.Logger.Log("A critical update for " + check.Mod + " is available, you should update immediately!", LogLevel.Warn);
                        }
                        else if (info.Recommended > check.Version)
                        {
                            if (ModEntry.Config.IngameUpdateNotices)
                                ModEntry.GetMessageBox().receiveMessage("Recommended " + check.Mod + " update", "VersionChecker", new Microsoft.Xna.Framework.Color(255, 128, 0));
                            ModEntry.Logger.Log("A recommended update for " + check.Mod + " is available, you should update as soon as possible.", LogLevel.Alert);
                        }
                        else if (info.Latest > check.Version)
                        {
                            if (ModEntry.Config.IngameUpdateNotices)
                                ModEntry.GetMessageBox().receiveMessage("Optional" + check.Mod + " update", "VersionChecker", new Microsoft.Xna.Framework.Color(0, 0, 255));
                            ModEntry.Logger.Log("A optional update for " + check.Mod + " is available.", LogLevel.Info);
                        }
                        else
                            ModEntry.Logger.Log("You have the latest available version of " + check.Mod + " installed.", LogLevel.Trace);
                    }
                    ModEntry.Logger.Log("Update checks have been completed.", LogLevel.Info);
                }
                catch(Exception err)
                {
                    ModEntry.Logger.Log("Ran into a unknown issue while performing the version check:" + Environment.NewLine + err.Message + Environment.NewLine + err.StackTrace, LogLevel.Error);
                }
            }).Start();
        }
        public static void AddCheck(string mod, Version version, string url)
        {
            cache.Add(new VersionCheck(mod, version, url));
        }
    }
}
