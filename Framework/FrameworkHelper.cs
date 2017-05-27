using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

using StardewModdingAPI;

using StardewValley;

namespace Entoarox.Framework
{
    using Core;
    using Core.Content;
    public class FrameworkHelper : IFrameworkHelper
    {
        private static WebClient Client = new WebClient();
        private static Dictionary<IMod, IFrameworkHelper> Cache = new Dictionary<IMod, IFrameworkHelper>();
        private static string _PlatformRelativeContent;
        private static IMessageHelper _MessageHelper;
        private static IConditionHelper _ConditionHelper;
        public static IFrameworkHelper Get(IMod mod)
        {
            if (!Cache.ContainsKey(mod))
                Cache.Add(mod, new FrameworkHelper(mod));
            return Cache[mod];
        }

        public IContentHelper Content { get; private set; }
        public IMessageHelper Messages
        {
            get
            {
                if (_MessageHelper == null)
                    _MessageHelper = new MessageHelper();
                return _MessageHelper;
            }
        }
        public IConditionHelper Conditions
        {
            get
            {
                if (_ConditionHelper == null)
                    _ConditionHelper = new ConditionHelper();
                return _ConditionHelper;
            }
        }
        public void CheckForUpdates(string url)
        {
            if (!UpdateInfo.Map.ContainsKey(Mod))
                UpdateInfo.Map.Add(Mod, url);
        }
        public void AddTypeToSerializer<T>()
        {
            if (ModEntry.SerializerInjected)
                Mod.Monitor.Log("Failed to augment the serializer, serializer has already been created", LogLevel.Error);
            else if(!ModEntry.SerializerTypes.Contains(typeof(T)))
                ModEntry.SerializerTypes.Add(typeof(T));
        }
        public string PlatformRelativeContent
        {
            get
            {
                if (_PlatformRelativeContent == null)
                    _PlatformRelativeContent = File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "Resources", Game1.content.RootDirectory, "XACT", "FarmerSounds.xgs")) ? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "Resources", Game1.content.RootDirectory) : Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Content");
                return _PlatformRelativeContent;
            }
        }

        internal IMod Mod;
        internal FrameworkHelper(IMod mod)
        {
            Mod = mod;
            Content = new ContentHelper(this);
        }
    }
}
