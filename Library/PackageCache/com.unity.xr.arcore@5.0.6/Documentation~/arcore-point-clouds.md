---
uid: arcore-point-clouds
---
# Point Clouds

Ray casts return a `Pose` for the item the ray cast hits. When you use a ray cast against feature points, the pose orientation provides an estimate for the surface the feature point might represent.

The point cloud subsystem doesn't require any additional resources, so it doesn't affect performance.

ARCore's point cloud subsystem only produces one [`XRPointCloud`](xref:UnityEngine.XR.ARSubsystems.XRPointCloud).

For more information, see AR Foundation [Point clouds](xref:arfoundation-point-clouds).
