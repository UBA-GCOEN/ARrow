using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.MagicLeap;

namespace UnityEditor.XR.MagicLeap
{
    /// <summary>
    /// Texture processor for Magic Leap
    /// Magic Leap 2 support only a sub-set of textures compression that Android can support (DXT is supported, 
    /// but ASTC/ETC/EAC should not be used as they can cause corruption), this processor will update
    /// the assets to use the closest DXT compression for each unsupported compression
    /// (for Android target only)
    /// </summary>
    class MagicLeapTexturePostprocessor : AssetPostprocessor
    {
        void OnPreprocessTexture()
        {
            // Only preprocess textures if the MagicLeap loader is active for Android
            if (!BuildHelperUtils.HasLoader(BuildTargetGroup.Android, typeof(MagicLeapLoader)))
                return;
            
            TextureImporter textureImporter  = assetImporter as TextureImporter;
            if (textureImporter)
            {
                TextureImporterPlatformSettings settings = textureImporter.GetPlatformTextureSettings("Android");
                TextureImporterFormat oldFormat = settings.format;
                TextureImporterFormat newFormat = GetCompatibleTextureCompression(oldFormat);
                if (oldFormat != newFormat)
                {
                    settings.format = newFormat;
                    string logMessage = "Magic Leap 2 Texture import - " + assetPath +
                                        " - Changing Android texture compression from " +
                                        oldFormat + " to " + newFormat;
                    context.LogImportWarning(logMessage);
                    textureImporter.SetPlatformTextureSettings(settings);
                }
            }
        }

        [MenuItem("Window/XR/MagicLeap/Fix texture compression...")]
        public static void FixTextureCompression()
        {
            MagicLeapTextureTools.FixTextureCompression();
        }

        public static TextureImporterFormat GetCompatibleTextureCompression(TextureImporterFormat format)
        {
            switch (format)
            {
                // ETC/ETC2
                case TextureImporterFormat.ETC_RGB4:
                case TextureImporterFormat.ETC2_RGB4:
                case TextureImporterFormat.ETC2_RGB4_PUNCHTHROUGH_ALPHA:
                    return TextureImporterFormat.DXT1;
                case TextureImporterFormat.ETC_RGB4Crunched:
                    return TextureImporterFormat.DXT1Crunched;
                case TextureImporterFormat.ETC2_RGBA8:
                    return TextureImporterFormat.DXT5;
                case TextureImporterFormat.ETC2_RGBA8Crunched:
                    return TextureImporterFormat.DXT5Crunched;
                // ASTC
                case TextureImporterFormat.ASTC_12x12:
                case TextureImporterFormat.ASTC_10x10:
                case TextureImporterFormat.ASTC_8x8:
                    return TextureImporterFormat.DXT1;
                case TextureImporterFormat.ASTC_6x6:
                case TextureImporterFormat.ASTC_5x5:
                case TextureImporterFormat.ASTC_4x4:
                    return TextureImporterFormat.DXT5;
                // ASTC HDR
                case TextureImporterFormat.ASTC_HDR_4x4:
                case TextureImporterFormat.ASTC_HDR_5x5:
                case TextureImporterFormat.ASTC_HDR_6x6:
                case TextureImporterFormat.ASTC_HDR_8x8:
                case TextureImporterFormat.ASTC_HDR_10x10:
                case TextureImporterFormat.ASTC_HDR_12x12:
                    return TextureImporterFormat.Automatic;
            }

            return format;
        }
    }
}
