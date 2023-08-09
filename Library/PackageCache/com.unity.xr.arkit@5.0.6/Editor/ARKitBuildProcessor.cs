using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.Rendering;
using UnityEditor.XR.ARSubsystems;
using UnityEditor.XR.Management;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.XR.ARKit;
using Unity.EditorCoroutines.Editor;
#if UNITY_IOS
using System.IO;
using UnityEditor.iOS;
using UnityEditor.iOS.Xcode;
#endif

using OSVersion = UnityEngine.XR.ARKit.OSVersion;

namespace UnityEditor.XR.ARKit
{
    static class ARKitBuildProcessor
    {
        public static bool loaderEnabled;
        public static bool faceTrackingEnabled;

        class Preprocessor : IPreprocessBuildWithReport, IPreprocessShaders
        {
            // Magic value according to
            // https://docs.unity3d.com/ScriptReference/PlayerSettings.GetArchitecture.html
            // "0 - None, 1 - ARM64, 2 - Universal."
            const int k_TargetArchitectureArm64 = 1;
            const int k_TargetArchitectureUniversal = 2;

            // The minimum target Xcode version for the plug-in
            const int k_TargetMinimumMajorXcodeVersion = 11;
            const int k_TargetMinimumMinorXcodeVersion = 0;
            const int k_TargetMinimumPatchXcodeVersion = 0;

            readonly string[] runtimePluginNames =
            {
                "libUnityARKit.a",
                "UnityARKit.m",
                "libUnityARKitFaceTracking.a",
            };

            public int callbackOrder => 0;
        
            void IPreprocessShaders.OnProcessShader(Shader shader, ShaderSnippetData snippet, IList<ShaderCompilerData> data)
            {
#if UNITY_IOS && UNITY_XR_ARKIT_LOADER_ENABLED
                ProcessShader(shader, snippet, data);
#endif
            }

            static void ProcessShader(Shader shader, ShaderSnippetData snippet, IList<ShaderCompilerData> data)
            {
                // Remove shader variants for the camera background shader that will fail compilation because of package dependencies.
                foreach (var backgroundShaderName in ARKitCameraSubsystem.backgroundShaderNames)
                {
                    if (backgroundShaderName.Equals(shader.name))
                    {
                        foreach (var backgroundShaderKeywordToNotCompile in ARKitCameraSubsystem.backgroundShaderKeywordsToNotCompile)
                        {
                            ShaderKeyword shaderKeywordToNotCompile = new ShaderKeyword(shader, backgroundShaderKeywordToNotCompile);

                            for (int i = (data.Count - 1); i >= 0; --i)
                            {
                                if (data[i].shaderKeywordSet.IsEnabled(shaderKeywordToNotCompile))
                                {
                                    data.RemoveAt(i);
                                }
                            }
                        }
                    }
                }
            }

            void IPreprocessBuildWithReport.OnPreprocessBuild(BuildReport report)
            {
#if UNITY_IOS && UNITY_XR_ARKIT_LOADER_ENABLED
                SetRuntimePluginCopyDelegate();
                PreprocessBuild(report);
#elif UNITY_XR_ARKIT_LOADER_ENABLED
                if (report.summary.platform == BuildTarget.iOS)
                {
                    Debug.LogWarning(
                        "Apple ARKit XR Plug-in requires the UNITY_IOS preprocessor directive to build a correctly " +
                        "configured app for XR. You are building for the iOS platform with the Apple ARKit XR Plug-in " +
                        "enabled, but Unity's active build target is not set to iOS. This may result in runtime " +
                        "errors in your build.\n To ensure that your build is correctly configured, set Unity's " +
                        "active build target to iOS and reload the domain before building, or use the " +
                        "'-buildTarget iOS' command line argument when building in batch mode.");
                }
#endif
            }

            // ReSharper disable once UnusedMember.Local
            void SetRuntimePluginCopyDelegate()
            {
                var allPlugins = PluginImporter.GetAllImporters();
                foreach (var plugin in allPlugins)
                {
                    if (plugin.isNativePlugin)
                    {
                        foreach (var pluginName in runtimePluginNames)
                        {
                            if (plugin.assetPath.Contains(pluginName))
                            {
                                plugin.SetIncludeInBuildDelegate(ShouldIncludeRuntimePluginsInBuild);
                                break;
                            }
                        }
                    }
                }
            }


            // ReSharper disable once UnusedMember.Local
            static void PreprocessBuild(BuildReport report)
            {
                if (report.summary.platform != BuildTarget.iOS || !loaderEnabled)
                    return;

                if (string.IsNullOrEmpty(PlayerSettings.iOS.cameraUsageDescription))
                    throw new BuildFailedException(
                        "ARKit requires a Camera Usage Description (Player Settings > iOS > Other Settings > Camera Usage Description)");

                EnsureMinimumXcodeVersion();

                EnsureMetalIsFirstApi();

                if (ARKitSettings.GetOrCreateSettings().requirement == ARKitSettings.Requirement.Required)
                {
                    EnsureMinimumBuildTarget();
                    EnsureTargetArchitecturesAreSupported(report.summary.platformGroup);
                }

                foreach (var backgroundShaderName in ARKitCameraSubsystem.backgroundShaderNames)
                {
                    BuildHelper.AddBackgroundShaderToProject(backgroundShaderName);
                }
            }

            static void EnsureMinimumBuildTarget()
            {
                var userSetTargetVersion = OSVersion.Parse(PlayerSettings.iOS.targetOSVersionString);
                if (userSetTargetVersion < new OSVersion(11))
                {
                    throw new BuildFailedException(
                        $"You have selected a minimum target iOS version of {userSetTargetVersion} and have " + 
                        " the Apple ARKit XR Plug-in package installed. ARKit requires at least iOS version 11.0. " +
                        "See Player Settings > Other Settings > Target minimum iOS Version.");
                }
            }

            static void EnsureMinimumXcodeVersion()
            {
#if UNITY_IOS && UNITY_EDITOR_OSX
                var xcodeIndex = Math.Max(0, XcodeApplications.GetPreferedXcodeIndex());
                var xcodeVersion = OSVersion.Parse(XcodeApplications.GetXcodeApplicationPublicName(xcodeIndex));
                if (xcodeVersion == new OSVersion(0))
                    throw new BuildFailedException(
                        $"Could not determine which version of Xcode was selected in the Build Settings. Xcode app was computed as \"{XcodeApplications.GetXcodeApplicationPublicName(xcodeIndex)}\".");

                if (xcodeVersion < new OSVersion(
                        k_TargetMinimumMajorXcodeVersion,
                        k_TargetMinimumMinorXcodeVersion,
                        k_TargetMinimumPatchXcodeVersion))
                    throw new BuildFailedException(
                        $"The selected Xcode version: {xcodeVersion} is below the minimum Xcode required Xcode version for the Apple ARKit XR Plug-in.  Please target at least Xcode version {k_TargetMinimumMajorXcodeVersion}.{k_TargetMinimumMinorXcodeVersion}.{k_TargetMinimumPatchXcodeVersion}.");
#endif
            }

            static void EnsureTargetArchitecturesAreSupported(BuildTargetGroup buildTargetGroup)
            {
                var buildTarget = NamedBuildTarget.FromBuildTargetGroup(buildTargetGroup);

                if (PlayerSettings.GetArchitecture(buildTarget) != k_TargetArchitectureArm64)
                    throw new BuildFailedException(
                        "Apple ARKit XR Plug-in only supports the ARM64 architecture. " +
                        "See Player Settings > Other Settings > Architecture.");
            }

            static void EnsureMetalIsFirstApi()
            {
                var graphicsApis = PlayerSettings.GetGraphicsAPIs(BuildTarget.iOS);
                if (graphicsApis.Length > 0 && graphicsApis[0] != GraphicsDeviceType.Metal)
                {
                    throw new BuildFailedException($"You currently have {graphicsApis[0]} at the top of the list of Graphics APis. However, Metal needs to be first in the list. (See Player Settings > Other Settings > Graphics APIs)");
                }
            }

            static bool ShouldIncludeRuntimePluginsInBuild(string path)
            {
                if (!loaderEnabled)
                    return false;

                if (path.Contains("libUnityARKitFaceTracking.a"))
                    return faceTrackingEnabled;

                return true;
            }
        }

// Our PostProcessor depends on UnityEditor.iOS.PlistDocument, a class in Unity's iOS module.
// To ensure that this package successfully compiles in projects that don't have the iOS module installed, we must
// wrap the PostProcessor with a UNITY_IOS preprocessor directive.
#if UNITY_IOS
        class PostProcessor : IPostprocessBuildWithReport
        {
            // Needs to be > 0 to make sure we remove the shader since the Input System overwrites the preloaded assets array
            public int callbackOrder => 1;

            void IPostprocessBuildWithReport.OnPostprocessBuild(BuildReport report)
            {
#if UNITY_XR_ARKIT_LOADER_ENABLED
                PostprocessBuild(report);
#endif
            }

            static void PostprocessBuild(BuildReport report)
            {
                if (report.summary.platform != BuildTarget.iOS)
                {
                    return;
                }

                foreach (var shaderName in ARKitCameraSubsystem.backgroundShaderNames)
                    BuildHelper.RemoveShaderFromProject(shaderName);

                HandleARKitRequiredFlag(report.summary.outputPath);
            }

            static void HandleARKitRequiredFlag(string pathToBuiltProject)
            {
                var arkitSettings = ARKitSettings.GetOrCreateSettings();
                string plistPath = Path.Combine(pathToBuiltProject, "Info.plist");
                PlistDocument plist = new PlistDocument();
                plist.ReadFromString(File.ReadAllText(plistPath));
                PlistElementDict rootDict = plist.root;

                // Get or create array to manage device capabilities
                const string capsKey = "UIRequiredDeviceCapabilities";
                PlistElementArray capsArray;
                if (rootDict.values.TryGetValue(capsKey, out PlistElement pel))
                {
                    capsArray = pel.AsArray();
                }
                else
                {
                    capsArray = rootDict.CreateArray(capsKey);
                }

                // Remove any existing "arkit" plist entries
                const string arkitStr = "arkit";
                capsArray.values.RemoveAll(x => arkitStr.Equals(x.AsString()));
                if (arkitSettings.requirement == ARKitSettings.Requirement.Required)
                {
                    // Add "arkit" plist entry
                    capsArray.AddString(arkitStr);
                }

                File.WriteAllText(plistPath, plist.WriteToString());
            }
        }
#endif // UNITY_IOS
    }

    [InitializeOnLoad]
    class LoaderEnabledCheck
    {
        static readonly NamedBuildTarget s_iOSTarget =
            NamedBuildTarget.FromBuildTargetGroup(BuildPipeline.GetBuildTargetGroup(BuildTarget.iOS));

        static ARKitSettings s_ARKitSettings;

        static LoaderEnabledCheck()
        {
            s_ARKitSettings = ARKitSettings.GetOrCreateSettings();
            ARKitBuildProcessor.loaderEnabled = false;

            UpdateARKitDefines();
            EditorCoroutineUtility.StartCoroutineOwnerless(UpdateARKitDefinesCoroutine());
        }

        static IEnumerator UpdateARKitDefinesCoroutine()
        {
            var waitObj = new EditorWaitForSeconds(.25f);

            while (true)
            {
                UpdateARKitDefines();
                yield return waitObj;
            }
        }

        static void UpdateARKitDefines()
        {
            var iOSXRSettings = XRGeneralSettingsPerBuildTarget.XRGeneralSettingsForBuildTarget(
                BuildPipeline.GetBuildTargetGroup(BuildTarget.iOS));

            if (iOSXRSettings == null)
                return;

            var arkitLoaderShouldBeEnabled = iOSXRSettings.Manager.activeLoaders.OfType<ARKitLoader>().Any();

            if (arkitLoaderShouldBeEnabled && !ARKitBuildProcessor.loaderEnabled)
            {
                AddDefine("UNITY_XR_ARKIT_LOADER_ENABLED");
            }
            else if (!arkitLoaderShouldBeEnabled && ARKitBuildProcessor.loaderEnabled)
            {
                RemoveDefine("UNITY_XR_ARKIT_LOADER_ENABLED");
            }

            if (arkitLoaderShouldBeEnabled && s_ARKitSettings.faceTracking && !ARKitBuildProcessor.faceTrackingEnabled)
            {
                AddDefine("UNITY_XR_ARKIT_FACE_TRACKING_ENABLED");
            }
            else if (!s_ARKitSettings.faceTracking && ARKitBuildProcessor.faceTrackingEnabled)
            {
                RemoveDefine("UNITY_XR_ARKIT_FACE_TRACKING_ENABLED");
            }

            ARKitBuildProcessor.loaderEnabled = arkitLoaderShouldBeEnabled;
            ARKitBuildProcessor.faceTrackingEnabled = arkitLoaderShouldBeEnabled && s_ARKitSettings.faceTracking;
        }

        static void AddDefine(string define)
        {
            var definesString = PlayerSettings.GetScriptingDefineSymbols(s_iOSTarget);
            var allDefines = new HashSet<string>(definesString.Split(';'));

            if (allDefines.Contains(define))
                return;

            allDefines.Add(define);
            PlayerSettings.SetScriptingDefineSymbols(s_iOSTarget, string.Join(";", allDefines));
        }

        static void RemoveDefine(string define)
        {
            var definesString = PlayerSettings.GetScriptingDefineSymbols(s_iOSTarget);
            var allDefines = new HashSet<string>(definesString.Split(';'));
            allDefines.Remove(define);
            PlayerSettings.SetScriptingDefineSymbols(s_iOSTarget, string.Join(";", allDefines));
        }
    }
}
