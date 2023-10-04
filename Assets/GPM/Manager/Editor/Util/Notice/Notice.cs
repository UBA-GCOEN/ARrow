using UnityEditor;
using UnityEngine;
using Gpm.Manager.Constant;
using Gpm.Common.Multilanguage;
using Gpm.Common.Util;
using Gpm.Common.Log;
using Gpm.Common;
using Gpm.Manager.Internal;
using Gpm.Manager.Ui.Helper;

namespace Gpm.Manager.Notice
{
    internal static class GpmNotice
    {
        public enum Status
        {
            NONE,
            LOADING,
            LOADED,
        }

        private static Status statusLanguage = Status.NONE;
        private static Status statusNotice = Status.NONE;
        private static NoticeInfo noticeInfo;
        
        public static void Initialize(string languageCode)
        {
            if (statusLanguage == Status.NONE)
            {
                statusLanguage = Status.LOADING;
                string languageUrl = GpmPathUtil.UrlCombine(GpmManager.CdnUri, ManagerPaths.NOTICE_FOLDER_NAME, ManagerPaths.SERVICE_LANGUAGE_FILE_NAME);
                GpmMultilanguage.Load(ManagerPaths.NOTICE_SERVICE_NAME, languageUrl,
                    (result, resultMessage) =>
                    {
                        if (result != MultilanguageResultCode.SUCCESS && result != MultilanguageResultCode.ALREADY_LOADED)
                        {
                            GpmLogger.Error(string.Format("(Service= {0}, Code= {1})", ManagerPaths.NOTICE_SERVICE_NAME, result), ManagerInfos.SERVICE_NAME, typeof(GpmManager));
                            statusLanguage = Status.NONE;
                            return;
                        }

                        SetLanguageCode(languageCode);

                        statusLanguage = Status.LOADED;
                    });
            }

            if (statusNotice == Status.NONE)
            {
                EditorCoroutine.Start(GpmManager.SendRequest(
                    string.Format("{0}/{1}",
                    ManagerPaths.NOTICE_FOLDER_NAME,
                    ManagerPaths.NOTICE_INFO_FILE_NAME),
                    (request) =>
                    {
                        statusNotice = Status.LOADING;
                        if (UnityWebRequestHelper.IsError(request) == true)
                        {
                            GpmLogger.Error(string.Format("Notice info get failed. (Error has occurred in network.)"), ManagerInfos.SERVICE_NAME, typeof(GpmManager));
                            statusNotice = Status.NONE;
                            return;
                        }

                        if (UnityWebRequestHelper.IsProtocolError(request) == true)
                        {
                            GpmLogger.Error(string.Format("Notice info get failed. (code= {0})", request.responseCode), ManagerInfos.SERVICE_NAME, typeof(GpmManager));
                            statusNotice = Status.NONE;
                            return;
                        }
                        else
                        {
                            XmlHelper.LoadXmlFromText<NoticeInfo>(
                                request.downloadHandler.text,
                                (responseCode, xmlData, message) =>
                                {
                                    if (XmlHelper.ResponseCode.SUCCESS != responseCode)
                                    {
                                        GpmLogger.Error(string.Format("Support info load failed. (code= {0})", responseCode), ManagerInfos.SERVICE_NAME, typeof(GpmManager));
                                        statusNotice = Status.NONE;
                                        return;
                                    }

                                    noticeInfo = xmlData;
                                    statusNotice = Status.LOADED;
                                });
                        }
                    }));
            }
        }


        public static bool HasNotice
        {
            get
            {
                if (statusLanguage == Status.LOADED &&
                    statusNotice == Status.LOADED &&
                    noticeInfo != null &&
                    noticeInfo.noticeList != null &&
                    noticeInfo.noticeList.list.Count > 0 &&
                    GpmMultilanguage.IsLoadService(ManagerPaths.NOTICE_SERVICE_NAME) == true)
                {
                    foreach (var notice in noticeInfo.noticeList.list)
                    {
                        if (notice.IsActiveTime() == true)
                        {
                            return true;
                        }
                    }
                }

                return false;
            }
        }

        public static void Draw()
        {
            if (HasNotice == true)
            {
                ManagerUi.DrawHorizontalSplitter();
                using (new EditorGUILayout.HorizontalScope())
                {
                    GUILayout.Space(14);

                    using (new EditorGUILayout.VerticalScope())
                    {
                        foreach (var notice in noticeInfo.noticeList.list)
                        {
                            if (notice.IsActiveTime() == true)
                            {
                                GUIContent content = new GUIContent(GpmMultilanguage.GetString(ManagerPaths.NOTICE_SERVICE_NAME, notice.text), ManagerUiIcon.DOT_ICON);
                                if (GUILayout.Button(content, ManagerUiStyle.NoticeLabel) == true)
                                {
                                    Application.OpenURL(GpmMultilanguage.GetString(ManagerPaths.NOTICE_SERVICE_NAME, notice.url));
                                }
                            }
                        }
                    }
                }
            }
        }
        

        public static void SetLanguageCode(string languageCode)
        {
            if (statusNotice == Status.LOADED &&
                GpmMultilanguage.IsLoadService(ManagerPaths.NOTICE_SERVICE_NAME) == true)
            {
                GpmMultilanguage.SelectLanguageByCode(ManagerPaths.NOTICE_SERVICE_NAME, languageCode, ((code, s) => { }));
            }
        }
    }
}
