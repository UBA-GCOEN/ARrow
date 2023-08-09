---
uid: arkit-point-clouds
---
# Point Clouds

Raycasts always return a `Pose` for the item the raycast hit. When raycasting against feature points, the pose is oriented to provide an estimate for the surface the feature point might represent.

The point cloud subsystem doesn't require additional resources, so enabling it doesn't affect performance

ARKit's point cloud subsystem only ever produces a single [XRPointCloud](xref:UnityEngine.XR.ARSubsystems.XRPointCloud).

For more information, see [Point clouds](xref:arfoundation-point-clouds).
