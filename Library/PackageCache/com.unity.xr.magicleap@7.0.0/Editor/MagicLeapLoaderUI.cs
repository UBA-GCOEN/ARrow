using System.Collections.Generic;
using System.Linq;
using UnityEditor.XR.Management;
using UnityEngine;
using UnityEngine.XR.MagicLeap;

namespace UnityEditor.XR.MagicLeap
{
    internal static class Content
    {
        public const float k_IconSize = 16.0f;

        public static readonly GUIContent k_WarningIcon = EditorGUIUtility.IconContent("Warning@2x");
        public static readonly GUIContent k_ErrorIcon = EditorGUIUtility.IconContent("Error@2x");
        public static readonly GUIContent k_HelpIcon = EditorGUIUtility.IconContent("_Help@2x");
        public static readonly string k_HelpUri = "https://docs.unity3d.com/Packages/com.unity.xr.magicleap@7.0/manual/index.html";

        public static readonly GUIContent k_LoaderName = new GUIContent("Magic Leap");
        public static readonly GUIContent k_Validation = new GUIContent("Your project has some settings that are incompatible with Magic Leap. Click to open the project validator.");
        public static readonly GUIContent k_ValidationErrorIcon = new GUIContent("", k_ErrorIcon.image, k_Validation.text);
        public static readonly GUIContent k_ValidationWarningIcon = new GUIContent("", k_WarningIcon.image, k_Validation.text);

        public static readonly GUIContent k_HelpContent = new GUIContent("", k_HelpIcon.image, "Magic Leap support.");
    }

    [XRCustomLoaderUI("UnityEngine.XR.MagicLeap.MagicLeapLoader", BuildTargetGroup.Android)]
    public class MagicLeapLoaderUI : IXRCustomLoaderUI
    {
        protected float renderLineHeight = 0.0f;
        protected Vector2 m_IconSize = new Vector2(Content.k_IconSize, Content.k_IconSize);
        public float RequiredRenderHeight { get; protected set; }

        private List<MagicLeapProjectValidation.ValidationRule> _validationRules = new List<MagicLeapProjectValidation.ValidationRule>();

        protected Rect CalculateRectForContent(float xMin, float yMin, GUIStyle style, GUIContent content)
        {
            var size = style.CalcSize(content);
            var rect = new Rect();
            rect.xMin = xMin;
            rect.yMin = yMin;
            rect.width = size.x;
            rect.height = renderLineHeight;
            return rect;
        }

        public void SetRenderedLineHeight(float height)
        {
            renderLineHeight = height;
            RequiredRenderHeight = height;
        }

        public void OnGUI(Rect rect)
        {
            var oldIconSize = EditorGUIUtility.GetIconSize();
            EditorGUIUtility.SetIconSize(m_IconSize);

            float xMin = rect.xMin;
            float yMin = rect.yMin;

            var labelRect = CalculateRectForContent(xMin, yMin, EditorStyles.toggle, Content.k_LoaderName);
            var newToggled = EditorGUI.ToggleLeft(labelRect, Content.k_LoaderName, IsLoaderEnabled);
            if (newToggled != IsLoaderEnabled)
            {
                IsLoaderEnabled = newToggled;
            }
            xMin = labelRect.xMax + 1.0f;

            if (IsLoaderEnabled)
            {
                var iconRect = CalculateRectForContent(xMin, yMin, EditorStyles.label, Content.k_HelpContent);
                if (GUI.Button(iconRect, Content.k_HelpContent, EditorStyles.label))
                {
                    System.Diagnostics.Process.Start(Content.k_HelpUri);
                }

                xMin += Content.k_IconSize + 1.0f;

                MagicLeapProjectValidation.GetCurrentValidationIssues(_validationRules);

                if (_validationRules.Count > 0)
                {
                    bool anyErrors = _validationRules.Any(rule => rule.error);
                    GUIContent icon = anyErrors ? Content.k_ValidationErrorIcon : Content.k_ValidationWarningIcon;
                    iconRect = CalculateRectForContent(xMin, yMin, EditorStyles.label, icon);

                    if (GUI.Button(iconRect, icon, EditorStyles.label))
                    {
                        MagicLeapProjectValidationRulesSetup.ShowWindow(ActiveBuildTargetGroup);
                    }
                }
            }
            EditorGUIUtility.SetIconSize(oldIconSize);
        }

        public bool IsLoaderEnabled { get; set; }

        /// <summary>
        /// Currently, we don't have any Incompatible loaders with Magic Leap.
        /// If we do run into any, we add the names of the loaders here.
        /// </summary>
        public string[] IncompatibleLoaders => new string[] { };

        public BuildTargetGroup ActiveBuildTargetGroup { get; set; }
    }
}
