using System;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;

namespace UnityEditor.XR.MagicLeap
{
    internal static class SDKUtility
    {
        const string kManifestPath = ".metadata/sdk.manifest";

        static class Native
        {
            [DllImport("UnityMagicLeap", EntryPoint = "UnityMagicLeap_PlatformGetAPILevel")]
            public static extern uint GetAPILevel();
        }

        public class SDKManifest
        {
            public string version;
        }

        internal static bool isCompatibleSDK
        {
            get
            {
                var min = pluginAPILevel;
                var max = sdkAPILevel;
                return min <= max;
            }
        }
        internal static int pluginAPILevel
        {
            get
            {
                return (int)Native.GetAPILevel();
            }
        }
        internal static int sdkAPILevel
        {
            get
            {
                return PrivilegeParser.ParsePlatformLevelFromHeader(Path.Combine(SDKUtility.sdkPath, PrivilegeParser.kPlatformHeaderPath));
            }
        }
        internal static bool sdkAvailable
        {
            get
            {
                if (string.IsNullOrEmpty(sdkPath)) return false;
                return File.Exists(Path.Combine(sdkPath, kManifestPath));
            }
        }
        internal static string sdkPath
        {
            get
            {
#if PLATFORM_ANDROID
                return EditorPrefs.GetString("RelishSDKRoot", null);
#else
                return null;
#endif
            }
        }
        internal static Version sdkVersion
        {
            get => new Version(JsonUtility.FromJson<SDKManifest>(File.ReadAllText(Path.Combine(sdkPath, kManifestPath))).version);
        }
    }
}
