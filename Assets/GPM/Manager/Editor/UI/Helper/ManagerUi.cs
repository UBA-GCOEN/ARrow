using Gpm.Common.Log;
using Gpm.Common.Multilanguage;
using Gpm.Manager.Constant;
using Gpm.Manager.Internal;
using UnityEditor;
using UnityEngine;

namespace Gpm.Manager.Ui.Helper
{
    internal static partial class ManagerUi
    {
        public static void Label(string key, params GUILayoutOption[] options)
        {
            GUILayout.Label(ManagerInfos.GetString(key), options);
        }
        public static void Label(string key, GUIStyle style, params GUILayoutOption[] options)
        {
            GUILayout.Label(ManagerInfos.GetString(key), style, options);
        }

        public static void LabelValue(string text, params GUILayoutOption[] options)
        {
            GUILayout.Label(GetValueString(text), options);
        }
        public static void LabelValue(string text, GUIStyle style, params GUILayoutOption[] options)
        {
            GUILayout.Label(GetValueString(text), style, options);
        }
        public static void LabelValue(Rect rect, string text, GUIStyle style)
        {
            GUI.Label(rect, GetValueString(text), style);
        }

        public static void LabelValue(Texture2D texture, params GUILayoutOption[] options)
        {
            GUIContent content = new GUIContent
            {
                image = texture,
            };

            GUILayout.Label(content, ManagerUiStyle.IconLabel, options);
        }


        public static bool Button(string key, params GUILayoutOption[] options)
        {
            return GUILayout.Button(ManagerInfos.GetString(key), options);
        }
        public static bool Button(string key, GUIStyle style, params GUILayoutOption[] options)
        {
            return GUILayout.Button(ManagerInfos.GetString(key), style, options);
        }
        public static bool Button(Rect rect, string key, GUIStyle style)
        {
            return GUI.Button(rect, ManagerInfos.GetString(key), style);
        }

        public static bool InfoButton(params GUILayoutOption[] options)
        {
            return GUILayout.Button(string.Empty, ManagerUiStyle.InfoButton, options);
        }
        public static bool LabelButton(string text, params GUILayoutOption[] options)
        {
            return GUILayout.Button(GetValueString(text), ManagerUiStyle.LinkLabel, options);
        }

        public static int Popup(int selectedIndex, string[] displayedOptions, GUIStyle style, params GUILayoutOption[] options)
        {
            string[] strings = new string[displayedOptions.Length];
            for (int i = 0; i < displayedOptions.Length; i++)
            {
                strings[i] = ManagerInfos.GetString(displayedOptions[i]);
            }

            return EditorGUILayout.Popup(selectedIndex, strings, style, options);
        }

        public static int PopupValue(int selectedIndex, string[] strings, GUIStyle style, params GUILayoutOption[] options)
        {
            return EditorGUILayout.Popup(selectedIndex, strings, style, options);
        }

        public static bool Dialog(string titleKey, string message, bool isMessageKey = true)
        {
            return EditorUtility.DisplayDialog(
                ManagerInfos.GetString(titleKey),
                isMessageKey ? ManagerInfos.GetString(message) : message,
                ManagerInfos.GetString(ManagerStrings.POPUP_OK));
        }

        public static bool TryDialog(string titleKey, string message, bool isMessageKey = true)
        {
            return EditorUtility.DisplayDialog(
                ManagerInfos.GetString(titleKey),
                isMessageKey ? ManagerInfos.GetString(message) : message,
                ManagerInfos.GetString(ManagerStrings.POPUP_OK),
                ManagerInfos.GetString(ManagerStrings.POPUP_CANCEL));
        }

        public static bool ErrorDialog(ManagerError error)
        {
            if (error == null)
            {
                return true;
            }

            string message;
            if (error.IsFullMessage == true)
            {
                message = error.Message;
            }
            else
            {
                message = ManagerInfos.GetString(error.Message);
            }

            if (string.IsNullOrEmpty(error.SubMessage) == false)
            {
                message = string.Format("{0}\n{1}", message,
                    string.Format(ManagerInfos.GetString(ManagerStrings.ERROR_MESSAGE_CUSTOM_MESSAGE), error.SubMessage));
            }

            
            if (error.IsOpenDialog == false)
            {
                GpmLogger.Error(string.Format("{0} - {1}", error.ErrorCode, message), ManagerInfos.SERVICE_NAME, typeof(ManagerUi));
                return true;
            }

            return EditorUtility.DisplayDialog(
                ManagerInfos.GetString(ManagerStrings.GetErrorCode(error.ErrorCode)),
                message,
                ManagerInfos.GetString(ManagerStrings.POPUP_OK));
        }

        public static Rect Window(int id, Rect screenRect, GUI.WindowFunction func, string text, params GUILayoutOption[] options)
        {
            return GUILayout.Window(id, screenRect, func, ManagerInfos.GetString(text), options);
        }

        public static GUIContent GetContent(string key)
        {
            return new GUIContent(ManagerInfos.GetString(key));
        }

        public static string GetValueString(string text)
        {
            if (string.IsNullOrEmpty(text) == false && text.StartsWith(ManagerInfos.SERVICE_INFO_MULTILANGUAGE_SEPARATOR) == true)
            {
                return GpmMultilanguage.GetString(GpmManager.Instance.ServiceLanguageName, text);
            }

            return text;
        }
    }
}
