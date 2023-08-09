using System.Collections.Generic;
using System.Linq;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.Callbacks;
using UnityEditor.PackageManager;
using UnityEditor.XR.OpenXR.Features;
using UnityEngine;
using UnityEngine.XR.OpenXR;
using UnityEngine.XR.OpenXR.Features;
using System;
using Unity.XR.CoreUtils.Editor;

namespace UnityEditor.XR.OpenXR
{
    internal class OpenXRProjectValidationRulesSetup
    {
        static BuildTargetGroup[] s_BuildTargetGroups =
            ((BuildTargetGroup[])Enum.GetValues(typeof(BuildTargetGroup))).Distinct().ToArray();

        static BuildTargetGroup s_SelectedBuildTargetGroup = BuildTargetGroup.Unknown;

        internal const string OpenXRProjectValidationSettingsPath = "Project/XR Plug-in Management/Project Validation";

        [InitializeOnLoadMethod]
        static void OpenXRProjectValidationCheck()
        {
            UnityEditor.PackageManager.Events.registeredPackages += (packageRegistrationEventArgs) =>
            {
                // In the Player Settings UI we have to delay the call one frame to let OpenXRSettings constructor to get initialized
                EditorApplication.delayCall += () =>
                {
                    if (HasXRPackageVersionChanged(packageRegistrationEventArgs))
                    {
                        ShowWindowIfIssuesExist();
                    }
                };
            };

            AddOpenXRValidationRules();
        }

        private static bool HasXRPackageVersionChanged(PackageRegistrationEventArgs packageRegistrationEventArgs)
        {
            bool packageChanged = packageRegistrationEventArgs.changedTo.Any(p => p.name.Equals(OpenXRManagementSettings.PackageId));
            OpenXRSettings.Instance.versionChanged = packageChanged;
            return packageRegistrationEventArgs.added.Any(p => p.name.Equals(OpenXRManagementSettings.PackageId)) || packageChanged;
        }

        private static void ShowWindowIfIssuesExist()
        {
            List<OpenXRFeature.ValidationRule> failures = new List<OpenXRFeature.ValidationRule>();
            BuildTargetGroup activeBuildTargetGroup = BuildPipeline.GetBuildTargetGroup(EditorUserBuildSettings.activeBuildTarget);
            OpenXRProjectValidation.GetCurrentValidationIssues(failures, activeBuildTargetGroup);

            if (failures.Count > 0)
            {
                ShowWindow();
            }
        }

        static void AddOpenXRValidationRules()
        {
            foreach (var buildTargetGroup in s_BuildTargetGroups)
            {
                var issues = new List<OpenXRFeature.ValidationRule>();
                OpenXRProjectValidation.GetAllValidationIssues(issues, buildTargetGroup);

                var coreIssues = new List<BuildValidationRule>();
                foreach (var issue in issues)
                {
                    var rule = new BuildValidationRule
                    {
                        // This will hide the rules given a condition so that when you click "Show all" it doesn't show up as passed
                        IsRuleEnabled = () =>
                        {
                            // If OpenXR isn't enabled, no need to show the rule
                            if (!BuildHelperUtils.HasLoader(buildTargetGroup, typeof(OpenXRLoaderBase)))
                                return false;

                            // If the feature isn't enabled, don't show this rule
                            if (issue.feature != null && !issue.feature.enabled)
                                return false;

                            return true;
                        },
                        CheckPredicate = issue.checkPredicate,
                        Error = issue.error,
                        FixIt = issue.fixIt,
                        FixItAutomatic = issue.fixItAutomatic,
                        FixItMessage = issue.fixItMessage,
                        HelpLink = issue.helpLink,
                        HelpText = issue.helpText,
                        Message = issue.message,
                        Category = issue.feature != null ? issue.feature.nameUi : "OpenXR",
                        SceneOnlyValidation = false
                    };

                    coreIssues.Add(rule);
                }

                BuildValidator.AddRules(buildTargetGroup, coreIssues);
            }
        }

        [MenuItem("Window/XR/OpenXR/Project Validation")]
        private static void MenuItem()
        {
            ShowWindow();
        }

        internal static void SetSelectedBuildTargetGroup(BuildTargetGroup buildTargetGroup)
        {
            if (s_SelectedBuildTargetGroup == buildTargetGroup)
                return;

            s_SelectedBuildTargetGroup = buildTargetGroup;
        }

        internal static void ShowWindow(BuildTargetGroup buildTargetGroup = BuildTargetGroup.Unknown)
        {
            // Delay opening the window since sometimes other settings in the player settings provider redirect to the
            // project validation window causing serialized objects to be nullified
            EditorApplication.delayCall += () =>
            {
                SetSelectedBuildTargetGroup(buildTargetGroup);
                SettingsService.OpenProjectSettings(OpenXRProjectValidationSettingsPath);
            };
        }

        internal static void CloseWindow() { }
    }

    internal class OpenXRProjectValidationBuildStep : IPreprocessBuildWithReport
    {
        [OnOpenAsset(0)]
        static bool ConsoleErrorDoubleClicked(int instanceId, int line)
        {
            var objName = EditorUtility.InstanceIDToObject(instanceId).name;
            if (objName == "OpenXRProjectValidation")
            {
                OpenXRProjectValidationRulesSetup.ShowWindow();
                return true;
            }

            return false;
        }

        public int callbackOrder { get; }

        public void OnPreprocessBuild(BuildReport report)
        {
            if (!BuildHelperUtils.HasLoader(report.summary.platformGroup, typeof(OpenXRLoaderBase)))
                return;

            if (OpenXRProjectValidation.LogBuildValidationIssues(report.summary.platformGroup))
                throw new BuildFailedException("OpenXR Build Failed.");
        }
    }
}
