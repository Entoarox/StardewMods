using System;
using System.IO;
using System.Net;
using System.Collections.Generic;
using Version = System.Version;

using StardewModdingAPI;

using Newtonsoft.Json;

namespace Entoarox.Framework
{
    internal class VersionCheck
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
        [Obsolete("Causes game loading issues, use AddCheck instead", true)]
        public static VersionInfo RetrieveInfo(string uri)
        {
            VersionInfo info = Get(uri);
            if (info == null)
                return new VersionInfo();
            return info;
        }
        internal static VersionInfo Get(string uri)
        {
            try
            {
                WebClient client = new WebClient();
                Stream stream = client.OpenRead(uri);
                StreamReader reader = new StreamReader(stream);
                return JsonConvert.DeserializeObject<VersionInfo>(reader.ReadToEnd(), new Newtonsoft.Json.Converters.VersionConverter());
            }
            catch
            {
                return null;
            }
        }
        internal static List<VersionCheck> cache = new List<VersionCheck>();
        internal static void DoChecks()
        {
            foreach(VersionCheck check in cache)
            {
                VersionInfo info = Get(check.Url);
                if (info == null && EntoFramework.Config.DebugMode)
                    EntoFramework.GetMessageBox().receiveMessage("Couldnt check for " + check.Mod + " updates", "VersionChecker", new Microsoft.Xna.Framework.Color(255, 128, 0));
                else if (info.Minimum > check.Version)
                    EntoFramework.GetMessageBox().receiveMessage("Critical " + check.Mod + " update", "VersionChecker", new Microsoft.Xna.Framework.Color(255, 0, 0));
                else if (info.Recommended > check.Version)
                    EntoFramework.GetMessageBox().receiveMessage("Recommended " + check.Mod + " update", "VersionChecker", new Microsoft.Xna.Framework.Color(255, 128, 0));
                else if (info.Latest > check.Version)
                    EntoFramework.GetMessageBox().receiveMessage("Optional" + check.Mod + " update", "VersionChecker", new Microsoft.Xna.Framework.Color(0, 0, 255));
            }
        }
        public static void AddCheck(string mod, Version version, string url)
        {
            cache.Add(new VersionCheck(mod, version, url));
        }
        [Obsolete("Causes game loading issues, use AddCheck instead",true)]
        public static bool CheckVersion(DataLogger logger, Version version, string uri)
        {
            VersionInfo v = RetrieveInfo(uri);
            if (v.Minimum > version)
            {
                logger.Error("A critical update is available, you should update immediately!");
                return false;
            }
            if (v.Recommended > version)
                logger.Warn("A recommended update is available, you should update as soon as possible.");
            else if (v.Latest > version)
                logger.Info("A optional update is available");
            return true;
        }
    }
}
