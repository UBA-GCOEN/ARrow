---
uid: arkit-changelog
---
# Changelog

All notable changes to this package will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/en/1.0.0/)
and this project adheres to [Semantic Versioning](http://semver.org/spec/v2.0.0.html).

## [5.0.6] - 2023-05-13

No changes

## [5.0.5] - 2023-03-04

### Fixed

- Fixed [issue ARKB-34](https://issuetracker.unity3d.com/issues/webgl-build-fails-when-apple-arkit-xr-plugin-is-used-in-the-project) where the ARKit package would break non-iOS builds.

## [5.0.4] - 2023-02-06

### Fixed

- Fixed a rare issue where the ARKit Build Processor could incorrectly add the "UNITY_XR_ARKIT_LOADER_ENABLED" preprocessor directive to non-iOS build targets.
- Fixed an issue where enabling or disabling the Apple ARKit XR Plug-in in **Project Settings** > **XR Plug-in Management** while iOS was not the active build target would not immediately add or remove the "UNITY_XR_ARKIT_LOADER_ENABLED" preprocessor directive for the iOS build target. (Previously this change was not applied until iOS became the Editor's active build target.)

## [5.0.3] - 2022-11-01

### Changed

- Rebuilt static libraries with Xcode 14.0. (14A309). You are now required to build iOS players with Xcode 14.0 or newer, a necessary requirement to support iOS 16 devices. Please note that Xcode no longer supports building iOS projects with deployment targets for the armv7 and armv7s architectures.
- ARKit 6 introduced a change to the way plane anchor rotations are updated on iOS 16 devices as they learn more about the environment during an AR Session. To preserve AR Foundation's default behavior and maintain cross-platform consistency, this package applies the new [ARPlaneExtent.rotationOnYAxis](xref:https://developer.apple.com/documentation/arkit/arplaneextent/3950861-rotationonyaxis?language=objc) transformation to plane trackables on iOS 16 devices before returning them. As a result of this change, your application will continue to behave identically on all iOS versions with no developer action required. For more information, see [Apple's ARKit documentation](https://developer.apple.com/documentation/arkit/arplaneextent?changes=latest_major&language=objc).

### Fixed

- Fixed [issue ARKB-23](https://issuetracker.unity3d.com/issues/ios-fps-in-the-player-is-very-low-when-using-xrcamerasubsystem-dot-trygetlatestframe) where the frame rate in the player is low when the `ARKitCameraSubsystem.TryGetLatestFrame(out XRCameraImage)` API is used.
- Fixed [issue ARKB-28](https://issuetracker.unity3d.com/issues/ios-disabling-ar-camera-background-kills-position-tracking-on-ios-in-arfoundation-4-dot-2-6) where disabling and re-enabling the `ARCameraBackground` component freezes the camera textures and device tracking.

## [5.0.2] - 2022-09-11

### Changed

- Static library was built with Xcode 13.4.1 (13F100)

### Fixed

- Fixed [an issue](https://issuetracker.unity3d.com/issues/ar-camera-feed-jitters-and-shakes-with-mutithreaded-rendering-when-running-on-ios-16) where rendered frames comes out of order on iOS 16 when built with multi-threaded rendering enabled in Unity.
- Fixed a bug where the `HandheldARInputDevice` did not appear in the Editor's InputActions UI. Now, if you are manually configuring the **Tracked Pose Driver** component on the camera under XR Origin, you can select **AR Handheld Device > devicePosition** and **AR Handheld Device > deviceRotation** for position and rotation input bindings respectively.
- Fixed a bug where `ARKitReferenceImageLibraryBuildProcessor` could cause compile error when build target is not iOS. 

## [5.0.0-pre.13] - 2022-06-28

No changes

## [5.0.0-pre.12] - 2022-05-19

### Added

- Added support for changing the Camera Background rendering order so that the background can be rendered either `BeforeOpaques` or `AfterOpaques` by setting the `ARKitCameraSubsystem.requestedRenderingMode`.
- Added a shader to support rendering `AfterOpaques` both with Occlusion and without. Shader code can be found in `com.unity.xr.arkit/Assets/ARKitBackgroundAfterOpaque.shader`.
- Added Project Validation for assessing project setup correctness. See [Project Validation manual](xref:arkit-project-config#project-validation) for details.

### Changed

- Static library was built with Xcode 13.3 (13E113)

### Deprecated

- `ARKitCameraSubsystem.backgroundShaderName` has been marked obsolete. Instead use `ARKitCameraSubsystem.backgroundShaderNames` to receive an array of available shader names.
- Deprecated the following depth subsystem APIs in favor of point cloud subsystem APIs. Unity's API Updater should automatically convert any deprecated APIs references to the new APIs when the project is loaded into the Editor again.
  - `ARKitXRDepthSubsystem` has been renamed to `ARKitXRPointCloudSubsystem`.
  - `ARKitLoader.depthSubsystem` has been deprecated. Use `ARKitLoader.pointCloudSubsystem` instead.

## [5.0.0-pre.9] - 2022-03-01

### Added

- Added support for a new [OcclusionPreferenceMode.NoOcclusion](xref:UnityEngine.XR.ARSubsystems.OcclusionPreferenceMode) mode that, when set, disables occlusion rendering on the camera background when using [ARCameraBackground](xref:UnityEngine.XR.ARFoundation.ARCameraBackground) and [AROcclusionManager](xref:UnityEngine.XR.ARFoundation.AROcclusionManager).

### Changed

- Static library was built with Xcode 13.2.1 (13C100)

## [5.0.0-pre.8] - 2022-02-09

No changes

## [5.0.0-pre.7] - 2021-12-10

### Fixed

- Fixed bug where setting a texture's [wrapMode](xref:UnityEngine.Texture.wrapMode) to clamp on the [facePrefab](xref:UnityEngine.XR.ARFoundation.ARFaceManager.facePrefab) would cause the texture to shift incorrectly. The issue tracker can be found [here](https://issuetracker.unity3d.com/issues/ios-arkit-face-mesh-displays-its-materials-texture-incorrectly-when-texturewrapmode-is-set-to-clamp).
- Fixed an issue where the target platform for `libUnityARKitFaceTracking.a` was not set correctly causing the plug-in library to be included in builds for both Android and iOS platforms.

## [5.0.0-pre.6] - 2021-11-17

### Fixed

- Fixed build issue [issue 1357108](https://issuetracker.unity3d.com/issues/unity-xr-arkit-loader-enabled-define-is-not-added-if-arkit-face-tracking-is-enabled-and-unity-editor-build-target-is-not-set-t) with the scripting define UNITY_XR_ARKIT_LOADER_ENABLED not being set correctly in some use cases.

## [5.0.0-pre.5] - 2021-10-28

### Changed

- `com.unity.xr.arkit-face-tracking` is no longer a separate package and has merged into `com.unity.xr.arkit`. The features that are now available with this package include (See the [old face tracking changelog](https://docs.unity3d.com/Packages/com.unity.xr.arkit-face-tracking@4.2/changelog/CHANGELOG.html) for more details):
  - Provides runtime support for Face Tracking on ARKit.
  - Support for ARKit 3 functionality: multiple face tracking and tracking a face (with front camera) while in world tracking (with rear camera).
- The minimum Unity version for this package is now 2021.2.

### Removed

- Removed conditional dependency on the deprecated Lightweight Renderpipeline (LWRP) package.

## [4.2.0] - 2021-08-11

### Fixed

- Fixed issue where selected build group was not the active build group.

## [4.2.0-pre.12] - 2021-07-08

### Added

- Added methods to get the [raw](xref:UnityEngine.XR.ARSubsystems.XROcclusionSubsystem.TryAcquireRawEnvironmentDepthCpuImage(UnityEngine.XR.ARSubsystems.XRCpuImage@)) and [smoothed](xref:UnityEngine.XR.ARSubsystems.XROcclusionSubsystem.TryAcquireSmoothedEnvironmentDepthCpuImage(UnityEngine.XR.ARSubsystems.XRCpuImage@)) depth images independently.
- Added support for the [depthSensorSupported](xref:UnityEngine.XR.ARSubsystems.XRCameraConfiguration.depthSensorSupported) flag on the `XRCameraConfiguration` to indicate whether or not a camera configuration supports using a depth sensor.

### Fixed

- Fixed [issue 1334331](https://issuetracker.unity3d.com/issues/ios-armeshmanager-meshes-flicker-when-arworldalignment-is-set-to-gravityandheading) which could cause meshes to occasionally become briefly invalid, resulting in a flickering effect when visualized.

## [4.2.0-pre.10] - 2021-06-28

No changes

## [4.2.0-pre.9] - 2021-05-27

### Fixed

- Fixed an issue which could cause some requested features to persist across session destruction and reinitialization.

## [4.2.0-pre.8] - 2021-05-20

No changes

## [4.2.0-pre.7] - 2021-05-13

### Added

- Added support for temporal smoothing of the environment depth image. When enabled and supported, this plug-in now provides the environment depth image using [ARFrame.smoothedSceneDepth](https://developer.apple.com/documentation/arkit/arframe/3674209-smoothedscenedepth?language=objc) instead of [ARFrame.sceneDepth](https://developer.apple.com/documentation/arkit/arframe/3566299-scenedepth?language=objc).
- Added support for [ARBuildProcessor](xref:UnityEditor.XR.ARSubsystems.ARBuildProcessor).

### Changed

- Static library was built with Xcode 12.4 (12D4e)

### Fixed

- Fixed object tracking.

## [4.2.0-pre.4] - 2021-03-19

No changes

## [4.2.0-pre.3] - 2021-03-15

No changes

## [4.2.0-pre.2] - 2021-03-03

### Added

- [ARKit reference objects](xref:UnityEngine.XR.ARKit.ARKitReferenceObjectEntry) now store their data in the entry asset, which allows them to be used in [AssetBundles](xref:UnityEngine.AssetBundle).

### Changed

- Update [XR Plug-in Management](https://docs.unity3d.com/Packages/com.unity.xr.management@4.0) dependency to 4.0.
- Deprecated [NSError.isNull](xref:UnityEngine.XR.ARKit.NSError.isNull). To determine whether an `NSError` is null, simply compare it to `null` using the `==` operator.
- Static library was built with Xcode 12.4 (12D4e).
- The minimum Unity version for this package is now 2020.3.

## [4.1.3] - 2021-01-05

No changes

## [4.1.1] - 2020-11-11

### Changed

- Released package for Unity 2021.1

## [4.1.0-preview.13] - 2020-11-09

### Fixed

- Fix crash when performing a synchronous [XRCpuImage conversion](xref:UnityEngine.XR.ARSubsystems.XRCpuImage.Convert(UnityEngine.XR.ARSubsystems.XRCpuImage.ConversionParams,System.IntPtr,System.Int32)) while an [asynchronous conversion](xref:UnityEngine.XR.ARSubsystems.XRCpuImage.ConvertAsync(UnityEngine.XR.ARSubsystems.XRCpuImage.ConversionParams)) is already running.

## [4.1.0-preview.12] - 2020-11-02

### Changed

- Removed a file called `NativeInterop.m` (it has been combined with an existing file to reduce name collisions). If you build for iOS by "appending" rather than "replacing" a previous Xcode project, you will need to manually remove this file reference. It is at `Libraries/com.unity.xr.arkit/Runtime/iOS/NativeInterop.m` in the Xcode project produced by Unity.

### Fixed

- Fix issue with z-depth calculations on iOS when AR Foundation camera is enabled. This issue would result in shader z-depth differences (e.g. during fog computation) between normal camera rendering compared to AR camera rendering.
- Fix a (mostly harmless) warning message emitted by ARKit if the [worldAlignment](https://developer.apple.com/documentation/arkit/arworldalignment?language=objc) is set when it is not allowed to be changed and is already at the requested value.

## [4.1.0-preview.11] - 2020-10-22

### Added

- Added a mechanism to receive callbacks for some ARKit-specific session events. See [ARKitSessionDelegate](xref:UnityEngine.XR.ARKit.ARKitSessionDelegate). This allows you to be notified when:
  - The [ARSession](https://developer.apple.com/documentation/arkit/arsession?language=objc) fails.
  - The [ARConfiguration](https://developer.apple.com/documentation/arkit/arconfiguration?language=objc) changes.
  - The [coaching overlay](https://developer.apple.com/documentation/arkit/arcoachingoverlayview?language=objc) [activates](https://developer.apple.com/documentation/arkit/arcoachingoverlayviewdelegate/3152985-coachingoverlayviewwillactivate?language=objc).
  - The [coaching overlay](https://developer.apple.com/documentation/arkit/arcoachingoverlayview?language=objc) [deactivates](https://developer.apple.com/documentation/arkit/arcoachingoverlayviewdelegate/3152983-coachingoverlayviewdiddeactivate?language=objc).
- Added a new [property](xref:UnityEngine.XR.ARKit.ARKitSessionSubsystem.requestedWorldAlignment) to set the session's [ARWorldAlignment](xref:UnityEngine.XR.ARKit.ARWorldAlignment).
- Added support for the new method [ScheduleAddImageWithValidationJob](xref:UnityEngine.XR.ARSubsystems.MutableRuntimeReferenceImageLibrary.ScheduleAddImageWithValidationJob(Unity.Collections.NativeSlice{System.Byte},UnityEngine.Vector2Int,UnityEngine.TextureFormat,UnityEngine.XR.ARSubsystems.XRReferenceImage,Unity.Jobs.JobHandle)) on the [MutableRuntimeReferenceImageLibrary](xref:UnityEngine.XR.ARSubsystems.MutableRuntimeReferenceImageLibrary).

### Changed

- The implementation of [XRCpuImage.ConvertAsync](xref:UnityEngine.XR.ARSubsystems.XRCpuImage.ConvertAsync(UnityEngine.XR.ARSubsystems.XRCpuImage.ConversionParams)) is now multithreaded across all hardware cores to produce the result faster. Previously, only the [synchronous version](xref:UnityEngine.XR.ARSubsystems.XRCpuImage.Convert(UnityEngine.XR.ARSubsystems.XRCpuImage.ConversionParams,System.IntPtr,System.Int32)) was multithreaded. However, on newer devices with high camera resolutions, the single threaded asynchronous conversion would often take multiple frames to complete. Now, both synchronous and asynchronous conversions are multithreaded.
- Static library was built with Xcode 12.1 (12A7403).

## [4.1.0-preview.10] - 2020-10-12

### Added

- The session configuration's [ARWorldMap](xref:UnityEngine.XR.ARKit.ARWorldMap) can be cleared by calling [ApplyWorldMap](xref:UnityEngine.XR.ARKit.ARKitSessionSubsystem.ApplyWorldMap(UnityEngine.XR.ARKit.ARWorldMap)) with a default-constructed `ARWorldMap`. Previously, this method would throw if the `ARWorldMap` was not valid.

### Changed

- Update [XR Plug-in Management](https://docs.unity3d.com/Packages/com.unity.xr.management@3.2) to 3.2.16.

## [4.1.0-preview.9] - 2020-09-25

### Changed

- Static library was built with Xcode 12.0.1 (12A7300).

### Fixed

- Fixed issue where ARKitSettings could throw a `NullReferenceException` when checking for face tracking.
- Fixed an issue with the importer for `ARObject`s (ARKit's format for object tracking targets) which could incorrectly parse floating point numbers when the host machine used a different floating point format (e.g., swapping `,` and `.`).
- Fixed some documentation links.
- Fixed a hang in the meshing subsystem implementation caused by resource contention.
- Fixed an issue where the environment depth confidence texture was not able to be sampled in a shader. Changed the texture format from MTLPixelFormatR8Uint to MTLPixelFormatR8Unorm.
- Handle non-unit scale in the background shader when calculating depth for occlusion.
- Fixed a sporadic app freeze after an ARKit session is destroyed.
- Fix ["batchmode" builds](https://docs.unity3d.com/Manual/CommandLineArguments.html), i.e., building the iOS Xcode project from the command line. Previously, this could errornously exclude `liblibUnityARKitFaceTracking.a` from the build.

## [4.1.0-preview.7] - 2020-08-26

### Changed

- Static library was built with Xcode 12.0 beta 6 (12A8189n).

### Fixed

- Fixed memory leak in the meshing subsystem implementation from unreleased MTLBuffers.

## [4.1.0-preview.6] - 2020-07-27

### Changed

- Static library was built with Xcode 12.0 beta 3 (12A8169g). Please note that target devices supporting environment depth now require iOS 14 beta 3.

### Fixed

- Fixing issue where [`ARPlaneManager.currentDetectionMode`](https://docs.unity3d.com/Packages/com.unity.xr.arfoundation@4.1/api/UnityEngine.XR.ARFoundation.ARPlaneManager.html#UnityEngine_XR_ARFoundation_ARPlaneManager_currentDetectionMode) was always reporting [`PlaneDetectionMode.None`](https://docs.unity3d.com/Packages/com.unity.xr.arsubsystems@4.1/api/UnityEngine.XR.ARSubsystems.PlaneDetectionMode.html).
- Removing a work around preventing environment depth and human segmentation to be simultaneously enabled because the source issue has been resolved with Xcode 12 beta 3 and iOS 14 beta 3.
- Fixed an issue where ARKit shaders could incorrectly remain in the [Preloaded Assets](https://docs.unity3d.com/ScriptReference/PlayerSettings.GetPreloadedAssets.html) array, which could interfere with builds on other platforms. For example, in some cases, if first you built for iOS, and then built for Android, you might have seen an error message like
<pre>
Shader error in 'Unlit/ARKitBackground': failed to open source file: 'Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl' at line 44 (on gles3)
</pre>
- Fixed crash on startup caused by devices that support iOS 11 but not ARKit.

## [4.1.0-preview.5] - 2020-07-16

### Changed

- Updated the CBUFFER name containing the UnityDisplayTransform matrix from AR Foundation to avoid collision.
- The ARKit static library is no longer part of the generated Xcode project when the ARKit loader is disabled in the ARKit settings in the `XR Plug-in Management` UI. Added default implementations of calls to native code when this static library is excluded so that linker errors do not occur.
- Updated the ARKit meshing documentation.
- Static library was built with Xcode 12.0 beta 2 (12A6163b).

### Fixed

- When using the Universal Rendering Pipeline, the background rendering used the graphics setting's render pipeline asset instead of the current render pipeline. This meant that render pipeline assets assigned to a quality level would not be respected. This has been fixed.

## [4.1.0-preview.3] - 2020-07-09

### Changed

- Static library was built with Xcode 12.0 beta 2 (12A6163b)
- Update [XR Plug-in Management](https://docs.unity3d.com/Packages/com.unity.xr.management@3.2) to 3.2.13.

### Fixed

- Updated the `ARObjectImporter` to account for a namespace which changed from `UnityEditor.Experimental.AssetImporters` to `UnityEditor.AssetImporters` in 2020.2.0a17.
- Fixed a crash when using [mesh face classifications](https://docs.unity3d.com/Packages/com.unity.xr.arkit@4.1/api/UnityEngine.XR.ARKit.ARKitMeshSubsystemExtensions.html#UnityEngine_XR_ARKit_ARKitMeshSubsystemExtensions_GetFaceClassifications_XRMeshSubsystem_TrackableId_Allocator_).
- Minor documentation fixes.

## [4.1.0-preview.2] - 2020-06-24

### Changed

- Adding documentation for the ARKit meshing and occlusion functionalities.

### Fixed

- Added a workaround for an issue where environment depth and human stencil & depth do not work together as expected. This workaround uses the OcclusionPreferenceMode setting to decide which mode should be enabled. This workaround will be removed once the original issue is fixed.
- Fixed a bug which caused the `ARKitRaycastSubsystem` to throw on devices running versions of iOS prior to 13. This would most commonly be seen when using ARFoundation's [ARRaycastManager](https://docs.unity3d.com/Packages/com.unity.xr.arfoundation@4.1/api/UnityEngine.XR.ARFoundation.ARRaycastManager.html).

## [4.1.0-preview.1] - 2020-06-23

### Added

- Add support for ARKit environment depth through the AROcclusionManager/XROcclusionSubsystem.

### Changed

- Minimum Unity version for this package is now 2019.4.
- Static library was built with Xcode 12.0 beta (12A6159).

## [4.0.1] - 2020-05-27

### Added

- Added ARKit Camera Grain exposure support for iOS 13 and above, (Support to convert to Texture3D only available in Unity 2020.2 and above). Can be applied to 3D content to give a camera grain noise effect. See [Camera Grain Documentation](https://developer.apple.com/documentation/arkit/arframe/3255173-cameragraintexture) for more details.
- Implemented `XROcclusionSubsystem.TryAcquireHumanStencilCpuImage` and `XROcclusionSubsystem.TryAcquireHumanDepthCpuImage` which provides access to the raw texture data on the CPU.

### Changed

- Updating dependency on AR Subsystems to 4.0.0.
- Added ARKit Camera Grain exposure support for iOS 13 and above, (Support to convert to Texture3D only available in Unity 2020.2 and above). Can be applied to 3D content to give a camera grain noise effect. See [Camera Grain Documentation](https://developer.apple.com/documentation/arkit/arframe/3255173-cameragraintexture) for more details.
- Removed support for Xcode versions below version 11.0 as per apple app store submission guidelines.  [See App Store submission guidelines for more information](https://developer.apple.com/app-store/submissions)
- Updated "camera image" APIs to use the new "CPU image" API. See the [ARFoundation migration guide](https://docs.unity3d.com/Packages/com.unity.xr.arfoundation@4.0/manual/migration-guide-3.html#xrcameraimage-is-now-xrcpuimage) for more details.
- Removed support for Xcode versions below version 11.0 as per apple app store submission guidelines.  [See App Store submission guidelines for more information](https://developer.apple.com/app-store/submissions)
- Previously, the trackable id associated with a point cloud was tied to the `XRDepthSubsystem`, and would only change if the subsystem was recreated. Now, the trackable id is tied to the session and will change if the session is recreated or reset. As before, there is only ever one point cloud.
- ARKitLoader now manages the initialization and destruction of its `XRMeshSubsystem`. This means that using ARKit with [XR Management](https://docs.unity3d.com/Packages/com.unity.xr.management@3.1/manual/index.html) with try to initialize (but not start) the meshing subsystem.
- The ARKit Settings has been moved from Project Settings > XR to Project Settings > XR Plug-in Management for consistency with other XR platforms. See [ARKitSettings](https://docs.unity3d.com/Packages/com.unity.xr.arkit@4.0/api/UnityEditor.XR.ARKit.ARKitSettings.html?q=ARKitSettings) for more information. [Apple ARKit Face Tracking XR Plug-in](https://docs.unity3d.com/Packages/com.unity.xr.arkit-face-tracking@4.0/manual/index.html) package is now installable through this menu. ARKit build settings will no longer have to be created manually, installing the package will automatically create ARKit settings.

### Fixed

- Fixed a bug where the [`XRParticipantSubsystem`](https://docs.unity3d.com/Packages/com.unity.xr.arsubsystems@4.0/api/UnityEngine.XR.ARSubsystems.XRParticipantSubsystem.html) was never initialized and was therefore unavailable.

## [4.0.0-preview.3] - 2020-05-04

### Added

- Add support for tracked (i.e., persistent) raycasts.
- Added support for scene mesh generation through `ARMeshManager`.

### Changed

- Updating dependency on com.unity.xr.management to 3.2.10.

### Fixed

- Fixed all broken or missing scripting API documentation.

## [4.0.0-preview.1] - 2020-03-11

### Changed

- The ARSubsystem implementions have been updated to reflect changes in the ARSubsystems API.
- See the [ARFoundation Migration Guide](https://docs.unity3d.com/Packages/com.unity.xr.arfoundation@4.0/manual/migration-guide-3.html) for more details.

### Changed

- `ARKitSessionSubsystem.collaborationEnabled` was both gettable and settable; now it is only gettable. To toggle the collaboration feature, set `ARKitSessionSubsystem.collaborationRequested` instead.
- The static library `UnityARKit.a` has been prefixed with `lib` to follow library naming conventions. Existing projects will need to either rebuild (Build and Run > Replace) the Unity generated Xcode project or manully remove `UnityARKit.a` from the `UnityFramework` "Frameworks and Libraries" section in Xcode.

## [3.1.3] - 2020-04-13

### Fixed

- Combines the three background shaders for different rendering pipelines into one shader file with variations. This eliminates compiler errors that started with Unity 2020.1.

## [3.1.0-preview.8] - 2020-03-12

No changes

## [3.1.0-preview.7] - 2020-03-03

### Fixed

- When auto focus was set to "Fixed" when starting an AR session, the iOS device would still perform auto focus. This has been fixed.
- The estimated main light direction was Y flipped, e.g., it pointed up when it should be pointing down. This has been fixed.
- Patched a memory leak by removing the coaching overly view from the superview.

## [3.1.0-preview.6] - 2020-02-18

No changes

## [3.1.0-preview.4] - 2020-01-08

### Fixed

- Fixes an issue with the 2D joint positions from human body tracking.

## [3.1.0-preview.2] - 2019-12-16

### Added

- Added support for HDR Light Estimation.  HDR Light Estimation only functions during Face-Tracking on ARKit.
- Exposed native camera configuration object by surfacing the object pointer to the managed ARSubsystems.

### Fixed

- Correcting the static library meta files that get corrupted when upgrading a project to Unity 2019.3.
- Fixing an issue where changing the AROcclusionManager.humanSegmentationStencilMode at runtime would sometime have no effect on the ARKit platform.
- Update documentation links to point to 3.1.
- Updating dependent version of com.unity.xr.management package to eliminate build warning message.

## [3.1.0-preview.1] - 2019-11-21

### Added

- Added `ARKitXROcclusionSubsystem` for managing occlusion textures, such as the human segmentation stencil and human segmentation depth on some iOS devices.

## [3.0.4] - 2020-04-08

### Changed

- Default ARKit loader for XR Management will now only start and stop the implementations of XRInputSubsystem, XRCameraSubsystem, and XRSessionSubsystem when the _Initialize on Startup_ option in XR Management is enabled.
- Static libraries were built with Xcode 11.3.1 (11C505) and Xcode 10.3 (10G8)

### Fixed

- Adding a minimum version restriction to the com.unity.inputsystem package for the conditional code that depends on that package.
- When auto focus was set to "Fixed" when starting an AR session, the iOS device would still perform auto focus. This has been fixed.
- Patched a memory leak by removing the coaching overly view from the superview.
- Fixed a crash that could occur when multithreaded rendering was enabled and [Stop](https://docs.unity3d.com/Packages/com.unity.xr.arsubsystems@4.0/api/UnityEngine.XR.ARSubsystems.XRSubsystem-1.html#UnityEngine_XR_ARSubsystems_XRSubsystem_1_Stop) was called on the `XRCameraSubsystem`. In ARFoundation, this happens when the [`ARCameraManager`](https://docs.unity3d.com/Packages/com.unity.xr.arfoundation@3.1/api/UnityEngine.XR.ARFoundation.ARCameraManager.html) is disabled. This happened because the textures owned by the subsystem are later manipulated on the render thread, and stopping the subsystem could invalidate the textures.

## [3.0.1] - 2019-11-27

### Changed

- 2020.1 verified release

### Fixed

- Correcting script compilation error when the com.unity.inputsystem package is also included in the project.

## [3.0.0] - 2019-11-05

### Changed

- Renamed the concept of `Reference Points` to `Anchors`. Public API changes are in `AR Foundation` and `AR Subsystems` packages.

## [3.0.0-preview.4] - 2019-10-22

### Added

- Add getter for the camera focus mode.
- Add support for plane classification for devices running iOS 12 with A12 CPU or later.

### Changed

- Static libraries were built with Xcode 11.1 (11A1027) and Xcode 10.3 (10G8)

### Fixed

- Allow building on non-macOS platforms. When building for iOS, this package determines which version of Xcode is selected in the Build Settings and enables the appropriate native library. This requires that Xcode be installed, which is not possible on non-macOS platforms. In this case, the library built with Xcode 11 is used.
- Fixed reporting of tracking state for ARHumanBodies. Previously, the tracking state was always `TrackingState.Tracking`. Now, the tracking state will change to `TrackingState.None` when the person is no longer being tracked.

## [3.0.0-preview.3] - 2019-09-26

### Added

- Added support for both linear and gamma color spaces.
- Register AR tracking inputs with the new [Input System](https://github.com/Unity-Technologies/InputSystem)

### Changed

- Build compiled binaries with Xcode 10.3 (10G8) and Xcode 11 (11A420a)

### Fixed

- Exclude tvOS as a supported platform.
- The ["match frame rate"](https://docs.unity3d.com/Packages/com.unity.xr.arfoundation@3.1/api/UnityEngine.XR.ARFoundation.ARSession.html#UnityEngine_XR_ARFoundation_ARSession_matchFrameRate) option could incorrectly cause execution to be blocked while waiting for a new frame, leading to long frame times. This has been fixed.
- The ["match frame rate"](https://docs.unity3d.com/Packages/com.unity.xr.arfoundation@3.1/api/UnityEngine.XR.ARFoundation.ARSession.html#UnityEngine_XR_ARFoundation_ARSession_matchFrameRate) option did not account for thermal throttling, which can put ARKit into a 30 frames per second update mode while Unity would keep trying to update at 60 frames per second. This could lead to visual artifacts like judder. The calculated frame rate now takes the thermal state into account and will do a better job matching ARKit's update rate.

## [3.0.0-preview.2] - 2019-09-05

### Added

- Added support for [ARCoachingOverlayView](https://developer.apple.com/documentation/arkit/arcoachingoverlayview)
- Added tracking input support for the [Input System](https://github.com/Unity-Technologies/InputSystem)

### Changed

- `ARKitSessionSubsystem.worldMapSupported` was previously an instance method; now it is a `static` method as it does not require an instance to perform this check.

### Fixed

- 3.0.0-preview.1 was not compatible with some older versions of Xcode. This has been fixed.

## [3.0.0-preview.1] - 2019-08-27

### Added

- Add support for [XR Management](https://docs.unity3d.com/Packages/com.unity.xr.management@3.1/manual/index.html).
- Add support for the [XRParticipantSubsystem](https://docs.unity3d.com/Packages/com.unity.xr.arsubsystems@3.1/manual/participant-subsystem.html), which can track other users in a multi-user collaborative session.
- Add support for [exposureDuration](https://developer.apple.com/documentation/arkit/arcamera/3182986-exposureduration?language=objc)
- Add support for [exposureOffset](https://developer.apple.com/documentation/arkit/arcamera/3194569-exposureoffset?language=objc)
- Add support for Lightweight Render Pipeline and Universal Render Pipeline.
- Add support for height scale estimatation for the 3D human body subsystem.
- This package now supports bulding with Xcode ~~9,~~ 10 and 11 beta 7.

### Fixed

- Enforce minimum target iOS version of 11.0 whenever ARKit is required.
- Setting the `ARHumanBodyManager.humanSegmentationDepthMode` value to either `HalfScreenResolution` or `FullScreenResolution` resulted in an invalid human segmentation depth image. This has been fixed.

## [2.2.0-preview.4] - 2019-07-29

### Fixed

- Update ARKit 3 compatibility for Xcode 11 beta 5.

## [2.2.0-preview.3] - 2019-07-18

### Fixed

- Update ARKit 3 compatibility for Xcode 11 beta 4.

## [2.2.0-preview.2] - 2019-07-16

### Added

- Add support for `NotTrackingReason`.
- Add support for matching the ARCore framerate with the Unity one. See `XRSessionSubsystem.matchFrameRate`.
- Expose the [priority](https://docs.unity3d.com/Packages/com.unity.xr.arkit@2.2/api/UnityEngine.XR.ARKit.ARCollaborationData.html#UnityEngine_XR_ARKit_ARCollaborationData_priority) property on the `ARCollaborationData`.
- Add support for getting the ambient light intensity in lumens.

### Fixed

- Update ARKit 3 compatibility for Xcode 11 beta 3. This fixes
  - [Collaborative sessions](https://docs.unity3d.com/Packages/com.unity.xr.arkit@2.2/api/UnityEngine.XR.ARKit.ARCollaborationData.html)
  - Human body tracking

## [2.2.0-preview.1] - 2019-06-05

### Added

- Adding support for ARKit 3 functionality: Human pose estimation, human segmentation images, session collaboration, multiple face tracking, and tracking a face (with front camera) while in world tracking (with rear camera).

## [2.1.0-preview.6] - 2019-06-03

### Fixed

- Use relative paths for Xcode asset catalogs. This allows the generated Xcode project to be moved to a different directory, or even a different machine. Previously, we used full paths, which prevented this.
- Conditionally compile subsystem registrations. This means the subsystems wont't register themselves in the Editor (and won't generate warnings if there are other subsystems for other platforms).

## [2.1.0-preview.5] - 2019-05-21

### Fixed

- Fix documentation links
- Fix iOS version number parsing. This caused
  - Editor Play Mode exceptions (trying to parse a desktop OS string)
  - Incorrect handling of iOS point releases (e.g., 12.1.3)

## [2.1.0-preview.3] - 2019-05-14

### Added

- Add [image tracking](https://developer.apple.com/documentation/arkit/recognizing_images_in_an_ar_experience) support.
- Add [environment probe](https://developer.apple.com/documentation/arkit/adding_realistic_reflections_to_an_ar_experience) support.
- Add [face tracking](https://developer.apple.com/documentation/arkit/creating_face-based_ar_experiences) support.
- Add [object tracking](https://developer.apple.com/documentation/arkit/scanning_and_detecting_3d_objects) support.

## [1.0.0-preview.23] - 2019-01-04

### Added

- Support the `CameraIntrinsics` API in ARExtensions.

### Fixed

- Refactor the way ARKit face tracking is in the build. Face tracking has been moved to a separate static lib so that it can be removed from the build when face tracking is not enabled. This was preventing apps from passing App Store validation, as face tracking types may not appear in the binary unless you include a privacy policy describing to users how you intend to use face tracking and face data.

### Fixed

- Fixed linker errors when linking `UnityARKit.a` with Xcode 9.x

## [1.0.0-preview.20] - 2018-12-13

### Fixed

- Fix package dependency.

## [1.0.0-preview.19] - 2018-12-13

### Added

- Add C header file necessary to interpret native pointers. See `Includes~/UnityXRNativePtrs.h`
- Add support for setting the camera focus mode.
- Add a build check to ensure only ARM64 is selected as the only target architecture.
- Implement `CameraConfiguration` support, allowing you to enumerate and set the resolution used by the hardware camera.
- Added Apple ARKit Face Tracking support via `com.unity.xr.facesubsystem`.
- Plane detection modes: Add ability to selectively enable detection for horizontal, vertical, or both types of planes.

## [1.0.0-preview.17] - 2018-10-06

### Added

- Add support for native pointer access for several ARSession-related native objects.
- Add [ARWorldMap](https://developer.apple.com/documentation/arkit/arworldmap) support.
- Add linker validation when building with the IL2CPP scripting backend to avoid stripping the Unity.XR.ARKit assembly.

### Fixed

- Fixed an issue where toggling plane detection or light estimation would momentarily pause the ARSession, causing tracking to become temporarily unstable.
- Fixed the (new) CameraImage API to work with the 2018.3 betas.
- ARKit's `ARTrackingStateLimited` was reported as `TrackingState.Tracking`. It is now reported as `TrackingState.Unavailable`.

## [1.0.0-preview.16] - 2018-10-10

### Added

- Added support for `XRCameraExtensions` API to get the raw camera image data on the CPU. See the [ARFoundation manual documentation](https://docs.unity3d.com/Packages/com.unity.xr.arfoundation@1.0/manual/cpu-camera-image.html) for more information.

## [1.0.0-preview.15] - 2018-09-18

### Fixed

- Fix memory leak when destroying the ARSession.

## [1.0.0-preview.14] - 2018-08-10

### Added

- Add a pre build check to make sure Metal is the first selected Graphics API in Player Settings.

### Changed

- Remove restriction on symlinking Unity libraries in Build Settings if using Unity 2018.3 or newer.

### Fixed

- Change plug-in entry point in UnityARKit.a to avoid name collisions with other libraries (was `UnityPluginLoad`).

## [1.0.0-preview.13] - 2018-07-17

### Changed

- Update plug-in to be compatible with Unity 2018.3

### Changed

- `ARPlane.trackingState` reports the session `TrackingState` for ARKit planes (previously it returned `TrackingState.Unknown`). ARKit planes do not have per-plane tracking states, so if they exist and the session is tracking, then the SDK will now report that the planes are tracked.

## [1.0.0-preview.12] - 2018-06-20

### Added

- Add -fembed-bitcode flag to UnityARKit.a to support archiving.

### Changed

- Fail the build if "Symlink Unity libraries" is checked.

## [1.0.0-preview.11] - 2018-06-14

### Added

- Add support for reference points attached to planes
-Created a Legacy XRInput interface to automate the switch between 2018.1 and 2018.2 XRInput versions.
- Availability check to determine runtime support for ARKit.
- Normalize average brightness reading from 0..1
This is the first release of the Apple ARKit package for multi-platform AR.

In this release we are shipping a working iteration of the Apple ARKit package for
Unity's native multi-platform AR support.
Included in the package are static libraries, configuration files, binaries
and project files needed to adapt ARKit to the Unity multi-platform AR API.

### Changed

- Fail the build if Camera Usage Description is blank
- Do not include build postprocessor when not on iOS

### Removed

- Remove extraneous debug log


<hr>
\* *Apple and ARKit are trademarks of Apple Inc., registered in the U.S. and other countries and regions.*

