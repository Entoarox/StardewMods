using System;

using StardewModdingAPI;

namespace Entoarox.Framework.Events
{
    using Core.Utilities;
    public static class MoreEvents
    {
        /// <summary>
        /// Triggered whenever a action tile is activated by the player
        /// </summary>
        public static event EventHandler<EventArgsActionTriggered> ActionTriggered;
        /// <summary>
        /// Triggered whenever a action tile is first walked onto by the player
        /// </summary>
        public static event EventHandler<EventArgsActionTriggered> TouchActionTriggered;

        internal static void FireActionTriggered(EventArgsActionTriggered eventArgs)
        {
            Events.FireEventSafely("ActionTriggered", ActionTriggered, eventArgs);
        }
        internal static void FireTouchActionTriggered(EventArgsActionTriggered eventArgs)
        {
            Events.FireEventSafely("TouchActionTriggered", TouchActionTriggered, eventArgs);
        }
    }
}
