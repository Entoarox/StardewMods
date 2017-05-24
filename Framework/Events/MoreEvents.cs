using System;

using StardewValley;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using StardewModdingAPI.Events;

using xTile.Tiles;
using xTile.ObjectModel;

namespace Entoarox.Framework.Events
{
    public static class MoreEvents
    {
        /// <summary>
        /// Triggered whenever a action tile is activated by the player
        /// </summary>
        public static event EventHandler<EventArgsActionTriggered> ActionTriggered;
        /// <summary>
        /// This event is fired whenever the item currently held by the player is changed
        /// </summary>
        public static event EventHandler<EventArgsActiveItemChanged> ActiveItemChanged;


        internal static void FireActionTriggered(EventArgsActionTriggered eventArgs)
        {
            try
            {
                ActionTriggered.Invoke(null, eventArgs);
            }
            catch(Exception err)
            {
                ModEntry.Logger.Log("A mod failed to handle the MoreEvents.ActionTriggered event"+Environment.NewLine+err.ToString(), StardewModdingAPI.LogLevel.Error);
            }
        }
        internal static void FireActiveItemChanged(EventArgsActiveItemChanged eventArgs)
        {
            try
            {
                ActiveItemChanged.Invoke(null, eventArgs);
            }
            catch (Exception err)
            {
                ModEntry.Logger.Log("A mod failed to handle the MoreEvents.ActiveItemChanged event" + Environment.NewLine + err.ToString(), StardewModdingAPI.LogLevel.Error);
            }
        }
    }
}
