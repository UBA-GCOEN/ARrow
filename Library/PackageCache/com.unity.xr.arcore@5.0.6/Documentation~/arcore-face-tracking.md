---
uid: arcore-face-tracking
---
# Face tracking

For information about face tracking, see AR Foundation [Face tracking](xref:arfoundation-face-tracking).

The ARCore face subsystem provides face tracking methods that allow access to "regions". Regions are specific to ARCore. ARCore provides access to the following regions that define features on a face:

- Nose tip
- Forehead left
- Forehead right

Each region has a [Pose](xref:UnityEngine.Pose) associated with it. To access face regions, obtain an instance of the [ARCoreFaceSubsystem](xref:UnityEngine.XR.ARCore.ARCoreFaceSubsystem) using the following script:

```csharp
XRFaceSubsystem faceSubsystem = ...
#if UNITY_ANDROID
var arcoreFaceSubsystem = faceSubsystem as ARCoreFaceSubsystem;
if (arcoreFaceSubsystem != null)
{
    var regionData = new NativeArray<ARCoreFaceRegionData>(0, Allocator.Temp);
    arcoreFaceSubsystem.GetRegionPoses(faceId, Allocator.Temp, ref regionData);
    using (regionData)
    {
        foreach (var data in regionData)
        {
            Debug.LogFormat("Region {0} is at {1}", data.region, data.pose);
        }
    }
}
#endif
```
