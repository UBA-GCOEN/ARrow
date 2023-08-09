using System.Runtime.CompilerServices;

using UnityEngine;

namespace UnityEngine.XR.MagicLeap.Rendering
{
    /// <summary>
    /// Static class for rendering utility methods
    /// </summary>
    public static class RenderingUtility
    {
        /// <summary>
        /// Returns the scale (in the x) of the parent transform, or 1.0f if no parent found.
        /// </summary>
        /// <param name="transform">The transform we are starting on.</param>
        /// <returns>X value of the scale of the parent.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float GetParentScale(Transform transform)
        {
            var scale = Vector3.one;
            var parent = transform.parent;
            if (parent)
                scale = parent.lossyScale;
#if ML_RENDERING_VALIDATION
            if (!(Mathf.Approximately(scale.x, scale.y) && Mathf.Approximately(scale.x, scale.z)))
            {
                MLWarnings.WarnedAboutNonUniformScale.Trigger();
                return (scale.x + scale.y + scale.z) / 3;
            }
#endif
            // Avoid precision error caused by averaging x, y and z components.
            return scale.x;
        }

        /// <summary>
        /// Get the scale of the Main Camera
        /// </summary>
        /// <returns>X value of the scale of the parent.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float GetMainCameraScale()
        {
            return GetParentScale(Camera.main.transform);
        }

        /// <summary>
        /// Convert a Unity value to Magic Leap unit
        /// </summary>
        /// <param name="val">input in Unity units</param>
        /// <returns>Converted unit</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float ToMagicLeapUnits(float val)
        {
            return ToMagicLeapUnits(val, RenderingSettings.s_CachedCameraScale);
        }

        /// <summary>
        /// Convert a Unity value to Magic Leap unit
        /// </summary>
        /// <param name="val">input in Unity units</param>
        /// <param name="scale">scale factor to apply</param>
        /// <returns>Converted unit</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static float ToMagicLeapUnits(float val, float scale)
        {
            return val / scale;
        }

        /// <summary>
        /// Convert a Magic Leap unit to a Unity unit
        /// </summary>
        /// <param name="val">input in Magic Leap units</param>
        /// <returns>Converted unit</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float ToUnityUnits(float val)
        {
            return ToUnityUnits(val, RenderingSettings.s_CachedCameraScale);
        }

        /// <summary>
        /// Convert a Magic Leap unit to a Unity unity, give a scale factor.
        /// </summary>
        /// <param name="val">input in Magic Leap units</param>
        /// <param name="scale">scale factor to apply</param>
        /// <returns>Converted unit</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static float ToUnityUnits(float val, float scale)
        {
            return val * scale;
        }
    }
}