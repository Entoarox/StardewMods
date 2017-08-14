using System;
using System.Collections.Generic;

using StardewModdingAPI;

namespace Entoarox.Framework
{
    public static class IMonitorExtensions
    {
        private static readonly HashSet<string> Cache = new HashSet<string>();
        public static void Log(this IMonitor self, string message, LogLevel level = LogLevel.Debug, Exception error = null)
        {
            if (error != null)
                message += Environment.NewLine + error.ToString();
            self.Log(message, level);
        }
        public static void LogOnce(this IMonitor self, string message, LogLevel level = LogLevel.Debug, Exception error = null)
        {
            if (error != null)
                message += Environment.NewLine + error.ToString();
            if (Cache.Add($"{level}:{message}"))
                self.Log(message, level);
        }
        public static void ExitGameImmediately(this IMonitor self, string message, Exception error = null)
        {
            if (error != null)
                message += Environment.NewLine + error.ToString();
            self.ExitGameImmediately(message);
        }
    }
}