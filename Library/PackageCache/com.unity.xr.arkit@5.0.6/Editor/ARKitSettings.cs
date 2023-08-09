using System;
using UnityEngine;
using UnityEngine.XR.Management;

namespace UnityEditor.XR.ARKit
{
    /// <summary>
    /// Holds settings that are used to configure the Apple ARKit XR Plug-in.
    /// </summary>
    [Serializable]
    [XRConfigurationData("Apple ARKit", "UnityEditor.XR.ARKit.ARKitSettings")]
    public class ARKitSettings : ScriptableObject
    {
        const string k_SettingsKey = "UnityEditor.XR.ARKit.ARKitSettings";
        const string k_OldConfigObjectName = "com.unity.xr.arkit.PlayerSettings";

        /// <summary>
        /// Enum which defines whether ARKit is optional or required.
        /// </summary>
        public enum Requirement
        {
            /// <summary>
            /// ARKit is required, which means the app cannot be installed on devices that do not support ARKit.
            /// </summary>
            Required,

            /// <summary>
            /// ARKit is optional, which means the the app can be installed on devices that do not support ARKit.
            /// </summary>
            Optional
        }

        [SerializeField, Tooltip("Toggles whether ARKit is required for this app. Will make app only downloadable by devices with ARKit support if set to 'Required'.")]
        Requirement m_Requirement;

        /// <summary>
        /// Determines whether ARKit is required for this app. If set to <see cref="Requirement.Required"/>, the app can only be downloaded on devices with ARKit support.
        /// </summary>
        public Requirement requirement
        {
            get => m_Requirement;
            set => m_Requirement = value;
        }

        /// <summary>
        /// If <c>true</c>, includes ARKit Face Tracking functionality. If <c>false</c>, doesn't include ARKit Face Tracking functionality.
        /// </summary>
        public bool faceTracking
        {
            get => m_FaceTracking;
            set => m_FaceTracking = value;
        }

        [SerializeField, Tooltip("Includes ARKit Face Tracking functionality when toggled on.")]
        bool m_FaceTracking;

        /// <summary>
        /// Gets the currently selected settings, or creates default settings if no <see cref="ARKitSettings"/> have been set in Player Settings.
        /// </summary>
        /// <returns>The ARKit settings to use for the current Player build.</returns>
        public static ARKitSettings GetOrCreateSettings()
        {
            var settings = currentSettings;
            if (settings != null)
                return settings;

            return CreateInstance<ARKitSettings>();
        }

        /// <summary>
        /// Get or set the <see cref="ARKitSettings"/> to use for the Player build.
        /// </summary>
        public static ARKitSettings currentSettings
        {
            get => EditorBuildSettings.TryGetConfigObject(k_SettingsKey, out ARKitSettings settings) ? settings : null;

            set
            {
                if (value == null)
                {
                    EditorBuildSettings.RemoveConfigObject(k_SettingsKey);
                }
                else
                {
                    EditorBuildSettings.AddConfigObject(k_SettingsKey, value, true);
                }
            }
        }

        internal static bool TrySelect()
        {
            var settings = currentSettings;
            if (settings == null)
                return false;

            Selection.activeObject = settings;
            return true;
        }

        internal static SerializedObject GetSerializedSettings() => new(GetOrCreateSettings());

        void Awake()
        {
            if (EditorBuildSettings.TryGetConfigObject(k_OldConfigObjectName, out ARKitSettings result))
            {
                EditorBuildSettings.RemoveConfigObject(k_OldConfigObjectName);
            }
        }
    }
}
