using UnityEditor;
using UnityEngine;

namespace Gpm.Manager.Ui.Helper
{
    internal static class ManagerUiStyle
    {
        private static readonly Color OVERLAY_COLOR = new Color(0.2f, 0.2f, 0.2f, 0.5f);

        public static readonly GUIStyle CopyrightBox;

        public static readonly GUIStyle WordWrapLabel;
        public static readonly GUIStyle ListLabel;
        public static readonly GUIStyle ListVersionLabel;
        public static readonly GUIStyle IconLabel;
        public static readonly GUIStyle MainTitleLabel;
        public static readonly GUIStyle TitleLabel;
        public static readonly GUIStyle RightAlignLabel;
        public static readonly GUIStyle NoticeLabel;
        public static readonly GUIStyle LinkLabel;
        public static readonly GUIStyle CopyrightLabel;
        public static readonly GUIStyle MiddleCenterLabel;
        public static readonly GUIStyle WarningVersionLabel;
        public static readonly GUIStyle ImageTitleLabel;
        public static readonly GUIStyle ToastLabel;

        public static readonly GUIStyle ToolbarButton;
        public static readonly GUIStyle ToolbarPopup;

        public static readonly GUIStyle ListNormalButton;
        public static readonly GUIStyle ListSelectedButton;
        public static readonly GUIStyle InfoButton;
        public static readonly GUIStyle ImageScrollButton;

        private static Texture2D overlayTexture;

        public static Texture2D OverlayTexture
        {
            get
            {
                if (overlayTexture == null)
                {
                    overlayTexture = new Texture2D(1, 1);
                    overlayTexture.SetPixel(0, 0, OVERLAY_COLOR);
                    overlayTexture.hideFlags = HideFlags.HideAndDontSave;
                    overlayTexture.name = "OverlayTexture";
                    overlayTexture.Apply();
                }

                return overlayTexture;
            }
        }

        static ManagerUiStyle()
        {
            #region Box

            CopyrightBox = new GUIStyle(GUI.skin.FindStyle("ProgressBarBack"))
            {
                alignment = TextAnchor.LowerCenter,
                margin = new RectOffset(0, 0, 0, 0),
                padding = new RectOffset(0, 0, 0, 0),
                stretchHeight = true,
                stretchWidth = true
            };

            #endregion

            #region Label

            WordWrapLabel = new GUIStyle(GUI.skin.label)
            {
                wordWrap = true
            };

            ListLabel = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleLeft,
                fontSize = 12,
                fixedWidth = 178,
                fixedHeight = 20,
                stretchWidth = false
            };

            ListVersionLabel = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleRight,
                fontSize = 9,
                wordWrap = true,
                fixedWidth = 44,
                fixedHeight = 20,
                margin = new RectOffset(4, 4, 4, 4),
            };

            IconLabel = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                padding = new RectOffset(4, 4, 4, 4),
                stretchWidth = false,
                stretchHeight = true
            };

            MainTitleLabel = new GUIStyle(GUI.skin.label)
            {
                fontStyle = FontStyle.Bold,
                fontSize = 18
            };

            TitleLabel = new GUIStyle(GUI.skin.label)
            {
                fontStyle = FontStyle.Bold,
                fontSize = 15
            };

            RightAlignLabel = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleRight
            };

            NoticeLabel = new GUIStyle(GUI.skin.label)
            {
                richText = true,
                stretchWidth = false,
                hover = { textColor = new Color(0.31f, 0.5f, 0.972f) }
            };

            LinkLabel = new GUIStyle(GUI.skin.label)
            {
                richText = true,
                stretchWidth = false,
                normal = { textColor = new Color(0.31f, 0.5f, 0.972f) }
            };

            CopyrightLabel = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 10,
                normal = { textColor = Color.gray}
            };

            MiddleCenterLabel = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                wordWrap = true,
                stretchHeight = true
            };

            WarningVersionLabel = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleRight,
                margin = new RectOffset(4, 4, 4, 4),
                normal = { textColor = new Color(1, 0.35f, 0.35f) }
            };

            ImageTitleLabel = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.UpperCenter,
                fontStyle = FontStyle.Bold,
                fontSize = 15
            };

            ToastLabel = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                wordWrap = true,
                stretchHeight = true,
                normal = { textColor = new Color(0.31f, 0.5f, 0.972f) }
            };

            #endregion

            ToolbarButton = EditorStyles.toolbarButton;
            ToolbarButton.stretchWidth = false;

            ToolbarPopup = EditorStyles.toolbarPopup;

            #region Button
            var ListButton = new Texture2D(1, 1);
            ListButton.SetPixel(0, 0, new Color(0.31f, 0.5f, 0.972f, 0.5f));
            ListButton.hideFlags = HideFlags.HideAndDontSave;
            ListButton.name = "OverlayTexture";
            ListButton.Apply();

            ListNormalButton = new GUIStyle(EditorStyles.toolbarButton)
            {
                margin = new RectOffset(0, 0, 0, 0),
                padding = new RectOffset(0, 0, 0, 0),
                fixedHeight = 0f,
                normal = { background = null }
            };

            ListSelectedButton = new GUIStyle(EditorStyles.toolbarButton)
            {
                margin = new RectOffset(0, 0, 0, 0),
                padding = new RectOffset(0, 0, 0, 0),
                fixedHeight = 0f,
                normal = { background = ListButton }
            };

            InfoButton = new GUIStyle()
            {
                fixedWidth = 16,
                fixedHeight = 16,
                alignment = TextAnchor.MiddleCenter,
                margin = new RectOffset(4, 0, 2, 0),
                padding = new RectOffset(6, 6, 3, 3),
                border = new RectOffset(6, 6, 6, 4),
                normal = { background = ManagerUiIcon.INFOMATION_ICON },
                hover = { background = ManagerUiIcon.INFOMATION_ICON },
                active = { background = ManagerUiIcon.INFOMATION_ICON }
            };

            var scrollButtonTexture = new Texture2D(1, 1);
            scrollButtonTexture.SetPixel(0, 0, new Color(0.4f, 0.4f, 0.4f, 0.5f));
            scrollButtonTexture.hideFlags = HideFlags.HideAndDontSave;
            scrollButtonTexture.name = "ScrollButtonTexture";
            scrollButtonTexture.Apply();

            ImageScrollButton = new GUIStyle(GUI.skin.button)
            {
                alignment = TextAnchor.MiddleCenter,
                margin = new RectOffset(0, 0, 0, 0),
                padding = new RectOffset(0, 0, 0, 0),
                normal = { background = scrollButtonTexture },
                hover = { background = scrollButtonTexture },
                active = { background = scrollButtonTexture }
            };

            #endregion
        }
    }
}