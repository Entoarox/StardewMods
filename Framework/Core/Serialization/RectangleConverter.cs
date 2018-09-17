using System;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Entoarox.Framework.Core.Serialization
{
    internal class RectangleConverter : JsonConverter
    {
        /*********
        ** Protected methods
        *********/
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            Rectangle rectangle = (Rectangle)value;

            int x = rectangle.X;
            int y = rectangle.Y;
            int width = rectangle.Width;
            int height = rectangle.Height;

            JObject o = JObject.FromObject(new { x, y, width, height });

            o.WriteTo(writer);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject o = JObject.Load(reader);

            int x = RectangleConverter.GetTokenValue(o, "x") ?? 0;
            int y = RectangleConverter.GetTokenValue(o, "y") ?? 0;
            int width = RectangleConverter.GetTokenValue(o, "width") ?? 0;
            int height = RectangleConverter.GetTokenValue(o, "height") ?? 0;

            return new Rectangle(x, y, width, height);
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Rectangle);
        }


        /*********
        ** Public methods
        *********/
        private static int? GetTokenValue(JObject o, string tokenName)
        {
            return o.TryGetValue(tokenName, StringComparison.InvariantCultureIgnoreCase, out JToken t) ? (int)t : (int?)null;
        }
    }
}
