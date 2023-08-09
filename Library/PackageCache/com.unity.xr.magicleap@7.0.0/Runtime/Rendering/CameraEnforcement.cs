using System;

namespace UnityEngine.XR.MagicLeap.Rendering
{
    /// <summary>
    /// Static class for enforcing camera restrictions.
    /// </summary>
    internal static class CameraEnforcement
    {
        internal static void EnforceCameraProperties()
        {
            if (!RenderingSettings.enforceNearClip || Camera.main == null)
            {
                return;
            }

            ValidateNearClip();

            // If the near clipping distance from MLGraphicsAPI is greater,
            // Set the main camera near clipping plane to this distance and notify the user.
            if (RenderingSettings.nearClipDistance - Camera.main.nearClipPlane > 0.001f)
            {
                Debug.LogWarning("The Camera\'s near clipping plane was set to a value that's beneath hardware limitations. Setting value to:" + RenderingSettings.nearClipDistance.ToString());
                Camera.main.nearClipPlane = RenderingSettings.nearClipDistance;
            }
        }

        /// <summary>
        /// Validate the Near Clip plane
        /// </summary>
        private static void ValidateNearClip()
        {
            float nearClip = Camera.main.nearClipPlane;
            float min = RenderingSettings.minNearClipDistance;

            if (nearClip < min)
            {
                Camera.main.nearClipPlane = min;
            }
        }
    }
}