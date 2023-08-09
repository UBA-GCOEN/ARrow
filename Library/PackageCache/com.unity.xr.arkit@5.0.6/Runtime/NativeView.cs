using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace UnityEngine.XR.ARKit
{
    unsafe struct NativeView
    {
        public void* data;
        public int count;
    }

    static class NativeViewExtensions
    {
        public static unsafe NativeView AsNativeView<T>(this NativeArray<T> array) where T : struct => new NativeView
        {
            data = array.GetUnsafePtr(),
            count = array.Length
        };

        public static unsafe NativeView AsNativeView<T>(this NativeSlice<T> slice) where T : struct => new NativeView
        {
            data = slice.GetUnsafePtr(),
            count = slice.Length
        };
    }
}
