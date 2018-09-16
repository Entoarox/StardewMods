using System;

using StardewModdingAPI;

using Microsoft.Xna.Framework.Input;

namespace Entoarox.Framework
{
    using Core;
    public static class IModHelperExtensions
    {
        private static IConditionHelper _ConditionHelper;
        private static IMessageHelper _MessageHelper;
        private static IPlayerHelper _PlayerHelper;
        /// <summary>
        /// Allows you to request a hotkey with the given label
        /// </summary>
        /// <param name="label">The label to use for this keybind in the settings menu</param>
        /// <param name="key">The preferred default key, if already taken a random unused key will be selected</param>
        /// <param name="handler">The method to invoke when the key is pressed</param>
        [Obsolete("This API member is not yet functional in the current development build.")]
        public static void RequestHotkey(this IModHelper helper, string label, Keys key, Action handler)
        {

        }
        /// <summary>
        /// Has Entoarox Framework perform a update check at the url given.
        /// </summary>
        /// <param name="helper">The <see cref="IModHelper"/> of the mod calling this method</param>
        /// <param name="uri">The url to retrieve the json file to parse from</param>
        public static void RequestUpdateCheck(this IModHelper helper, string uri)
        {
            IManifest manifest = helper.ModRegistry.Get((helper as IModLinked).ModID).Manifest;
            if (!UpdateHandler.Map.ContainsKey(manifest))
                UpdateHandler.Map.Add(manifest, uri);
        }
        /// <summary>
        /// Has Entoarox Framework perform a update check at the url given.
        /// This overload is intended for mods that need to perform update checks for non-SMAPI mods.
        /// </summary>
        /// <param name="helper">The <see cref="IModHelper"/> of the mod calling this method</param>
        /// <param name="name">The name to use when reporting on update requirements</param>
        /// <param name="version">What version to check the parsed json agains</param>
        /// <param name="uri">The url to retrieve the json file to parse from</param>
        public static void RequestUpdateCheck(this IModHelper helper, string name, SemanticVersion version, string uri)
        {
            IManifest manifest = new FakeManifest(name, version);
            if (!UpdateHandler.Map.ContainsKey(manifest))
                UpdateHandler.Map.Add(manifest, uri);
        }
        /// <summary>
        /// When called, this triggers API code in Entoarox Framework that makes it so the intro credits are skipped.
        /// </summary>
        /// <param name="helper">The <see cref="IModHelper"/> of the mod calling this method</param>
        public static void RequestCreditsSkip(this IModHelper helper)
        {
            EntoaroxFrameworkMod.SkipCredits = true;
        }
        /// <summary>
        /// This registers the given instance as a dynamic config.
        /// Dynamic configs can be edited in the main menu using EntoaroxFramework's config editing menu.
        /// </summary>
        /// <typeparam name="T">The type of the config class</typeparam>
        /// <param name="helper">The <see cref="IModHelper"/> of the mod calling this method</param>
        /// <param name="config">The instance to parse for <see cref="Config.ConfigAttribute"/> attributes.</param>
        /// <param name="configChanged">The method to invoke when the player accepts changes to the config file.</param>
        [Obsolete("This API member is not yet functional in the current development build.")]
        public static void RegisterDynamicConfig<T>(this IModHelper helper, T config, Action<T> configChanged)
        {

        }
        /// <summary>
        /// Provides access to the Entoarox Framework's <see cref="IConditionHelper"/> API.
        /// </summary>
        /// <param name="helper">The <see cref="IModHelper"/> of the mod calling this method</param>
        /// <returns></returns>
        public static IConditionHelper Conditions(this IModHelper helper)
        {
            if (_ConditionHelper == null)
                _ConditionHelper = new ConditionHelper();
            return _ConditionHelper;
        }
        /// <summary>
        /// Provides access to the Entoarox Framework's <see cref="IMessageHelper"/> API.
        /// </summary>
        /// <param name="helper">The <see cref="IModHelper"/> of the mod calling this method</param>
        /// <returns></returns>
        public static IMessageHelper Messages(this IModHelper helper)
        {
            if (_MessageHelper == null)
                _MessageHelper = new MessageHelper();
            return _MessageHelper;
        }
        /// <summary>
        /// Provides access to the Entoarox Framework's <see cref="IPlayerHelper"/> API.
        /// </summary>
        /// <param name="helper">The <see cref="IModHelper"/> of the mod calling this method</param>
        /// <returns></returns>
        public static IPlayerHelper Player(this IModHelper helper)
        {
            if (_PlayerHelper == null)
                _PlayerHelper = new PlayerHelper();
            return _PlayerHelper;
        }
        /// <summary>
        /// Provides access to the Entoarox Framework's <see cref="IInterModHelper"/> API.
        /// </summary>
        /// <param name="helper">The <see cref="IModHelper"/> of the mod calling this method</param>
        /// <returns></returns>
        public static IInterModHelper InterMod(this IModHelper helper)
        {
            return InterModHelper.Get((helper as IModLinked).ModID);
        }
    }
}
