using System.Reflection;
using System.Xml;
using System.Xml.Serialization;

using StardewValley;

namespace Entoarox.Framework.Core.Override
{
    internal class HookedSerializableDictionary
    {
        public MethodInfo ReadXmlMethod=typeof(HookedSerializableDictionary).GetMethod("RealReadXml", BindingFlags.Instance | BindingFlags.Public);
        public MethodInfo WriteXmlMethod=typeof(HookedSerializableDictionary).GetMethod("RealWriteXml", BindingFlags.Instance | BindingFlags.Public);
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
