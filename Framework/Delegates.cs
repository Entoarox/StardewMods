using System;

namespace Entoarox.Framework
{
    public delegate T AssetLoader<T>(string assetName);
    public delegate void AssetInjector<T>(string assetName, ref T asset);
    public delegate void ReceiveMessage(string modID, string channel, string message, bool broadcast);
}
