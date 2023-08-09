---
uid: arfoundation-whats-new
---
# What's new in version 5.0

Summary of changes in AR Foundation package version 5.0.

The main updates in this release include:

**Added**

- Added `ARDebugMenu` that will help in visualizing the location of the [XROrigin](xref:Unity.XR.CoreUtils.XROrigin), planes, anchors and the current FPS and tracking state. See the [manual entry for AR Debug Menu](xref:arfoundation-debug-menu) for more information.
- Added a configuration sub-menu to `ARDebugMenu`. See the [manual entry for AR Debug Menu](xref:arfoundation-debug-menu) for more information.
- Added support new [XRMeshSubsystem](xref:UnityEngine.XR.XRMeshSubsystem) interface available in 2021.2, which allows providers to specify a separate transform for each mesh.
- Added support for a new [OcclusionPreferenceMode.NoOcclusion](xref:UnityEngine.XR.ARSubsystems.OcclusionPreferenceMode) mode that, when set, disables occlusion rendering on the camera background when using [ARCameraBackground](xref:UnityEngine.XR.ARFoundation.ARCameraBackground) and [AROcclusionManager](xref:UnityEngine.XR.ARFoundation.AROcclusionManager).
- Added [simulation](xref:arfoundation-simulation) of environments that allows testing of AR scenes in the editor.

**Updated**

- `com.unity.xr.arsubsystems` has been merged into `com.unity.xr.arfoundation`. All the subsystems that were part of `com.unity.xr.arsubsystems` package are now available with this package (See the [old AR Subsystems Package changelog](https://docs.unity3d.com/Packages/com.unity.xr.arsubsystems@4.2/changelog/CHANGELOG.html) for more details).
- The minimum Unity version for this package is now 2021.2.
- Depth subsystem has been renamed to point cloud subsystem: 
  - `XRDepthSubsystem` renamed to `XRPointCloudSubsystem`
  - `XRDepthSubsystemDescriptor` renamed to `XRPointCloudSubsystemDescriptor`

**Deprecated**

- Deprecated the [XRSubsystem](xref:UnityEngine.XR.ARSubsystems.XRSubsystem%601). Use [SubsystemWithProvider](xref:UnityEngine.SubsystemsImplementation.SubsystemWithProvider) base class instead with an implementation of [SubsystemDescriptorWithProvider](xref:UnityEngine.SubsystemsImplementation.SubsystemDescriptorWithProvider) and [SubsystemProvider](xref:UnityEngine.SubsystemsImplementation.SubsystemProvider).
- `ARSessionOrigin` is now deprecated. Use [XROrigin](xref:Unity.XR.CoreUtils.XROrigin) from the XR Core Utilities package instead.
- `ARPoseDriver` is now deprecated. Use [TrackedPoseDriver](https://docs.unity3d.com/Packages/com.unity.inputsystem@1.1/api/UnityEngine.InputSystem.XR.TrackedPoseDriver.html) from the Input System package instead.

**Fixed**

- Fixed a missing dependency on built-in particle system module.

For a full list of changes and updates in this version, see the [AR Foundation package changelog](xref:arfoundation-changelog).
