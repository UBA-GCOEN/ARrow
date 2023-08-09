---
uid: arkit-whats-new
---
# What's new in version 5.0

Summary of changes in ARCore XR Plug-in package version 5.0.

The main updates in this release include:

**Added**

- Added support for a new [OcclusionPreferenceMode.NoOcclusion](xref:UnityEngine.XR.ARSubsystems.Configuration.OcclusionPreferenceMode) mode that, when set, disables occlusion rendering on the camera background when using [ARCameraBackground](xref:UnityEngine.XR.ARFoundation.ARCameraBackground) and [AROcclusionManager](xref:UnityEngine.XR.ARFoundation.AROcclusionManager).

**Changed**

- `com.unity.xr.arkit-face-tracking` is no longer a separate package and has merged into `com.unity.xr.arkit`. The features that are now available with this package include (See the [old face tracking changelog](https://docs.unity3d.com/Packages/com.unity.xr.arkit-face-tracking@4.2/changelog/CHANGELOG.html) for more details):
  - Provides runtime support for Face Tracking on ARKit.
  - Support for ARKit 3 functionality: multiple face tracking and tracking a face (with front camera) while in world tracking (with rear camera).
- The minimum Unity version for this package is now 2021.2.

For a full list of changes and updates in this version, see the [Apple ARKit XR Plug-in package changelog](xref:arkit-changelog).

[!include[](snippets/apple-arkit-trademark.md)]
