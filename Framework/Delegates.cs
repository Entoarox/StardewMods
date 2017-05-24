using System;

namespace Entoarox.Framework
{
    public delegate T AssetLoader<T>(string assetName, Func<string, T> loadBase);
    public delegate void AssetInjector<T>(string assetName, ref T asset);
}
