using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using StardewModdingAPI;

namespace Entoarox.Framework
{
    public static class MonitorLogExtensions
    {
        private static HashSet<string> Cache = new HashSet<string>();
        private static void ResolvedLogger(IMonitor self, LogLevel level, string message, Exception error=null, string[] replacements=null, bool once=false)
        {
            if (replacements != null && replacements.Count() > 0)
                message = string.Format(message, replacements);
            if (error != null)
                message = message + Environment.NewLine + error.Message + Environment.NewLine + error.StackTrace;
            if (once)
                if (Cache.Contains(level.ToString() + ':' + message))
                    return;
                else
                    Cache.Add(level.ToString() + ':' + message);
            self.Log(message, level);
        }
        public static void LogTrace(this IMonitor self, string message)
        {
            ResolvedLogger(self, LogLevel.Trace, message);
        }
        public static void LogTrace(this IMonitor self, string message, Exception error)
        {
            ResolvedLogger(self, LogLevel.Trace, message, error);
        }
        public static void LogTrace(this IMonitor self, string message, params string[] replacements)
        {
            ResolvedLogger(self, LogLevel.Trace, message, null, replacements);
        }
        public static void LogTrace(this IMonitor self, string message, Exception error, params string[] replacements)
        {
            ResolvedLogger(self, LogLevel.Trace, message, error, replacements);
        }
        public static void LogTraceOnce(this IMonitor self, string message)
        {
            ResolvedLogger(self, LogLevel.Trace, message, null, null, true);
        }
        public static void LogTraceOnce(this IMonitor self, string message, Exception error)
        {
            ResolvedLogger(self, LogLevel.Trace, message, error, null, true);
        }
        public static void LogTraceOnce(this IMonitor self, string message, params string[] replacements)
        {
            ResolvedLogger(self, LogLevel.Trace, message, null, replacements, true);
        }
        public static void LogTraceOnce(this IMonitor self, string message, Exception error, params string[] replacements)
        {
            ResolvedLogger(self, LogLevel.Trace, message, error, replacements, true);
        }
    }
}
