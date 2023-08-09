---
uid: arkit-face-tracking
---
# Face tracking

ARKit provides a series of [blend shapes](https://developer.apple.com/documentation/arkit/arfaceanchor/2928251-blendshapes?language=objc) to describe different features of a face. Each blend shape is modulated from 0..1. For example, one blend shape defines how open the mouth is.

## Front facing camera

Face tracking requires the use of the front-facing or "selfie" camera. When the front-facing camera is active, other tracking subsystems like plane tracking or image tracking may not be available. If the rear-facing camera is active, face tracking might not be available.

Different iOS devices support different combinations of features. If you `Start` a subsystem that requires the rear-facing camera, the Apple ARKit package might decide to use the rear-facing camera instead. For more information, see [Camera and Tracking Mode Selection](https://docs.unity3d.com/Packages/com.unity.xr.arfoundation@4.1/manual/migration-guide-3.html#camera-and-tracking-mode-selection).

## Technical details

### Requirements

Face tracking supports devices with Apple Neural Engine in iOS 14 and iPadOS 14 and requires a device with a TrueDepth camera on iOS 13 and iPadOS 13 and earlier. See Apple's [Tracking and Visualizing Faces](https://developer.apple.com/documentation/arkit/content_anchors/tracking_and_visualizing_faces?language=objc) documentation for more information.

### Contents

**Apple ARKit XR Plug-in** includes a static library that provides an implementation of the AR Foundation [Face tracking](xref:arfoundation-face-tracking) feature.

[!include[](snippets/apple-arkit-trademark.md)]
