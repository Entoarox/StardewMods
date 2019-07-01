using System;
using System.Linq;

using Newtonsoft.Json.Linq;

using StardewModdingAPI;

using SObject = StardewValley.Object;

namespace Entoarox.Framework.Core.Serialization
{
    class InstanceState
    {
        public string Type;
        public JToken Data;

        public InstanceState()
        {

        }
        public InstanceState(string type, JToken data)
        {
            this.Type = type;
            this.Data = data;
        }
        private T Placeholder<T>()
        {
            if (typeof(T) == typeof(SObject))
                return (T)(object)new Placeholder(this.Type, this.Data);
            return default;
        }
        public T Restore<T>()
        {
            return this.Restore<T>(false, default);
        }
        public T Restore<T>(T output)
        {
            return this.Restore(true, output);
        }
        public T Restore<T>(bool forceTarget, T output)
        {
            Type type = System.Type.GetType(this.Type);
            if (type == null)
            {
                EntoaroxFrameworkMod.Logger.Log("Unable to deserialize custom class, the type does not exist: " + this.Type, LogLevel.Error);
                return forceTarget ? output : this.Placeholder<T>();
            }
            else if (!typeof(T).IsAssignableFrom(type))
            {
                EntoaroxFrameworkMod.Logger.Log("Unable to deserialize custom class, the type does not inherit from the " + typeof(T).FullName + " class: " + this.Type, LogLevel.Error);
                return forceTarget ? output : this.Placeholder<T>();
            }
            else if (!type.GetInterfaces().Contains(typeof(ICustomItem)))
            {
                EntoaroxFrameworkMod.Logger.Log("Unable to deserialize custom class, the type does not implement the ICustomItem interface: " + this.Type, LogLevel.Error);
                return forceTarget ? output : this.Placeholder<T>();
            }
            else
            {
                try
                {
                    if (forceTarget)
                        using (var sr = this.Data.CreateReader())
                            EntoaroxFrameworkMod.Serializer.Populate(sr, output);
                    else
                        output = (T)this.Data.ToObject(type, EntoaroxFrameworkMod.Serializer);
                    if (output is IDeserializationHandler test && test.ShouldDelete())
                        return default;
                    return output;
                }
                catch (Exception err)
                {
                    EntoaroxFrameworkMod.Logger.Log("Unable to deserialize custom class of type " + this.Type + ", unknown error:\n" + err.ToString(), LogLevel.Error);
                    return this.Placeholder<T>();
                }
            }
        }
    }
}
