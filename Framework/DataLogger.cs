using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;

namespace Entoarox.Framework
{
    public class DataLogger
    {
        /**
         * <summary>The current logging level, this level controls what messages are output to the console<para>Does not affect what messages are output to the log files</para></summary>
         */
        public int LogLevel
        {
            get
            {
                return _LogLevel;
            }
            set
            {
                _LogLevel = Math.Max(0, Math.Min(6, value));
            }
        }
        private int _LogLevel;
        private string Module;
        private static StreamWriter _writer;
        private static StreamWriter writer;
        private static MethodInfo _logToFile;
        public DataLogger(string module, int logLevel=5)
        {
            Module = module;
            LogLevel = logLevel;
            if (writer == null)
            {
                string LogPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "StardewValley", "logs");
                string LogFile = Path.Combine(LogPath, "modded_"+DateTime.Now.ToString("yyyy-MM-dd_hh-mm")+".txt");
                Directory.CreateDirectory(LogPath);
                writer = new StreamWriter(LogFile, true);
                writer.WriteLine(LogFile);
                try
                {
                    if (StardewModdingAPI.Constants.Version.MajorVersion > 0)
                    {
                        _logToFile = typeof(StardewModdingAPI.Log).GetMethod("LogToFile", BindingFlags.Static | BindingFlags.NonPublic);
                        if (_logToFile == null)
                            throw new NullReferenceException("Attempt to hook logging method resulted in a null value.");
                    }
                    else
                    {
                        _writer = Reflection.FieldHelper.GetField<StreamWriter>(typeof(StardewModdingAPI.LogWriter), "_stream", false);
                    }
                }
                catch (Exception err)
                {
                    StardewModdingAPI.Log.SyncColour("[EntoaroxFramework/FATAL] EntoaroxFramework failed to hook into loader for logging" + Environment.NewLine + err.Message + Environment.NewLine + err.StackTrace, ConsoleColor.Red);
                }
            }
        }
        private List<KeyValuePair<string,string>> Cache = new List<KeyValuePair<string, string>>();
        public void Log(string prefix, string message, ConsoleColor color,int level=0)
        {
            string format = '[' + DateTime.Now.ToString("HH:mm:ss.ffff") + "][" + Module + '/' + prefix + "] " + message;
            if (LogLevel > level)
            {
                ConsoleColor prev = Console.ForegroundColor;
                Console.ForegroundColor = color;
                Console.WriteLine(format);
                Console.ForegroundColor = prev;
            }
            lock (writer)
            {
                writer.WriteLine(format);
                writer.Flush();
            }
            if (_writer != null)
                lock (_writer)
                {
                    _writer.WriteLine(format);
                    _writer.Flush();
                    return;
                }
            if (_logToFile == null)
                return;
            _logToFile.Invoke(null, new object[] { format });
        }
        public void LogOnce(string prefix, string message, ConsoleColor color,int level=0)
        {
            KeyValuePair<string, string> item = new KeyValuePair<string, string>(prefix, message);
            if (!Cache.Contains(item))
            {
                Cache.Add(item);
                Log(prefix, message, color, level);
            }
        }
        private void LogError(ConsoleColor color, string prefix, string message, Exception error, int level)
        {
            if(error!=null)
                while (error.InnerException != null)
                    error = error.InnerException;
            if (error == null)
                Log(prefix, message, color, level);
            else
                Log(prefix, message + Environment.NewLine + error.Message + Environment.NewLine + error.StackTrace, color, level);
        }
        private void LogErrorOnce(ConsoleColor color, string prefix, string message, Exception error, int level)
        {
            if (error != null)
                if (error.InnerException != null)
                    error = error.InnerException;
            if (error == null)
                LogOnce(prefix, message, color, level);
            else
                LogOnce(prefix, message + Environment.NewLine + error.Message + Environment.NewLine + error.StackTrace, color, level);
        }
        public void Fatal(string message, Exception error=null)
        {
            LogError(ConsoleColor.Red, "FATAL", message, error,0);
        }
        public void FatalOnce(string message, Exception error = null)
        {
            LogErrorOnce(ConsoleColor.Red, "FATAL", message, error,0);
        }
        public void Error(string message, Exception error=null)
        {
            LogError(ConsoleColor.DarkRed, "ERROR", message, error,1);
        }
        public void ErrorOnce(string message, Exception error = null)
        {
            LogErrorOnce(ConsoleColor.DarkRed, "ERROR", message, error,1);
        }
        public void Warn(string message,Exception error=null)
        {
            LogError(ConsoleColor.DarkMagenta, "WARN", message, error,2);
        }
        public void WarnOnce(string message, Exception error = null)
        {
            LogErrorOnce(ConsoleColor.DarkMagenta, "WARN", message, error,2);
        }
        public void Info(string message, Exception error = null)
        {
            LogError(ConsoleColor.Cyan, "INFO", message, error,3);
        }
        public void InfoOnce(string message, Exception error = null)
        {
            LogErrorOnce(ConsoleColor.Cyan, "INFO", message, error,3);
        }
        public void Debug(string message, Exception error = null)
        {
            LogError(ConsoleColor.DarkYellow, "DEBUG", message, error,4);
        }
        public void DebugOnce(string message, Exception error = null)
        {
            LogErrorOnce(ConsoleColor.DarkYellow, "DEBUG", message, error,4);
        }
        public void Trace(string message, Exception error = null)
        {
            LogError(ConsoleColor.Gray, "TRACE", message, error,5);
        }
        public void TraceOnce(string message, Exception error=null)
        {
            LogErrorOnce(ConsoleColor.Gray, "TRACE", message, error,5);
        }
        public void LogModInfo(string author, Version version)
        {
            string mode = "unknown";
            switch (Environment.OSVersion.Platform)
            {
                case PlatformID.Win32NT:
                    mode = "xna";
                    break;
                case PlatformID.MacOSX:
                case PlatformID.Unix:
                    mode = "mono";
                    break;
                default:
                    mode = "other";
                    break;
            }
            Log("INFO","Version " + version + '/' + mode + " by " + author + ", do not redistribute without permission",ConsoleColor.Cyan);
        }
    }
}
