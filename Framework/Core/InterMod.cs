using System;
using System.Collections.Generic;

namespace Entoarox.Framework.Core
{
    class InterMod : IInterMod
    {
        private static Dictionary<string, InterMod> _Cache = new Dictionary<string, InterMod>();
        internal static IInterMod Get(string modID)
        {
            if (!_Cache.ContainsKey(modID))
                _Cache.Add(modID, new InterMod(modID));
            return _Cache[modID];
        }

        private string ModID;
        internal InterMod(string modID) => ModID=modID;

        internal void FireMessageReceived(EventArgsMessageReceived evt)
        {
            MessageReceived?.Invoke(null, evt);
        }
        public event EventHandler<EventArgsMessageReceived> MessageReceived;

        public void BroadcastMessage(string message)
        {
            EventArgsMessageReceived evt = new EventArgsMessageReceived(ModID, message, true);
            foreach (InterMod receiver in _Cache.Values)
                receiver.FireMessageReceived(evt);
        }
        public void SendMessage(string modID, string message)
        {
            if (_Cache.ContainsKey(modID))
                _Cache[modID].FireMessageReceived(new EventArgsMessageReceived(modID, message, false));
        }
    }
}
