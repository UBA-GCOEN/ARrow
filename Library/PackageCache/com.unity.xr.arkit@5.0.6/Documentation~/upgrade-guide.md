---
uid: arkit-upgrade-guide
---
# Upgrading to Apple ARKit XR Plug-in version 5.0

To upgrade to Apple ARKit XR Plug-in package version 5.0, you need to do the following:

- Use Unity 2021.2 or newer.
- `ARKitXRDepthSubsystem` is now `ARKitXRPointCloudSubsystem`.

**Use Unity 2021.2 or newer**

This version of the package requires Unity 2021.2 or newer.

**`ARKitXRDepthSubsystem` is now `ARKitXRPointCloudSubsystem`**

Due to the rename of `XRDepthSubsystem` to `XRPointCloudSubsystem` in the AR Foundation package, the ARKit provider implementation of the subsystem has been renamed from `ARKitXRDepthSubsystem` to `ARKitXRPointCloudSubsystem`. All the APIs within the implementation remain the same as before. This renamed shouldn't require any actions from most users of AR Foundation.

[!include[](snippets/apple-arkit-trademark.md)]
