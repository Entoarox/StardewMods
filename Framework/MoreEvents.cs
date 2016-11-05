using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using StardewValley;

using Microsoft.Xna.Framework;

namespace Entoarox.Framework.Events
{
    public class EventArgsActionTriggered : EventArgs
    {
        /**
         * <summary>The action that was triggered</summary>
         */
        public readonly string Action;
        /**
         * <summary>The space-separated argument list that was given for the action</summary>
         */
        public readonly string[] Arguments;
        /**
         * <summary>The player that triggered the action tile in question</summary>
         */
        public Farmer Who;
        /**
         * <summary>The position of the tile that the action was triggered on</summary>
         */
        public Vector2 Position;
        internal EventArgsActionTriggered(Farmer who, string action, string[] arguments, Vector2 position)
        {
            Action = action;
            Arguments = arguments;
            Who = who;
            Position = position;
        }
    }
    public static class MoreEvents
    {
        /**
         * <summary>Triggered whenever a action tile is activated by the player</summary>
         */
        public static event EventHandler<EventArgsActionTriggered> ActionTriggered;
        /**
         * <summary>Triggered when the game world is ready for us modders to mess with it</summary>
         */
        public static event EventHandler WorldReady;

        // Internal methods
        internal static void FireActionTriggered(Farmer who, string action, string[] arguments, Vector2 position)
        {
            if (ActionTriggered.GetInvocationList().Length > 0)
                ActionTriggered(null, new EventArgsActionTriggered(who, action, arguments, position));
        }
        internal static bool ShouldFireActionTriggered()
        {
            return ActionTriggered.GetInvocationList().Length > 0;
        }

        internal static void FireWorldReady()
        {
            if(WorldReady.GetInvocationList().Length>0)
                WorldReady(null, new EventArgs());
        }
    }
}
