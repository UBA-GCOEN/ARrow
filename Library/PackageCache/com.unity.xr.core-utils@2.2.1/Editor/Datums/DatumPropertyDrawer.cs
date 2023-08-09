using UnityEditor;
using UnityEngine;

namespace Unity.XR.CoreUtils.Datums.Editor
{
    /// <summary>
    /// Base <see cref="DatumProperty{TValue,TDatum}"/> drawer used to better present the options between constants and <see cref="ScriptableObject"/> references.
    /// </summary>
    /// <seealso cref="PropertyDrawer"/>
    /// <seealso cref="DatumProperty{TValue,TDatum}"/>
    public abstract class DatumPropertyDrawer : PropertyDrawer
    {
        readonly string[] m_PopupOptions = { "Use Asset", "Use Value" };

        GUIStyle m_PopupStyle;

        /// <summary>
        /// Calculates the height of the property based on the number of children of the property.
        /// </summary>
        /// <param name="property">The SerializedProperty to make the custom GUI for.</param>
        /// <param name="label">The label of this property.</param>
        /// <returns>The height in pixels.</returns>
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            SerializedProperty selectedValue = GetSelectedProperty(property);
            if (selectedValue.hasVisibleChildren)
            {
                return EditorGUIUtility.singleLineHeight * (selectedValue.CountInProperty() + 1);
            }

            return base.GetPropertyHeight(property, label);
        }

        /// <summary>
        /// Draws the property using [immediate mode](xref:GUIScriptingGuide).
        /// </summary>
        /// <param name="position">Rectangle on the screen to use for the property GUI.</param>
        /// <param name="property">The SerializedProperty to make the custom GUI for.</param>
        /// <param name="label">The label of this property.</param>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (m_PopupStyle == null)
            {
                m_PopupStyle = new GUIStyle(UnityEngine.GUI.skin.GetStyle("PaneOptions")) { imagePosition = ImagePosition.ImageOnly };
            }

            label = EditorGUI.BeginProperty(position, label, property);
            position = EditorGUI.PrefixLabel(position, label);

            EditorGUI.BeginChangeCheck();

            SerializedProperty useConstant = property.FindPropertyRelative("m_UseConstant");
            SerializedProperty selectedValue = GetSelectedProperty(property);

            // Calculate rect for configuration button
            Rect buttonRect = new Rect(position);
            buttonRect.yMin += m_PopupStyle.margin.top;
            buttonRect.width = m_PopupStyle.fixedWidth + m_PopupStyle.margin.right;
            position.xMin = buttonRect.xMax;

            // nudge foldout arrow to right a little more
            if (selectedValue.hasVisibleChildren) position.xMin += 12;

            // Store old indent level and set it to 0, the PrefixLabel takes care of it
            int indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            int result = EditorGUI.Popup(buttonRect, useConstant.boolValue ? 1 : 0, m_PopupOptions, m_PopupStyle);

            useConstant.boolValue = result == 1;

            EditorGUI.PropertyField(position, selectedValue, GUIContent.none, true);

            if (EditorGUI.EndChangeCheck())
                property.serializedObject.ApplyModifiedProperties();

            EditorGUI.indentLevel = indent;
            EditorGUI.EndProperty();
        }

        /// <summary>
        /// Gets the property that represents the correct variable reference value.
        /// </summary>
        /// <param name="property">The SerializedProperty that contains the variable reference values.</param>
        /// <returns>If the VariableReference is set to use a constant value, the constant value will be returned,
        /// otherwise the variable value will be returned.</returns>
        /// <seealso cref="DatumProperty{TValue,TDatum}"/>
        protected SerializedProperty GetSelectedProperty(SerializedProperty property)
        {
            SerializedProperty useConstant = property.FindPropertyRelative("m_UseConstant");
            SerializedProperty constantValue = property.FindPropertyRelative("m_ConstantValue");
            SerializedProperty variable = property.FindPropertyRelative("m_Variable");
            return useConstant.boolValue ? constantValue : variable;
        }
    }
}
