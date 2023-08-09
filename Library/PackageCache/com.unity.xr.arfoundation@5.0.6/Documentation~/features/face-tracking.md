---
uid: arfoundation-face-tracking
---
# AR Face Manager component

The face manager is a type of [trackable manager](xref:arfoundation-managers#trackables-and-trackable-managers).

![AR Face Manager component](../images/ar-face-manager.png)<br/>*AR Face Manager component*

The face manager creates GameObjects for each face detected in the environment. The system only detects human faces.

In some implementations, face tracking requires a different camera (for example, front-facing vs. rear-facing) and might be incompatible with other features, such as plane or image tracking. Consider disabling other trackable managers.

## Responding to faces

Faces can be added, updated, and removed. Once per frame, if the application detects a face, the AR face manager invokes the `facesChanged` event. This event contains three `List`s of faces that have been added, updated, and removed since the last frame.

When a face is detected, the AR face manager instantiates the Face Prefab to represent the face. The Face Prefab can be left `null`, but the face manager ensures the instantiated `GameObject` has an `ARFace` component on it. The `ARFace` component only contains data about the detected face.

## Visualizing faces

The face provider might provide a mesh that represents the face. The `ARFace` component exposes `vertices`, `normals`, `indices`, and `uvs` (texture coordinates). Some or all of these can be available.

The `ARFaceMeshVisualizer` component generates a `UnityEngine.Mesh` and updates the `MeshFilter` on the same `GameObject` based on the data that the `ARFace` provides.

Check the face subsystem's `SubsystemDescriptor` (`ARFaceManager.descriptor`) for provider-specific features.
