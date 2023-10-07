using System.IO;

namespace Gpm.Common.Util
{
    public static class GpmFileUtil
    {
        public static void CopyDirectory(string sourceDirName, string destDirName, bool copySubDirs)
        {
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);
            if (dir.Exists == false)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            DirectoryInfo[] dirs = dir.GetDirectories();
            if (Directory.Exists(destDirName) == false)
            {
                Directory.CreateDirectory(destDirName);
            }

            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string temppath = Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, true);
            }

            if (copySubDirs == true)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string temppath = Path.Combine(destDirName, subdir.Name);
                    CopyDirectory(subdir.FullName, temppath, copySubDirs);
                }
            }
        }

        public static bool IsScriptFile(string filePath)
        {
            string[] files = Directory.GetFiles(filePath, "*.cs", SearchOption.AllDirectories);
            return files.Length > 0;
        }

        public static void DeleteDirectory(string path)
        {
            var pathInfo = new DirectoryInfo(path);
            pathInfo.Delete(true);
        }

        public static void DeleteFile(string filePath)
        {
            var fileInfo = new FileInfo(filePath);
            fileInfo.Delete();
        }

        public static void CopyFile(string source, string dest, bool overwrite = true)
        {
            var directoryInfo = new FileInfo(dest).Directory;
            if (directoryInfo != null)
            {
                var targetPath = directoryInfo.FullName;

                if (Directory.Exists(targetPath) == false)
                {
                    Directory.CreateDirectory(targetPath);
                }
            }

            if (File.Exists(source) == true)
            {
                File.Copy(source, dest, overwrite);
            }
        }

        public static void MoveFile(string source, string dest, bool overwrite = true)
        {
            var directoryInfo = new FileInfo(dest).Directory;
            if (directoryInfo != null)
            {
                var targetPath = directoryInfo.FullName;

                if (Directory.Exists(targetPath) == false)
                {
                    Directory.CreateDirectory(targetPath);
                }
            }

            if (File.Exists(source) == true)
            {
                if (overwrite == true)
                {
                    if (File.Exists(dest) == true)
                    {
                        File.Delete(dest);
                    }
                }
                File.Move(source, dest);
            }
        }
    }
}
