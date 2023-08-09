---
uid: magic-leap-home
---

# About Magic Leap XR Support

Use the *Magic Leap XR Support* package to enable Magic Leap via Unity's multi-platform XR SDK. This package implements the following XR Subsystems:

* [Display](xref:UnityEngine.XR.XRDisplaySubsystem)
* [Input](xref:UnityEngine.XR.XRInputSubsystem)
* [Meshing](xref:UnityEngine.XR.XRMeshingSubsystem)

This version of *Magic Leap XR Support* supports the meshing functionality provided by the Magic Leap 2:

* Generate meshes of the environment
* Generate point clouds of the environment

# Installing Magic Leap XR Support

This topic only discusses how to install the individual *Magic Leap XR Support* package. Follow these instructions when you need to install or reinstall this package on its own. This process does not install the Magic Leap SDK package or the other tools provided by Magic Leap. To set up the full Magic Leap 2 development environment, refer to the [Magic Leap 2 Unity Getting Started Guide](https://developer-docs.magicleap.cloud/docs/guides/unity/getting-started/unity-getting-started) on the Magic Leap Developer Portal. 

To install this package:

1. Open the project that you plan to use (in Unity 2022.2+).
2. Click the following link to begin the installation: [com.unity.xr.magicleap](com.unity3d.kharma:upmpackage/com.unity.xr.magicleap).

   The Unity Package Manager window opens with the package name entered in the **Add package by name** dialog.

   ![](images/install.png)

3. (Optional) Enter the full version number, such as `7.0.0-exp.3`, to install. If left blank, Unity chooses the "best" version -- typically the latest, release version for the current Unity Editor.  See the [Changelog](xref:magic-leap-changelog) for a list of versions available at the time this documentation page was published.
4. Click **Add**.

> [!NOTE]
> Magic Leap 2 runs on the Android operating system. If you don't already have the Unity Android platform module installed, then you must use the Unity Hub to install it for the current Unity Editor. See [Add modules](https://docs.unity3d.com/hub/manual/AddModules.html) in the Unity Hub documentation for instructions. 

| Additional resources | Description | 
| :------------------- | :---------- |
| [Adding a registry package by name](xref:upm-ui-quick) | More detailed instructions for adding a package by name. |
| [The Package Manager tutorial](https://learn.unity.com/tutorial/the-package-manager#) | An overview of installing and managing packages in Unity. |
| [Package Manager](https://docs.unity3d.com/Packages/com.unity.package-manager-ui@latest/index.html) | Detailed documentation about the Package Manager in the Unity Manual. |
| [Magic Leap Unity Development](https://developer-docs.magicleap.cloud/docs/category/unity) | Unity documentation maintained by the Magic Leap Developer Portal. ||


# Using Magic Leap XR Support

The *Magic Leap XR Support* implements the native endpoints required for meshing using Unity's multi-platform XR API.

Inclusion of the *Magic Leap XR Support* will result in the inclusion of a C# component, runtime libraries, and plugin metadata.

## Spatial Mapper

This package includes the `MLSpatialMapper` component:

![](images/mlspatialmapper_component.png "MLSpatialMapper Component")

The spatial mapper generates [`Mesh`](xref:UnityEngine.Mesh) objects from data collected from the Magic Leap 2's depth sensor. Each mesh is a relatively small area of the environment. A separate [`GameObject`](xref:UnityEngine.GameObject) is created for each generated mesh.

|Field|Description|
|-|-|
|Mesh Prefab|This is the prefab that will be instantiated for each generated mesh. The prefab should at least have a [`MeshFilter`](xref:UnityEngine.MeshFilter) on it. To visualize the meshes, add a [`MeshRenderer`](xref:UnityEngine.MeshRenderer). If a [`MeshCollider`](xref:UnityEngine.MeshCollider) is present, then a physics collider will be generated as well. This happens on a background thread, so it will not stall the main thread.|
|Compute Normals|If `true`, will request that the device also generate a normal for each vertex. If `false`, normals will be calculated from the triangle data.|
|Mesh Parent|The parent transform for the generated meshes. By default, the `MLSpatialMapper` will select its own parent, so that the generated meshes will be siblings of the `MLSpatialMapper`'s [`GameObject`](xref:UnityEngine.GameObject).|
|Type|Whether to generate triangle meshes or a point cloud. If you select point cloud, then the resulting [`Mesh`](xref:UnityEngine.Mesh) will have [`MeshTopology.Points`](xref:UnityEngine.MeshTopology.Points).
|Level of Detail (Deprecated)|The detail of the generated meshes. Lower levels of detail will result in simplified meshes, and will reduce CPU and GPU load, but will be less accurate. Higher LOD levels will be more accurate, but require more CPU and GPU resources. This property is deprecated, and has been replaced by density.|
|Mesh Density|Controls the size and spacing of triangles in meshes. The scale goes from low density at 0 (big triangles) to high density at 1 (small triangles). Higher density meshes require more CPU and GPU resources.|
|Mesh Queue Size|The number of concurrent meshes to generate. Each frame, the `MLSpatialMapper` will add meshes to a generation queue as necessary. Larger values will lead to heavier CPU usage, but will generate meshes faster.|
|Fill Hole Length|Perimeter (in meters) of holes you wish to have filled.|
|Polling Rate|How often to check for updates, in seconds. More frequent updates will increase CPU usage.|
|Batch Size|Maximum number of meshes to update per batch. Larger values are more efficient, but have higher latency.|
|Planarize|If `true`, the system will planarize the returned mesh (planar regions will be smoothed out).|
|Disconnected Component Area|Any component that is disconnected from the main mesh and which has an area (in meters squared) less than this size will be removed.|
|Request Vertex Confidence|If `true`, the system will compute the confidence values for each vertex, ranging from 0 to 1.|
|Remove Mesh Skirt|If `true`, the mesh skirt (overlapping area between two meshes) will be removed.|

### Bounds

Meshes are only generated inside a specific region, relative to the Magic Leap 2's starting position. The `MLSpatialMapper`'s [`transform`](xref:UnityEngine.Transform) is used to define this region. The [`transform`](xref:UnityEngine.Transform)'s `localPosition`, `localRotation`, and `localScale` are used for this calculation.

A green, transparent box is drawn in Unity's Scene View to indicate the area in which meshing will occur:

![](images/meshing_gizmo.png "Meshing Gizmo")

## Spaces

Meshes are generated in "device space", also know as "session relative space". When the Magic Leap 2 boots up, its initial position is `(0, 0, 0)`. All meshing data is relative to this initial location.

The [`Camera`](xref:UnityEngine.Camera)'s [`GameObject`](xref:UnityEngine.GameObject) should have an identity transform to begin with.

![](images/magic_leap_root_camera.png "Magic Leap Root Camera")

If you wish to have the device start at a different location within the Unity scene, we recommend you use a parent [`GameObject`](xref:UnityEngine.GameObject) to provide position, rotation, and scale (the `Magic Leap Root` in this image):

![](images/magic_leap_root_with_transform.png "Magic Leap Root")

## Gesture

Subsystem implementation to provide for recognition and tracking of gestures provided from the appropriate device.  This subsystem relies on the `com.unity.xr.interactionsubsystem` package for it's core implementation (see that package's [documentation](https://docs.unity3d.com/Packages/com.unity.xr.interactionsubsystems@latest) for further details/types).

The `MagicLeapGestureSubsystem` component manages a low-level interface for polling for Magic Leap gesture changes.  If this component is added to a scene, it is possible to poll for any gesture events each frame.  The following gestures and gesture data are provided:

* __MagicLeapKeyPoseGestureEvent__ - Event that fires when a key pose gesture changes.  See the Magic Leap documentation for further documentation on key poses.
  * __id__ - Unique `GestureId` that identifies this gesture.
  * __state__ - `GestureState` that indicates the state of this gesture (`Started`, `Updated`, `Completed`, `Canceled` or `Discrete`).
  * __keyPose__ - `MagicLeapKeyPose` indicating type of key pose that has been detected.  Valid key poses are `Finger`, `Fist`, `Pinch`, `Thumb`, `LShape`, `OpenHand`, `Ok`, `CShape`, `NoPose`, `NoHand` are all valid key poses.
  * __hand__ - `MagicLeapHand` indicating the hand source for this gesture (`Left` or `Right`);
* __MagicLeapTouchpadGestureEvent__ - Event that fires when a touchpad gesture changes.  See the Magic Leap documentation for further documentation on the touchpad.
  * __id__ - Unique `GestureId` that identifies this gesture.
  * __state__ - `GestureState` that indicates the state of this gesture (`Started`, `Updated`, `Completed`, `Canceled` or `Discrete`).
  * __controllerId__ - The controller id associated with this gesture.
  * __angle__ - Angle from the center of the touchpad to the finger.
  * __direction__ - `MagicLeapTouchpadGestureDirection` indicating the direction of this gesture (`None`, `Up`, `Down`, `Left`, `Right`, `Clockwise`, `CounterClockwise`).
  * __positionAndForce__ - Gesture position (x,y) and force (z). Position is in the [-1.0,1.0] range and force is in the [0.0,1.0] range.
  * __radius__ - For radial gestures, this is the radius of the gesture. The touchpad is defined as having extents of [-1.0,1.0] so this radius has a range of [0.0,2.0].
  * __speed__ - Speed of gesture. Note that this takes on different meanings depending on the gesture type being performed:
  * __type__ - `MagicLeapInputControllerTouchpadGestureType` indicating the type of this gesture (`None`, `Tap`, `ForceTapDown`, `ForceTapUp`, `ForceDwell`, `SecondForceDown`, `RadialScroll`, `Swipe`).

Additionally, the `MagicLeapGestures` component can be used to provide a simpler polling mechanism for a simpler, event-based interface for listening to gestures.  This component provides a number of events that be hooked into to detect gesture events when they occur:

* __onKeyPoseGestureChanged__ - Occurs whenever a key pose gesture changes state.
* __onTouchpadGestureChanged__ - Occurs whenever a touchpad gesture changes state.
* __onActivate__ - Occurs whenever the cross-platform activate gesture occurs.  See the [com.unity.xr.interactionsubsystems](https://docs.unity3d.com/Packages/com.unity.xr.interactionsubsystems@latest) package documentation for more details.

Also see the relevant Magic Leap documentation about gestures for supported device information.

# Technical details
## Requirements

This version of *Magic Leap XR Support* is compatible with the following versions of the Unity Editor:

* Unity 2022.2+

## Known limitations

No known issues.

## Package contents

This version of *Magic Leap XR Support* includes:

* A shared library which provides implementation of the XR Subsystems listed above
* A plugin metadata file

