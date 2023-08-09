---
uid: arkit-object-tracking
---
# Object tracking

To use object tracking on ARKit, you first need to create a reference object library. See [Object tracking](xref:arfoundation-object-tracking) for instructions.

Next, you need to create an ARKit-specific reference object entry. The [Scanning and Detecting 3D Objects](https://developer.apple.com/documentation/arkit/scanning_and_detecting_3d_objects) page on Apple's developer website allows you to download an app that you can use with an iOS device to produce such a scan. Note that this is a third-party application, and Unity is not involved in its development.

The scanning app produces a file with the extension `.arobject`. Drag each `.arobject` file into your Unity project, and Unity generates an `ARKitReferenceObjectEntry` for it.

![Example scan](images/arobject-inspector.png "Example scan")<br/>*Example scan*

You should see some metadata and a preview image of the scan in the Inspector.

You can now add the `.arobject` to a reference object in your Reference Object Library as shown below:

![Example Reference Object Library](images/reference-object-library-inspector.png)<br/>*Example Reference Object Library*

[!include[](snippets/apple-arkit-trademark.md)]
