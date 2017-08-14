using StardewModdingAPI;

namespace Entoarox.Framework
{
    using Core;
    public static class IModHelperExtensions
    {
        private static IConditionHelper _ConditionHelper;
        private static IMessageHelper _MessageHelper;
        private static IPlayerHelper _PlayerHelper;
        public static void RequestUpdateCheck(this IModHelper helper, string uri)
        {
            IManifest manifest = helper.ModRegistry.Get((helper as IModLinked).ModID);
            if (!UpdateHandler.Map.ContainsKey(manifest))
                UpdateHandler.Map.Add(manifest, uri);
        }
        public static void RequestCreditsSkip(this IModHelper helper)
        {
            ModEntry.SkipCredits = true;
        }
        public static IConditionHelper Conditions(this IModHelper helper)
        {
            if (_ConditionHelper == null)
                _ConditionHelper = new ConditionHelper();
            return _ConditionHelper;
        }
        public static IMessageHelper Messages(this IModHelper helper)
        {
            if (_MessageHelper == null)
                _MessageHelper = new MessageHelper();
            return _MessageHelper;
        }
        public static IPlayerHelper Player(this IModHelper helper)
        {
            if (_PlayerHelper == null)
                _PlayerHelper = new PlayerHelper();
            return _PlayerHelper;
        }
    }
}
