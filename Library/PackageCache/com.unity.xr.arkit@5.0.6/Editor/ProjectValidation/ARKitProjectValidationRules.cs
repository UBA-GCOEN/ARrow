using System.Linq;
using Unity.XR.CoreUtils.Editor;
using UnityEditor.XR.Management;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARKit;

namespace UnityEditor.XR.ARKit
{
    static class ARKitProjectValidationRules
    {
        const string k_ProjectXRPlugInManagement = "Project/XR Plug-in Management";
        const string k_Category = "Apple ARKit";

        static readonly System.Type[] k_FaceTrackingTypes =
        {
            typeof(ARFace),
            typeof(ARFaceManager),
            typeof(ARFaceMeshVisualizer)
        };

        [InitializeOnLoadMethod]
        static void AddARKitValidationRules()
        {
            // When adding a new validation rule, please renemeber to add it in the docs also with a user-friendly description
            var iOSGlobalRules = new[]
            {
                new BuildValidationRule
                {
                    Category = k_Category,
                    Message = $"Apple ARKit requires targeting minimum iOS 11.0.",
                    IsRuleEnabled = IsARKitPluginEnabled,
                    CheckPredicate = () =>
                    {
                        var userSetTargetVersion = OSVersion.Parse(PlayerSettings.iOS.targetOSVersionString);
                        return userSetTargetVersion >= new OSVersion(11);
                    },
                    FixItMessage =
                        "Open Project Settings > Player Settings > iOS tab and increase the 'Target minimum " +
                        "iOS Version' to '11.0' or greater.",
                    FixIt = () =>
                    {
                        PlayerSettings.iOS.targetOSVersionString = "11.0";
                    },
                    Error = true
                },
                new BuildValidationRule
                {
                    Category = k_Category,
                    Message = "The camera usage description in the Player Settings needs to be set to use the " +
                              "camera feed in the app.",
                    IsRuleEnabled = IsARKitPluginEnabled,
                    CheckPredicate = () => !string.IsNullOrEmpty(PlayerSettings.iOS.cameraUsageDescription),
                    FixItMessage =
                        "Open Project Settings > Player Settings > iOS tab and set 'Camera Usage Description*'" +
                        " to a message explaining the camera usage.",
                    Error = true,
                    FixIt = () =>
                    {
                        PlayerSettings.iOS.cameraUsageDescription = "Augmented Reality requires the camera";
                    },
                },
                new BuildValidationRule
                {
                    Category = k_Category,
                    Message = "The currently opened scene uses face tracking components and require `Face Tracking` to be enabled.",
                    IsRuleEnabled = IsARKitPluginEnabled,
                    CheckPredicate = () => ARKitSettings.GetOrCreateSettings().faceTracking || !BuildValidator.HasTypesInSceneSetup(k_FaceTrackingTypes),
                    FixItMessage = "Open Project Setting > XR Plug-in Management > Apple ARKit and enable 'Face Tracking'.",
                    FixIt = () =>
                    {
                        ARKitSettings.GetOrCreateSettings().faceTracking = true;
                    },
                },
            };

            var iosARFoundationRules = new[]
            {
                new BuildValidationRule()
                {
                    Category = k_Category,
                    Message = "Please enable the 'Apple ARKit' plugin in 'XR Plug-in Management'.",
                    CheckPredicate = IsARKitPluginEnabled,
                    FixItMessage = "Open Project Setting > XR Plug-in Management > iOS tab and enable `Apple ARKit`.",
                    FixIt = () => { SettingsService.OpenProjectSettings(k_ProjectXRPlugInManagement); },
                    Error = false,
                    FixItAutomatic = false
                },
            };

            BuildValidator.AddRules(BuildTargetGroup.iOS, iOSGlobalRules);
            BuildValidator.AddRules(BuildTargetGroup.iOS, iosARFoundationRules);
        }

        static bool IsARKitPluginEnabled()
        {
            var generalSettings = XRGeneralSettingsPerBuildTarget.XRGeneralSettingsForBuildTarget(
                BuildTargetGroup.iOS);
            if (generalSettings == null)
                return false;

            var managerSettings = generalSettings.AssignedSettings;

            return managerSettings != null && managerSettings.activeLoaders.Any(loader => loader is ARKitLoader);
        }
    }
}
