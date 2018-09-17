using System;
using System.Collections.Generic;
using StardewModdingAPI;

namespace Entoarox.Framework.Core
{
    internal class FakeManifest : IManifest
    {
        public string Name { get; }

        public string Description => throw new NotImplementedException();

        public string Author => throw new NotImplementedException();

        public ISemanticVersion Version { get; }

        public ISemanticVersion MinimumApiVersion => throw new NotImplementedException();

        public string UniqueID => throw new NotImplementedException();

        public string EntryDll => throw new NotImplementedException();

        public IManifestContentPackFor ContentPackFor => throw new NotImplementedException();

        public IManifestDependency[] Dependencies => throw new NotImplementedException();

        public string[] UpdateKeys
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        public IDictionary<string, object> ExtraFields => throw new NotImplementedException();

        public FakeManifest(string name, ISemanticVersion version)
        {
            this.Name = name;
            this.Version = version;
        }
    }
}
