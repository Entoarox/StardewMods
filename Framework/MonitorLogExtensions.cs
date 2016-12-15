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
        private static void ResolvedLogger(IMonitor self, LogLevel level, string message, Exception error, string[] replacements, bool once)
        {
            if (new List<string>(replacements).Count>0)
                message = string.Format(message, replacements.ToArray());
            if (error != null)
                message = message + Environment.NewLine + error.Message + Environment.NewLine + error.StackTrace;
            if (once)
                if (Cache.Contains(level.ToString() + ':' + message))
                    return;
                else
                    Cache.Add(level.ToString() + ':' + message);
            self.Log(message, level);
        }
        public static void Log(this IMonitor self, LogLevel level, string message, Exception error=null, params string[] replacements)
        {
            ResolvedLogger(self, level, message, error, replacements, false);
        }
        public static void LogOnce(this IMonitor self, LogLevel level, string message, Exception error=null, params string[] replacements)
        {
            ResolvedLogger(self, level, message, error, replacements, true);
        }
        public static void ExitGameImmediately(this IMonitor self, string message, Exception error = null, params string[] replacements)
        {
            if (new List<string>(replacements).Count > 0)
                message = string.Format(message, replacements.ToArray());
            if (error != null)
                message = message + Environment.NewLine + error.Message + Environment.NewLine + error.StackTrace;
            self.ExitGameImmediately(message);
        }
    }
}
