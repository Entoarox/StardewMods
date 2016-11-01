using System;
using System.Collections.Generic;

namespace Entoarox.Framework
{
    /**
     * <summary>Utility class to allow for multiple types in the same list or dictionary, including lists and dictionaries themselves</summary>
     * <remarks>This class is mainly intended to mimic the type-freedom available in JSON arrays and objects.</remarks>
     */
    public abstract class PackedValue
    {
        public abstract Type GetValueType();
        public abstract T Cast<T>();
        public abstract T TryCast<T>();
        public abstract bool Is<T>();
        public static implicit operator PackedValue(int value)
        {
            return new PackedValue<int>(value);
        }
        public static implicit operator PackedValue(double value)
        {
            return new PackedValue<double>(value);
        }
        public static implicit operator PackedValue(bool value)
        {
            return new PackedValue<bool>(value);
        }
        public static implicit operator PackedValue(string value)
        {
            return new PackedValue<string>(value);
        }
        public static implicit operator PackedValue(List<PackedValue> value)
        {
            return new PackedValue<List<PackedValue>>(value);
        }
        public static implicit operator PackedValue(Dictionary<string, PackedValue> value)
        {
            return new PackedValue<Dictionary<string, PackedValue>>(value);
        }
        public static implicit operator int(PackedValue value)
        {
            return value.Cast<int>();
        }
        public static implicit operator double(PackedValue value)
        {
            return value.Cast<double>();
        }
        public static implicit operator bool(PackedValue value)
        {
            return value.Cast<bool>();
        }
        public static implicit operator string(PackedValue value)
        {
            return value.Cast<string>();
        }
        public static implicit operator List<PackedValue>(PackedValue value)
        {
            return value.Cast<List<PackedValue>>();
        }
        public static implicit operator Dictionary<string, PackedValue>(PackedValue value)
        {
            return value.Cast<Dictionary<string, PackedValue>>();
        }
    }
    public class PackedValue<T> : PackedValue
    {
        internal T Value;
        internal PackedValue(T value)
        {
            Value = value;
        }
        public override TCast Cast<TCast>()
        {
            if (typeof(TCast) == typeof(T))
                return (TCast)(object)Value;
            throw new InvalidCastException();
        }
        public override TCast TryCast<TCast>()
        {
            try
            {
                return Cast<TCast>();
            }
            catch
            {
                return default(TCast);
            }
        }
        public override bool Is<TCast>()
        {
            return typeof(TCast) == typeof(T);
        }
        public override Type GetValueType()
        {
            return typeof(T);
        }
        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }
        public override bool Equals(object obj)
        {
            return Value.Equals(obj is PackedValue<T> ? obj as PackedValue<T> : obj);
        }
    }
}
