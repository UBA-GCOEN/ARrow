using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine.XR;

namespace UnityEngine.XR.MagicLeap
{
    /// <summary>
    /// Monobehaviour for a Magic Leap Controller
    /// </summary>
    [AddComponentMenu("AR/Magic Leap/MagicLeap Input")]
    public sealed class MagicLeapInput : MonoBehaviour
    {
    }

    /// <summary>
    /// Input extensions for Magic Leap
    /// </summary>
    public static class MagicLeapInputExtensions
    {
        static class Native
        {
#if UNITY_ANDROID
            const string Library = "UnityMagicLeap";

            [DllImport(Library, EntryPoint="UnityMagicLeap_InputGetControllerTrackerActive")]
            [return: MarshalAs(UnmanagedType.I1)]
            /// <summary>
            /// C# interface to native method.
            /// Check to see if the controller is active
            /// </summary>
            public static extern bool GetControllerActive();

            [DllImport(Library, EntryPoint="UnityMagicLeap_InputSetControllerTrackerActive")]
            /// <summary>
            /// C# interface to native method.
            /// Set the active state of a controller
            /// </summary>
            public static extern void SetControllerActive([MarshalAs(UnmanagedType.I1)]bool value);

            [DllImport(Library, EntryPoint="UnityMagicLeap_InputGetEyeTrackerActive")]
            [return: MarshalAs(UnmanagedType.I1)]
            /// <summary>
            /// C# interface to native method.
            /// Get the active state of the Eye tracker
            /// </summary>
            public static extern bool GetEyeTrackerActive();

            [DllImport(Library, EntryPoint="UnityMagicLeap_InputSetEyeTrackerActive")]
            /// <summary>
            /// C# interface to native method.
            /// Set the active state of the eye tracker
            /// </summary>
            public static extern void SetEyeTrackerActive([MarshalAs(UnmanagedType.I1)]bool value);
#else
            /// <summary>
            /// default, unbound method
            /// Always returns false
            /// </summary>
            public static bool GetControllerActive() => false;
            /// <summary>
            /// default, unbound method
            /// Noop
            /// </summary>
            public static void SetControllerActive(bool value) {}
            /// <summary>
            /// default, unbound method
            /// Always returns false
            /// </summary>
            public static bool GetEyeTrackerActive() => false;
            /// <summary>
            /// default, unbound method
            /// Noop
            /// </summary>
            public static void SetEyeTrackerActive(bool value) {}

#endif // UNITY_ANDROID
        }

        /// <summary>
        /// Is the controller API being active
        /// </summary>
        /// <param name="self">unused</param>
        /// <returns>true if the controller is active</returns>
        public static bool IsControllerApiEnabled(this XRInputSubsystem self)
        {
            return Native.GetControllerActive();
        }

        /// <summary>
        /// Is the Eye Tracking API enabled/
        /// </summary>
        /// <param name="self">unused</param>
        /// <returns>true if the eye tracker is active.</returns>
        public static bool IsEyeTrackingApiEnabled(this XRInputSubsystem self)
        {
            return Native.GetEyeTrackerActive();
        }

        /// <summary>
        /// Enable/disable the Controller API
        /// </summary>
        /// <param name="self">unused</param>
        /// <param name="enabled">true to enable, false to disable</param>
        public static void SetControllerApiEnabled(this XRInputSubsystem self, bool enabled)
        {
            Native.SetControllerActive(enabled);
        }

        /// <summary>
        /// Enable/disable the Eye Tracking API
        /// </summary>
        /// <param name="self">unused</param>
        /// <param name="enabled">true to enable, false to disable</param>
        public static void SetEyeTrackingApiEnabled(this XRInputSubsystem self, bool enabled)
        {
            Native.SetEyeTrackerActive(enabled);
        }
    }

    /// <summary>
    /// Utility static class
    /// </summary>
    public static class MagicLeapInputUtility
    {
        /// <summary>
        /// Parse a byte array, aligned on 4 byte boundary, as a list of floatd
        /// </summary>
        /// <param name="input">4 byte aligned stream of bytes</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">No input detected.</exception>
        /// <exception cref="ArgumentException">Byte stream does not align on a 4 byte boundary.</exception>
        public static float[] ParseData(byte[] input)
        {
            if (input == null)
                throw new ArgumentNullException("input");
            if ((input.Length % 4) != 0)
                throw new ArgumentException("malformed input array; incorrect number of bytes");

            var list = new List<float>();

            for (int i = 0; i < input.Length; i += 4)
            {
                list.Add(BitConverter.ToSingle(input, i));
            }
            return list.ToArray();
        }
    }
}
