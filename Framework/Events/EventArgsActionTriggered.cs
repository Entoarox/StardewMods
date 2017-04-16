using System;

namespace Entoarox.Framework.Events
{
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
        public StardewValley.Farmer Who;

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
        internal EventArgsActionTriggered(StardewValley.Farmer who, string action, string[] arguments, Microsoft.Xna.Framework.Vector2 position)
        {
            this.Action = action;
            this.Arguments = arguments;
            this.Who = who;
            this.Position = position;
        }
    }
}
