using System;
using System.Runtime.InteropServices;
using Unity.Collections;
using UnityEngine.Scripting;
using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.ARKit
{
    /// <summary>
    /// An ARKit-specific implementation of the <c>XRObjectTrackingSubsystem</c>. Do not create this directly. Use the <c>SubsystemManager</c> instead.
    /// </summary>
    [Preserve]
    public sealed class ARKitXRObjectTrackingSubsystem : XRObjectTrackingSubsystem
    {
        internal void AddReferenceObject(XRReferenceObjectLibrary library, ARReferenceObject referenceObject)
        {
            if (library == null)
                throw new ArgumentNullException(nameof(library));

            if (referenceObject == null)
                throw new ArgumentNullException(nameof(referenceObject));

            if (this.library == library)
            {
                ARKitProvider.AddReferenceObject(referenceObject);
            }
        }

        class ARKitProvider : Provider
        {
            /// <summary>
            /// Invoked when <c>Start</c> is called on the subsystem. This method is only called if the subsystem was not previously running.
            /// </summary>
            public override void Start() { }

            /// <summary>
            /// Invoked when <c>Stop</c> is called on the subsystem. This method is only called if the subsystem was previously running.
            /// </summary>
            public override void Stop() { }

#if UNITY_XR_ARKIT_LOADER_ENABLED
            [DllImport("__Internal")]
            static extern void UnityARKit_ObjectTracking_Initialize();

            [DllImport("__Internal")]
            static extern void UnityARKit_ObjectTracking_Shutdown();

            [DllImport("__Internal")]
            static extern void UnityARKit_ObjectTracking_Stop();

            [DllImport("__Internal", EntryPoint = "UnityARKit_ObjectTracking_SetLibrary")]
            static extern void SetLibrary(IntPtr referenceObjects);

            [DllImport("__Internal")]
            static extern unsafe void* UnityARKit_ObjectTracking_AcquireChanges(
                out void* addedPtr, out int addedLength,
                out void* updatedPtr, out int updatedLength,
                out void* removedPtr, out int removedLength,
                out int elementSize);

            [DllImport("__Internal")]
            static extern unsafe void UnityARKit_ObjectTracking_ReleaseChanges(void* changes);

            [DllImport("__Internal", EntryPoint = "UnityARKit_ObjectTracking_AddReferenceObject")]
            public static extern void AddReferenceObject(ARReferenceObject referenceObject);
#else
            static readonly string k_ExceptionMsg = "Apple ARKit XR Plug-in Provider not enabled in project settings.";

            static void UnityARKit_ObjectTracking_Initialize()
            {
                throw new System.NotImplementedException(k_ExceptionMsg);
            }

            static void UnityARKit_ObjectTracking_Shutdown()
            {
                throw new System.NotImplementedException(k_ExceptionMsg);
            }

            static void UnityARKit_ObjectTracking_Stop()
            {
                throw new System.NotImplementedException(k_ExceptionMsg);
            }

            static void SetLibrary(IntPtr referenceObjects)
            {
                throw new System.NotImplementedException(k_ExceptionMsg);
            }

            static unsafe void* UnityARKit_ObjectTracking_AcquireChanges(
                out void* addedPtr, out int addedLength,
                out void* updatedPtr, out int updatedLength,
                out void* removedPtr, out int removedLength,
                out int elementSize)
            {
                throw new System.NotImplementedException(k_ExceptionMsg);
            }

            static unsafe void UnityARKit_ObjectTracking_ReleaseChanges(void* changes)
            {
                throw new System.NotImplementedException(k_ExceptionMsg);
            }

            public static void AddReferenceObject(ARReferenceObject referenceObject)
            {
                throw new System.NotImplementedException(k_ExceptionMsg);
            }
#endif
            public override unsafe XRReferenceObjectLibrary library
            {
                set
                {
                    if (value == null)
                    {
                        UnityARKit_ObjectTracking_Stop();
                    }
                    else
                    {
                        var referenceObjects = new NSMutableSet<ARReferenceObject>(NSObject.Initialization.Default);
                        try
                        {
                            foreach (var obj in value)
                            {
                                var entry = obj.FindEntry<ARKitReferenceObjectEntry>();
                                if (entry)
                                {
                                    var referenceObject = entry.GetARKitReferenceObject(obj);
                                    if (referenceObject != null)
                                    {
                                        referenceObjects.Add(referenceObject);
                                    }
                                }
                            }

                            SetLibrary(referenceObjects.AsIntPtr());
                        }
                        finally
                        {
                            referenceObjects.Dispose();
                        }
                    }
                }
            }

            public override unsafe TrackableChanges<XRTrackedObject> GetChanges(
                XRTrackedObject defaultTrackedObject,
                Allocator allocator)
            {
                int addedLength, updatedLength, removedLength, elementSize;
                void* addedPtr, updatedPtr, removedPtr;

                var context = UnityARKit_ObjectTracking_AcquireChanges(
                    out addedPtr, out addedLength,
                    out updatedPtr, out updatedLength,
                    out removedPtr, out removedLength,
                    out elementSize);

                try
                {
                    return new TrackableChanges<XRTrackedObject>(
                        addedPtr, addedLength,
                        updatedPtr, updatedLength,
                        removedPtr, removedLength,
                        defaultTrackedObject, elementSize,
                        allocator);
                }
                finally
                {
                    UnityARKit_ObjectTracking_ReleaseChanges(context);
                }
            }

            public override void Destroy() => UnityARKit_ObjectTracking_Shutdown();

            public ARKitProvider() => UnityARKit_ObjectTracking_Initialize();
        }

        /// <summary>
        /// This method runs when the app starts to register this provider with XR Subsystem Manager.
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void RegisterDescriptor()
        {
            // No support before iOS 12.0
            if (!Api.AtLeast12_0())
                return;

            var capabilities = new XRObjectTrackingSubsystemDescriptor.Capabilities
            {
            };

            Register<ARKitXRObjectTrackingSubsystem.ARKitProvider, ARKitXRObjectTrackingSubsystem>("ARKit-ObjectTracking", capabilities);
        }
    }
}
