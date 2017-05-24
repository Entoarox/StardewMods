using System;
using System.Net;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using StardewModdingAPI;

using Newtonsoft.Json;

namespace Entoarox.Framework
{
    using Core;
    using Core.Content;
    public class FrameworkHelper : IFrameworkHelper
    {
        private static WebClient Client = new WebClient();
        private static Dictionary<IMod, IFrameworkHelper> Cache = new Dictionary<IMod, IFrameworkHelper>();
        public static IFrameworkHelper Get(IMod mod)
        {
            if (!Cache.ContainsKey(mod))
                Cache.Add(mod, new FrameworkHelper(mod));
            return Cache[mod];
        }

        public IContentHelper Content { get; private set; }
        public void CheckForUpdates(string url)
        {
            if (!UpdateInfo.Map.ContainsKey(Mod))
                UpdateInfo.Map.Add(Mod, url);
        }

        internal IMod Mod;
        internal FrameworkHelper(IMod mod)
        {
            Mod = mod;
            Content = new ContentHelper(this);
        }
    }
}
