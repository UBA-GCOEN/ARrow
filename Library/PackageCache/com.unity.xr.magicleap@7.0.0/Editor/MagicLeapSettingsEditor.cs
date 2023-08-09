using System;
using UnityEditor;
using UnityEngine;

using UnityEngine.XR.MagicLeap;
using UnityEditor.XR.Management;

namespace UnityEditor.XR.MagicLeap
{
    [CustomPropertyDrawer(typeof(DisabledAttribute))]
    internal class DisabledDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            using (new EditorGUI.DisabledScope(true))
                EditorGUI.PropertyField(position, property, label);
        }
    }

    /// <summary>Custom editor settings support for this XR Plugin.</summary>
    [CustomEditor(typeof(MagicLeapSettings))]
    public class MagicLeapSettingsEditor : UnityEditor.Editor
    {
        static GUIContent s_EnableMLAudioLabel = new GUIContent("Use MLAudio");
        static GUIContent s_ShowGraphicsSettingsLabel = new GUIContent("Graphics Settings");
        static GUIContent s_ShowAudioSettingsLabel = new GUIContent("Audio Settings");

        Vector2 m_ScrollPos = Vector2.zero;
        private bool m_ShowGraphicsSettings = true;
        private bool m_ShowAudioSettings = true;

        /// <summary>
        /// GUI for MagicLeapSettingsEditor class.
        /// </summary>
        public override void OnInspectorGUI()
        {
            if (serializedObject == null || serializedObject.targetObject == null)
                return;

            MagicLeapSettings settings = serializedObject.targetObject as MagicLeapSettings;

            BuildTargetGroup buildTargetGroup = EditorGUILayout.BeginBuildTargetSelectionGrouping();

            m_ScrollPos = EditorGUILayout.BeginScrollView(m_ScrollPos);

            if (buildTargetGroup == BuildTargetGroup.Android)
            {
                serializedObject.Update();
                
                m_ShowGraphicsSettings = EditorGUILayout.Foldout(m_ShowGraphicsSettings, s_ShowGraphicsSettingsLabel);
                if (m_ShowGraphicsSettings)
                {
                    EditorGUI.indentLevel++;
                    DrawPropertiesExcluding(serializedObject, "m_EnableMLAudio");
                    EditorGUI.indentLevel--;
                }
                
                
                m_ShowAudioSettings = EditorGUILayout.Foldout(m_ShowAudioSettings, s_ShowAudioSettingsLabel);

                if (m_ShowAudioSettings)
                {
                    EditorGUI.indentLevel++;
                    DrawPropertiesExcluding(serializedObject, new string[] { "m_Script", "m_DepthPrecision", "m_ForceMultipass", "m_HeadlockGraphics" });
                    EditorGUI.indentLevel--;
                }
                
                serializedObject.ApplyModifiedProperties();
            }
            else
            {
                EditorGUILayout.HelpBox("Settings for this package are unsupported for this target platform.", MessageType.Info);
            }
            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndBuildTargetSelectionGrouping();
        }
    }
}
