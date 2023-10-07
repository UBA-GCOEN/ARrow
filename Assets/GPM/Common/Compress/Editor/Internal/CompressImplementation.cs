using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Gpm.Common.Util;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;




namespace Gpm.Common.Compress.Internal
{
#if CSHARP_7_3_OR_NEWER
    using ThirdParty.SharpCompress.Common;
    using ThirdParty.SharpCompress.Readers;
#endif

    internal static class CompressImplementation
    {
#if UNITY_EDITOR_OSX
        private static readonly string COMPRESS_PROGRAM_PATH = Path.Combine(Path.GetDirectoryName(EditorApplication.applicationContentsPath), "Contents/Tools/7za");
#elif UNITY_EDITOR_WIN
        private static readonly string COMPRESS_PROGRAM_PATH = Path.Combine(Path.GetDirectoryName(EditorApplication.applicationPath), "Data/Tools/7z.exe");
#endif

        private static Dictionary<CompressFormat, string> formatExtensions = new Dictionary<CompressFormat, string>
        {
            { CompressFormat.SEVEN_ZIP,     "7z"    },
            { CompressFormat.GZIP,          "gzip"  },
            { CompressFormat.TAR,           "tar"   },
            { CompressFormat.ZIP,           "zip"   }
        };
#if CSHARP_7_3_OR_NEWER
        internal static CompressResultCode ExtractCode(string filePath, string outputPath)
        {
            if (Directory.Exists(outputPath) == true)
            {
                Directory.Delete(outputPath, true);
            }

            if (Directory.Exists(outputPath) == false)
            {
                Directory.CreateDirectory(outputPath);
            }

            try
            {
                using (var stream = File.OpenRead(filePath))
                {
                    var reader = ReaderFactory.Open(stream);
                    while (reader.MoveToNextEntry() == true)
                    {
                        if (!reader.Entry.IsDirectory)
                        {
                            reader.WriteEntryToDirectory(outputPath, new ExtractionOptions()
                            {
                                ExtractFullPath = true,
                                Overwrite = true
                            });
                        }
                    }
                }
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError(string.Format("Extract process error (Message: {0})", e.Message));
                return CompressResultCode.ERROR_EXTRACT;
            }

            return CompressResultCode.SUCCESS;
        }
#endif
        internal static CompressResultCode Extract(string filePath, string outputPath, CompressFormat format)
        {
#if !UNITY_EDITOR_OSX && !UNITY_EDITOR_WIN
            return CompressResultCode.NOT_SUPPORT_PLATFORM;
#else
            if (File.Exists(COMPRESS_PROGRAM_PATH) == false)
            {
                throw new FileNotFoundException("Compress executable program", COMPRESS_PROGRAM_PATH);
            }

            if (Directory.Exists(outputPath) == true)
            {
                Directory.Delete(outputPath, true);
            }

            var systemEncoding = Encoding.GetEncoding(CultureInfo.CurrentCulture.TextInfo.OEMCodePage);

            var startInfo = new ProcessStartInfo
            {
#if UNITY_EDITOR_OSX
                FileName = "/bin/bash",
#elif UNITY_EDITOR_WIN
                FileName = "cmd.exe",
#endif
                Arguments = GetExtractArgument(format, filePath, outputPath),
                CreateNoWindow = true,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                WorkingDirectory = Application.dataPath + "/..",
                StandardOutputEncoding = systemEncoding,
                StandardErrorEncoding = systemEncoding
            };

            using (var process = Process.Start(startInfo))
            {
                try
                {
                    process.WaitForExit();
                    int exitCode = process.ExitCode;
                    string stdout = process.StandardOutput.ReadToEnd();
                    string stderr = process.StandardError.ReadToEnd();

                    if (exitCode != 0)
                    {
                        UnityEngine.Debug.LogError(string.Format("Extract process error (Arguments: {0}, ExitCode: {1}, Output: {2}, Error: {3})", startInfo.Arguments, exitCode, stdout, stderr));
                        return CompressResultCode.ERROR_EXTRACT;
                    }
                }
                catch (Exception e)
                {
                    UnityEngine.Debug.LogError(string.Format("Extract process error (Message: {0})", e.Message));
                    return CompressResultCode.ERROR_EXTRACT;
                }
            }

            return CompressResultCode.SUCCESS;
#endif
        }

        private static string GetExtractArgument(CompressFormat format, string filePath, string outputPath)
        {
#if !UNITY_EDITOR_OSX && !UNITY_EDITOR_WIN
            return string.Empty;
#else
            if (format == CompressFormat.TAR_GZ)
            {
#if UNITY_EDITOR_OSX
                return string.Format(@"-c ""'{0}' x '{1}' -so | '{0}' x -aoa -si -ttar -o'{2}'""", COMPRESS_PROGRAM_PATH, filePath, outputPath);
#elif UNITY_EDITOR_WIN
                return string.Format(@"/c """"{0}"" x ""{1}"" -so | ""{0}"" x -aoa -si -ttar -o""{2}""""", COMPRESS_PROGRAM_PATH, filePath, outputPath);
#endif
            }

#if UNITY_EDITOR_OSX
            return string.Format(@"-c ""'{0}' x '{1}' -t{3} -o'{2}'""", COMPRESS_PROGRAM_PATH, filePath, outputPath, formatExtensions[format]);
#elif UNITY_EDITOR_WIN
            return string.Format(@"/c """"{0}"" x ""{1}"" -t{3} -o""{2}""""", COMPRESS_PROGRAM_PATH, filePath, outputPath, formatExtensions[format]);
#endif

#endif
        }

        public static class UnityPackage
        {
            public static CompressResultCode Unpack(string packagePath, string tempPath, string resultPath)
            {
#if !UNITY_EDITOR_OSX && !UNITY_EDITOR_WIN
                return CompressResultCode.NOT_SUPPORT_PLATFORM;
#else
                string filename = Path.GetFileNameWithoutExtension(packagePath);
                string tempFullPath = GpmPathUtil.Combine(tempPath, filename);

#if CSHARP_7_3_OR_NEWER
                CompressResultCode extractResultCode = ExtractCode(packagePath, tempFullPath);
#else
                CompressResultCode extractResultCode = Extract(packagePath, tempFullPath, CompressFormat.TAR_GZ);
#endif
                if (extractResultCode != CompressResultCode.SUCCESS)
                {
                    return extractResultCode;
                }

                if (RemapPackageToAsset(tempFullPath, resultPath) == false)
                {
                    return CompressResultCode.ERROR_REMAP;
                }

                return CompressResultCode.SUCCESS;
            }

            private static bool RemapPackageToAsset(string originPath, string outputPath)
            {
                outputPath = outputPath.Replace('/', Path.DirectorySeparatorChar);

                try
                {
                    foreach (var directoryInfo in new DirectoryInfo(originPath).GetDirectories())
                    {
                        var remapPath = File.ReadAllLines(GpmPathUtil.Combine(directoryInfo.FullName, "pathname")).First();
                        remapPath = remapPath.Replace('/', Path.DirectorySeparatorChar);

                        var assetFilePath = GpmPathUtil.Combine(directoryInfo.FullName, "asset");
                        var metaFilePath = GpmPathUtil.Combine(directoryInfo.FullName, "asset.meta");

                        GpmFileUtil.MoveFile(assetFilePath, GpmPathUtil.Combine(outputPath, remapPath));
                        GpmFileUtil.MoveFile(metaFilePath, GpmPathUtil.Combine(outputPath, string.Format("{0}.meta", remapPath)));
                    }
                }
                catch (Exception e)
                {
                    UnityEngine.Debug.LogError(string.Format("Remap unity package to asset error (Message: {0})", e.Message));
                    return false;
                }

                return true;
#endif
            }
        }

    }
}
