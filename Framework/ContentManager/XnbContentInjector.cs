using System;
using System.Collections.Generic;

namespace Entoarox.Framework.ContentManager
{
    class XnbContentInjector : ContentInjector
    {
        private static Dictionary<Asset, string> Mapping = new Dictionary<Asset, string>();
        public static void Register<T>(string assetName, string filePath)
        {
            Mapping.Add(new Asset(typeof(T), assetName), filePath);
        }
        public static bool TryRegister<T>(string assetName, string filePath)
        {
            Asset asset = new Asset(typeof(T), assetName);
            if (Mapping.ContainsKey(asset))
                return false;
            Mapping.Add(asset, filePath);
            return true;
        }

        public override bool CanLoad<T>(string assetName)
        {
            return Mapping.ContainsKey(new Asset(typeof(T), assetName));
        }
        public override T Load<T>(string assetName, Func<string, T> loadBase)
        {
            return ModManager.Load<T>(assetName.Replace(ModManager.RootDirectory + System.IO.Path.PathSeparator, ""));
        }
        public override void Inject<T>(string assetName, ref T asset)
        {
            throw new NotImplementedException();
        }
    }
}
