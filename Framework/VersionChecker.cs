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
    internal struct VersionCheck
    {
        public string Mod;
        public Version Version;
        public string Url;
        public VersionCheck(string mod, Version version, string url)
        {
            Mod = mod;
            Version = version;
            Url = url;
        }
    }
    public class VersionInfo
    {
        public Version Latest;
        public Version Recommended;
        public Version Minimum;
        public VersionInfo()
        {
            Latest = new Version(0, 0, 0);
            Recommended = new Version(0, 0, 0);
            Minimum = new Version(0, 0, 0);
        }
    }
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
                    EntoFramework.Logger.Log("Checking for updates...", LogLevel.Info);
                    foreach (VersionCheck check in cache)
                    {
                        VersionInfo info = Get(check.Url);
                        if (info == null)
                            EntoFramework.Logger.Log("Could not check for updates to " + check.Mod + ", if you are not connected to the internet you can ignore this error.", LogLevel.Error);
                        else if (info.Minimum > check.Version)
                        {
                            if(EntoFramework.Config.IngameUpdateNotices)
                                EntoFramework.GetMessageBox().receiveMessage("Critical " + check.Mod + " update", "VersionChecker", new Microsoft.Xna.Framework.Color(255, 0, 0));
                            EntoFramework.Logger.Log("A critical update for " + check.Mod + " is available, you should update immediately!", LogLevel.Warn);
                        }
                        else if (info.Recommended > check.Version)
                        {
                            if (EntoFramework.Config.IngameUpdateNotices)
                                EntoFramework.GetMessageBox().receiveMessage("Recommended " + check.Mod + " update", "VersionChecker", new Microsoft.Xna.Framework.Color(255, 128, 0));
                            EntoFramework.Logger.Log("A recommended update for " + check.Mod + " is available, you should update as soon as possible.", LogLevel.Alert);
                        }
                        else if (info.Latest > check.Version)
                        {
                            if (EntoFramework.Config.IngameUpdateNotices)
                                EntoFramework.GetMessageBox().receiveMessage("Optional" + check.Mod + " update", "VersionChecker", new Microsoft.Xna.Framework.Color(0, 0, 255));
                            EntoFramework.Logger.Log("A optional update for " + check.Mod + " is available.", LogLevel.Info);
                        }
                        else
                            EntoFramework.Logger.Log("You have the latest available version of " + check.Mod + " installed.", LogLevel.Trace);
                    }
                    EntoFramework.Logger.Log("Update checks have been completed.", LogLevel.Info);
                }
                catch(Exception err)
                {
                    EntoFramework.Logger.Log("Ran into a unknown issue while performing the version check:" + Environment.NewLine + err.Message + Environment.NewLine + err.StackTrace, LogLevel.Error);
                }
            }).Start();
        }
        public static void AddCheck(string mod, Version version, string url)
        {
            cache.Add(new VersionCheck(mod, version, url));
        }
    }
}
