---
uid: arcore-manual
---
# Google ARCore XR Plug-in

Use the Google ARCore XR Plug-in package to enable ARCore support in your [AR Foundation](https://docs.unity3d.com/Packages/com.unity.xr.arfoundation@latest) project. This package implements the following AR Foundation features using ARCore 1.31:

| Feature | Description |
| :------ | :---------- |
| [Session](xref:arcore-session) | Enable, disable, and configure AR on the target platform. |
| [Device tracking](xref:arfoundation-device-tracking) | Track the device's position and rotation in physical space. |
| [Camera](xref:arcore-camera) | Render images from device cameras and perform light estimation. |
| [Plane detection](xref:arcore-plane-detection) | Detect and track surfaces. |
| [Image tracking](xref:arcore-image-tracking) | Detect and track 2D images. |
| [Face tracking](xref:arcore-face-tracking) | Detect and track human faces. |
| [Point clouds](xref:arcore-point-clouds) | Detect and track feature points. |
| [Raycasts](xref:arfoundation-raycasts) | Cast rays against tracked items. |
| [Anchors](xref:arfoundation-anchors) | Track arbitrary points in space. |
| [Environment probes](xref:arfoundation-environment-probes) | Generate cubemaps of the environment. |
| [Occlusion](xref:arcore-occlusion) | Occlude AR content with physical objects and perform human segmentation. |

## Unsupported features

This package does not implement the following AR Foundation features as they are not supported by ARCore 1.31:

| Feature | Description |
| :------ | :---------- |
| [Object tracking](xref:arfoundation-object-tracking) | Detect and track 3D objects. |
| [Body tracking](xref:arfoundation-body-tracking) | Detect and track a human body. |
| [Meshing](xref:arfoundation-meshing) | Generate meshes of the environment. |
| [Participants](xref:arfoundation-participant-tracking) | Track other devices in a shared AR session. |

# Install the Google ARCore XR Plug-in

When you enable the Google ARCore XR Plug-in in **Project Settings** > **XR Plug-in Management**, Unity automatically installs this package (if necessary). See [Enable the ARCore plug-in](xref:arcore-project-config#enable-arcore) for instructions.

You can also install and uninstall this package using the [Package Manager](https://learn.unity.com/tutorial/the-package-manager). Installing through the Package Manager does not automatically enable the plug-in. You must still enable it in **Project Settings** > **XR Plug-in Management**.

# Project configuration

See [Project configuration](xref:arcore-project-config) for information about the project settings that affect ARCore applications. 

## Require AR

You can define ARCore as either required or optional in Android apps. By default, ARCore is required, which means your app can only be installed on AR-supported devices. If you specify that AR is optional, your app can be installed on all Android devices.

See [Set the ARCore support Requirement](xref:arcore-project-config#arcore-required) for instructions on how to change this setting.

## Project Validation

The Google ARCore XR Plug-in package supports project validation. Project validation is a set of rules that the Unity Editor checks to detect possible problems with your project's configuration. See [Project Validation](xref:arcore-project-config#project-validation) section for more information about the rules checked for Google ARCore XR Plug-in.

# Usage

In most cases, you should use the scripts, prefabs, and assets provided by the AR Foundation package as the basis for your handheld AR apps rather than using ARCore APIs directly. The Google ARCore XR Plug-in supports AR Foundation features on the Android platform by implementing the native endpoints required to target Google’s ARCore platform using Unity's multi-platform XR API.

Use the Google ARCore XR Plug-in APIs when you need access to Android ARCore-specific features. The ARCoreFaceRegions sample in the [AR Foundation Samples](https://github.com/Unity-Technologies/arfoundation-samples#ARCoreFaceRegions) repository provides an example of using an ARCore feature.

# Technical details

## Requirements

This version of Google ARCore XR Plug-in is compatible with the following versions of the Unity Editor:

* 2021.2
* 2021.3
* 2022.1
* 2022.2

## Known limitations

* Color Temperature in degrees Kelvin is not currently supported.
* The [XROcclusionSubsystemDescriptor](xref:UnityEngine.XR.ARSubsystems.XROcclusionSubsystemDescriptor) properties [supportsEnvironmentDepthImage](xref:UnityEngine.XR.ARSubsystems.XROcclusionSubsystemDescriptor.supportsEnvironmentDepthImage) and [supportsEnvironmentDepthConfidenceImage](xref:UnityEngine.XR.ARSubsystems.XROcclusionSubsystemDescriptor.supportsEnvironmentDepthConfidenceImage) require a session before support can be determined. If there is no session, then these properties return `false`. They might return `true` later when a session has been established.

## Package contents

This version of Google ARCore XR Plug-in includes:

* A shared library which provides implementation of the XR Subsystems listed above
* A shader used for rendering the camera image
* A plug-in metadata file
