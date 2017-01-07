using System;

namespace Entoarox.Framework.Abstraction
{
    public class WrappedContentManager
    {
        private StardewValley.LocalizedContentManager Manager;
        private IContentRegistry Registry = EntoFramework.GetContentRegistry();
        internal WrappedContentManager(StardewValley.LocalizedContentManager manager)
        {
            Manager = manager;
        }
        public string RootDirectory => Manager.RootDirectory;
        public IServiceProvider ServiceProvider => Manager.ServiceProvider;
        public void RegisterXnb(string assetName, string filePath)
        {
            Registry.RegisterXnb(assetName, filePath);
        }
        public void RegisterTexture(string assetName, string filePath)
        {
            Registry.RegisterTexture(assetName, filePath);
        }
        public void RegisterHandler<T>(string assetName, FileLoadMethod<T> handler)
        {
            Registry.RegisterHandler(assetName, handler);
        }
        public T Load<T>(string assetName)
        {
            return Manager.Load<T>(assetName);
        }
        public string LoadString(string path, params object[] substitutions)
        {
            return Manager.LoadString(path, substitutions);
        }
    }
}
