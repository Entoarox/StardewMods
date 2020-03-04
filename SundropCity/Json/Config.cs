using System;

namespace SundropCity.Json
{
    [Flags]
    public enum DebugFlags
    {
        None = 0,
        Files = 1,
        Functions = 2,
        Quickload = 4
    }
    public class Config
    {
        public DebugFlags DebugFlags = DebugFlags.None;
    }
}
