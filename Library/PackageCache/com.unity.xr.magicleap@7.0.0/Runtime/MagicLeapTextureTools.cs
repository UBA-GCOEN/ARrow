
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.XR.Management;
#endif
using System.Linq;
using UnityEngine.XR.Management;

namespace UnityEngine.XR.MagicLeap
{
    public class MagicLeapTextureTools
    {
#if UNITY_EDITOR
        public static bool CheckTextureCompression()
        {
            foreach (string guid in AssetDatabase.FindAssets("t:texture", null))
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                TextureImporter textureImporter = AssetImporter.GetAtPath(assetPath) as TextureImporter;
                if (textureImporter)
                    if (IsIncompatibleComprestion(textureImporter.GetPlatformTextureSettings("Android").format))
                        return false;
            }

            return true;
        }
        
        public static bool HasXRLoader(BuildTargetGroup targetGroup, System.Type loader)
        {
            var settings = XRGeneralSettingsPerBuildTarget.XRGeneralSettingsForBuildTarget(targetGroup);

            if (settings)
            {
#pragma warning disable CS0618
                return settings.Manager.loaders.Any(loader.IsInstanceOfType);
#pragma warning restore CS0618
            }

            return false;
        }

        public static void FixTextureCompression()
        {
            // Only preprocess textures if the MagicLeap loader is active for Android
            if (!HasXRLoader(BuildTargetGroup.Android, typeof(MagicLeapLoader)))
            {
                string logMessage = "Magic Leap XR Module is not selected for Android, please enable it before running \"Fix texture compression\"";
                Debug.LogWarning(logMessage);
                return;
            }
            
            foreach (string guid in AssetDatabase.FindAssets("t:texture", null))
            {
                // Re-import incompatible assets
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                TextureImporter textureImporter = AssetImporter.GetAtPath(assetPath) as TextureImporter;
                if (textureImporter)
                    if (IsIncompatibleComprestion(textureImporter.GetPlatformTextureSettings("Android").format))
                        AssetDatabase.ImportAsset(assetPath);
            }
        }

        public static bool IsIncompatibleComprestion(TextureImporterFormat format)
        {
            return IsASTC(format) || IsASTCHDR(format) || IsETC(format);
        }

        public static bool IsASTC(TextureImporterFormat format)
        {
            return format == TextureImporterFormat.ASTC_4x4
                   || format == TextureImporterFormat.ASTC_5x5
                   || format == TextureImporterFormat.ASTC_6x6
                   || format == TextureImporterFormat.ASTC_8x8
                   || format == TextureImporterFormat.ASTC_10x10
                   || format == TextureImporterFormat.ASTC_12x12;
        }

        public static bool IsASTCHDR(TextureImporterFormat format)
        {
            return format == TextureImporterFormat.ASTC_HDR_4x4
                   || format == TextureImporterFormat.ASTC_HDR_5x5
                   || format == TextureImporterFormat.ASTC_HDR_6x6
                   || format == TextureImporterFormat.ASTC_HDR_8x8
                   || format == TextureImporterFormat.ASTC_HDR_10x10
                   || format == TextureImporterFormat.ASTC_HDR_12x12;
        }

        public static bool IsETC(TextureImporterFormat format)
        {
            return format == TextureImporterFormat.ETC_RGB4
                   || format == TextureImporterFormat.ETC_RGB4Crunched
                   || format == TextureImporterFormat.ETC2_RGB4
                   || format == TextureImporterFormat.ETC2_RGB4_PUNCHTHROUGH_ALPHA
                   || format == TextureImporterFormat.ETC2_RGBA8
                   || format == TextureImporterFormat.ETC2_RGBA8Crunched;
        }
#endif
    }
}
