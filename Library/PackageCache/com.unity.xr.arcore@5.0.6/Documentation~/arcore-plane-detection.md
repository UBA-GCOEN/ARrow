---
uid: arcore-plane-detection
---
# Plane tracking

ARCore supports plane subsumption. This means that you can include a plane inside another. Unity keeps the included (subsumed) plane and doesn't update it.

ARCore provides boundary points for all its planes.

The ARCore plane subsystem requires additional CPU resources and can use a lot of energy. The horizontal and vertical plane detection require additional resources when enabled. To save energy, disable plane detection when your app doesn't need it.

Setting the plane detection mode to `PlaneDetectionMode.None` works in the same way as using `Stop` for a subsystem.

For more information, see AR Foundation [Plane detection](xref:arfoundation-plane-detection).
