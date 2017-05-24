using System;
using System.Runtime.CompilerServices;
using System.Reflection;
using System.Reflection.Emit;

namespace Entoarox.Framework.Core.Utilities
{
    unsafe internal class UnsafeHelper
    {
        private static IntPtr GetMethodAddress(MethodBase method)
        {
            if ((method is DynamicMethod))
            {
                byte* ptr = (byte*)GetDynamicMethodRuntimeHandle(method).ToPointer();
                if (IntPtr.Size == 8)
                {
                    ulong* address = (ulong*)ptr;
                    address += 6;
                    return new IntPtr(address);
                }
                else
                {
                    uint* address = (uint*)ptr;
                    address += 6;
                    return new IntPtr(address);
                }
            }

            RuntimeHelpers.PrepareMethod(method.MethodHandle);
            int skip = 10;

            UInt64* location = (UInt64*)(method.MethodHandle.Value.ToPointer());
            int index = (int)(((*location) >> 32) & 0xFF);

            if (IntPtr.Size == 8)
            {
                ulong* classStart = (ulong*)method.DeclaringType.TypeHandle.Value.ToPointer();
                ulong* address = classStart + index + skip;
                return new IntPtr(address);
            }
            else
            {
                uint* classStart = (uint*)method.DeclaringType.TypeHandle.Value.ToPointer();
                uint* address = classStart + index + skip;
                return new IntPtr(address);
            }
        }

        private static IntPtr GetDynamicMethodRuntimeHandle(MethodBase method)
        {
            return ((RuntimeMethodHandle)typeof(DynamicMethod).GetField("m_method", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(method)).Value;
        }
        internal static void ReplaceMethod(MethodBase source, MethodBase dest)
        {
            IntPtr srcAdr = GetMethodAddress(source);
            IntPtr destAdr = GetMethodAddress(dest);
            if (IntPtr.Size == 8)
            {
                ulong* d = (ulong*)destAdr.ToPointer();
                *d = *((ulong*)srcAdr.ToPointer());
            }
            else
            {
                uint* d = (uint*)destAdr.ToPointer();
                *d = *((uint*)srcAdr.ToPointer());
            }
        }
    }
}