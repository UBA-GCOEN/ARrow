using System;
using System.Collections.Generic;
using UnityEngine.XR.Management;

namespace UnityEngine.XR.MagicLeap
{
    internal class DisabledAttribute : PropertyAttribute
    {
        public DisabledAttribute() {}
    }

    /// <summary>
    /// Scriptable object containing project-wide Magic Leap settings
    /// </summary>
    [Serializable]
    [XRConfigurationData("Magic Leap Settings", MagicLeapConstants.kSettingsKey)]
    public class MagicLeapSettings : ScriptableObject
    {
#if UNITY_EDITOR
        internal bool versionChanged = false;
#endif

        /// <summary>
        /// Subsystems implemented outside the MagicLeap XR Plugin can utilize this class to be loaded
        /// by the MagicLeapLoader instead of its usual defaults.
        /// </summary>
        internal class Subsystems
        {
            private static Dictionary<Type, string> s_OverrideMap = new Dictionary<Type, string>();

            /// <summary>
            /// Register a subsystem id to be instantiated by the MagicLeapLoader instead of its default of
            /// the same type.
            /// This should be called before MagicLeapLoader is initialized. An ideal place would be inside
            /// a function marked with the [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
            /// attribute.
            /// </summary>
            /// <typeparam name="T">Subsystem type</typeparam>
            /// <param name="subsystemId">Id of the subsystem to be created</param>
            public static void RegisterSubsystemOverride<T>(string subsystemId) => s_OverrideMap.Add(typeof(T), subsystemId);

            /// <summary>
            /// Get the Id registered for a given subsystem type.
            /// </summary>
            /// <typeparam name="T">Subsystem type</typeparam>
            /// <param name="defaultId">Id to return if no override has been registered for the provided type</param>
            /// <returns>Subsystemy id to create for the provided type</returns>
            public static string GetSubsystemOverrideOrDefault<T>(string defaultId) => s_OverrideMap.TryGetValue(typeof(T), out string subsystemId) ? subsystemId : defaultId;
        }

    #if !UNITY_EDITOR
        static MagicLeapSettings s_RuntimeInstance = null;
    #endif // !UNITY_EDITOR

        /// <summary>
        /// Get the current MagicLeap Settings.
        /// </summary>
        public static MagicLeapSettings currentSettings
        {
            get
            {
                MagicLeapSettings settings = null;
            #if UNITY_EDITOR
                UnityEditor.EditorBuildSettings.TryGetConfigObject(MagicLeapConstants.kSettingsKey, out settings);
            #else
                settings = s_RuntimeInstance;
            #endif // UNITY_EDITOR
                return settings;
            }
        }

        [SerializeField, Tooltip("Defines the precision of the depth buffers; higher values allow a wider range of values, but are usually slower")]
        Rendering.DepthPrecision m_DepthPrecision;

        [SerializeField, Tooltip("Force Multipass rendering. Select this option when shaders are incompatible with Single Pass Instancing")]
        bool m_ForceMultipass;

        [SerializeField, Tooltip("When enabled, all of the content rendered by the application and system notifications will be headlocked. Use this mode with caution.")]
        bool m_HeadlockGraphics;

        [SerializeField, Tooltip("Enable MLAudio Support. Replaces default Android Audio Support.")]
        bool m_EnableMLAudio;

        public Rendering.DepthPrecision depthPrecision
        {
            get { return m_DepthPrecision; }
            set { m_DepthPrecision = value; }
        }

        /// <summary>
        /// Get/set if we wish to force Multipass rendering.
        /// </summary>
        public bool forceMultipass
        {
            get { return m_ForceMultipass; }
            set { m_ForceMultipass = value; }
        }

        public bool headlockGraphics
        {
            get { return m_HeadlockGraphics; }
            set { m_HeadlockGraphics = value; }
        }

        public bool enableMLAudio
        {
            get { return m_EnableMLAudio; }
            set { m_EnableMLAudio = value; }
        }

        void Awake()
        {
            #if !UNITY_EDITOR
            s_RuntimeInstance = this;
            #endif // !UNITY_EDITOR
        }
    }
}
