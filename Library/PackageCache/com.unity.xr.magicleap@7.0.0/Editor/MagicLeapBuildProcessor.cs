using System;
using System.Linq;
using System.Collections.Generic;
using UnityEditor.Android;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.XR.Management;

using UnityEngine;
using UnityEngine.XR.Management;

using UnityEngine.XR.MagicLeap;

namespace UnityEditor.XR.MagicLeap
{
    /// <summary>
    /// Small utility class for reading, updating and writing boot config.
    /// </summary>
    class BootConfig
    {
        static readonly string kXrBootSettingsKey = "xr-boot-settings";
        Dictionary<string, string> bootConfigSettings;

        private readonly BuildReport buildReport;

        public BootConfig(BuildReport report)
        {
            buildReport = report;
        }

        public void ReadBootConfg()
        {
            bootConfigSettings = new Dictionary<string, string>();

            string buildTargetName = BuildPipeline.GetBuildTargetName(buildReport.summary.platform);
            string xrBootSettings = EditorUserBuildSettings.GetPlatformSettings(buildTargetName, kXrBootSettingsKey);
            if (!String.IsNullOrEmpty(xrBootSettings))
            {
                // boot settings string format
                // <boot setting>:<value>[;<boot setting>:<value>]*
                var bootSettings = xrBootSettings.Split(';');
                foreach (var bootSetting in bootSettings)
                {
                    var setting = bootSetting.Split(':');
                    if (setting.Length == 2 && !String.IsNullOrEmpty(setting[0]) && !String.IsNullOrEmpty(setting[1]))
                    {
                        bootConfigSettings.Add(setting[0], setting[1]);
                    }
                }
            }

        }

        public void SetValueForKey(string key, string value, bool replace = false)
        {
            if (bootConfigSettings.ContainsKey(key))
            {
                bootConfigSettings[key] = value;
            }
            else
            {
                bootConfigSettings.Add(key, value);
            }
        }

        public void ClearEntryForKey(string key)
        {
            if (bootConfigSettings.ContainsKey(key))
            {
                bootConfigSettings.Remove(key);
            }
        }

        public void WriteBootConfig()
        {
            // boot settings string format
            // <boot setting>:<value>[;<boot setting>:<value>]*
            bool firstEntry = true;
            var sb = new System.Text.StringBuilder();
            foreach (var kvp in bootConfigSettings)
            {
                if (!firstEntry)
                {
                    sb.Append(";");
                }
                sb.Append($"{kvp.Key}:{kvp.Value}");
                firstEntry = false;
            }

            string buildTargetName = BuildPipeline.GetBuildTargetName(buildReport.summary.platform);
            EditorUserBuildSettings.SetPlatformSettings(buildTargetName, kXrBootSettingsKey, sb.ToString());
        }
    }

    /// <summary>
    /// Build processor for Magic Leap
    /// Main entry point for setting up, generating Gradle projects and deployable binaries.
    /// </summary>
    public class MagicLeapBuildProcessor : IPreprocessBuildWithReport, IPostprocessBuildWithReport, IPostGenerateGradleAndroidProject
    {
        /// <summary>
        /// Overridden from IOrderedCallback.
        /// Returns the relative callback order for callbacks.
        /// Callbacks with lower values are called before ones with higher values.
        /// </summary>
        public int callbackOrder => 0;

        private static readonly string kHaveAndroidWindowSupportBootSettingsKey = "android-device-have-window-support";
        private static readonly string kUseNullDisplayManagerBootSettingsKey = "android-device-use-null-display-manager";
        private static readonly string kAndroidAudioUseMLAudio = "android-audio-use-MLAudio";
        private static readonly string kVulkanForceDisableETCSupport = "vulkan-force-disable-ETC-support";
        private static readonly string kVulkanForceDisableASTCSupport = "vulkan-force-disable-ASTC-support";
        private static readonly string kVulkanDisablePreTransform = "vulkan-disable-pre-transform";

        private readonly string[] runtimePluginNames = new string[] { "UnityMagicLeap.elf", "UnityMagicLeap.so", "libUnityMagicLeap.so" };
        private readonly string[] remotingPluginNames = new string[] { "UnityMagicLeap.dll", "UnityMagicLeap.dylib" };

        void CleanOldSettings()
        {
            UnityEngine.Object[] preloadedAssets = PlayerSettings.GetPreloadedAssets();
            if (preloadedAssets == null)
                return;

            var oldSettings = from s in preloadedAssets
                where (s != null) && (s.GetType() == typeof(MagicLeapSettings))
                select s;

            if (oldSettings.Any())
            {
                var assets = preloadedAssets.ToList();
                foreach (var s in oldSettings)
                {
                    assets.Remove(s);
                }

                PlayerSettings.SetPreloadedAssets(assets.ToArray());
            }
        }

        /// <summary>
        /// Check to see if we should include Runtime Plugins for non-Magicleap builds
        /// </summary>
        /// <param name="path">Unused</param>
        /// <returns>Return false for MagicLeap supported platforms. True otherwise.</returns>
        public bool ShouldIncludeRuntimePluginsInBuild(string path)
        {
#if UNITY_ANDROID
            XRGeneralSettings generalSettings = XRGeneralSettingsPerBuildTarget.XRGeneralSettingsForBuildTarget(BuildPipeline.GetBuildTargetGroup(EditorUserBuildSettings.activeBuildTarget));
            if (generalSettings == null)
                return false;

            foreach (var loader in generalSettings.Manager.activeLoaders)
            {
                if (loader is MagicLeapLoader)
                    return true;
            }
#endif // UNITY_ANDROID

            return false;
        }

        /// <summary>
        /// Remoting is only intended to work in the editor so builds are disallowed to have the libraries
        /// </summary>
        /// <param name="path">Unused</param>
        /// <returns>Always returns false</returns>
        public bool ShouldIncludeRemotingPluginsInBuild(string path) => false;

        void AssignNativePluginIncludeInBuildDelegates()
        {
            // For each plugin within the project, check if it is a plugin generated by this
            // package and assign the Include in build delegate to prevent magic leap libraries
            // from being included on other platforms
            var allPlugins = PluginImporter.GetAllImporters();
            foreach (var plugin in allPlugins)
            {
                if (plugin.isNativePlugin)
                {
                    CheckEachPlugin(plugin);
                }
            }
        }

        /// <summary>
        /// For every plugin, check to see if there is a build delegate. As well, check to see if we should set the
        /// include for that build delegate.
        /// </summary>
        /// <param name="plugin">The PluginImporter to check.</param>
        private void CheckEachPlugin(PluginImporter plugin)
        {
            foreach (var pluginName in runtimePluginNames)
            {
                if (plugin.assetPath.Contains(pluginName))
                {
                    plugin.SetIncludeInBuildDelegate(ShouldIncludeRuntimePluginsInBuild);
                    break;
                }
            }

            foreach (var pluginName in remotingPluginNames)
            {
                if (plugin.assetPath.Contains(pluginName))
                {
                    plugin.SetIncludeInBuildDelegate(ShouldIncludeRemotingPluginsInBuild);
                    break;
                }
            }
        }

        /// <summary>
        /// Update BootConfig settings
        /// </summary>
        /// <param name="report">Report containing information about the build.</param>
        void UpdateBootConfig(BuildReport report, MagicLeapSettings mlSettings)
        {
            BootConfig bootConfig = new BootConfig(report);
            bootConfig.ReadBootConfg();

            if (report.summary.platform == BuildTarget.Android)
            {
                bootConfig.SetValueForKey(kHaveAndroidWindowSupportBootSettingsKey, "0", true);
                bootConfig.SetValueForKey(kUseNullDisplayManagerBootSettingsKey, "1", true);
                bootConfig.SetValueForKey(kVulkanForceDisableETCSupport, "1", true);
                bootConfig.SetValueForKey(kVulkanForceDisableASTCSupport, "1", true);
                bootConfig.SetValueForKey(kVulkanDisablePreTransform, "1", true);

                if (mlSettings.enableMLAudio)
                    bootConfig.SetValueForKey(kAndroidAudioUseMLAudio, "1", true);
                else
                    bootConfig.SetValueForKey(kAndroidAudioUseMLAudio, "0", true);
            }
            else
            {
                bootConfig.ClearEntryForKey(kHaveAndroidWindowSupportBootSettingsKey);
                bootConfig.ClearEntryForKey(kUseNullDisplayManagerBootSettingsKey);
                bootConfig.ClearEntryForKey(kVulkanForceDisableETCSupport);
                bootConfig.ClearEntryForKey(kVulkanForceDisableASTCSupport);
                bootConfig.ClearEntryForKey(kVulkanDisablePreTransform);
                bootConfig.ClearEntryForKey(kAndroidAudioUseMLAudio);
            }

            bootConfig.WriteBootConfig();
        }

        /// <summary>
        /// Overridden from IPreprocessBuildWithReport.
        /// Callback hook to perform operations before the build starts.
        /// </summary>
        /// <param name="report">Report containing information about the build.</param>
        public void OnPreprocessBuild(BuildReport report)
        {
            // Assign each library a "ShouldIncludeInBuild" delegate to indicate whether the plugin
            // should be placed in a build on a specific platform.  As of right now it's only important
            // for runtime on the device but that could change to have standalone include remoting libs
            AssignNativePluginIncludeInBuildDelegates();

            // Always remember to cleanup preloaded assets after build to make sure we don't
            // dirty later builds with assets that may not be needed or are out of date.
            CleanOldSettings();

            MagicLeapSettings settings = null;
            EditorBuildSettings.TryGetConfigObject(MagicLeapConstants.kSettingsKey, out settings);
            if (settings == null)
                return;

            UpdateBootConfig(report, settings);

            UnityEngine.Object[] preloadedAssets = PlayerSettings.GetPreloadedAssets();

            if (!preloadedAssets.Contains(settings))
            {
                var assets = preloadedAssets.ToList();
                assets.Add(settings);
                PlayerSettings.SetPreloadedAssets(assets.ToArray());
            }
        }

        /// <summary>
        /// Overridden from IPostprocessBuildWithReport.
        /// Callback hook that is called after a successful build.
        /// </summary>
        /// <param name="report">Report containing information about the build.</param>
        public void OnPostprocessBuild(BuildReport report)
        {
            // Always remember to cleanup preloaded assets after build to make sure we don't
            // dirty later builds with assets that may not be needed or are out of date.
            CleanOldSettings();
        }

        /// <summary>
        /// Overridden from IPostGenerateGradleAndroidProject.
        /// Callback hook that is called after the Android Gradle project is generated, but
        /// before the build starts.
        /// </summary>
        /// <param name="path">The path to the root of the Unity Library Gradle project.</param>
        public void OnPostGenerateGradleAndroidProject(string path)
        {
            string manifestPath = System.IO.Path.Combine(path, "src", "main", "AndroidManifest.xml");
            Debug.Log($"Android manifest path = {manifestPath}");

            ModifyAndroidManifestMagicLeap modifyManifest = new ModifyAndroidManifestMagicLeap(manifestPath);
            bool shouldInclude = ShouldIncludeRuntimePluginsInBuild(null);
            modifyManifest.IncludeMagicLeapMetaData(shouldInclude);
            modifyManifest.Save();
        }
    }
}
