using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using UnityEngine.Scripting;
using UnityEngine.XR.ARSubsystems;
#if UNITY_XR_ARKIT_LOADER_ENABLED
using System.Runtime.InteropServices;
#endif

namespace UnityEngine.XR.ARKit
{
    /// <summary>
    /// The ARKit implementation of the <c>XRPlaneSubsystem</c>. Do not create this directly. Use the <c>SubsystemManager</c> instead.
    /// </summary>
    [Preserve]
    public sealed class ARKitXRPlaneSubsystem : XRPlaneSubsystem
    {
        class ARKitProvider : Provider
        {
            public override void Destroy() => NativeApi.UnityARKit_Planes_Shutdown();

            public override void Start() =>  NativeApi.UnityARKit_Planes_Start();

            public override void Stop() => NativeApi.UnityARKit_Planes_Stop();

            /// <summary>
            /// Get the current plane detection mode in use.
            /// </summary>
            public override PlaneDetectionMode currentPlaneDetectionMode => NativeApi.UnityARKit_Planes_GetCurrentPlaneDetectionMode();

            public override unsafe void GetBoundary(
                TrackableId trackableId,
                Allocator allocator,
                ref NativeArray<Vector2> boundary)
            {
                void* plane = NativeApi.UnityARKit_Planes_AcquireBoundary(
                    trackableId,
                    out void* verticesPtr,
                    out int numPoints);

                try
                {
                    CreateOrResizeNativeArrayIfNecessary(numPoints, allocator, ref boundary);
                    var transformPositionsHandle = new TransformBoundaryPositionsJob
                    {
                        positionsIn = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<Vector4>(verticesPtr, numPoints, Allocator.None),
                        positionsOut = boundary
                    }.Schedule(numPoints, 1);

                    new FlipBoundaryWindingJob
                    {
                        positions = boundary
                    }.Schedule(transformPositionsHandle).Complete();
                }
                finally
                {
                    NativeApi.UnityARKit_Planes_ReleaseBoundary(plane);
                }
            }

            struct FlipBoundaryWindingJob : IJob
            {
                public NativeArray<Vector2> positions;

                public void Execute()
                {
                    var half = positions.Length / 2;
                    for (int i = 0; i < half; ++i)
                    {
                        var j = positions.Length - 1 - i;
                        (positions[i], positions[j]) = (positions[j], positions[i]);
                    }
                }
            }

            struct TransformBoundaryPositionsJob : IJobParallelFor
            {
                [ReadOnly]
                public NativeArray<Vector4> positionsIn;

                [WriteOnly]
                public NativeArray<Vector2> positionsOut;

                public void Execute(int index)
                {
                    positionsOut[index] = new Vector2(
                        // https://developer.apple.com/documentation/arkit/arplanegeometry/2941052-boundaryvertices?language=objc
                        // "The owning plane anchor's transform matrix defines the coordinate system for these points."
                        // It doesn't explicitly state the y component is zero, but that must be the case if the
                        // boundary points are in plane-space. Emperically, it has been true for horizontal and vertical planes.
                        // This IS explicitly true for the extents (see above) and would follow the same logic.
                        //
                        // Boundary vertices are in right-handed coordinates and clockwise winding order. To convert
                        // to left-handed, we flip the Z coordinate, but that also flips the winding, so we have to
                        // flip the winding back to clockwise by reversing the polygon index (j).
                         positionsIn[index].x,
                        -positionsIn[index].z);
                }
            }

            public override unsafe TrackableChanges<BoundedPlane> GetChanges(
                BoundedPlane defaultPlane,
                Allocator allocator)
            {
                var context = NativeApi.UnityARKit_Planes_AcquireChanges(
                    out void* addedArrayPtr, out int addedLength,
                    out void* updatedArrayPtr, out int updatedLength,
                    out void* removedArrayPtr, out int removedLength,
                    out int elementSize);

                try
                {
                    return new TrackableChanges<BoundedPlane>(
                        addedArrayPtr, addedLength,
                        updatedArrayPtr, updatedLength,
                        removedArrayPtr, removedLength,
                        defaultPlane, elementSize,
                        allocator);
                }
                finally
                {
                    NativeApi.UnityARKit_Planes_ReleaseChanges(context);
                }
            }

            public override PlaneDetectionMode requestedPlaneDetectionMode
            {
                get => NativeApi.UnityARKit_Planes_GetRequestedPlaneDetectionMode();
                set => NativeApi.UnityARKit_Planes_SetRequestedPlaneDetectionMode(value);
            }
        }

        /// <summary>
        /// Container to wrap the native ARKit APIs needed at registration.
        /// </summary>
        static class NativeApi
        {
#if UNITY_XR_ARKIT_LOADER_ENABLED
            [DllImport("__Internal")]
            internal static extern unsafe bool UnityARKit_Planes_SupportsClassification();

            [DllImport("__Internal")]
            internal static extern void UnityARKit_Planes_Shutdown();

            [DllImport("__Internal")]
            internal static extern void UnityARKit_Planes_Start();

            [DllImport("__Internal")]
            internal static extern void UnityARKit_Planes_Stop();

            [DllImport("__Internal")]
            internal static extern unsafe void* UnityARKit_Planes_AcquireChanges(
                out void* addedPtr, out int addedLength,
                out void* updatedPtr, out int updatedLength,
                out void* removedPtr, out int removedLength,
                out int elementSize);

            [DllImport("__Internal")]
            internal static extern unsafe void UnityARKit_Planes_ReleaseChanges(void* changes);

            [DllImport("__Internal")]
            internal static extern PlaneDetectionMode UnityARKit_Planes_GetRequestedPlaneDetectionMode();

            [DllImport("__Internal")]
            internal static extern void UnityARKit_Planes_SetRequestedPlaneDetectionMode(PlaneDetectionMode mode);

            [DllImport("__Internal")]
            internal static extern PlaneDetectionMode UnityARKit_Planes_GetCurrentPlaneDetectionMode();

            [DllImport("__Internal")]
            internal static extern unsafe void* UnityARKit_Planes_AcquireBoundary(
                TrackableId trackableId,
                out void* verticiesPtr,
                out int numPoints);

            [DllImport("__Internal")]
            internal static extern unsafe void UnityARKit_Planes_ReleaseBoundary(
                void* boundary);
#else
            static readonly string k_ExceptionMsg = "Apple ARKit XR Plug-in Provider not enabled in project settings.";

            internal static unsafe bool UnityARKit_Planes_SupportsClassification() => false;

            internal static void UnityARKit_Planes_Shutdown()
            {
                throw new System.NotImplementedException(k_ExceptionMsg);
            }

            internal static void UnityARKit_Planes_Start()
            {
                throw new System.NotImplementedException(k_ExceptionMsg);
            }

            internal static void UnityARKit_Planes_Stop()
            {
                throw new System.NotImplementedException(k_ExceptionMsg);
            }

            internal static unsafe void* UnityARKit_Planes_AcquireChanges(
                out void* addedPtr, out int addedLength,
                out void* updatedPtr, out int updatedLength,
                out void* removedPtr, out int removedLength,
                out int elementSize)
            {
                throw new System.NotImplementedException(k_ExceptionMsg);
            }

            internal static unsafe void UnityARKit_Planes_ReleaseChanges(void* changes)
            {
                throw new System.NotImplementedException(k_ExceptionMsg);
            }

            internal static PlaneDetectionMode UnityARKit_Planes_GetRequestedPlaneDetectionMode()
            {
                throw new System.NotImplementedException(k_ExceptionMsg);
            }

            internal static void UnityARKit_Planes_SetRequestedPlaneDetectionMode(PlaneDetectionMode mode)
            {
                throw new System.NotImplementedException(k_ExceptionMsg);
            }

            internal static PlaneDetectionMode UnityARKit_Planes_GetCurrentPlaneDetectionMode()
            {
                throw new System.NotImplementedException(k_ExceptionMsg);
            }

            internal static unsafe void* UnityARKit_Planes_AcquireBoundary(
                TrackableId trackableId,
                out void* verticiesPtr,
                out int numPoints)
            {
                throw new System.NotImplementedException(k_ExceptionMsg);
            }

            internal static unsafe void UnityARKit_Planes_ReleaseBoundary(void* boundary)
            {
                throw new System.NotImplementedException(k_ExceptionMsg);
            }
#endif
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void RegisterDescriptor()
        {
            if (!Api.AtLeast11_0())
                return;

            var cinfo = new XRPlaneSubsystemDescriptor.Cinfo
            {
                id = "ARKit-Plane",
                providerType = typeof(ARKitProvider),
                subsystemTypeOverride = typeof(ARKitXRPlaneSubsystem),
                supportsHorizontalPlaneDetection = true,
                supportsVerticalPlaneDetection = Api.AtLeast11_3(),
                supportsArbitraryPlaneDetection = false,
                supportsBoundaryVertices = true,
                supportsClassification = NativeApi.UnityARKit_Planes_SupportsClassification(),
            };

            XRPlaneSubsystemDescriptor.Create(cinfo);
        }
    }
}
