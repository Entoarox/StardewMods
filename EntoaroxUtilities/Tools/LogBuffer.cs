using System;
using System.Collections.Generic;

using StardewModdingAPI;

namespace Entoarox.Utilities.Tools
{
    public class LogBuffer : IDisposable
    {
        private readonly List<KeyValuePair<string, LogLevel>> Buffer = new List<KeyValuePair<string, LogLevel>>();
        private readonly IMonitor Monitor;

        public LogBuffer(IMonitor monitor)
        {
            this.Monitor = monitor;
        }
        public void Log(string message, LogLevel logLevel = LogLevel.Trace)
        {
            this.Buffer.Add(new KeyValuePair<string, LogLevel>(message, logLevel));
        }
        private bool disposedValue = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposedValue)
            {
                if (disposing)
                {
                    foreach (var pair in this.Buffer)
                        this.Monitor.Log(pair.Key, pair.Value);
                }

                this.disposedValue = true;
            }
        }
        public void Dispose()
        {
            this.Dispose(true);
        }
    }
}
