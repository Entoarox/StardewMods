using System;

namespace Entoarox.Framework
{
    public interface IContentHandler
    {
        bool IsLoader { get; }
        bool IsInjector { get; }
        bool CanInject<T>(string assetName);
        bool CanLoad<T>(string assetName);
        void Inject<T>(string assetName, ref T asset);
        T Load<T>(string assetName, Func<string, T> loadBase);
    }
}
