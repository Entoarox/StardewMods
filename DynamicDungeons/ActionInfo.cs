using Microsoft.Xna.Framework;
using StardewValley;

namespace Entoarox.DynamicDungeons
{
    internal class ActionInfo
    {
        /*********
        ** Accessors
        *********/
        public Farmer Player { get; }
        public string Action { get; }
        public string[] Arguments { get; }
        public Vector2 Position { get; }


        /*********
        ** Public methods
        *********/
        public ActionInfo(Farmer player, string action, string[] arguments, Vector2 position)
        {
            this.Player = player;
            this.Action = action;
            this.Arguments = arguments;
            this.Position = position;
        }
    }
}