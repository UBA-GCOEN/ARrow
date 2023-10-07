using Gpm.Common.Util;
using System;
using System.Text;
using UnityEngine;

namespace Gpm.Manager.Constant
{
    public static class ManagerPaths
    {
        public const string RELEASE_URI_ROOT = "https://raw.githubusercontent.com/nhn/gpm.unity/main/release/";
        public const string DOCS_URI_ROOT = "https://github.com/nhn/gpm.unity/blob/main/docs/";

        public const string RELEASE_NOTES_URI_PATH = "Manager/ReleaseNotes.md";

        public const string STORE_URI = "https://assetstore.unity.com/packages/tools/utilities/game-package-manager-147711";

        public static readonly string GPM_ROOT = "GPM";

        public static readonly string LANGUAGE_FILE_PATH = string.Format("{0}/Manager/Data/strings.xml", GPM_ROOT);

        public const string INSTALL_INFO_FILE_NAME = "install.json";
        public const string CDN_INFO_FILE_NAME = "cdn.xml";
        public const string SERVICE_LIST_FILE_NAME = "servicelist.xml";
        public const string SERVICE_FILE_NAME = "service.xml";
        public const string SERVICE_LANGUAGE_FILE_NAME = "language.xml";
        public const string AD_FILE_NAME = "advertisement.xml";
        public const string SUPPORT_INFO_FILE_NAME = "support.xml";
        public const string NOTICE_INFO_FILE_NAME = "notice.xml";

        public const string COMMON_SERVICE_NAME = "Common";
        public const string AD_FOLDER_NAME = "Ad";
        public const string CONFIG_FOLDER_NAME = "Config";

        public const string NOTICE_SERVICE_NAME = "Notice";
        public const string NOTICE_FOLDER_NAME = "Notice";

        public static readonly string PROJECT_ROOT_PATH = Application.dataPath.Replace("/Assets", "");
        public static readonly string PROJECT_ASSETS_PATH = Application.dataPath;
        public static readonly string PROJECT_DOWNLOAD_PATH = Application.dataPath.Replace("Assets", ManagerInfos.BRAND_NAME);

        public static readonly string CACHING_PATH = string.Format("{0}/{1}", Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), GPM_ROOT);
        public static readonly string LIBRARY_PATH = Application.dataPath.Replace("/Assets", string.Format("/Library/{0}", GPM_ROOT));
        public static readonly string TEMP_PATH = Application.dataPath.Replace("/Assets", string.Format("/Temp/{0}", GPM_ROOT));
        public static readonly string BACKUP_PATH = GpmPathUtil.Combine(TEMP_PATH, "Backup");

        public static readonly string TEMP_REFRESH_FILE_PATH = GpmPathUtil.Combine(LIBRARY_PATH, "Refresh.json");

        public static readonly string PROJECT_PREV_ASSETS_SHORT_PATH = "Assets/TOAST/Kit";
        public static readonly string PROJECT_PREV_ASSETS_FULL_PATH = Application.dataPath.Replace("Assets", PROJECT_PREV_ASSETS_SHORT_PATH);
        public static readonly string PROJECT_PREV_DOWNLOAD_PATH = Application.dataPath.Replace("Assets", "TOAST Kit");

        public static string GetCachingPath(params string[] path)
        {
            StringBuilder builder = new StringBuilder(CACHING_PATH);

            for (int i = 0; i < path.Length; i++)
            {
                builder.AppendFormat("/{0}", path[i]);
            }

            return builder.ToString();
        }
    }
}