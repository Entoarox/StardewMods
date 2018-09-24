using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Entoarox.Framework.Core.Serialization
{
    internal class ReadonlyContractResolver : DefaultContractResolver
    {
        /*********
        ** Protected methods
        *********/
        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            return base.CreateProperties(type, memberSerialization).Where(p => p.Writable).ToList();
        }
    }
}
