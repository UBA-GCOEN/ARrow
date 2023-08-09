using System.Runtime.InteropServices;
using UnityEngine.Scripting;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.MagicLeap.Internal;

namespace UnityEngine.XR.MagicLeap
{
    /// <summary>
    /// The Magic Leap implementation of the <c>XRRaycastSubsystem</c>. Do not create this directly.
    /// Use <c>XRRaycastSubsystemDescriptor.Create()</c> instead.
    /// </summary>
    [Preserve]
    public sealed class MagicLeapRaycastSubsystem : XRRaycastSubsystem
    {
        /// <summary>
        /// Asynchronously casts a ray. Use the returned <see cref="AsyncRaycastResult"/> to check for completion and
        /// retrieve the raycast hit results.
        /// </summary>
        /// <param name="query">The input query for the raycast job.</param>
        /// <returns>An <see cref="AsyncRaycastResult"/> which can be used to check for completion and retrieve the raycast result.</returns>
        public AsyncRaycastResult AsyncRaycast(RaycastQuery query)
        {
            return magicLeapProvider.AsyncRaycast(query);
        }

#if UNITY_2020_2_OR_NEWER
        MagicLeapProvider magicLeapProvider => (MagicLeapProvider)provider;
#else
        MagicLeapProvider magicLeapProvider;

        /// <summary>
        /// Create a Magic Leap provider
        /// </summary>
        /// <returns>Concrete implementation of raycast provider</returns>
        protected override Provider CreateProvider()
        {
            magicLeapProvider = new MagicLeapProvider();
            return magicLeapProvider;
        }
#endif

        class MagicLeapProvider : Provider
        {
            ulong m_TrackerHandle = Native.InvalidHandle;

            PerceptionHandle m_PerceptionHandle;

            static Vector3 FlipHandedness(Vector3 v)
            {
                return new Vector3(v.x, v.y, -v.z);
            }

            public AsyncRaycastResult AsyncRaycast(RaycastQuery query)
            {
                return new AsyncRaycastResult(m_TrackerHandle, query);
            }

            public MagicLeapProvider()
            {
                m_PerceptionHandle = PerceptionHandle.Acquire();
            }

            public override void Start()
            {
                var result = Native.Create(out m_TrackerHandle);
                if (result != MLApiResult.Ok)
                {
                    m_TrackerHandle = Native.InvalidHandle;
                }
                MagicLeapFeatures.SetFeatureRequested(Feature.Raycast, true);
            }

            public override void Stop()
            {
                if (m_TrackerHandle != Native.InvalidHandle)
                {
                    Native.Destroy(m_TrackerHandle);
                    m_TrackerHandle = Native.InvalidHandle;
                }
                MagicLeapFeatures.SetFeatureRequested(Feature.Raycast, false);
            }

            public override void Destroy()
            {
                m_PerceptionHandle.Dispose();
            }
        }

        static class Native
        {
            public const ulong InvalidHandle = ulong.MaxValue;

#if UNITY_ANDROID
        const string Library = "perception.magicleap";
#else
        const string Library = "ml_perception_client";
#endif

            [DllImport(Library, CallingConvention = CallingConvention.Cdecl, EntryPoint = "MLRaycastCreate")]
            public static extern MLApiResult Create(out ulong handle);

            [DllImport(Library, CallingConvention = CallingConvention.Cdecl, EntryPoint = "MLRaycastDestroy")]
            public static extern MLApiResult Destroy(ulong handle);
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void RegisterDescriptor()
        {
#if UNITY_ANDROID
            XRRaycastSubsystemDescriptor.RegisterDescriptor(new XRRaycastSubsystemDescriptor.Cinfo
            {
                id = "MagicLeap-Raycast",
#if UNITY_2020_2_OR_NEWER
                providerType = typeof(MagicLeapRaycastSubsystem.MagicLeapProvider),
                subsystemTypeOverride = typeof(MagicLeapRaycastSubsystem),
#else
                subsystemImplementationType = typeof(MagicLeapRaycastSubsystem),
#endif
                supportsViewportBasedRaycast = false,
                supportsWorldBasedRaycast = false,
                supportedTrackableTypes = TrackableType.None,
            });
#endif // UNITY_ANDROID
        }
    }
}
