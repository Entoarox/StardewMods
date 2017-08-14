using System;

namespace Entoarox.Framework
{
    public class EventArgsMessageReceived : EventArgs
    {
        public readonly string ModID;
        public readonly bool Broadcast;
        public readonly string Message;

        internal EventArgsMessageReceived(string modID, string message, bool broadcast)
        {
            ModID = modID;
            Message = message;
            Broadcast = broadcast;
        }
    }
}
