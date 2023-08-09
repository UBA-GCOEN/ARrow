---
uid: arfoundation-occlusion
---
# Occlusion

Some devices offer depth information about the real world. For instance, with a feature known as person occlusion, iOS devices with the A12 Bionic chip (and newer) provide depth information for humans detected in the AR Camera frame. Newer Android phones and iOS devices equipped with a LiDAR scanner can provide an environment depth image where each pixel contains a depth estimate between the device and physical surroundings.

Adding the [AROcclusionManager](xref:UnityEngine.XR.ARFoundation.AROcclusionManager) component to the Camera with the [ARCameraBackground](xref:UnityEngine.XR.ARFoundation.ARCameraBackground) component automatically enables the background rendering pass to incorporate any available depth information when rendering the depth buffer. This allows for rendered geometry to be occluded by detected geometry from the real world. For example, in the case of iOS devices that support person occlusion, detected humans occlude rendered content that exists behind them.

# AR Occlusion Manager component

The [AROcclusionManager](xref:UnityEngine.XR.ARFoundation.AROcclusionManager) component exposes per-frame images representing depth or stencil images. Incorporating these depth images into the rendering process are often the best approach for achieving realistic-looking blending of augmented and real-world content by making sure that nearby physical objects occlude virtual content that is located behind them in the shared AR space.

The types of depth images supported are:
- **Environment Depth**: distance from the device to any part of the environment in the camera field of view.
- **Human Depth**: distance from the device to any part of a human recognized within the camera field of view.
- **Human Stencil**: value designating, for each pixel, whether that pixel contains a recognized human.

The scripting interface allows for:
- Enabling, disabling, and quality configuration for the various supported depth images.
- Querying the availability of each type of depth image.
- Access to the depth image data.
