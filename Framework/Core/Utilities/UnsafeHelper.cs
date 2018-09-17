using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;

namespace Entoarox.Framework.Core.Utilities
{
    internal unsafe class UnsafeHelper
    {
        /*********
        ** Public methods
        *********/
        internal static void ReplaceMethod(MethodBase source, MethodBase dest)
        {
            IntPtr srcAdr = UnsafeHelper.GetMethodAddress(source);
            IntPtr destAdr = UnsafeHelper.GetMethodAddress(dest);
            if (IntPtr.Size == 8)
            {
                ulong* d = (ulong*)destAdr.ToPointer();
                *d = *(ulong*)srcAdr.ToPointer();
            }
            else
            {
                uint* d = (uint*)destAdr.ToPointer();
                *d = *(uint*)srcAdr.ToPointer();
            }
        }


        /*********
        ** Protected methods
        *********/
        private static IntPtr GetMethodAddress(MethodBase method)
        {
            if (method is DynamicMethod)
            {
                byte* ptr = (byte*)UnsafeHelper.GetDynamicMethodRuntimeHandle(method).ToPointer();
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

            ulong* location = (ulong*)method.MethodHandle.Value.ToPointer();
            int index = (int)((*location >> 32) & 0xFF);

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
    }
}
