using UnityEngine;

namespace Unity.MagicLeap.Samples.Rendering
{
    /// <summary>
    /// MonoBehaviour for the Stabilization component
    /// This component is used as a part of the 'Furthest Object' implementation of `stabilizationMode`. Users can use the two to
    /// explicitly define which objects are considered for determining the 'furthest object' in the scene.
    /// </summary>
    public sealed class StabilizationComponent : MonoBehaviour
    {
        void OnBecameInvisible()
        {
            enabled = false;
        }
        void OnBecameVisible()
        {
            enabled = true;
        }
        void Update()
        {
            // normally, we'd want to cache the camera reference to save a couple cycles,
            // but since it can change, we need to sample it every frame.
            var camera = Camera.main;
            if (camera)
                camera.SendMessage("UpdateTransformList", transform);
        }
    }
}
