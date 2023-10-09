namespace Gpm.Manager.Constant
{
    internal static class ManagerStrings
    {
        public const string WINDOW_TITLE = "WINDOW_TITLE";
        public const string VERSION = "VERSION";
        public const string COPYRIGHT = "COPYRIGHT";
        public const string LANGUAGE = "LANGUAGE";
        public const string SERVICE_LIST = "SERVICE_LIST";
        public const string SERVICE_INSTALL = "SERVICE_INSTALL";
        public const string SERVICE_UPDATE = "SERVICE_UPDATE";
        public const string SERVICE_UPDATE_VERSION = "SERVICE_UPDATE_VERSION";
        public const string SERVICE_UNINSTALL = "SERVICE_UNINSTALL";
        public const string SERVICE_PREPARE = "SERVICE_PREPARE";
        public const string SUPPORT_UNITY_VERSION = "SUPPORT_UNITY_VERSION";
        public const string SERVICE_LIST_LOADING = "SERVICE_LIST_LOADING";
        public const string SERVICE_LIST_NOT_FOUND = "SERVICE_LIST_NOT_FOUND";
        public const string SERVICE_INFO_LOADING = "SERVICE_INFO_LOADING";
        public const string SERVICE_INFO_NOT_FOUND = "SERVICE_INFO_NOT_FOUND";
        public const string SERVICE_INFO_UPDATE_COMPLETED = "SERVICE_INFO_UPDATE_COMPLETED";
        public const string SERVICE_DEPENDENCY = "SERVICE_DEPENDENCY";
        public const string SERVICE_DEPENDENCY_IS_USING = "SERVICE_DEPENDENCY_IS_USING";
        public const string SERVICE_DEPENDENCY_IS_USING_EMPTY = "SERVICE_DEPENDENCY_IS_USING_EMPTY";
        public const string SERVICE_DEPENDENCY_USED_BY = "SERVICE_DEPENDENCY_USED_BY";
        public const string SERVICE_DEPENDENCY_USED_BY_EMPTY = "SERVICE_DEPENDENCY_USED_BY_EMPTY";
        public const string UNITY_NOT_SUPPORT_VERSION = "UNITY_NOT_SUPPORT_VERSION";
        public const string LIST_SCROLL_UP = "LIST_SCROLL_UP";
        public const string LIST_SCROLL_DOWN = "LIST_SCROLL_DOWN";
        public const string INFO_TITLE = "INFO_TITLE";
        public const string INFO_SUPPORT_MAIL = "INFO_SUPPORT_MAIL";
        public const string INFO_SUPPORT_MAIL_COPY = "INFO_SUPPORT_MAIL_COPY";
        public const string INFO_SUPPORT_MAIL_COPY_COMPLETED = "INFO_SUPPORT_MAIL_COPY_COMPLETED";
        public const string INFO_RELEASE_NOTES = "INFO_RELEASE_NOTES";
        public const string UPDATEABLE = "UPDATEABLE";
        public const string POPUP_OK = "POPUP_OK";
        public const string POPUP_CANCEL = "POPUP_CANCEL";
        public const string SERVICE_REMOVE_TRY_TITLE = "SERVICE_REMOVE_TRY_TITLE";
        public const string SERVICE_REMOVE_TRY_MESSAGE = "SERVICE_REMOVE_TRY_MESSAGE";
        public const string WARNING_TITLE_PREV_BRAND_SERVICE = "WARNING_TITLE_PREV_BRAND_SERVICE";
        public const string WARNING_MESSAGE_PREV_BRAND_SERVICE = "WARNING_MESSAGE_PREV_BRAND_SERVICE";
        public const string ERROR_TITLE_UNKNOWN = "ERROR_TITLE_UNKNOWN";
        public const string ERROR_TITLE_CDN = "ERROR_TITLE_CDN";
        public const string ERROR_TITLE_INSTALL = "ERROR_TITLE_INSTALL";
        public const string ERROR_TITLE_UNINSTALL = "ERROR_TITLE_UNINSTALL";
        public const string ERROR_TITLE_SERVICE_LIST = "ERROR_TITLE_SERVICE_LIST";
        public const string ERROR_TITLE_SERVICE_INFO = "ERROR_TITLE_SERVICE_INFO";
        public const string ERROR_TITLE_SERVICE_INFO_UPDATE = "ERROR_TITLE_SERVICE_INFO_UPDATE";
        public const string ERROR_TITLE_SETTING = "ERROR_TITLE_SETTING";
        public const string ERROR_TITLE_NETWORK = "ERROR_TITLE_NETWORK";
        public const string ERROR_MESSAGE_CUSTOM_MESSAGE = "ERROR_MESSAGE_CUSTOM_MESSAGE";
        public const string ERROR_MESSAGE_PARAMETER = "ERROR_MESSAGE_PARAMETER";
        public const string ERROR_MESSAGE_CHANGE_LANGUAGE = "ERROR_MESSAGE_CHANGE_LANGUAGE";
        public const string ERROR_MESSAGE_NETWORK = "ERROR_MESSAGE_NETWORK";
        public const string ERROR_MESSAGE_CDN_INFO = "ERROR_MESSAGE_CDN_INFO";
        public const string ERROR_MESSAGE_INSTALL_INFO_LOAD = "ERROR_MESSAGE_INSTALL_INFO_LOAD";
        public const string ERROR_MESSAGE_SERVICE_LIST_GET_FAILED = "ERROR_MESSAGE_SERVICE_LIST_GET_FAILED";
        public const string ERROR_MESSAGE_SERVICE_INFO_LOAD_FAILED = "ERROR_MESSAGE_SERVICE_INFO_LOAD_FAILED";
        public const string ERROR_MESSAGE_SERVICE_LANGAGE_LOAD_FAILED = "ERROR_MESSAGE_SERVICE_LANGAGE_LOAD_FAILED";
        public const string ERROR_MESSAGE_SERVICE_IMAGE_GET_FAILED = "ERROR_MESSAGE_SERVICE_IMAGE_GET_FAILED";
        public const string ERROR_MESSAGE_MANUAL_UPDATE_SERVICE = "ERROR_MESSAGE_MANUAL_UPDATE_SERVICE";
        public const string ERROR_MESSAGE_INSTALL_FAILED = "ERROR_MESSAGE_INSTALL_FAILED";
        public const string ERROR_MESSAGE_REMOVE_FAILED = "ERROR_MESSAGE_REMOVE_FAILED";
        public const string ERROR_MESSAGE_INSTALL_PATH_NOT_FOUND = "ERROR_MESSAGE_INSTALL_PATH_NOT_FOUND";
        public const string ERROR_MESSAGE_DOWNLOAD_FAILED = "ERROR_MESSAGE_DOWNLOAD_FAILED";
        public const string ERROR_MESSAGE_DOWNLOAD_FAILED_WAIT = "ERROR_MESSAGE_DOWNLOAD_FAILED_WAIT";
        public const string ERROR_MESSAGE_ALREADY_INSTALL = "ERROR_MESSAGE_ALREADY_INSTALL";
        public const string ERROR_MESSAGE_ALREADY_INSTALLED = "ERROR_MESSAGE_ALREADY_INSTALLED";
        public const string ERROR_MESSAGE_DEPENDENCY_SERVICE_INSTALL_FAILED = "ERROR_MESSAGE_DEPENDENCY_SERVICE_INSTALL_FAILED";
        public const string ERROR_MESSAGE_DEPENDENCY_SERVICE_REMOVE_FAILED = "ERROR_MESSAGE_DEPENDENCY_SERVICE_REMOVE_FAILED";

        public const string LAST_UPDATE = "LAST_UPDATE";

        public static string GetErrorCode(ManagerErrorCode code)
        {
            return string.Format("ERROR_TITLE_{0}", code);
        }
    }
}
