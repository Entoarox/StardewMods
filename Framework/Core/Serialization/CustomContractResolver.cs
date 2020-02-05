using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

using System.Xml.Serialization;
using System.Reflection;

namespace Entoarox.Framework.Core.Serialization
{
    internal class CustomContractResolver : DefaultContractResolver
    {
        /*********
        ** Protected methods
        *********/
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            // get property info
            var prop = base.CreateProperty(member, memberSerialization);
            var elementAttr = (XmlElementAttribute)prop.AttributeProvider.GetAttributes(typeof(XmlElementAttribute), true).FirstOrDefault();
            var ignoreAttr = (XmlIgnoreAttribute)prop.AttributeProvider.GetAttributes(typeof(XmlIgnoreAttribute), true).FirstOrDefault();

            // set name
            if (!string.IsNullOrWhiteSpace(elementAttr?.ElementName))
                prop.PropertyName = elementAttr.ElementName;

            // set whether to seralize
            prop.ShouldSerialize = value => ignoreAttr == null;
            return prop;
        }
    }
}
