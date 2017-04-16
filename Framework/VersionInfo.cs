using System;

namespace Entoarox.Framework
{
    public class VersionInfo
    {
        public Version Latest;
        public Version Recommended;
        public Version Minimum;
        public VersionInfo()
        {
            this.Latest = new Version(0, 0, 0);
            this.Recommended = new Version(0, 0, 0);
            this.Minimum = new Version(0, 0, 0);
        }
    }
}
