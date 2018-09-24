using System.Collections.Generic;

namespace Entoarox.Framework.Core
{
    internal class InterModHelper : IInterModHelper
    {
        /*********
        ** Fields
        *********/
        private static readonly Dictionary<string, InterModHelper> Cache = new Dictionary<string, InterModHelper>();
        private static readonly Dictionary<string, List<ReceiveMessage>> Map = new Dictionary<string, List<ReceiveMessage>>();
        private readonly string ModID;


        /*********
        ** Public methods
        *********/
        public InterModHelper(string modID)
        {
            this.ModID = modID;
        }

        public static IInterModHelper Get(string modID)
        {
            if (!InterModHelper.Cache.ContainsKey(modID))
                InterModHelper.Cache.Add(modID, new InterModHelper(modID));
            return InterModHelper.Cache[modID];
        }

        public void Subscribe(string channel, ReceiveMessage handler)
        {
            if (!InterModHelper.Map.ContainsKey(channel))
                InterModHelper.Map.Add(channel, new List<ReceiveMessage>());
            InterModHelper.Map[channel].Add(handler);
        }

        public void Subscribe(ReceiveMessage handler)
        {
            this.Subscribe(this.ModID, handler);
        }

        public void Publish(string channel, string message)
        {
            if (InterModHelper.Map.ContainsKey(channel))
                foreach (ReceiveMessage handler in InterModHelper.Map[channel])
                    handler(this.ModID, channel, message, false);
        }
    }
}
