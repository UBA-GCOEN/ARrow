using UnityEngine;
using UnityEditor;

namespace Gpm.Manager.Ui.Helper
{
    internal static partial class ManagerUi
    {
        private static readonly Texture2D splitTexture;

        static ManagerUi()
        {
            splitTexture = new Texture2D(1, 1);
            splitTexture.SetPixel(0, 0, new Color(0.16f, 0.16f, 0.16f));
            splitTexture.hideFlags = HideFlags.HideAndDontSave;
            splitTexture.name = "SplitTexture";
            splitTexture.Apply();
        }

        internal static void DrawHorizontalSplitter()
        {
            Rect splitterRect = EditorGUILayout.GetControlRect(false, 1);
            GUI.DrawTexture(splitterRect, splitTexture);
        }

        internal static void DrawHorizontalSplitter(float x, float y, float length)
        {
            if (Event.current.type != EventType.Repaint)
            {
                return;
            }

            Rect splitterRect = new Rect(x - 1, y, 1, length);
            GUI.DrawTexture(splitterRect, splitTexture);
        }

        public static void DrawVerticalSplitter(float x, float y, float length)
        {
            if (Event.current.type != EventType.Repaint)
            {
                return;
            }

            Rect splitterRect = new Rect(x, y - 1, length, 1);
            GUI.DrawTexture(splitterRect, splitTexture);
        }
    }
}
