using System;

using StardewModdingAPI;

namespace Entoarox.Framework.Events
{
    using Core.Utilities;
    public static class ItemEvents
    {
        /// <summary>
        /// This event fires before saving, and before custom items are wrapped for serializing
        /// </summary>
        public static event EventHandler BeforeSerialize;
        /// <summary>
        /// This event fires before saving, but after custom items are wrapped for serializing
        /// </summary>
        public static event EventHandler AfterSerialize;

        /// <summary>
        /// This event fires after saving, but before custom items have been restored
        /// </summary>
        public static event EventHandler BeforeDeserialize;
        /// <summary>
        /// This event is fired after custom items have been restored
        /// </summary>
        public static event EventHandler AfterDeserialize;
        /// <summary>
        /// This event is fired whenever the item currently held by the player is changed
        /// </summary>
        public static event EventHandler<EventArgsActiveItemChanged> ActiveItemChanged;



        internal static void FireBeforeSerialize()
        {
            Events.FireEventSafely("BeforeSerialize", BeforeSerialize, EventArgs.Empty);
        }
        internal static void FireAfterSerialize()
        {
            Events.FireEventSafely("AfterSerialize", AfterSerialize, EventArgs.Empty);
        }
        internal static void FireBeforeDeserialize()
        {
            Events.FireEventSafely("BeforeDeserialize", BeforeDeserialize, EventArgs.Empty);
        }
        internal static void FireAfterDeserialize()
        {
            Events.FireEventSafely("AfterDeserialize", AfterDeserialize, EventArgs.Empty);
        }
        internal static void FireActiveItemChanged(EventArgsActiveItemChanged eventArgs)
        {
            Events.FireEventSafely("ActiveItemChanged", ActiveItemChanged, eventArgs, true);
        }
    }
}
