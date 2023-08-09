
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace UnityEngine.XR.MagicLeap.Rendering
{
    /// <summary>
    /// Enumeration representing the Depth precision
    /// </summary>
    public enum DepthPrecision : int
    {
        /// <summary>
        /// 32 bit depth precision
        /// </summary>
        [InspectorName("32 bit Depth Precision")]
        Depth32,
        /// <summary>
        /// 24 bit depth precision with an 8 bit stencil buffer
        /// </summary>
        [InspectorName("24 bit Depth, 8 bit Stencil buffer")]
        Depth24Stencil8
    }

    /// <summary>
    /// Enumeration for the Stabilization mode
    /// </summary>
    public enum StabilizationMode : byte
    {
        /// <summary>
        /// No stabilization
        /// </summary>
        None,
        /// <summary>
        /// Far clip plane stabilization
        /// </summary>
        [InspectorName("Far Clip stabilization")]
        FarClip,
        /// <summary>
        /// Furthest Object stabilization
        /// </summary>
        [InspectorName("Furthest Object stabilization")]
        FurthestObject,
        /// <summary>
        /// Custom stabilization
        /// Should you decide to write your own custom stabilization, please ensure that you set the property
        /// <c>RenderingSettings.stabilizationDistance</c> on each frame. Please ensure you understand the ramifications of
        /// rolling your own stabilization code before using this enum.
        /// </summary>
        [InspectorName("Custom stabilization")]
        Custom
    }

    /// <summary>
    /// Static class for Render settings
    /// </summary>
    public static class RenderingSettings
    {
        const float kDefaultFarClip = 10f;
        const float kDefaultNearClip = 5f;
        // TODO / FIXME :: All the string marshalling being done here is probably sub-optimal,
        // but it needs to be profiled first.

        // All values here are expected to be in Unity units, but need to be stored in
        // in MagicLeap units (meters), so we do all the conversion here to keep logic
        // elsewhere as simple as possible.
        internal static float s_CachedCameraScale = 1.0f;

        /// <summary>
        /// Getter/internal setter for the camera scale
        /// </summary>
        public static float cameraScale
        {
            get
            {
                return (UnityMagicLeap_RenderingTryGetParameter("CameraScale", out s_CachedCameraScale)) ? s_CachedCameraScale : 1f;
            }
            internal set
            {
                s_CachedCameraScale = value;
                UnityMagicLeap_RenderingSetParameter("CameraScale", s_CachedCameraScale);
            }
        }
        internal static DepthPrecision depthPrecision
        {
            get
            {
                return UnityMagicLeap_RenderingGetDepthPrecision();
            }

            set
            {
                UnityMagicLeap_RenderingSetDepthPrecision(value);
            }
        }

        /// <summary>
        /// Allows apps to choose whether the system recommended near clip distance should be enforced by Unity or not. 
        /// Disable this enforcement at your own risk as a clipping plane lower than the system recommended value can 
        /// cause discomfort to some users.
        /// </summary>
        /// <value></value>
        public static bool enforceNearClip { get; set; } = true;

        internal static bool headlocked
        {
            get
            {
                return UnityMagicLeap_RenderingGetIsHeadlocked();
            }
            set
            {
                UnityMagicLeap_RenderingSetIsHeadlocked(value);
            }
        }

        /// <summary>
        /// Getter/internal Setter for the far clip distance
        /// </summary>
        public static float farClipDistance
        {
            get
            {
                float farClip = kDefaultFarClip;
                UnityMagicLeap_RenderingTryGetParameter("FarClipDistance", out farClip);
                return farClip;
            }
            internal set { UnityMagicLeap_RenderingSetParameter("FarClipDistance", value); }
        }

        /// <summary>
        /// Getter/internal Setter for the Focal distance
        /// </summary>
        public static float focusDistance
        {
            get
            {
                float focus = 0f;
                UnityMagicLeap_RenderingTryGetParameter("FocusDistance", out focus);
                return focus;
            }
            internal set { UnityMagicLeap_RenderingSetParameter("FocusDistance", value); }
        }

        /// <summary>
        /// Getter for the maximum Far Clip distance
        /// </summary>
        public static float maxFarClipDistance
        {
            get
            {
                float maxFarClip = float.PositiveInfinity;
                UnityMagicLeap_RenderingTryGetParameter("MaxFarClipDistance", out maxFarClip);
                return RenderingUtility.ToUnityUnits(maxFarClip, s_CachedCameraScale);
            }
        }

        /// <summary>
        /// TODO: Delete
        /// </summary>
        [Obsolete("use minNearClipDistance instead")]
        public static float maxNearClipDistance
        {
            get { return minNearClipDistance;}
        }

        /// <summary>
        /// Getter for the minimum Near Clip Distance
        /// </summary>
        public static float minNearClipDistance
        {
            get
            {
                float minNearClip = 0.5f;
                UnityMagicLeap_RenderingTryGetParameter("MinNearClipDistance", out minNearClip);
                return minNearClip;
            }
        }

        /// <summary>
        /// Getter/internal Setter for the near Clip distance
        /// </summary>
        public static float nearClipDistance
        {
            get
            {
                float nearClip = 0.5f;
                UnityMagicLeap_RenderingTryGetParameter("NearClipDistance", out nearClip);
                return nearClip;
            }
            internal set { UnityMagicLeap_RenderingSetParameter("NearClipDistance", value); }
        }


        /// <summary>
        /// TODO: Remove
        /// </summary>
        [Obsolete("use MagicLeapSettings.forceMultipass to force multipass rendering instead")]
        public static bool singlePassEnabled
        {
            get
            {
                float enabled = 0.0f;
                UnityMagicLeap_RenderingTryGetParameter("SinglePassEnabled", out enabled);
                return IsFlagSet(enabled);
            }
            internal set { UnityMagicLeap_RenderingSetParameter("SinglePassEnabled", value ? 1.0f : 0.0f); }
        }


        /// <summary>
        /// Getter/internal Setter for the Protected surface property
        /// </summary>
        public static bool useProtectedSurface
        {
            get
            {
                float enabled = 0f;
                UnityMagicLeap_RenderingTryGetParameter("UseProtectedSurface", out enabled);
                return IsFlagSet(enabled);
            }
            internal set { UnityMagicLeap_RenderingSetParameter("UseProtectedSurface", value ? 1.0f : 0.0f); }
        }


        /// <summary>
        /// TODO: Remove
        /// </summary>
        [Obsolete("Use UnityEngine.XR.XRSettings.renderViewportScale instead")]
        public static float surfaceScale
        {
            get
            {
                float scale = 1f;
                UnityMagicLeap_RenderingTryGetParameter("SurfaceScale", out scale);
                return scale;
            }
            internal set { UnityMagicLeap_RenderingSetParameter("SurfaceScale", value); }
        }

        /// <summary>
        /// TODO: Remove
        /// </summary>
        [Obsolete("useLegacyFrameParameters is ignored on XR SDK")]
        internal static bool useLegacyFrameParameters
        {
            get
            {
                float enabled = 0f;
                UnityMagicLeap_RenderingTryGetParameter("UseLegacyFrameParameters", out enabled);
                return IsFlagSet(enabled);
            }
            set { UnityMagicLeap_RenderingSetParameter("UseLegacyFrameParameters", value ? 1.0f : 0.0f); }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsFlagSet(float val)
        {
            return Mathf.Approximately(val, 0f);
        }

#if UNITY_ANDROID
        const string kLibrary = "UnityMagicLeap";
        [DllImport(kLibrary, CharSet = CharSet.Ansi)]
        internal static extern void UnityMagicLeap_RenderingSetParameter(string key, float newValue);
        [DllImport(kLibrary, CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool UnityMagicLeap_RenderingTryGetParameter(string key, out float value);
        [DllImport(kLibrary)]
        internal static extern DepthPrecision UnityMagicLeap_RenderingGetDepthPrecision();
        [DllImport(kLibrary)]
        internal static extern void UnityMagicLeap_RenderingSetDepthPrecision(DepthPrecision depthPrecision);
        [DllImport(kLibrary)]
        internal static extern bool UnityMagicLeap_RenderingGetIsHeadlocked();
        [DllImport(kLibrary)]
        internal static extern void UnityMagicLeap_RenderingSetIsHeadlocked(bool isHeadlocked);
#else
        internal static void UnityMagicLeap_RenderingSetParameter(string key, float newValue) {}
        internal static bool UnityMagicLeap_RenderingTryGetParameter(string key, out float value) { value = 0f; return false; }
        internal static DepthPrecision UnityMagicLeap_RenderingGetDepthPrecision() { return DepthPrecision.Depth32; }
        internal static void UnityMagicLeap_RenderingSetDepthPrecision(DepthPrecision depthPrecision) {}
        internal static bool UnityMagicLeap_RenderingGetIsHeadlocked() { return false; }
        internal static void UnityMagicLeap_RenderingSetIsHeadlocked(bool isHeadlocked) {}
#endif // UNITY_ANDROID

        // device-specific calls.
#if UNITY_ANDROID && !UNITY_EDITOR
        [DllImport("libc", EntryPoint = "__system_property_get")]
        private static extern int _GetSystemProperty(string name, StringBuilder @value);

        /// <summary>
        /// Get a value from a system property
        /// <param="name">Key of the system property you're looking for</param>
        /// <return>Value for the given key. If the key is not found, return null.</return>
        public static string GetSystemProperty(string name)
        {
            var sb = new StringBuilder(255);
            var ret = _GetSystemProperty(name, sb);
            return ret == 0 ? sb.ToString() : null;
        }
#endif // UNITY_ANDROID && !UNITY_EDITOR
    }
}
