using System;

namespace Entoarox.Framework
{
    internal struct VersionCheck
    {
        public string Mod;
        public Version Version;
        public string Url;
        public VersionCheck(string mod, Version version, string url)
        {
            this.Mod = mod;
            this.Version = version;
            this.Url = url;
        }
    }
}
