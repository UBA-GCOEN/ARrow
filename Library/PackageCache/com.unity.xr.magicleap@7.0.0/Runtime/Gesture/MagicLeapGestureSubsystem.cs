using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using System.Runtime.InteropServices;
using UnityEngine.Scripting;
using UnityEngine.XR.InteractionSubsystems;

namespace UnityEngine.XR.MagicLeap
{
    /// <summary>
    /// MagicLeap implementation of the <c>XRGestureSubsystem</c>. Do not create this directly. Use the <c>SubsystemManager</c> instead.
    /// </summary>
    [Preserve]
    public sealed class MagicLeapGestureSubsystem : XRGestureSubsystem
    {
        /// <summary>
        /// A collection of all MagicLeapTouchpadGestureEvents managed by this subsystem.
        /// This is cleared every frame and refreshed with new gesture events.
        /// </summary>
        public NativeArray<MagicLeapTouchpadGestureEvent> touchpadGestureEvents => ((provider as MagicLeapGestureProvider)!).touchpadGestureEvents;

        internal bool ControllerGesturesEnabled
        {
            get
            {
#if UNITY_ANDROID
                return NativeApi.IsControllerGesturesEnabled();
#else
                Debug.LogError($"Cannot get {nameof(MagicLeapGestureSubsystem.ControllerGesturesEnabled)} while not on the Magic Leap platform.  This will be ignored.");
                return false;
#endif // UNITY_ANDROID
            }
            set
            {
#if UNITY_ANDROID
                NativeApi.SetControllerGesturesEnabled(value);
#else
                Debug.LogError($"Cannot set {nameof(MagicLeapGestureSubsystem.ControllerGesturesEnabled)} while not on the Magic Leap platform.  This will be ignored.");
#endif // UNITY_ANDROID
            }
        }

        internal class MagicLeapGestureProvider : Provider
        {
            public MagicLeapGestureProvider()
            {
#if UNITY_ANDROID
                NativeApi.Create();
#endif // UNITY_ANDROID
            }

            public override void Start()
            {
#if UNITY_ANDROID
                NativeApi.Start();
#endif // UNITY_ANDROID
            }

            public override void Stop()
            {
#if UNITY_ANDROID
                NativeApi.Stop();
#endif // UNITY_ANDROID
            }

            public override void Update()
            {
#if UNITY_ANDROID
                NativeApi.Update();
                RetrieveGestureEvents();
#endif // UNITY_ANDROID
            }

            public unsafe delegate void* GetGesturesDelegate(out int gestureEventsLength, out int elementSize);

            unsafe void GetGestureEvents<T>(ref NativeArray<T> gestureEventsArray, GetGesturesDelegate getGesturesAction) where T : struct
            {
                // gestureEventsPtr is not owned by this code, contents must be copied out in this function.
                int gestureEventsLength, elementSize;
                void* gestureEventsPtr = getGesturesAction(out gestureEventsLength, out elementSize);

                if (!(gestureEventsLength == 0 && gestureEventsArray.Length == 0))
                {
                    if (gestureEventsArray.IsCreated)
                        gestureEventsArray.Dispose();
                    gestureEventsArray = new NativeArray<T>(gestureEventsLength, Allocator.Persistent);

                    var sizeOfGestureEvent = UnsafeUtility.SizeOf<T>();
                    UnsafeUtility.MemCpy(gestureEventsArray.GetUnsafePtr(), gestureEventsPtr, elementSize * gestureEventsLength);
                }
            }

            unsafe void RetrieveGestureEvents()
            {
#if UNITY_ANDROID
                if (NativeApi.IsControllerGesturesEnabled())
                    GetGestureEvents<MagicLeapTouchpadGestureEvent>(ref m_TouchpadGestureEvents, NativeApi.GetTouchpadGestureEventsPtr);
#endif // UNITY_ANDROID
            }

            public override void Destroy()
            {
#if UNITY_ANDROID
                NativeApi.Destroy();
#endif // UNITY_ANDROID

                m_TouchpadGestureEvents.Dispose();
                base.Destroy();
            }

            public NativeArray<MagicLeapTouchpadGestureEvent> touchpadGestureEvents { get { return m_TouchpadGestureEvents; } }
            NativeArray<MagicLeapTouchpadGestureEvent> m_TouchpadGestureEvents = new NativeArray<MagicLeapTouchpadGestureEvent>(0, Allocator.Persistent);
        }

#if UNITY_EDITOR || UNITY_ANDROID
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
#endif
        static void RegisterDescriptor()
        {
#if UNITY_ANDROID
            XRGestureSubsystemDescriptor.RegisterDescriptor(
                new XRGestureSubsystemDescriptor.Cinfo
                {
                    id = "MagicLeap-Gesture",
                    providerType = typeof(MagicLeapGestureSubsystem.MagicLeapGestureProvider),
                    subsystemTypeOverride = typeof(MagicLeapGestureSubsystem),
                }
            );
#endif // UNITY_ANDROID
        }

        static class NativeApi
        {
            const string Library = "UnityMagicLeap";

            [DllImport(Library, EntryPoint="UnityMagicLeap_GesturesCreate")]
            public static extern void Create();

            [DllImport(Library, EntryPoint="UnityMagicLeap_GesturesUpdate")]
            public static extern void Update();

            [DllImport(Library, EntryPoint="UnityMagicLeap_GesturesStart")]
            public static extern void Start();

            [DllImport(Library, EntryPoint="UnityMagicLeap_GesturesGetTouchpadGestureEventsPtr")]
            public static extern unsafe void* GetTouchpadGestureEventsPtr(out int gesturesLength, out int elementSize);

            [DllImport(Library, EntryPoint="UnityMagicLeap_GesturesDestroy")]
            public static extern void Destroy();

            [DllImport(Library, EntryPoint="UnityMagicLeap_GesturesStop")]
            public static extern void Stop();

            [DllImport(Library, EntryPoint="UnityMagicLeap_GesturesIsControllerGesturesEnabled")]
            [return: MarshalAs(UnmanagedType.I1)]
            public static extern bool IsControllerGesturesEnabled();

            [DllImport(Library, EntryPoint="UnityMagicLeap_GesturesSetControllerGesturesEnabled")]
            public static extern void SetControllerGesturesEnabled([MarshalAs(UnmanagedType.I1)] bool value);
        }

        // High GUID bits saved for common (Activate) gesture for this subsystem
        static GestureId s_NextGUID = new GestureId(1, 0);
        static GestureId GetNextGUID()
        {
            unchecked
            {
                s_NextGUID.subId1 += 1;
                if (s_NextGUID.subId1 != 0) return s_NextGUID;
                s_NextGUID.subId1 += 1;
            }

            return s_NextGUID;
        }
    }
}
