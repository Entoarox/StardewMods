using System;
using System.IO;
using System.Collections.Generic;

namespace StardewModdingAPI.Content.Plugins
{
    class XnbLoader : IContentHandler
    {
        internal static Dictionary<string, string> Map = new Dictionary<string, string>();
        internal static StardewValley.LocalizedContentManager ContentManager;

        public bool IsLoader { get; } = true;
        public bool CanLoad<T>(string assetName)
        {
            return Map.ContainsKey(assetName);
        }
        public T Load<T>(string assetName, Func<string,T> loadBase)
        {
            if (ContentManager == null)
                ContentManager = new StardewValley.LocalizedContentManager(StardewValley.Game1.content.ServiceProvider, Path.Combine(Constants.ExecutionPath, "Mods"));
            return ContentManager.Load<T>(Map[assetName]);
        }

        public bool IsInjector { get; } = false;
        public bool CanInject<T>(string assetName)
        {
            throw new NotImplementedException();
        }
        public void Inject<T>(string assetName, ref T asset)
        {
            throw new NotImplementedException();
        }
    }
}
