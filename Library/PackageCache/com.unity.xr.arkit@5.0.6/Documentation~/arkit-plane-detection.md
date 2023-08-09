---
uid: arkit-plane-detection
---
# Plane tracking

ARKit doesn't support plane subsumption (that is, one plane can't be included in another plane); there is no merge event. If two planes are determined to be separate parts of the same surface, one plane might be removed while the other expands to the explored surface.

ARKit provides boundary points for all its planes on iOS 11.3 and later.

The ARKit plane subsystem requires additional CPU resources and can be energy-intensive. Enabling both horizontal and vertical plane detection (available in iOS 11.3+) requires additional resources. Consider disabling plane detection when your app doesn't need it to save energy.

Setting the plane detection mode to [PlaneDetectionMode.None](xref:UnityEngine.XR.ARSubsystems.PlaneDetectionMode.None) is equivalent to calling `Stop()` on the subsystem.

For more information, see [Plane detection](xref:arfoundation-plane-detection).
