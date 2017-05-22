using System;

namespace Entoarox.Framework.Events
{
    public static partial class MoreEvents
    {
        /// <summary>
        /// Triggered whenever a action tile is activated by the player
        /// </summary>
        public static event EventHandler<EventArgsActionTriggered> ActionTriggered;
        /// <summary>
        /// Triggered when the game world is ready for us modders to mess with it
        /// </summary>
        [Obsolete("SMAPI has its own event for this now")]
        public static event EventHandler WorldReady;
        /// <summary>
        /// This event is fired just before the game is saved, giving you the time to remove custom objects
        /// </summary>
        [Obsolete("SMAPI has its own event for this now")]
        public static event EventHandler BeforeSaving;
        /// <summary>
        /// This event is fired after the game is saved, so you can restore custom objects
        /// </summary>
        [Obsolete("SMAPI has its own event for this now")]
        public static event EventHandler AfterSaving;
        /// <summary>
        /// This event is fired whenever the item currently held by the player is changed
        /// </summary>
        public static event EventHandler<EventArgsActiveItemChanged> ActiveItemChanged;
        /// <summary>
        /// This event is fired after the framework content manager has been registered, or directly after LoadContent if the Farmhand content manager is active
        /// </summary>
        [Obsolete("Any mod that loads after EntoaroxFramework can now use the smart manager at all times")]
        public static event EventHandler SmartManagerReady;
    }
}
