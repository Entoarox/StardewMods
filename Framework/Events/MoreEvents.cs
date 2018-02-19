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
        /// <summary>
        /// This event is fired whenever the item currently held by the player is changed
        /// MoreEvents.ActiveItemChanged been rendered obsolete with the addition of the <see cref="ItemEvents"/> class and its own ActiveItemChanged event.
        /// </summary>
        [Obsolete("This event has been moved into the ItemEvents class", true)]
        public static event EventHandler<EventArgsActiveItemChanged> ActiveItemChanged;



        internal static void FireActionTriggered(EventArgsActionTriggered eventArgs)
        {
            Events.FireEventSafely("ActionTriggered", ActionTriggered, eventArgs);
        }
        internal static void FireTouchActionTriggered(EventArgsActionTriggered eventArgs)
        {
            Events.FireEventSafely("TouchActionTriggered", TouchActionTriggered, eventArgs);
        }
        internal static void FireActiveItemChanged(EventArgsActiveItemChanged eventArgs)
        {
            Events.FireEventSafely("ActiveItemChanged", ActiveItemChanged, eventArgs, true);
        }
    }
}
