namespace Entoarox.Framework.ContentManager
{
    public abstract class ContentInjector
    {
        private static StardewValley.LocalizedContentManager _ModManager;
        protected static StardewValley.LocalizedContentManager ModManager
        {
            get
            {
                if (_ModManager == null)
                    _ModManager = new StardewValley.LocalizedContentManager(StardewValley.Game1.content.ServiceProvider, System.IO.Path.Combine(StardewModdingAPI.Constants.ExecutionPath, "Mods"));
                return _ModManager;
            }
        }
        protected static string GetModsRelativePath(string file)
        {
            return file.Replace(ModManager.RootDirectory + System.IO.Path.PathSeparator, "");
        }
        protected static string GetPlatformSafePath(string file)
        {
            return file.Replace(System.IO.Path.PathSeparator == '/' ? '\\' : '/', System.IO.Path.PathSeparator);
        }
        public virtual bool CanLoad<T>(string assetName)
        {
            return false;
        }
        public virtual bool CanInject<T>(string assetName)
        {
            return false;
        }
        public abstract T Load<T>(string assetName, System.Func<string, T> loadBase);
        public abstract void Inject<T>(string assetName, ref T asset);
    }
}
