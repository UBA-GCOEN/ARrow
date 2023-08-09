#if USE_LEGACY_INPUT_HELPERS

using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Experimental.XR.Interaction;
#if LIH_2_OR_NEWER
using UnityEngine.SpatialTracking;
#endif
using UnityEngine.XR;
#if UNITY_ANDROID
using UnityEngine.XR.MagicLeap;
#endif

namespace UnityEngine.XR.MagicLeap.Samples
{
    [AddComponentMenu("AR/Magic Leap/Samples/XR Input Hand Provider")]
    public class XRInputHandProvider : BasePoseProvider
    {
        public enum Hand
        {
            Left,
            Right
        }

        public Hand hand = Hand.Left;

#if UNITY_ANDROID
        private InputDeviceCharacteristics m_Characteristics => InputDeviceCharacteristics.HandTracking | ((hand == Hand.Left) ? InputDeviceCharacteristics.Left : InputDeviceCharacteristics.Right);

        public bool TryGetHandDevice(out InputDevice device)
        {
            device = default(InputDevice);

            List<InputDevice> devices = new List<InputDevice>();
            InputDevices.GetDevicesWithCharacteristics(m_Characteristics, devices);
            if (devices.Count == 0)
            {
                MagicLeapLogger.Debug("XRInputHandProvider", "Unable to find hand tracking device!");
                return false;
            }
            device = devices.First();
            return true;
        }

#if LIH_2_OR_NEWER
        public override PoseDataFlags GetPoseFromProvider(out Pose output)
        {
            output = default(Pose);
            var result = PoseDataFlags.NoData;

            if (!TryGetHandDevice(out var device) || !device.isValid)
                return result;

            if (device.TryGetFeatureValue(CommonUsages.devicePosition, out output.position))
                result = result | PoseDataFlags.Position;
            if (device.TryGetFeatureValue(CommonUsages.deviceRotation, out output.rotation))
                result = result | PoseDataFlags.Rotation;
            return result;
        }
#else
        public override bool TryGetPoseFromProvider(out Pose output)
        {
            output = default(Pose);
            var result = false;

            if (!TryGetHandDevice(out var device) || !device.isValid)
                return result;

            if (device.TryGetFeatureValue(CommonUsages.devicePosition, out output.position))
                result = true;
            if (device.TryGetFeatureValue(CommonUsages.deviceRotation, out output.rotation))
                result = true;
            return result;
        }
#endif // LIH_2_OR_NEWER
#else// UNITY_ANDROID
#if LIH_2_OR_NEWER
        public override PoseDataFlags GetPoseFromProvider(out Pose output)
        {
            output = new Pose();
            return (PoseDataFlags)0;
        }
#else
        public override bool TryGetPoseFromProvider(out Pose output)
        {
            output = new Pose();
            return false;
        }
#endif
#endif// UNITY_ANDROID
    }
}

#endif