using System.Collections.Generic;

using StardewModdingAPI;

namespace Entoarox.Framework.Core
{
    class WrappedManifest : IManifest
    {
        public string Name { get; }
        public string Description { get; }
        public string Author { get; }
        public ISemanticVersion Version { get; }
        public ISemanticVersion MinimumApiVersion { get; }
        public string UniqueID { get; }
        public string EntryDll { get; }
        public IManifestDependency[] Dependencies { get; }
        public IDictionary<string,object> ExtraFields { get; }

        internal WrappedManifest(string name, SemanticVersion version)
        {
            Name = name;
            Version = version;
        }
    }
}
