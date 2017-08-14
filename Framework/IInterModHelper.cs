using System;

namespace Entoarox.Framework
{
    public interface IInterModHelper
    {
        /// <summary>
        /// Subscribe for any messages posted to the given channel
        /// </summary>
        /// <param name="channel">The channel to subscribe to</param>
        /// <param name="handler">The handler to receive any messages with</param>
        void Subscribe(string channel, ReceiveMessage handler);
        /// <summary>
        /// Publish any given message to the given channel
        /// </summary>
        /// <param name="channel">The channel to publish to</param>
        /// <param name="message">The message to publish</param>
        void Publish(string channel, string message);
    }
}