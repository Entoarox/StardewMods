using System;

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


        internal static void FireActionTriggered(EventArgsActionTriggered eventArgs)
        {
            try
            {
                ActionTriggered?.Invoke(null, eventArgs);
            }
            catch(Exception err)
            {
                ModEntry.Logger.Log("A mod failed to handle the MoreEvents.ActionTriggered event"+Environment.NewLine+err.ToString(), StardewModdingAPI.LogLevel.Error);
            }
        }
        internal static void FireTouchActionTriggered(EventArgsActionTriggered eventArgs)
        {
            try
            {
                TouchActionTriggered?.Invoke(null, eventArgs);
            }
            catch (Exception err)
            {
                ModEntry.Logger.Log("A mod failed to handle the MoreEvents.TouchActionTriggered event" + Environment.NewLine + err.ToString(), StardewModdingAPI.LogLevel.Error);
            }
        }
        internal static void FireActiveItemChanged(EventArgsActiveItemChanged eventArgs)
        {
            try
            {
                ActiveItemChanged?.Invoke(null, eventArgs);
            }
            catch (Exception err)
            {
                ModEntry.Logger.Log("A mod failed to handle the MoreEvents.ActiveItemChanged event" + Environment.NewLine + err.ToString(), StardewModdingAPI.LogLevel.Error);
            }
        }
    }
}
