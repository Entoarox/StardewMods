namespace Entoarox.Utilities.Internals.Helpers
{
    using Internals;
    public static class ModApi
    {
        public static bool IsLoadedAndApiAvailable<T>(string mod, out T api) where T : class
        {
            api = null;
            if(EntoUtilsMod.Instance.Helper.ModRegistry.IsLoaded(mod))
                api = EntoUtilsMod.Instance.Helper.ModRegistry.GetApi<T>(mod);
            return api != null;
        }
    }
}
