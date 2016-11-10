using System;

using StardewValley;

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
        public static event EventHandler WorldReady;
        /// <summary>
        /// This event is fired just before the game is saved, giving you the time to remove custom objects
        /// </summary>
        public static event EventHandler BeforeSaving;
        /// <summary>
        /// This event is fired after the game is saved, so you can restore custom objects
        /// </summary>
        public static event EventHandler AfterSaving;
        /// <summary>
        /// This event is fired whenever the item currently held by the player is changed
        /// </summary>
        public static event EventHandler<EventArgsActiveItemChanged> ActiveItemChanged;
    }
    public class EventArgsActionTriggered : EventArgs
    {
        /// <summary>
        /// The action that was triggered
        /// </summary>
        public readonly string Action;
        /// <summary>
        /// The space-separated argument list that was given for the action
        /// </summary>
        public readonly string[] Arguments;
        /// <summary>
        /// The player that triggered the action tile in question
        /// </summary>
        public Farmer Who;
        /// <summary>
        /// The position of the tile that the action was triggered on
        /// </summary>
        public Microsoft.Xna.Framework.Vector2 Position;
        /// <summary>
        /// Internal constructor used by the code that fires the ActionTriggered event
        /// </summary>
        /// <param name="who">The player that triggered the action</param>
        /// <param name="action">The action they triggered</param>
        /// <param name="arguments">Any arguments that are attached to the action</param>
        /// <param name="position">The tile position of the action triggered</param>
        internal EventArgsActionTriggered(Farmer who, string action, string[] arguments, Microsoft.Xna.Framework.Vector2 position)
        {
            Action = action;
            Arguments = arguments;
            Who = who;
            Position = position;
        }
    }
    public class EventArgsActiveItemChanged : EventArgs
    {
        public readonly Item OldItem;
        public readonly Item NewItem;
        public EventArgsActiveItemChanged(Item oldItem, Item newItem)
        {
            OldItem = oldItem;
            NewItem = newItem;
        }
    }
}
