using UnityEditor;
using UnityEngine;

namespace Gpm.Manager.Ui.Helper
{
    internal static class ManagerUiIcon
    {
        public const string WAIT_SPIN = "WaitSpin";

        public static readonly Texture2D DOT_ICON = EditorGUIUtility.FindTexture("winbtn_win_min");
        public static readonly Texture2D CHECK_ICON = EditorGUIUtility.FindTexture("CollabNew");
        public static readonly Texture2D REFRESH_ICON = EditorGUIUtility.FindTexture("d_Refresh");
        public static readonly Texture2D INFOMATION_ICON = EditorGUIUtility.FindTexture("ClothInspector.SelectTool");
        public static readonly Texture2D NEXT_ICON = EditorGUIUtility.FindTexture("tab_next@2x");
        public static readonly Texture2D PREV_ICON = EditorGUIUtility.FindTexture("tab_prev@2x");

        private static Texture2D[] statusWheel;
        private static int statusWheelFrame;

        public static Texture2D StatusWheel
        {
            get
            {
                if (statusWheel == null)
                {
                    statusWheel = new Texture2D[12];
                    for (int i = 0; i < 12; i++)
                    {
                        statusWheel[i] = EditorGUIUtility.Load(WAIT_SPIN + i.ToString("00")) as Texture2D;
                    }
                }

                statusWheelFrame = (int)Mathf.Repeat(Time.realtimeSinceStartup * 10, 11.99f);

                if (statusWheelFrame == 12)
                {
                    statusWheelFrame = 0;
                }

                return statusWheel[statusWheelFrame++];
            }
        }
    }
}
