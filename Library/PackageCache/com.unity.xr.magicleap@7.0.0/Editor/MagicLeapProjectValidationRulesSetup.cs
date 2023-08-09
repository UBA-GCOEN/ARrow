using System.Collections.Generic;
using System.Linq;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.PackageManager;
using UnityEngine.XR.MagicLeap;

#if PROJECT_VALIDATION_AVAILABLE
using System;
using Unity.XR.CoreUtils.Editor;
#endif

namespace UnityEditor.XR.MagicLeap
{
#if PROJECT_VALIDATION_AVAILABLE
    internal class MagicLeapProjectValidationRulesSetup
    {
        static BuildTargetGroup[] s_BuildTargetGroups =
            ((BuildTargetGroup[])Enum.GetValues(typeof(BuildTargetGroup))).Distinct().ToArray();

        private const string MagicLeapProjectValidationSettingsPath = "Project/XR Plug-in Management/Project Validation";

        [InitializeOnLoadMethod]
        static void MagicLeapProjectValidationCheck()
        {
            UnityEditor.PackageManager.Events.registeredPackages += (packageRegistrationEventArgs) =>
            {
                // In the Player Settings UI we have to delay the call one frame to let the underlying MagicLeap constructor initialize
                EditorApplication.delayCall += () =>
                {
                    if (HasMagicLeapPackageVersionChanged(packageRegistrationEventArgs))
                    {
                        ShowWindowIfIssuesExist();
                    }
                };
            };
            AddMagicLeapValidationRules();
        }

        private static bool HasMagicLeapPackageVersionChanged(PackageRegistrationEventArgs packageRegistrationEventArgs)
        {
            var packageId = new XRPackage().metadata.packageId;
            bool packageChanged = packageRegistrationEventArgs.changedTo.Any(p => p.name.Equals(packageId));
            MagicLeapSettings.currentSettings.versionChanged = packageChanged;
            return packageRegistrationEventArgs.added.Any(p => p.name.Equals(packageId)) || packageChanged;
        }

        private static void ShowWindowIfIssuesExist()
        {
            List<MagicLeapProjectValidation.ValidationRule> issues = new List<MagicLeapProjectValidation.ValidationRule>();
            MagicLeapProjectValidation.GetCurrentValidationIssues(issues);

            if (issues.Count > 0)
            {
                ShowWindow();
            }
        }

        static void AddMagicLeapValidationRules()
        {
            foreach (var buildTargetGroup in s_BuildTargetGroups)
            {
                var issues = new List<MagicLeapProjectValidation.ValidationRule>();
                MagicLeapProjectValidation.GetCurrentValidationIssues(issues);

                var coreIssues = new List<BuildValidationRule>();
                foreach (var issue in issues)
                {

                    var rule = new BuildValidationRule
                    {
                        // This will hide the rules given a condition so that when you click "Show all" it doesn't show up as passed
                        IsRuleEnabled = () =>
                        {
                            // If the MagicLeap Loader isn't enabled, no need to show the rule
                            if (!BuildHelperUtils.HasLoader(buildTargetGroup, typeof(MagicLeapLoader)))
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
                        Category = "MagicLeap",
                        SceneOnlyValidation = false
                    };

                    coreIssues.Add(rule);
                }

                BuildValidator.AddRules(buildTargetGroup, coreIssues);
            }
        }

        [MenuItem("Window/XR/MagicLeap/Project Validation")]
        private static void MenuItem()
        {
            ShowWindow();
        }

        internal static void ShowWindow(BuildTargetGroup buildTargetGroup = BuildTargetGroup.Unknown)
        {
            // Delay opening the window since sometimes other settings in the player settings provider redirect to the
            // project validation window causing serialized objects to be nullified
            EditorApplication.delayCall += () =>
            {
                SettingsService.OpenProjectSettings(MagicLeapProjectValidationSettingsPath);
            };
        }

        internal static void CloseWindow()
        {
            // Method intentionally left empty.
        }
    }
#else // !PROJECT_VALIDATION_AVAILABLE - fallback to display the original editor window
    [InitializeOnLoad]
    internal class MagicLeapProjectValidationWindow : EditorWindow
    {
        private Vector2 m_ScrollViewPos = Vector2.zero;

        [MenuItem("Window/XR/MagicLeap/Project Validation")]
        private static void MenuItem()
        {
            ShowWindow();
        }

        internal static void SetSelectedBuildTargetGroup(BuildTargetGroup buildTargetGroup)
        {
            if (s_SelectedBuildTargetGroup == buildTargetGroup)
                return;

            s_Dirty = true;
            s_SelectedBuildTargetGroup = buildTargetGroup;
        }

        internal static void ShowWindow(BuildTargetGroup buildTargetGroup = BuildTargetGroup.Unknown)
        {
            SetSelectedBuildTargetGroup(buildTargetGroup);

            var window = (MagicLeapProjectValidationWindow) GetWindow(typeof(MagicLeapProjectValidationWindow));
            window.titleContent = Content.k_Title;
            window.minSize = new Vector2(500.0f, 300.0f);
            window.UpdateIssues();
            window.Show();
        }

        internal static void CloseWindow()
        {
            var window = (MagicLeapProjectValidationWindow) GetWindow(typeof(MagicLeapProjectValidationWindow));
            window.Close();
        }

        private static void InitStyles()
        {
            if (Styles.s_ListLabel != null)
                return;

            Styles.s_ListLabel = new GUIStyle(Styles.s_SelectionStyle)
            {
                border = new RectOffset(0, 0, 0, 0),
                padding = new RectOffset(5, 5, 5, 5),
                margin = new RectOffset(5, 5, 5, 5)
            };

            Styles.s_IssuesTitleLabel = new GUIStyle(EditorStyles.label)
            {
                fontSize = 14,
                fontStyle = FontStyle.Bold,
                padding = new RectOffset(10, 10, 0, 0)
            };

            Styles.s_Wrap = new GUIStyle(EditorStyles.label)
            {
                wordWrap = true,
                alignment = TextAnchor.MiddleLeft,
                padding = new RectOffset(0, 5, 1, 1)
            };

            Styles.s_Icon = new GUIStyle(EditorStyles.label)
            {
                margin = new RectOffset(5, 5, 0, 0),
                fixedWidth = Content.k_IconSize.x * 2
            };

            Styles.s_InfoBanner = new GUIStyle(EditorStyles.label)
            {
                padding = new RectOffset(10, 10, 15, 5)
            };

            Styles.s_FixAll = new GUIStyle(EditorStyles.miniButton)
            {
                stretchWidth = false,
                fixedWidth = 80,
                margin = new RectOffset(0, 10, 2, 2)
            };
        }

        private readonly List<MagicLeapProjectValidation.ValidationRule> m_Failures = new List<MagicLeapProjectValidation.ValidationRule>();

        // Fix all state
        private List<MagicLeapProjectValidation.ValidationRule> m_FixAllStack = new List<MagicLeapProjectValidation.ValidationRule>();

        /// <summary>
        /// Last time the issues in the window were updated
        /// </summary>
        private double m_LastUpdate;

        /// <summary>
        /// Interval that that issues should be updated
        /// </summary>
        private const double UpdateInterval = 1.0;

        /// <summary>
        /// Interval that that issues should be updated when the window does not have focus
        /// </summary>
        private const double BackgroundUpdateInterval = 3.0;

        private static class Content
        {
            public static readonly GUIContent k_WarningIcon = EditorGUIUtility.IconContent("Warning@2x");
            public static readonly GUIContent k_ErrorIcon = EditorGUIUtility.IconContent("Error@2x");
            public static readonly GUIContent k_HelpIcon = EditorGUIUtility.IconContent("_Help@2x");
            public static readonly GUIContent k_Title = new GUIContent(" MagicLeap Project Validation", "");
            public static readonly GUIContent k_FixButton = new GUIContent("Fix", "");
            public static readonly GUIContent k_EditButton = new GUIContent("Edit", "");
            public static readonly GUIContent k_PlayMode = new GUIContent("  Exit play mode before fixing project validation issues.", EditorGUIUtility.IconContent("console.infoicon").image);
            public static readonly GUIContent k_HelpButton = new GUIContent(k_HelpIcon.image);
            public static readonly Vector2 k_IconSize = new Vector2(16.0f, 16.0f);
        }

        private static class Styles
        {
            public static GUIStyle s_SelectionStyle = "TV Selection";
            public static GUIStyle s_IssuesBackground = "ScrollViewAlt";
            public static GUIStyle s_ListLabel;
            public static GUIStyle s_IssuesTitleLabel;
            public static GUIStyle s_Wrap;
            public static GUIStyle s_Icon;
            public static GUIStyle s_InfoBanner;
            public static GUIStyle s_FixAll;
        }

        private static bool s_Dirty = true;
        private static BuildTargetGroup s_SelectedBuildTargetGroup = BuildTargetGroup.Unknown;

        protected void OnFocus() => UpdateIssues(true);

        protected void Update() => UpdateIssues();

        private void DrawIssuesList()
        {
            var hasFix = m_Failures.Any(f => f.FixIt != null);
            var hasAutoFix = hasFix && m_Failures.Any(f => f.FixIt != null && f.FixItAutomatic);

            // Header
            EditorGUILayout.BeginHorizontal();
            using (new EditorGUI.DisabledScope(EditorApplication.isPlaying))
            {
                EditorGUILayout.LabelField($"Issues ({m_Failures.Count})", Styles.s_IssuesTitleLabel);
            }

            // FixAll button
            if (hasAutoFix)
            {
                using (new EditorGUI.DisabledScope(EditorApplication.isPlaying || m_FixAllStack.Count > 0))
                {
                    if (GUILayout.Button("Fix All", Styles.s_FixAll))
                        m_FixAllStack = m_Failures.Where(i => i.FixIt != null && i.FixItAutomatic).ToList();
                }
            }
            EditorGUILayout.EndHorizontal();

            m_ScrollViewPos = EditorGUILayout.BeginScrollView(m_ScrollViewPos, Styles.s_IssuesBackground, GUILayout.ExpandHeight(true));

            using (new EditorGUI.DisabledScope(EditorApplication.isPlaying))
            {
                foreach (var result in m_Failures)
                {
                    EditorGUILayout.BeginHorizontal(Styles.s_ListLabel);

                    GUILayout.Label(result.Error ? Content.k_ErrorIcon : Content.k_WarningIcon, Styles.s_Icon, GUILayout.Width(Content.k_IconSize.x));

                    string message = result.Message;
                    GUILayout.Label(message, Styles.s_Wrap);
                    GUILayout.FlexibleSpace();

                    if (!string.IsNullOrEmpty(result.HelpText) || !string.IsNullOrEmpty(result.HelpLink))
                    {
                        Content.k_HelpButton.tooltip = result.HelpText;
                        if (GUILayout.Button(Content.k_HelpButton, Styles.s_Icon, GUILayout.Width(Content.k_IconSize.x * 1.5f)) &&
                            !string.IsNullOrEmpty(result.HelpLink))
                        {
                            UnityEngine.Application.OpenURL(result.HelpLink);
                        }
                    }
                    else
                        GUILayout.Label("", GUILayout.Width(Content.k_IconSize.x * 1.5f));


                    if (result.FixIt != null)
                    {
                        using (new EditorGUI.DisabledScope(m_FixAllStack.Count != 0))
                        {
                            var button = result.FixItAutomatic ? Content.k_FixButton : Content.k_EditButton;
                            button.tooltip = result.FixItMessage;
                            if (GUILayout.Button(button, GUILayout.Width(80.0f)))
                            {
                                if (result.FixItAutomatic)
                                    m_FixAllStack.Add(result);
                                else
                                    result.FixIt();
                            }
                        }
                    }
                    else if (hasFix)
                    {
                        GUILayout.Label("", GUILayout.Width(80.0f));
                    }


                    EditorGUILayout.EndHorizontal();
                }
            }

            EditorGUILayout.EndScrollView();
        }

        private void UpdateIssues(bool force = false)
        {
            var interval = EditorWindow.focusedWindow == this ? UpdateInterval : BackgroundUpdateInterval;
            if (!s_Dirty && !force && EditorApplication.timeSinceStartup - m_LastUpdate < interval)
                return;

            s_Dirty = false;

            if (m_FixAllStack.Count > 0)
            {
                m_FixAllStack[0].FixIt?.Invoke();
                m_FixAllStack.RemoveAt(0);
            }
            var failureCount = m_Failures.Count;

            MagicLeapProjectValidation.GetCurrentValidationIssues(m_Failures);

            // Repaint the window if the failure count has changed
            if(m_Failures.Count > 0 || failureCount > 0)
                Repaint();

            m_LastUpdate = EditorApplication.timeSinceStartup;
        }

        public void OnGUI()
        {
            InitStyles();

            EditorGUIUtility.SetIconSize(Content.k_IconSize);
            EditorGUILayout.BeginVertical();

            if (EditorApplication.isPlaying && m_Failures.Count > 0)
            {
                GUILayout.Label(Content.k_PlayMode, Styles.s_InfoBanner);
            }

            EditorGUILayout.Space();

            DrawIssuesList();

            EditorGUILayout.EndVertical();
        }
    }
#endif // PROJECT_VALIDATION_AVAILABLE

    internal class MagicLeapProjectValidationBuildStep : IPreprocessBuildWithReport
    {
        public int callbackOrder { get; }

        public void OnPreprocessBuild(BuildReport report)
        {
            if (!BuildHelperUtils.HasLoader(report.summary.platformGroup, typeof(MagicLeapLoader)))
                return;

            if (report.summary.platformGroup == BuildTargetGroup.Android && MagicLeapProjectValidation.LogBuildValidationIssues())
                throw new BuildFailedException("MagicLeap Build Failed.");
        }
    }
}
