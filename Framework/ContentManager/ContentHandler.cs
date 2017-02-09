using System;
using System.IO;

namespace Entoarox.Framework.ContentManager
{
    public abstract class ContentHandler
    {
        private static StardewValley.LocalizedContentManager _ModManager;
        protected static StardewValley.LocalizedContentManager ModManager
        {
            get
            {
                if (_ModManager == null)
                    _ModManager = new StardewValley.LocalizedContentManager(StardewValley.Game1.content.ServiceProvider, Path.Combine(StardewModdingAPI.Constants.ExecutionPath, "Mods"));
                return _ModManager;
            }
        }
        public static string GetModsRelativePath(string file)
        {
            Uri fromUri = new Uri(ModManager.RootDirectory + Path.PathSeparator);
            Uri toUri = new Uri(file);
            if (fromUri.Scheme != toUri.Scheme) { throw new InvalidOperationException("Unable to make path relative to the Mods directory: " + file); }
            return GetPlatformSafePath(Uri.UnescapeDataString(fromUri.MakeRelativeUri(toUri).ToString()));
        }
        public static string GetPlatformSafePath(string file)
        {
            return file.Replace(Path.DirectorySeparatorChar == '/' ? '\\' : '/', Path.DirectorySeparatorChar);
        }
        public virtual bool Loader { get; } = false;
        public virtual bool Injector { get; } = false;
        public virtual bool CanLoad<T>(string assetName)
        {
            return false;
        }
        public virtual bool CanInject<T>(string assetName)
        {
            return false;
        }
        public abstract T Load<T>(string assetName, Func<string, T> loadBase);
        public abstract void Inject<T>(string assetName, ref T asset);
    }
}
