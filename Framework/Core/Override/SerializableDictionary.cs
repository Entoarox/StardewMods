using System;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;

using StardewValley;
using StardewValley.Locations;
using StardewValley.TerrainFeatures;
using SObject = StardewValley.Object;

using Microsoft.Xna.Framework;

namespace Entoarox.Framework.Core.Override
{
    using Utilities;
    internal class HookedSerializableDictionary
    {
        public MethodInfo ReadXmlMethod=typeof(HookedSerializableDictionary).GetMethod("RealReadXml", BindingFlags.Instance | BindingFlags.Public);
        public MethodInfo WriteXmlMethod=typeof(HookedSerializableDictionary).GetMethod("RealWriteXml", BindingFlags.Instance | BindingFlags.Public);
        public static void Hook()
        {
            Type[] Whitelist = new Type[]
            {
                typeof(SerializableDictionary<int,int>),
                typeof(SerializableDictionary<int,int[]>),
                typeof(SerializableDictionary<int,bool>),
                typeof(SerializableDictionary<int,bool[]>),
                typeof(SerializableDictionary<int,MineInfo>),
                typeof(SerializableDictionary<string,int>),
                typeof(SerializableDictionary<string,int[]>),
                typeof(SerializableDictionary<long,FarmAnimal>),
                typeof(SerializableDictionary<Vector2,int>),
                typeof(SerializableDictionary<Vector2,SObject>),
                typeof(SerializableDictionary<Vector2,TerrainFeature>),
                typeof(SerializableDictionary<,>)
            };
            MethodInfo ReadXml = typeof(HookedSerializableDictionary).GetMethod("ReadXml", BindingFlags.Instance | BindingFlags.Public);
            MethodInfo WriteXml = typeof(HookedSerializableDictionary).GetMethod("ReadXml", BindingFlags.Instance | BindingFlags.Public);
            foreach (Type type in Whitelist)
            {
                UnsafeHelper.ReplaceMethod(type.GetMethod("ReadXml", BindingFlags.Instance | BindingFlags.Public), ReadXml);
                UnsafeHelper.ReplaceMethod(type.GetMethod("ReadXml", BindingFlags.Instance | BindingFlags.Public), WriteXml);
            }
        }
        public void RealReadXml<TKeys,TValues>(SerializableDictionary<TKeys,TValues> self, XmlReader reader)
        {
            XmlSerializer xmlSerializer1 = new XmlSerializer(typeof(TValues));
            XmlSerializer xmlSerializer2 = new XmlSerializer(typeof(TKeys), ModEntry.SerializerTypes.ToArray());
            int num1 = reader.IsEmptyElement ? 1 : 0;
            reader.Read();
            if (num1 != 0)
                return;
            while (reader.NodeType != XmlNodeType.EndElement)
            {
                reader.ReadStartElement("item");
                reader.ReadStartElement("key");
                TKeys key = (TKeys)xmlSerializer1.Deserialize(reader);
                reader.ReadEndElement();
                reader.ReadStartElement("value");
                TValues obj = (TValues)xmlSerializer2.Deserialize(reader);
                reader.ReadEndElement();
                self.Add(key, obj);
                reader.ReadEndElement();
                int num2 = (int)reader.MoveToContent();
            }
            reader.ReadEndElement();
        }
        public void RealWriteXml<TKeys,TValues>(SerializableDictionary<TKeys, TValues> self, XmlWriter writer)
        {
            XmlSerializer xmlSerializer1 = new XmlSerializer(typeof(TKeys));
            XmlSerializer xmlSerializer2 = new XmlSerializer(typeof(TValues), ModEntry.SerializerTypes.ToArray());
            foreach (TKeys index in self.Keys)
            {
                writer.WriteStartElement("item");
                writer.WriteStartElement("key");
                xmlSerializer1.Serialize(writer, index);
                writer.WriteEndElement();
                writer.WriteStartElement("value");
                TValues obj = self[index];
                xmlSerializer2.Serialize(writer, obj);
                writer.WriteEndElement();
                writer.WriteEndElement();
            }
        }
        public void HookedReadXml(XmlReader reader)
        {
            ReadXmlMethod.MakeGenericMethod(this.GetType().BaseType.GenericTypeArguments).Invoke(null, new object[] { this, reader });
        }
        public void HookedWriteXml(XmlWriter writer)
        {
            WriteXmlMethod.MakeGenericMethod(this.GetType().BaseType.GenericTypeArguments).Invoke(null, new object[] { this, writer });
        }
    }
}
