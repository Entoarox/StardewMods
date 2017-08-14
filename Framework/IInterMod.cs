using System;

namespace Entoarox.Framework
{
    public interface IInterMod
    {
        /// <summary>
        /// The listener your mod should subscribe to in order to handle any messages it receives
        /// </summary>
        event EventHandler<EventArgsMessageReceived> MessageReceived;
        /// <summary>
        /// Broadcasting means every other mod, if it is both loaded and listening, will receive the message
        /// </summary>
        /// <param name="message">The message to broadcast</param>
        void BroadcastMessage(string message);
        /// <summary>
        /// Sending means only the targeted mod, if it is both loaded and listening, will receive the message
        /// </summary>
        /// <param name="modID">The unique SMAPI ID for the mod</param>
        /// <param name="message">The message to send</param>
        void SendMessage(string modID, string message);
    }
}