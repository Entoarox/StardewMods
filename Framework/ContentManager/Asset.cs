using System;

namespace Entoarox.Framework.ContentManager
{
    struct Asset
    {
        private readonly Type Type;
        private readonly string AssetName;
        public Asset(Type type, string assetName)
        {
            Type = type;
            AssetName = assetName;
        }
        public override bool Equals(object obj)
        {
            if (!(obj is Asset))
                return false;
            Asset asset = (Asset)obj;
            return asset.Type.Equals(Type) && asset.AssetName.Equals(AssetName);
        }
        public override int GetHashCode()
        {
            return Type.GetHashCode() * AssetName.GetHashCode();
        }
    }
}
