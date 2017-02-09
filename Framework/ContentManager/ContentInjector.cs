namespace Entoarox.Framework.ContentManager
{
    public abstract class ContentInjector
    {
        private StardewValley.LocalizedContentManager _ModManager;
        protected StardewValley.LocalizedContentManager ModManager
        {
            get
            {
                if (_ModManager == null)
                    _ModManager = new StardewValley.LocalizedContentManager(StardewValley.Game1.content.ServiceProvider, System.IO.Path.Combine(StardewModdingAPI.Constants.ExecutionPath, "Mods"));
                return _ModManager;
            }
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
