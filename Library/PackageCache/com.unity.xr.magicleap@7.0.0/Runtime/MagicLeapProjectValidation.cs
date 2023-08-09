using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine.Rendering;

#if UNITY_EDITOR
using System.Reflection;
using UnityEditor;
using UnityEditor.Build;
#endif

[assembly:InternalsVisibleTo("Unity.XR.MagicLeap.Editor")]
namespace UnityEngine.XR.MagicLeap
{
#if UNITY_EDITOR
    public static class MagicLeapProjectValidation
    {
        /// <summary>
        /// A Build-time validation rule.
        /// </summary>
        public class ValidationRule
        {
            internal ValidationRule()
            {}

            /// <summary>
            /// Message describing the rule that will be showed to the developer if it fails.
            /// </summary>
            public string message;

            /// <summary>
            /// Lambda function that returns true if validation passes, false if validation fails.
            /// </summary>
            public Func<bool> checkPredicate;

            /// <summary>
            /// Lambda function that fixes the issue, if possible.
            /// </summary>
            public Action fixIt;

            /// <summary>
            /// Text describing how the issue is fixed, shown in a tooltip.
            /// </summary>
            public string fixItMessage;

            /// <summary>
            /// True if the fixIt Lambda function performs a function that is automatic and does not require user input.  If your fixIt
            /// function requires user input, set fixitAutomatic to false to prevent the fixIt method from being executed during fixAll
            /// </summary>
            public bool fixItAutomatic = true;

            /// <summary>
            /// If true, failing the rule is treated as an error and stops the build.
            /// If false, failing the rule is treated as a warning and it doesn't stop the build. The developer has the option to correct the problem, but is not required to.
            /// </summary>
            public bool error;

            /// <summary>
            /// If true, will deny the project from entering playmode in editor.
            /// If false, can still enter playmode in editor if this issue isn't fixed.
            /// </summary>
            public bool errorEnteringPlaymode;

            /// <summary>
            /// Optional text to display in a help icon with the issue in the validator.
            /// </summary>
            public string helpText;

            /// <summary>
            /// Optional link that will be opened if the help icon is clicked.
            /// </summary>
            public string helpLink;
        }


        private static readonly bool s_isTextureCompressionAPIOk;
        private static readonly MethodInfo s_GetDefaultTextureCompressionFormat;
        private static int s_DXTCEnumValue;
        private static int s_DXTC_RGTCEnumValue;
        static MagicLeapProjectValidation()
        {
            // Texture compression is internal, as long as the API don't change, we can use it
            // The code will gracefully ignore this if the API changes
            s_GetDefaultTextureCompressionFormat = TryGetPlayerSettingsMethod("GetDefaultTextureCompressionFormat");
            s_isTextureCompressionAPIOk = TryGetDXTCEnum();

            // Since we are using internal API via reflection, validate the athe API is still the same
            s_isTextureCompressionAPIOk &= IsDefaultTextureCompressionAPIValid();
        }

        private static bool TryGetDXTCEnum()
        {
            s_DXTCEnumValue = -1;
            s_DXTC_RGTCEnumValue = -1;
            try
            {
                var textureCompressionFormatEnum = Type.GetType("UnityEditor.TextureCompressionFormat,UnityEditor.dll");
                if (textureCompressionFormatEnum != null && textureCompressionFormatEnum.IsEnum)
                {
                    string[] enumNames = textureCompressionFormatEnum.GetEnumNames();
                    Array enumValues = textureCompressionFormatEnum.GetEnumValues();
                    for (int i = 0; i < enumValues.Length; ++i)
                    {
                        if (enumNames[i] == "DXTC")
                            s_DXTCEnumValue = Convert.ToInt32(enumValues.GetValue(i));
                        if (enumNames[i] == "DXTC_RGTC")
                            s_DXTC_RGTCEnumValue = Convert.ToInt32(enumValues.GetValue(i));
                    }
                }
                return s_DXTCEnumValue != -1 && s_DXTC_RGTCEnumValue != -1;
            }
            catch (Exception)
            {
                // ignored
            }
            return false;
        }

        private static MethodInfo TryGetPlayerSettingsMethod(string methodName)
        {
            MethodInfo playerSettingsMethod = null;
            try
            {
                var playerSettingsType = Type.GetType("UnityEditor.PlayerSettings,UnityEditor.dll");
                playerSettingsMethod = playerSettingsType?.GetMethod(methodName, BindingFlags.Static | BindingFlags.NonPublic);
            }
            catch (Exception)
            {
                //Simply return null if something failed in the reflection.
            }

            return playerSettingsMethod;
        }

        private static bool ValidateEnumParameter(ParameterInfo param, string enumName, string parameterName)
        {
            return param.Name == parameterName && param.ParameterType.Name == enumName && param.ParameterType.IsEnum;
        }

        private static bool IsDefaultTextureCompressionAPIValid()
        {
            if (s_GetDefaultTextureCompressionFormat == null || s_GetDefaultTextureCompressionFormat.MemberType != MemberTypes.Method)
                return false;
            var getterReturnType = s_GetDefaultTextureCompressionFormat.ReturnType;
            if (!getterReturnType.IsEnum || getterReturnType.Name != "TextureCompressionFormat")
                return false;
            var getterParameters = s_GetDefaultTextureCompressionFormat.GetParameters();
            if (getterParameters.Length != 1
                || !ValidateEnumParameter(getterParameters[0], "BuildTargetGroup", "platform"))
                return false;

            return s_DXTCEnumValue != -1 || s_DXTC_RGTCEnumValue != -1;
        }

        private static bool IsDefaultTextureCompressionFormatDxtForAndroid()
        {
            if (!s_isTextureCompressionAPIOk || s_GetDefaultTextureCompressionFormat == null)
                return true;

            object enabledStateResult = s_GetDefaultTextureCompressionFormat.Invoke(null, new object[] { BuildTargetGroup.Android });
            var textureCompression = Convert.ToInt32(enabledStateResult);
            return textureCompression == s_DXTCEnumValue || textureCompression == s_DXTC_RGTCEnumValue;
        }

        // ReSharper disable once HeapView.ObjectAllocation
        private static readonly ValidationRule[] BuiltinValidationRules =
        {
            new ValidationRule()
            {
                message = "MagicLeap build target should be set to Android.",
                checkPredicate = () => EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android,
                fixItMessage = "Select Android as your current build target in 'Build Settings'",
                error = false,
                errorEnteringPlaymode = false,
            },
            new ValidationRule()
            {
                message = "Gamma Color Space is not supported.",
                checkPredicate = () => PlayerSettings.colorSpace == ColorSpace.Linear,
                fixIt = () => PlayerSettings.colorSpace = ColorSpace.Linear,
                fixItMessage = "Set PlayerSettings.colorSpace to ColorSpace.Linear",
                error = true,
                errorEnteringPlaymode = false,
            },
            new ValidationRule()
            {
                message = "Only Vulkan Graphic API is supported",
                checkPredicate = () =>
                {
                    var graphicAPIs = PlayerSettings.GetGraphicsAPIs(BuildTarget.Android);
                    return graphicAPIs.Length == 1 && graphicAPIs[0] == GraphicsDeviceType.Vulkan;
                },
                fixIt = () =>
                {
                    // If "Auto Graphics API" is enabled, we have to disable it, since it can add OpenGL
                    var autoGraphicAPI = PlayerSettings.GetUseDefaultGraphicsAPIs(BuildTarget.Android);
                    if (autoGraphicAPI)
                        PlayerSettings.SetUseDefaultGraphicsAPIs(BuildTarget.Android, false);

                    // Now we make sure only Vulkan is selected
                    GraphicsDeviceType[] graphicAPIs = { GraphicsDeviceType.Vulkan };
                    PlayerSettings.SetGraphicsAPIs(BuildTarget.Android, graphicAPIs);
                },
                fixItMessage = "Set PlayerSettings 'Graphic Apis' to only contain Vulkan on Android target",
                error = true,
                errorEnteringPlaymode = true,
            },
            new ValidationRule()
            {
                message = "Select IL2CPP as the default Scripting Backend",
                checkPredicate = () => PlayerSettings.GetScriptingBackend(NamedBuildTarget.Android) == ScriptingImplementation.IL2CPP,
                fixIt = () =>
                {
                    PlayerSettings.SetScriptingBackend(NamedBuildTarget.Android, ScriptingImplementation.IL2CPP);
                },
                fixItMessage = "Set PlayerSettings default Scripting Backend to IL2CPP on Android target",
                error = true,
                errorEnteringPlaymode = true,
            },
            new ValidationRule()
            {
                message = "\"x86-64\" needs to be selected as Android's Target Architecture",
                checkPredicate = () => (((int)PlayerSettings.Android.targetArchitectures & (int)AndroidArchitecture.X86_64) != 0),
                fixIt = () =>
                {
                    var arch = (int)PlayerSettings.Android.targetArchitectures;
                    arch |= (int)AndroidArchitecture.X86_64;
                    PlayerSettings.Android.targetArchitectures = (AndroidArchitecture)arch;
                },
                fixItMessage = "Set PlayerSettings Target Architecture to contain x86-64",
                error = true,
                errorEnteringPlaymode = true,
            },
            new ValidationRule()
            {
                message = "Select only x86-64 as one of the Target Architecture to improve build times",
                checkPredicate = () => (((int)PlayerSettings.Android.targetArchitectures & (~(int)AndroidArchitecture.X86_64)) == 0),
                fixIt = () =>
                {
                    PlayerSettings.Android.targetArchitectures = AndroidArchitecture.X86_64;
                },
                fixItMessage = "Set PlayerSettings Target Architecture to be x86-64",
                error = false,
                errorEnteringPlaymode = false,
            },
            new ValidationRule()
            {
                // For texture compression, we have to use reflection since the API is internal
                // This check validate that all the different parts of the API did not change
                message = "API for PlayerSettings \"Texture compression format\" validation has changed, validation code needs to be updated.\nUntil then, validation for the DXT texture format will bi disabled, please manually make sure that setting is correct.",
                checkPredicate = () => s_isTextureCompressionAPIOk,
                fixIt = () =>
                {
                    SettingsService.OpenProjectSettings("Project/Player");
                },
                fixItAutomatic = false,
                error = false,
                errorEnteringPlaymode = false,
            },
            new ValidationRule()
            {
                message = "Set PlayerSettings \"Texture compression format\" to use \"DXT + RGTC\" for Android target",
                checkPredicate = IsDefaultTextureCompressionFormatDxtForAndroid,
                fixIt = () =>
                {
                    // For now we must let the user fix the setting manually, there is no simple way to update
                    SettingsService.OpenProjectSettings("Project/Player");
                },
                fixItMessage = "Set PlayerSettings \"Texture compression format\" to use \"DXT + RGTC\" for Android target",
                fixItAutomatic = false,
                error = true,
                errorEnteringPlaymode = false,
            },
            new ValidationRule()
            {
                message = "Set Android Build config \"Texture Compression\" to use either \"DXT\" or \"Use Player Settings\"",
                checkPredicate = () => EditorUserBuildSettings.androidBuildSubtarget is MobileTextureSubtarget.Generic or MobileTextureSubtarget.DXT,
                fixIt = () =>
                {
                    EditorUserBuildSettings.androidBuildSubtarget = MobileTextureSubtarget.Generic;
                },
                fixItMessage = "Setting Android Build config \"Texture Compression\" to use \"Use Player Settings\"",
                error = true,
                errorEnteringPlaymode = false,
            },
            new ValidationRule()
            {
                message = "Some texture asset(s) have an incompatible texture format. For Magic Leap (on Android) those invalid formats are: \"ASTC\", \"ETC\", \"ETC2\".",
                checkPredicate = () => MagicLeapTextureTools.CheckTextureCompression(),
                fixIt = () =>
                {
                    MagicLeapTextureTools.FixTextureCompression();
                },
                fixItMessage = "Setting texture asset to use Android texture compression \"DXT#\"/\"BC#\" if an incompatible format was selected",
                error = false,
                errorEnteringPlaymode = false,
            },
        };

        /// <summary>
        /// Gathers and evaluates validation issues and adds them to a list.
        /// </summary>
        /// <param name="issues">List of validation issues to populate. List is cleared before populating.</param>
        public static void GetCurrentValidationIssues(List<ValidationRule> issues)
        {
            issues.Clear();
            foreach (var validation in BuiltinValidationRules)
            {
                if (!validation.checkPredicate?.Invoke() ?? false)
                {
                    issues.Add(validation);
                }
            }
        }

        /// <summary>
        /// Logs playmode validation issues (any rule that fails with errorEnteringPlaymode set to true).
        /// </summary>
        /// <returns>true if there were any errors that should prevent MagicLeap starting in editor playmode</returns>
        internal static bool LogPlaymodeValidationIssues()
        {
            var failures = new List<ValidationRule>();
            GetCurrentValidationIssues(failures);

            if (failures.Count <= 0) return false;

            bool playmodeErrors = false;
            foreach (var result in failures)
            {
                if (result.errorEnteringPlaymode)
                    Debug.LogError(result.message);
                playmodeErrors |= result.errorEnteringPlaymode;
            }

            return playmodeErrors;
        }

        /// <summary>
        /// Logs validation issues to console.
        /// </summary>
        /// <returns>true if there were any errors that should stop the build</returns>
        internal static bool LogBuildValidationIssues()
        {
            var failures = new List<ValidationRule>();
            GetCurrentValidationIssues(failures);

            if (failures.Count <= 0) return false;

            bool anyErrors = false;
            foreach (var result in failures)
            {
                if (result.error)
                    Debug.LogError(result.message);
                else
                    Debug.LogWarning(result.message);
                anyErrors |= result.error;
            }

            return anyErrors;
        }
    }
#endif
}
