namespace Entoarox.Framework
{
    public interface IFrameworkHelper
    {
        IContentHelper Content { get; }
        void CheckForUpdates(string url);
    }
}
