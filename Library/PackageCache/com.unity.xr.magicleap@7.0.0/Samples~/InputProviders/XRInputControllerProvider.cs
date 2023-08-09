#if USE_LEGACY_INPUT_HELPERS
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Events;
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
    [Serializable]
    public class FloatEvent : UnityEvent<float>
    {

    }

    [AddComponentMenu("AR/Magic Leap/Samples/XR Input Controller Provider")]
    public class XRInputControllerProvider : BasePoseProvider
    {
        const string kLogTag = "XRInputControllerProvider";
        [Range(0,1)]
        public int controllerIndex;

        [SerializeField]
        private bool m_LogTrackedPosition;

        public UnityEvent bumperPressed;
        public UnityEvent homePressed;
        public UnityEvent triggerPressed;
        public FloatEvent triggerRawValue;

        public bool logTrackedPosition
        {
            get { return m_LogTrackedPosition; }
            set { m_LogTrackedPosition = value; }
        }

#if UNITY_ANDROID
        void Update()
        {
            var device = default(InputDevice);

            List<InputDevice> devices = new List<InputDevice>();
            InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.Controller | InputDeviceCharacteristics.HeldInHand, devices);
            if (devices.Count == 0 && devices.Count <= controllerIndex)
            {
                //MagicLeapLogger.Debug(kLogTag, "Unable to find a valid controller device!");
                return;
            }
            device = devices.ElementAt(controllerIndex);

            if (!device.isValid)
                return;

            if (bumperPressed != null && device.TryGetFeatureValue(CommonUsages.secondaryButton, out var bumper))
            {
                if (bumper)
                    bumperPressed.Invoke();
            }

            if (triggerPressed != null && device.TryGetFeatureValue(CommonUsages.triggerButton, out var trigger))
            {
                if (trigger)
                    triggerPressed.Invoke();
            }

            if (homePressed != null && device.TryGetFeatureValue(CommonUsages.menuButton, out var home))
            {
                if (home)
                    homePressed.Invoke();
            }

            if (triggerRawValue != null && device.TryGetFeatureValue(CommonUsages.trigger, out var triggerRaw))
                triggerRawValue.Invoke(triggerRaw);
        }

#if LIH_2_OR_NEWER
        public override PoseDataFlags GetPoseFromProvider(out Pose output)
        {
            output = default(Pose);
            var device = default(InputDevice);
            var flags = PoseDataFlags.NoData;

            List<InputDevice> devices = new List<InputDevice>();
            InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.Controller | InputDeviceCharacteristics.HeldInHand, devices);
            if (devices.Count == 0 && devices.Count <= controllerIndex)
            {
                MagicLeapLogger.Debug(kLogTag, "Unable to find a valid controller device!");
                return flags;
            }
            device = devices.ElementAt(controllerIndex);

            if (!device.isValid)
                return flags;

            if (device.TryGetFeatureValue(CommonUsages.devicePosition, out output.position))
                flags = flags | PoseDataFlags.Position;
            if (device.TryGetFeatureValue(CommonUsages.deviceRotation, out output.rotation))
                flags = flags | PoseDataFlags.Rotation;

            if (logTrackedPosition)
                MagicLeapLogger.Debug(kLogTag, $"pos: {output.position}, rot: {output.rotation}");

            return flags;
        }
#else
        public override bool TryGetPoseFromProvider(out Pose output)
        {
            output = default(Pose);
            var device = default(InputDevice);
            var flags = false;

            List<InputDevice> devices = new List<InputDevice>();
            InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.Controller | InputDeviceCharacteristics.HeldInHand, devices);
            if (devices.Count == 0 && devices.Count <= controllerIndex)
            {
                MagicLeapLogger.Debug(kLogTag, "Unable to find a valid controller device!");
                return flags;
            }
            device = devices.ElementAt(controllerIndex);

            if (!device.isValid)
                return flags;

            if (device.TryGetFeatureValue(CommonUsages.devicePosition, out output.position))
                flags = true;
            if (device.TryGetFeatureValue(CommonUsages.deviceRotation, out output.rotation))
                flags = true;

            if (logTrackedPosition)
                MagicLeapLogger.Debug(kLogTag, $"pos: {output.position}, rot: {output.rotation}");

            return flags;
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

#endif // USE_LEGACY_INPUT_HELPERS