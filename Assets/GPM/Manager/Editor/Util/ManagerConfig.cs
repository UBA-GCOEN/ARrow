namespace Gpm.Manager.Util
{
    using System;
    using System.IO;
    using System.Text;
    using Constant;

    internal static class ManagerConfig
    {
        public static int GetInt(string keyName)
        {
            string forder = ManagerPaths.GetCachingPath(ManagerPaths.CONFIG_FOLDER_NAME);
            if (Directory.Exists(forder) == true)
            {
                try
                {
                    string fileName = string.Format("{0}/{1}", forder, keyName);
                    if (File.Exists(fileName) == true)
                    {
                        using (var stream = File.Open(fileName, FileMode.Open))
                        {
                            using (var reader = new BinaryReader(stream, Encoding.UTF8, false))
                            {
                                return reader.ReadInt32();
                            }
                        }
                    }
                }
                catch
                {
                }
            }

            return 0;
        }

        public static void SetInt(string keyName, int value)
        {
            string forder = ManagerPaths.GetCachingPath(ManagerPaths.CONFIG_FOLDER_NAME);
            if (Directory.Exists(forder) == false)
            {
                Directory.CreateDirectory(forder);
            }

            try
            {
                string fileName = string.Format("{0}/{1}", forder, keyName);
                using (var stream = File.Open(fileName, FileMode.OpenOrCreate))
                {
                    using (var reader = new BinaryWriter(stream, Encoding.UTF8, false))
                    {
                        reader.Write(value);
                    }
                }
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogWarning(e.Message);
            }
        }
    }
}