namespace Entoarox.Utilities
{
    public static class EUGlobals
    {
        public static IEntoUtilsApi Api => Internals.Api.EntoUtilsApi.Instance;

        public delegate bool TypeIdResolverDelegate(StardewValley.Item item, ref string typeId);
    }
}
