using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace UnityEditor.XR.MagicLeap
{
    internal static class PlatformLevelSelector
    {
        public static int SelectorGUI(int value)
        {
            if (SDKUtility.sdkAvailable)
                return EditorGUILayout.IntPopup("Minimum API Level",
                    EnsureValidValue(value),
                    GetChoices().Select(c => $"API Level {c}").ToArray(),
                    GetChoices().ToArray());
            else
                using (new EditorGUI.DisabledScope(true))
                    return EditorGUILayout.IntPopup("Minimum API Level",
                        value,
                        new string[] { $"API Level {value}" },
                        new int[] { value });
        }

        public static IEnumerable<int> GetChoices()
        {
            int min = SDKUtility.pluginAPILevel;
            int max = SDKUtility.sdkAPILevel;
            for (int i = min; i <= max; i++)
                yield return i;
        }

        public static int EnsureValidValue(int input)
        {
            var max = GetChoices().Max();
            var min = GetChoices().Min();
            if (input < min)
                return min;
            if (input > max)
                return max;
            return input;
        }
    }
}