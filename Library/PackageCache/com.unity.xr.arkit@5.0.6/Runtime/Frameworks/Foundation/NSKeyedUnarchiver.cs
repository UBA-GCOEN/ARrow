using System;
using System.IO;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace UnityEngine.XR.ARKit
{
    static class NSKeyedUnarchiver
    {
        public static T Unarchive<T>(NSData data) where T : struct, INSObject
        {
            var unarchivedObject = Unarchive<T>(data, out var error);
            try
            {
                if (error != null)
                    throw new InvalidDataException($"Unable to deserialize {typeof(T).FullName}: {error.localizedDescription}");

                return unarchivedObject;
            }
            finally
            {
                error.Dispose();
            }
        }

        public static T Unarchive<T>(NSData data, out NSError error) where T : struct, INSObject
        {
            var t = default(T);
            var ptr = UnarchivedObjectOfClass(t.staticClass, data, out error);
            t.SetUnderlyingNativePtr(ptr);
            return t;
        }

        public static unsafe T Unarchive<T>(void* bytes, int byteCount) where T : struct, INSObject
        {
            using var data = NSData.CreateWithBytesNoCopy(bytes, byteCount);
            return Unarchive<T>(data);
        }

        public static T Unarchive<T>(NativeSlice<byte> bytes) where T : struct, INSObject
        {
            unsafe
            {
                return Unarchive<T>(bytes.GetUnsafeReadOnlyPtr(), bytes.Length);
            }
        }

        public static T Unarchive<T>(byte[] bytes) where T : struct, INSObject
        {
            if (bytes == null)
            {
                return default;
            }

            unsafe
            {
                fixed (void* ptr = bytes)
                {
                    return Unarchive<T>(ptr, bytes.Length);
                }
            }
        }

#if UNITY_EDITOR || !UNITY_XR_ARKIT_LOADER_ENABLED
        static IntPtr UnarchivedObjectOfClass(Class cls, NSData data, out NSError error)
        {
            error = default;
            return default;
        }
#else
        [DllImport("__Internal", EntryPoint = "NSKeyedUnarchiver_unarchivedObjectOfClass_fromData_error_")]
        static extern IntPtr UnarchivedObjectOfClass(Class cls, NSData data, out NSError error);
#endif
    }
}
