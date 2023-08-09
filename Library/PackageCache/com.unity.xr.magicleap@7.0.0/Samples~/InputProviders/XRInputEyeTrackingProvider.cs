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
    [AddComponentMenu("AR/Magic Leap/Samples/XR Input Eye Tracking Provider")]
    public class XRInputEyeTrackingProvider : BasePoseProvider
    {
        public enum TrackingInformation
        {
            FixationPoint,
            LeftEye,
            RightEye
        }

        public TrackingInformation trackingInformation;
#if UNITY_ANDROID
#if LIH_2_OR_NEWER
        public override PoseDataFlags GetPoseFromProvider(out Pose output)
        {
            output = default(Pose);
            var device = default(InputDevice);
            var result = PoseDataFlags.NoData;

            List<InputDevice> devices = new List<InputDevice>();
            InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.EyeTracking | InputDeviceCharacteristics.HeadMounted, devices);
            if (devices.Count == 0)
            {
                MagicLeapLogger.Debug("XRInputEyeTrackingProvider", "Unable to find a valid eye tracking device!");
                return result;
            }
            device = devices.First();

            if (!device.isValid)
                return result;

            var eyes = default(Eyes);
            if (!device.TryGetFeatureValue(CommonUsages.eyesData, out eyes))
            {
                MagicLeapLogger.Debug("XRInputEyeTrackingProvider", "Unable to read eye tracking data!");
                return result;
            }

            switch (trackingInformation)
            {
                case TrackingInformation.FixationPoint:
                {
                    if (eyes.TryGetFixationPoint(out output.position))
                        result = result | PoseDataFlags.Position;
                    else
                        MagicLeapLogger.Debug("XRInputEyeTrackingProvider", "Unable to read fixation point!");
                    break;
                }
                case TrackingInformation.LeftEye:
                {
                    if (eyes.TryGetLeftEyePosition(out output.position))
                        result = result | PoseDataFlags.Position;
                    else
                        MagicLeapLogger.Debug("XRInputEyeTrackingProvider", "Unable to read left eye position!");
                    if (eyes.TryGetLeftEyeRotation(out output.rotation))
                        result = result | PoseDataFlags.Rotation;
                    else
                        MagicLeapLogger.Debug("XRInputEyeTrackingProvider", "Unable to read left eye rotation!");
                    break;
                }
                case TrackingInformation.RightEye:
                {
                    if (eyes.TryGetRightEyePosition(out output.position))
                        result = result | PoseDataFlags.Position;
                    else
                        MagicLeapLogger.Debug("XRInputEyeTrackingProvider", "Unable to read right eye position!");
                    if (eyes.TryGetRightEyeRotation(out output.rotation))
                        result = result | PoseDataFlags.Rotation;
                    else
                        MagicLeapLogger.Debug("XRInputEyeTrackingProvider", "Unable to read right eye rotation!");
                    break;
                }
            }

            return result;
        }
#else // LIH_2_OR_NEWER
        public override bool TryGetPoseFromProvider(out Pose output)
        {
            output = default(Pose);
            var device = default(InputDevice);
            var result = false;

            List<InputDevice> devices = new List<InputDevice>();
            InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.EyeTracking | InputDeviceCharacteristics.HeadMounted, devices);
            if (devices.Count == 0)
            {
                MagicLeapLogger.Debug("XRInputEyeTrackingProvider", "Unable to find a valid eye tracking device!");
                return result;
            }
            device = devices.First();

            if (!device.isValid)
                return result;

            var eyes = default(Eyes);
            if (!device.TryGetFeatureValue(CommonUsages.eyesData, out eyes))
            {
                MagicLeapLogger.Debug("XRInputEyeTrackingProvider", "Unable to read eye tracking data!");
                return result;
            }

            switch (trackingInformation)
            {
                case TrackingInformation.FixationPoint:
                {
                    if (eyes.TryGetFixationPoint(out output.position))
                        result = true;
                    else
                        MagicLeapLogger.Debug("XRInputEyeTrackingProvider", "Unable to read fixation point!");
                    break;
                }
                case TrackingInformation.LeftEye:
                {
                    if (eyes.TryGetLeftEyePosition(out output.position))
                        result = true;
                    else
                        MagicLeapLogger.Debug("XRInputEyeTrackingProvider", "Unable to read left eye position!");
                    if (eyes.TryGetLeftEyeRotation(out output.rotation))
                        result = true;
                    else
                        MagicLeapLogger.Debug("XRInputEyeTrackingProvider", "Unable to read left eye rotation!");
                    break;
                }
                case TrackingInformation.RightEye:
                {
                    if (eyes.TryGetRightEyePosition(out output.position))
                        result = true;
                    else
                        MagicLeapLogger.Debug("XRInputEyeTrackingProvider", "Unable to read right eye position!");
                    if (eyes.TryGetRightEyeRotation(out output.rotation))
                        result = true;
                    else
                        MagicLeapLogger.Debug("XRInputEyeTrackingProvider", "Unable to read right eye rotation!");
                    break;
                }
            }

            return result;
        }
#endif // LIH_2_OR_NEWER

        void OnEnable()
        {
            // enable ML Eye tracking if it's not already enabled
            var list = new List<XRInputSubsystem>();
            SubsystemManager.GetInstances(list);
            if (list.Count > 0)
            {
                if (!list[0].IsEyeTrackingApiEnabled())
                {
                    MagicLeapLogger.Debug("XRInputEyeTrackingProvider", "Enabling Eye Tracking!");
                    list[0].SetEyeTrackingApiEnabled(true);
                }
            }
        }
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

#endif // USE_LEGACY_INPUT_HELPERS