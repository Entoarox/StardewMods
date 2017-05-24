namespace Entoarox.Framework
{
    public interface IFrameworkHelper
    {
        string PlatformRelativeContent { get; }
        IContentHelper Content { get; }
        void CheckForUpdates(string url);
        void AddTypeToSerializer<T>();
    }
}
