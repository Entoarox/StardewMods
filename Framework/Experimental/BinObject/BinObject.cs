using System;
using System.Collections;
using System.Collections.Generic;

namespace Entoarox.Framework.Experimental.BinObject
{
    public abstract class BinObject
    {
        public BinType Type { get; private set; }
        internal BinObject(BinType type)
        {
            this.Type = type;
        }
        public static implicit operator BinObject(Dictionary<string, BinObject> value) => value == null ? (BinObject)new BinNull() : new BinMap(value);

        public static implicit operator BinObject(List<BinObject> value) => value == null ? (BinObject)new BinNull() : new BinList(value);
        public static implicit operator BinObject(string value) => value == null ? (BinObject)new BinNull() : new BinObject<string>(BinType.String, value);
        public static implicit operator BinObject(bool value) => new BinObject<bool>(value ? BinType.BoolTrue : BinType.BoolFalse, value);
        public static implicit operator BinObject(float value) => new BinObject<float>(BinType.Single, value);
        public static implicit operator BinObject(double value) => new BinObject<double>(BinType.Double, value);
        public static implicit operator BinObject(sbyte value) => new BinObject<sbyte>(BinType.Int8, value);
        public static implicit operator BinObject(short value) => new BinObject<short>(BinType.Int16, value);
        public static implicit operator BinObject(int value) => new BinObject<int>(BinType.Int32, value);
        public static implicit operator BinObject(long value) => new BinObject<long>(BinType.Int64, value);
        public static implicit operator BinObject(byte value) => new BinObject<byte>(BinType.UInt8, value);
        public static implicit operator BinObject(ushort value) => new BinObject<ushort>(BinType.UInt16, value);
        public static implicit operator BinObject(uint value) => new BinObject<uint>(BinType.UInt32, value);
        public static implicit operator BinObject(ulong value) => new BinObject<ulong>(BinType.UInt64, value);

        private static List<Type> _AcceptedTypes = new List<Type>()
        {
            typeof(IDictionary<string, BinObject>),
            typeof(IList<BinObject>),
            typeof(string),
            typeof(bool),
            typeof(float),
            typeof(double),
            typeof(sbyte),
            typeof(short),
            typeof(int),
            typeof(long),
            typeof(byte),
            typeof(ushort),
            typeof(uint),
            typeof(ulong)
        };
        /// <summary>
        /// Checks if the given <see cref="object"/> is valid for use in a BinObject structure
        /// </summary>
        /// <param name="value">The instance to check for validity</param>
        /// <returns>If the instance can be converted</returns>
        public static bool IsValid(object value)
        {
            if (value == null && ! (value is Type))
                return true;
            Type type = value is Type ? (Type)value : value.GetType();
            if (_AcceptedTypes.Contains(type))
                return true;
            foreach (Type t in _AcceptedTypes)
                if (t.IsAssignableFrom(type))
                    return true;
            return false;
        }
        /// <summary>
        /// Checks if the given <see cref="Dictionary{TKey, TValue}"/> is valid for use in a BinObject structure
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsValid<TKey, TValue>(IDictionary<TKey, TValue> value)
        {
            return IsValid(typeof(TKey)) && IsValid(typeof(TValue));
        }
        public static bool IsValid<T>(IList<T> value)
        {
            return IsValid(typeof(T));
        }
    }
    public class BinObject<T> : BinObject
    {
        protected T Value;
        internal BinObject(BinType type, T value) : base(type)
        {
            this.Value = value;
        }
        public static implicit operator T(BinObject<T> value)
        {
            return value.Value;
        }
    }
}
