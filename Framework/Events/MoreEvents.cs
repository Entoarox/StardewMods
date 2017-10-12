using System;

using StardewModdingAPI;

namespace Entoarox.Framework.Events
{
    using Core;
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
        /// </summary>
        public static event EventHandler<EventArgsActiveItemChanged> ActiveItemChanged;


        private static void FireEventSafely<TArgs>(string name, Delegate evt, TArgs args) where TArgs : EventArgs
        {
            if (evt == null)
                return;
            foreach (EventHandler<TArgs> handler in evt.GetInvocationList())
                try
                {
                    handler.Invoke(null, args);
                }
                catch (Exception ex)
                {
                    ModEntry.Logger.Log($"A mod failed handling the {name} event:\n{ex}", LogLevel.Error);
                }
        }
        internal static void FireActionTriggered(EventArgsActionTriggered eventArgs)
        {
            FireEventSafely("ActionTriggered", ActionTriggered, eventArgs);
        }
        internal static void FireTouchActionTriggered(EventArgsActionTriggered eventArgs)
        {
            FireEventSafely("TouchActionTriggered", TouchActionTriggered, eventArgs);
        }
        internal static void FireActiveItemChanged(EventArgsActiveItemChanged eventArgs)
        {
            FireEventSafely("ActiveItemChanged", ActiveItemChanged, eventArgs);
        }
    }
}
