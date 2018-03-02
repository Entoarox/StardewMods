using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Entoarox.Framework.Core.Serialization
{
    class RectangleConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var rectangle = (Rectangle)value;

            int x = rectangle.X;
            int y = rectangle.Y;
            int width = rectangle.Width;
            int height = rectangle.Height;

            var o = JObject.FromObject(new { x, y, width, height });

            o.WriteTo(writer);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var o = JObject.Load(reader);

            int x = GetTokenValue(o, "x") ?? 0;
            int y = GetTokenValue(o, "y") ?? 0;
            int width = GetTokenValue(o, "width") ?? 0;
            int height = GetTokenValue(o, "height") ?? 0;

            return new Rectangle(x, y, width, height);
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Rectangle);
        }

        private static int? GetTokenValue(JObject o, string tokenName)
        {
            return o.TryGetValue(tokenName, StringComparison.InvariantCultureIgnoreCase, out JToken t) ? (int)t : (int?)null;
        }
    }
}
