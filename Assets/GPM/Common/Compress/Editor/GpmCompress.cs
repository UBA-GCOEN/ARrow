using Gpm.Common.Compress.Internal;

namespace Gpm.Common.Compress
{
    public enum CompressResultCode
    {
        SUCCESS,
        ERROR_EXTRACT,
        ERROR_REMAP,
        NOT_SUPPORT_PLATFORM,
        NOT_FOUND_EXTRACT_APP,
    }

    public enum CompressFormat
    {
        SEVEN_ZIP,
        GZIP,
        TAR,
        ZIP,
        TAR_GZ,
    }

    public static class GpmCompress
    {
        public static CompressResultCode Extract(string filePath, string outputPath, CompressFormat format)
        {
            return CompressImplementation.Extract(filePath, outputPath, format);
        }

        public static CompressResultCode ExtractUnityPackage(string packagePath, string tempPath, string resultPath)
        {
            return CompressImplementation.UnityPackage.Unpack(packagePath, tempPath, resultPath);
        }
    }
}
