using System;
using System.Runtime.InteropServices;
using Unity.Jobs;

namespace UnityEngine.XR.ARKit
{
    /// <summary>
    /// An interface for Objective-C objects that derive from NSObject.
    /// </summary>
    interface INSObject
    {
        /// <summary>
        /// Gets the underlying Objective-C pointer.
        /// </summary>
        /// <returns>Returns the underlying Objective-C pointer.</returns>
        IntPtr AsIntPtr();

        /// <summary>
        /// Sets the underlying native pointer for the object.
        /// </summary>
        /// <remarks>
        /// Warning: This allows you to change this object into any arbitrary object, even one of a different type.
        /// Incorrect usage is undefined behavior and can cause your application to crash.
        /// </remarks>
        /// <param name="ptr">The pointer to which the object's underlying pointer will be set.</param>
        void SetUnderlyingNativePtr(IntPtr ptr);

        /// <summary>
        /// The <see cref="Class"/> of the type of this <see cref="INSObject"/>.
        /// </summary>
        Class staticClass { get; }
    }

    /// <summary>
    /// Utility methods for interacting with [NSObjects](https://developer.apple.com/documentation/objectivec/nsobject?language=objc).
    /// </summary>
    static class NSObject
    {
        /// <summary>
        /// May be used by constructors to specify the type of object initialization.
        /// </summary>
        public enum Initialization
        {
            /// <summary>
            /// Use for default constructors, since structs cannot have parameterless constructors.
            /// </summary>
            Default
        }

        public static string ToString<T>(T instance) where T : struct, INSObject
        {
            using (var description = GetDescription(instance.AsIntPtr()))
            {
                return description.ToString();
            }
        }

        public static bool ArePointersEqual<T>(T? lhs, T? rhs) where T : struct, INSObject
        {
            // Both have a value, so compare values
            if (lhs.HasValue && rhs.HasValue)
                return lhs.Value.AsIntPtr() == rhs.Value.AsIntPtr();

            // lhs has a value; rhs is null
            if (lhs.HasValue)
                return lhs.Value.AsIntPtr() == IntPtr.Zero;

            // rhs has a value; lhs is null
            if (rhs.HasValue)
                return rhs.Value.AsIntPtr() == IntPtr.Zero;

            // Neither has a value
            return true;
        }

        public static bool IsEqual<T, U>(T lhs, U rhs)
            where T : struct, INSObject
            where U : struct, INSObject
            => IsEqual(lhs.AsIntPtr(), rhs.AsIntPtr());

        public static int GetHashCode<T>(T instance) where T : struct, INSObject =>
            GetHash(instance.AsIntPtr()).GetHashCode();

        public static int GetRetainCount<T>(T obj) where T : struct, INSObject => CFGetRetainCount(obj.AsIntPtr());

        public static void Release(IntPtr ptr)
        {
            if (ptr != IntPtr.Zero)
            {
                CFRelease(ptr);
            }
        }

        public static void Dispose(ref IntPtr ptr)
        {
            Release(ptr);
            ptr = IntPtr.Zero;
        }

        struct ReleaseJob : IJob
        {
            public IntPtr ptr;

            void IJob.Execute()
            {
                if (ptr != IntPtr.Zero)
                {
                    CFRelease(ptr);
                }
            }
        }

        public static JobHandle Dispose(IntPtr ptr, JobHandle inputDependencies) => new ReleaseJob
        {
            ptr = ptr
        }.Schedule(inputDependencies);

        public static void Retain(IntPtr ptr)
        {
            if (ptr != IntPtr.Zero)
            {
                CFRetain(ptr);
            }
        }

        public static void Retain<T>(T obj) where T : struct, INSObject => Retain(obj.AsIntPtr());

#if UNITY_EDITOR || !UNITY_XR_ARKIT_LOADER_ENABLED
        static void CFRetain(IntPtr obj) { }
        static void CFRelease(IntPtr obj) { }
        static int CFGetRetainCount(IntPtr obj) => default;
        static NSString GetDescription(IntPtr self) => default;
        static bool IsEqual(IntPtr self, IntPtr other) => default;
        static ulong GetHash(IntPtr self) => default;
#else
        [DllImport("__Internal")]
        static extern void CFRetain(IntPtr obj);

        [DllImport("__Internal")]
        static extern void CFRelease(IntPtr obj);

        [DllImport("__Internal")]
        static extern int CFGetRetainCount(IntPtr obj);

        [DllImport("__Internal", EntryPoint = "NSObject_get_description")]
        static extern NSString GetDescription(IntPtr self);

        [DllImport("__Internal", EntryPoint = "NSObject_isEqual_")]
        static extern bool IsEqual(IntPtr self, IntPtr other);

        [DllImport("__Internal", EntryPoint = "NSObject_get_hash")]
        static extern ulong GetHash(IntPtr self);
#endif
    }
}
