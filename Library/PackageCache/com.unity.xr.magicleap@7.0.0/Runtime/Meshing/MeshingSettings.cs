using System;
using System.Runtime.InteropServices;

using UnityEngine;
using UnityEngine.XR;

namespace UnityEngine.XR.MagicLeap.Meshing
{
    /// <summary>
    /// Static class for managing setting for the Meshing feature.
    /// </summary>
    public static class MeshingSettings
    {
        /// <summary>
        /// Acquire the confidence level of a given Mesh ID
        /// </summary>
        /// <param name="meshId">Given Mesh Identifier</param>
        /// <param name="count">Count</param>
        /// <returns>Pointer to confidence struct</returns>

        public static IntPtr AcquireConfidence(MeshId meshId, out int count) => UnityMagicLeap_MeshingAcquireConfidence(meshId, out count);
        /// <summary>
        /// Release a reference to the confidence struct we've acquired from <c>AcquireConfidence</c>
        /// </summary>
        /// <param name="meshId">The Mesh ID</param>
        public static void ReleaseConfidence(MeshId meshId) => UnityMagicLeap_MeshingReleaseConfidence(meshId);

        /// <summary>
        /// Set a bounds on the meshing subsystem
        /// </summary>
        /// <param name="transform">Unity Transform indicating the centroid of the space</param>
        /// <param name="extents">Bounding rectangle of the meshing space</param>
        public static void SetBounds(Transform transform, Vector3 extents)
        {
            SetBounds(transform.localPosition, transform.localRotation, extents);
        }

        /// <summary>
        /// Set the bounding area for the meshing subsystem.
        /// </summary>
        /// <param name="position">Vector3 representing the center of the bounding rectangle.</param>
        /// <param name="rotation">Quaternion representing the orientation of the bounding rectangle.</param>
        /// <param name="extents">Vector3 representing the extents of the bounding rectangle.</param>
        public static void SetBounds(Vector3 position, Quaternion rotation, Vector3 extents)
        {
            UnityMagicLeap_MeshingSetBounds(position, rotation, extents);
        }

        /// <summary>
        /// Setter for the meshing batch size.
        /// </summary>
        public static int batchSize
        {
            set { UnityMagicLeap_MeshingSetBatchSize(value); }
        }

        /// <summary>
        /// Setter for the meshing density.
        /// </summary>
        public static float density
        {
            set { UnityMagicLeap_MeshingSetDensity(value); }
        }

        /// <summary>
        /// Setter for the Meshing settings.
        /// </summary>
        public static MLMeshingSettings meshingSettings
        {
            set { UnityMagicLeap_MeshingUpdateSettings(ref value); }
        }

#if UNITY_ANDROID
        [DllImport("UnityMagicLeap")]
        internal static extern void UnityMagicLeap_MeshingUpdateSettings(ref MLMeshingSettings newSettings);

        [DllImport("UnityMagicLeap")]
        internal static extern void UnityMagicLeap_MeshingSetDensity(float density);

        [DllImport("UnityMagicLeap")]
        internal static extern void UnityMagicLeap_MeshingSetBounds(Vector3 center, Quaternion rotation, Vector3 extents);

        [DllImport("UnityMagicLeap")]
        internal static extern void UnityMagicLeap_MeshingSetBatchSize(int batchSize);

        [DllImport("UnityMagicLeap")]
        internal static extern IntPtr UnityMagicLeap_MeshingAcquireConfidence(MeshId meshId, out int count);

        [DllImport("UnityMagicLeap")]
        internal static extern void UnityMagicLeap_MeshingReleaseConfidence(MeshId meshId);
#else
        internal static void UnityMagicLeap_MeshingUpdateSettings(ref MLMeshingSettings newSettings) { }

        internal static void UnityMagicLeap_MeshingSetDensity(float density) { }

        internal static void UnityMagicLeap_MeshingSetBounds(Vector3 center, Quaternion rotation, Vector3 extents) { }

        internal static void UnityMagicLeap_MeshingSetBatchSize(int batchSize) {}

        internal static IntPtr UnityMagicLeap_MeshingAcquireConfidence(MeshId meshId, out int count) { count = 0; return IntPtr.Zero; }

        internal static void UnityMagicLeap_MeshingReleaseConfidence(MeshId meshId) { }
#endif // UNITY_ANDROID
    }

    /// <summary>
    /// Magic Leap Meshing flags
    /// </summary>
    [Flags]
    public enum MLMeshingFlags
    {
        /// <summary>
        /// None
        /// </summary>
        None = 0,
        /// <summary>
        /// Point Cloud
        /// </summary>
        PointCloud = 1 << 0,
        /// <summary>
        /// Compute Normals on the mesh
        /// </summary>
        ComputeNormals = 1 << 1,
        /// <summary>
        /// Compute Mesh confidence
        /// </summary>
        ComputeConfidence = 1 << 2,
        /// <summary>
        /// Planarize the mesh
        /// </summary>
        Planarize = 1 << 3,
        /// <summary>
        /// Remove any mesh skirting
        /// </summary>
        RemoveMeshSkirt = 1 << 4,
        /// <summary>
        /// Counter Clockwise Indexing
        /// </summary>
        IndexOrderCCW = 1 << 5
    }

    /// <summary>
    /// Magic Leap Meshing settings
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct MLMeshingSettings
    {
        /// <summary>
        /// All flags to be passed into the meshing subsystem
        /// </summary>
        public MLMeshingFlags flags;
        /// <summary>
        /// Minimum size to fill in holes in the meshing subsystem
        /// </summary>
        public float fillHoleLength;
        /// <summary>
        /// Area used to calculate a disconnected component
        /// </summary>
        public float disconnectedComponentArea;
    }
}