using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Runtime.Serialization;

using Microsoft.Xna.Framework.Graphics;

namespace SundropCity.Hotel
{
    class SetFurniture : Furniture
    {
        public static Dictionary<string, FurnitureInfo> SetTypeInfo;
        public string Set;
        [XmlIgnore]
        public FurnitureInfo Info;

        static SetFurniture()
        {
            // When this class is ever used, we need to make sure that the info for the set furniture types is read
            SetTypeInfo = SundropCityMod.SHelper.Data.ReadJsonFile<Dictionary<string, FurnitureInfo>>("assets/Hotel/Data/SetFurnitureTypes.json");
        }
        public SetFurniture(string set, string type)
        {
            if (!SetTypeInfo.ContainsKey(type))
                throw new KeyNotFoundException("Unable to initialize set furniture, type is not defined.");
            this.Set = set;
            this.Type = type;
            this.Subtype = 0;
            this.Info = SetTypeInfo[type];
        }
        [Obsolete("This constructor exists for serializer reasons and should not be used directly.")]
        public SetFurniture()
        {

        }

        [OnDeserialized]
        public void Repair()
        {
            if (!SetTypeInfo.ContainsKey(this.Type))
                throw new KeyNotFoundException("Unable to repair set furniture, type is not defined.");
            this.Info = SetTypeInfo[this.Type];
        }

        public override void Draw(SpriteBatch b)
        {

        }
    }
}
