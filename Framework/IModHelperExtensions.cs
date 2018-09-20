using System;
using Entoarox.Framework.Core;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;

namespace Entoarox.Framework
{
    public static class IModHelperExtensions
    {
        /*********
        ** Fields
        *********/
        private static IConditionHelper ConditionHelper;
        private static IMessageHelper MessageHelper;
        private static IPlayerHelper PlayerHelper;


        /*********
        ** Accessors
        *********/
        internal static PlayerModifierHelper PlayerModifierHelper { get; } = new PlayerModifierHelper();


        /*********
        ** Public methods
        *********/
        /// <summary>Allows you to request a hotkey with the given label.</summary>
        /// <param name="label">The label to use for this keybind in the settings menu</param>
        /// <param name="key">The preferred default key, if already taken a random unused key will be selected</param>
        /// <param name="handler">The method to invoke when the key is pressed</param>
        [Obsolete("This API member is not yet functional in the current development build.")]
        public static void RequestHotkey(this IModHelper helper, string label, Keys key, Action handler) { }

        /// <summary>When called, this triggers API code in Entoarox Framework that makes it so the intro credits are skipped.</summary>
        /// <param name="helper">The <see cref="IModHelper" /> of the mod calling this method</param>
        public static void RequestCreditsSkip(this IModHelper helper)
        {
            EntoaroxFrameworkMod.SkipCredits = true;
        }

        /// <summary>This registers the given instance as a dynamic config. Dynamic configs can be edited in the main menu using EntoaroxFramework's config editing menu.</summary>
        /// <typeparam name="T">The type of the config class</typeparam>
        /// <param name="helper">The <see cref="IModHelper" /> of the mod calling this method</param>
        /// <param name="config">The instance to parse for <see cref="Config.ConfigAttribute" /> attributes.</param>
        /// <param name="configChanged">The method to invoke when the player accepts changes to the config file.</param>
        [Obsolete("This API member is not yet functional in the current development build.")]
        public static void RegisterDynamicConfig<T>(this IModHelper helper, T config, Action<T> configChanged)
        {
        }

        /// <summary>Provides access to the Entoarox Framework's <see cref="IConditionHelper" /> API.</summary>
        /// <param name="helper">The <see cref="IModHelper" /> of the mod calling this method</param>
        public static IConditionHelper Conditions(this IModHelper helper)
        {
            if (IModHelperExtensions.ConditionHelper == null)
                IModHelperExtensions.ConditionHelper = new ConditionHelper();
            return IModHelperExtensions.ConditionHelper;
        }

        /// <summary>Provides access to the Entoarox Framework's <see cref="IMessageHelper" /> API.</summary>
        /// <param name="helper">The <see cref="IModHelper" /> of the mod calling this method</param>
        public static IMessageHelper Messages(this IModHelper helper)
        {
            if (IModHelperExtensions.MessageHelper == null)
                IModHelperExtensions.MessageHelper = new MessageHelper();
            return IModHelperExtensions.MessageHelper;
        }

        /// <summary>Provides access to the Entoarox Framework's <see cref="IPlayerHelper" /> API.</summary>
        /// <param name="helper">The <see cref="IModHelper" /> of the mod calling this method</param>
        public static IPlayerHelper Player(this IModHelper helper)
        {
            if (IModHelperExtensions.PlayerHelper == null)
                IModHelperExtensions.PlayerHelper = new PlayerHelper(IModHelperExtensions.PlayerModifierHelper);
            return IModHelperExtensions.PlayerHelper;
        }

        /// <summary>Provides access to the Entoarox Framework's <see cref="IInterModHelper" /> API.</summary>
        /// <param name="helper">The <see cref="IModHelper" /> of the mod calling this method</param>
        public static IInterModHelper InterMod(this IModHelper helper)
        {
            return InterModHelper.Get((helper as IModLinked).ModID);
        }
    }
}
