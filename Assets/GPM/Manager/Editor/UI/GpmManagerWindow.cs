using Gpm.Common.Log;
using Gpm.Common.Multilanguage;
using Gpm.Common.Util;
using Gpm.Manager.Ad;
using Gpm.Manager.Notice;
using Gpm.Manager.Util;
using Gpm.Manager.Constant;
using Gpm.Manager.Internal;
using Gpm.Manager.Ui.Helper;
using System;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Gpm.Manager.Ui
{
    internal class GpmManagerWindow : EditorWindow
    {
        public static GpmManagerWindow window;

        private GpmServiceList serviceList;
        private GpmServiceDetail serviceDetail;

        private string[] languages;
        private int selectedLanguageIndex;

        private bool openedImageViewer;
        private bool openedInfoViewer;

        private long openTime;

        public static void OpenWindow()
        {
            LanguageLoad(() =>
            {
                CheckInstalledPrevBrandService();
                window = GetWindow<GpmManagerWindow>();
            }, false);
        }

        private static void LanguageLoad(Action callback, bool opened = true)
        {
            GpmMultilanguage.Load(
                ManagerInfos.SERVICE_NAME,
                ManagerPaths.LANGUAGE_FILE_PATH,
                (result, resultMsg) =>
                {
                    if (result != MultilanguageResultCode.SUCCESS && result != MultilanguageResultCode.ALREADY_LOADED)
                    {
                        GpmLogger.Error(string.Format("Language load failed. (type= {0})", result), ManagerInfos.SERVICE_NAME, typeof(GpmManagerWindow));
                        return;
                    }

                    callback();
                });
        }

        private void OnEnable()
        {
            minSize = new Vector2(800, 380);
            Initialize();

            openTime = ManagerTime.Now.Ticks;

        }

        private void OnDestroy()
        {
            SendRuntimeLog();

            GpmManager.Instance.Clear();
        }

        private void SendRuntimeLog()
        {
            long endTime = ManagerTime.Now.Ticks;
            
            string startTimeText = ManagerTime.ToLogString(openTime);
            string endTimeText = ManagerTime.ToLogString(endTime);

            string totalText;
            long totalSecond = (long)TimeSpan.FromTicks(endTime - openTime).TotalSeconds;
            if (totalSecond < 60)
            {
                totalText = string.Format("{0} sec", totalSecond);
            }
            else
            {
                long totalMinute = totalSecond / 60;
                long second = totalSecond % 60;
                if (totalMinute > 60)
                {
                    long totalHour = totalMinute / 60;
                    long minute = totalMinute % 60;

                    totalText = string.Format("{0}:{1}:{2}", totalHour, minute, second);
                }
                else
                {
                    totalText = string.Format("{0}:{1}", totalMinute, second);
                }
            }

            GpmManagerIndicator.SendRunTime(startTimeText, endTimeText, totalText);
        }

        private void Initialize()
        {
            titleContent = ManagerUi.GetContent(ManagerStrings.WINDOW_TITLE);

            EditorApplication.playModeStateChanged -= OnPlaymodeChanged;
            EditorApplication.playModeStateChanged += OnPlaymodeChanged;

            serviceList = new GpmServiceList(this);
            serviceDetail = new GpmServiceDetail(this);

            GpmManager.Instance.Initialize(OnErrorCallback, this,
                new Rect(0, 0, ManagerUiDefine.LEFT_FRAME_WIDTH, ManagerUiDefine.AD_FRAME_HEIGHT));

            if (GpmMultilanguage.IsLoadService(ManagerInfos.SERVICE_NAME) == true)
            {
                languages = GpmMultilanguage.GetSupportLanguages(ManagerInfos.SERVICE_NAME, true);
                if (languages != null)
                {
                    string lastLanguage = ManagerInfos.LastLanguage;
                    if (string.IsNullOrEmpty(lastLanguage) == false)
                    {
                        GpmManager.Instance.ChangeLanguageCode(ManagerInfos.LastLanguage);
                    }

                    selectedLanguageIndex = GetSelectLanguageIndex(GpmMultilanguage.GetSelectLanguage(ManagerInfos.SERVICE_NAME, true));
                }
                else
                {
                    languages = new[] { ManagerUiDefine.EMPTY_LANGUAGES_VALUE };
                    selectedLanguageIndex = 0;
                }
            }
            else
            {
                languages = new[] { ManagerUiDefine.EMPTY_LANGUAGES_VALUE };
                selectedLanguageIndex = 0;

                Reload();
            }
        }

        private void OnPlaymodeChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.EnteredPlayMode)
            {
                Reload();
            }
        }

        private void Reload()
        {
            LanguageLoad(() =>
            {
                Initialize();
                Repaint();
            });
        }

        private void OnGUI()
        {
            DoToolbarGUI();
            DoInfoGUI();

            if (GpmManager.Instance.Initialized == false)
            {
                GUILayout.BeginArea(new Rect(position.width * 0.5f - 100, (position.height - ManagerUiDefine.COPYRIGHT_HEIGHT) * 0.5f - 50, 400, 100));
                EditorGUILayout.BeginHorizontal();
                {
                    ManagerUi.LabelValue(ManagerUiIcon.StatusWheel);
                    ManagerUi.Label(ManagerStrings.SERVICE_LIST_LOADING, ManagerUiStyle.IconLabel);
                }
                EditorGUILayout.EndHorizontal();
                GUILayout.EndArea();
            }
            else
            {
                DoManagerGUI();
            }

            DoCopyrightGUI();

            if (Event.current.type == EventType.ExecuteCommand)
            {
                switch (Event.current.commandName)
                {
                    case ManagerEvents.IMAGE_SELECT:
                        {
                            openedImageViewer = true;
                            Repaint();
                            break;
                        }
                    case ManagerEvents.CHANGE_SERVICE:
                        {
                            serviceDetail.LoadService(serviceList.SelectedService);
                            break;
                        }
                    case ManagerEvents.INFO_RESET:
                        {
                            serviceList.Clear();
                            serviceDetail.Clear();
                            break;
                        }
                }
            }

            if (Event.current.type == EventType.MouseDown)
            {
                openedImageViewer = false;
                openedInfoViewer = false;
                Repaint();
            }
        }

        private void Update()
        {
            serviceList.Update();
            serviceDetail.Update();

            if (EditorUtility.scriptCompilationFailed == true)
            {
                GpmManager.IsLock = false;
            }
        }

        private void DoToolbarGUI()
        {
            if(GpmManager.Instance.SupportInfo == null)
            {
                return;
            }

            using (new EditorGUILayout.HorizontalScope(EditorStyles.toolbar))
            {
                using (new EditorGUILayout.HorizontalScope(GUILayout.ExpandWidth(true)))
                {
                    if(GpmManager.Instance.SupportInfo.menuList != null)
                    {
                        foreach (var menu in GpmManager.Instance.SupportInfo.menuList.list)
                        {
                            if (ManagerUi.Button(menu.text, ManagerUiStyle.ToolbarButton) == true)
                            {
                                Application.OpenURL(ManagerInfos.GetString(menu.url));
                            }
                        }
                    }
                }
                EditorGUI.BeginChangeCheck();
                {
                    selectedLanguageIndex = ManagerUi.PopupValue(selectedLanguageIndex, languages, ManagerUiStyle.ToolbarPopup, GUILayout.Width(100));
                }
                if (EditorGUI.EndChangeCheck() == true)
                {
                    GpmManager.Instance.ChangeLanguageCode(GetSelectLanguageCode());
                }

                if (ManagerUi.InfoButton() == true)
                {
                    openedInfoViewer = true;
                }
            }
        }

        private float noticeHeight = 0;
        private void DoInfoGUI()
        {
            using (new EditorGUILayout.VerticalScope())
            {
                GUILayout.Space(4);

                using (new EditorGUILayout.HorizontalScope())
                {
                    GUILayout.Label(titleContent, ManagerUiStyle.MainTitleLabel);

                    if (GpmManager.Instance.SupportInfo != null)
                    {
                        string lastVersion = GpmManager.Instance.SupportInfo.version;
                        if(lastVersion != null)
                        {
                            bool canUpdate = lastVersion.VersionGreaterThan(GpmManagerVersion.VERSION);

                            if (canUpdate == true)
                            {
                                string updateText = string.Format(ManagerInfos.GetString(ManagerStrings.SERVICE_UPDATE_VERSION), lastVersion);
                                ManagerUi.Label(ManagerStrings.UPDATEABLE, ManagerUiStyle.WarningVersionLabel);
                                if (GUILayout.Button(updateText, GUILayout.ExpandWidth(false)) == true)
                                {
                                    Application.OpenURL(ManagerPaths.STORE_URI);
                                }
                            }
                        }
                    }
                }

                using (new EditorGUILayout.HorizontalScope())
                {
                    GUILayout.Space(10);

                    ManagerUi.Label(ManagerStrings.VERSION, GUILayout.ExpandWidth(false));
                    ManagerUi.LabelValue(GpmManagerVersion.VERSION);
                }

                if (GpmNotice.HasNotice == true)
                {
                    GpmNotice.Draw();
                }
            }

            ManagerUi.DrawHorizontalSplitter();

            Rect splitterRect = EditorGUILayout.GetControlRect(false, 1);
            if (splitterRect.y > 0)
            {
                noticeHeight = splitterRect.y;
            }
        }

        private void DoManagerGUI()
        {
            float y = noticeHeight;
            EditorGUI.BeginDisabledGroup(GpmManager.IsLock);

            EditorGUI.BeginDisabledGroup(openedImageViewer || openedInfoViewer);
            Rect contentRect = new Rect(0, y, position.width, position.height - y - ManagerUiDefine.COPYRIGHT_HEIGHT);
            GUILayout.BeginArea(contentRect);
            {
                Rect listRect = new Rect(0, 0, ManagerUiDefine.LEFT_FRAME_WIDTH, position.height - ManagerUiDefine.AD_FRAME_HEIGHT - ManagerUiDefine.REFRESH_FRAME_HEIGHT);
                GUILayout.BeginArea(listRect);
                {
                    GUILayout.BeginHorizontal(EditorStyles.toolbar);
                    {
                        ManagerUi.Label(ManagerStrings.SERVICE_LIST);
                    }
                    GUILayout.EndHorizontal();

                    serviceList.OnGUI(listRect);
                }
                GUILayout.EndArea();

                DoRefreshGUI(contentRect);

                float adY = contentRect.height - ManagerUiDefine.AD_FRAME_HEIGHT;
                ManagerUi.DrawVerticalSplitter(0, adY, ManagerUiDefine.LEFT_FRAME_WIDTH);

                Rect adRect = new Rect(0, adY, ManagerUiDefine.LEFT_FRAME_WIDTH, ManagerUiDefine.AD_FRAME_HEIGHT);
                GUILayout.BeginArea(adRect);
                {
                    Advertisement.Draw();
                }
                GUILayout.EndArea();

                ManagerUi.DrawHorizontalSplitter(ManagerUiDefine.LEFT_FRAME_WIDTH, 0, position.height);

                Rect detailRect = new Rect(
                    ManagerUiDefine.LEFT_FRAME_WIDTH + ManagerUiDefine.DETAIL_MARGIN,
                    0,
                    position.width - ManagerUiDefine.LEFT_FRAME_WIDTH - ManagerUiDefine.DETAIL_MARGIN,
                    position.height);
                GUILayout.BeginArea(detailRect);
                {
                    if (serviceDetail != null)
                    {
                        serviceDetail.OnGUI(detailRect);
                    }
                }
                GUILayout.EndArea();
            }
            GUILayout.EndArea();
            EditorGUI.EndDisabledGroup();

            if (openedImageViewer == true)
            {
                GUI.DrawTexture(new Rect(0, 0, position.width, position.height), ManagerUiStyle.OverlayTexture);

                if (serviceDetail.SelectedImageInfo != null)
                {
                    GUILayout.BeginArea(contentRect);
                    DoImageViewerGUI(contentRect);
                    GUILayout.EndArea();
                }
            }

            if (openedInfoViewer == true)
            {
                GUILayout.BeginArea(contentRect);
                {
                    GUI.DrawTexture(new Rect(0, 0, position.width, position.height), ManagerUiStyle.OverlayTexture);

                    BeginWindows();
                    {
                        ManagerUi.Window(
                            1,
                            new Rect(
                                (contentRect.width * 0.5f) - (ManagerUiDefine.INFO_WINDOW_WIDTH * 0.5f),
                                (contentRect.height * 0.5f) - (ManagerUiDefine.INFO_WINDOW_HEIGHT * 0.5f),
                                ManagerUiDefine.INFO_WINDOW_WIDTH,
                                ManagerUiDefine.INFO_WINDOW_HEIGHT),
                            DoInformationGUI,
                            ManagerStrings.INFO_TITLE);
                    }
                    EndWindows();
                }
                GUILayout.EndArea();
            }

            EditorGUI.EndDisabledGroup();
        }

        private void DoRefreshGUI(Rect rect)
        {
            float refreshY = rect.height - ManagerUiDefine.AD_FRAME_HEIGHT - ManagerUiDefine.REFRESH_FRAME_HEIGHT;

            ManagerUi.DrawVerticalSplitter(0, refreshY, ManagerUiDefine.LEFT_FRAME_WIDTH);

            Rect refreshRect = new Rect(0, refreshY, ManagerUiDefine.LEFT_FRAME_WIDTH, ManagerUiDefine.REFRESH_FRAME_HEIGHT);
            GUILayout.BeginArea(refreshRect);
            {
                using (new EditorGUILayout.HorizontalScope(GUILayout.Height(ManagerUiDefine.REFRESH_FRAME_HEIGHT)))
                {
                    DateTime lastDateTime = new DateTime(GpmManager.Instance.InitializeDate);
                    string lastDateText = string.Format("{0} {1}", ManagerInfos.GetString(ManagerStrings.LAST_UPDATE), lastDateTime.ToString("HH:mm"));

                    GUILayout.Label(lastDateText, ManagerUiStyle.MiddleCenterLabel);
                    if (GUILayout.Button(ManagerUiIcon.REFRESH_ICON, GUILayout.Width(ManagerUiDefine.REFRESH_FRAME_HEIGHT), GUILayout.Height(ManagerUiDefine.REFRESH_FRAME_HEIGHT)) == true)
                    {
                        GpmManager.Instance.RefreshServiceData();
                    }
                }
            }
            GUILayout.EndArea();
        }

        private void DoCopyrightGUI()
        {
            GUILayout.BeginArea(new Rect(0, position.height - ManagerUiDefine.COPYRIGHT_HEIGHT, position.width, ManagerUiDefine.COPYRIGHT_HEIGHT));
            EditorGUILayout.BeginVertical(ManagerUiStyle.CopyrightBox);
            {
                ManagerUi.Label(ManagerStrings.COPYRIGHT, ManagerUiStyle.CopyrightLabel);
            }
            EditorGUILayout.EndVertical();
            GUILayout.EndArea();
        }

        private double printCopyCompleteTime;

        private void DoInformationGUI(int unusedWindowId)
        {
            EditorGUILayout.BeginVertical();
            {
                EditorGUILayout.BeginHorizontal();
                {
                    ManagerUi.Label(ManagerStrings.VERSION, GUILayout.Width(ManagerUiDefine.INFO_SUBJECT_WIDTH));

                    EditorGUILayout.BeginVertical();
                    {
                        ManagerUi.LabelValue(GpmManagerVersion.VERSION);

                        if (ManagerUi.LabelButton(ManagerInfos.GetString(ManagerStrings.INFO_RELEASE_NOTES)) == true)
                        {
                            Application.OpenURL(GpmPathUtil.UrlCombine(GpmManager.DocsUri, ManagerPaths.RELEASE_NOTES_URI_PATH));
                        }
                    }
                    EditorGUILayout.EndVertical();
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.Space();

                EditorGUILayout.BeginHorizontal();
                {
                    ManagerUi.Label(ManagerStrings.INFO_SUPPORT_MAIL, GUILayout.Width(ManagerUiDefine.INFO_SUBJECT_WIDTH));
                    ManagerUi.LabelValue(GpmManager.SupportMail);

                    if (ManagerUi.Button(ManagerStrings.INFO_SUPPORT_MAIL_COPY, EditorStyles.miniButton) == true)
                    {
                        GUIUtility.systemCopyBuffer = GpmManager.SupportMail;

                        printCopyCompleteTime = EditorApplication.timeSinceStartup + ManagerUiDefine.TOAST_TIMER;
                    }
                }
                EditorGUILayout.EndHorizontal();

                if (printCopyCompleteTime > EditorApplication.timeSinceStartup)
                {
                    ManagerUi.Label(ManagerStrings.INFO_SUPPORT_MAIL_COPY_COMPLETED, ManagerUiStyle.ToastLabel);
                }
            }
            EditorGUILayout.EndVertical();
        }

        private void DoImageViewerGUI(Rect contentRect)
        {
            Texture2D texture = serviceDetail.GetImage();

            float imageWidth = 0;
            float imageHeight = 0;

            if (texture.height > contentRect.height)
            {
                imageHeight = contentRect.height - ManagerUiDefine.IMAGE_VIEWER_MIN_HEIGHT_PADDING;
                imageWidth = texture.width * (imageHeight / texture.height);
            }
            else
            {
                imageWidth = texture.width;
                imageHeight = texture.height;
            }

            Rect imageRect = new Rect((position.width - imageWidth) * 0.5f, (position.height - imageHeight) * 0.5f, imageWidth, imageHeight);
            Rect textRect = new Rect(imageRect.x, imageRect.y - 30f, imageWidth, 100);            
            

            EditorGUI.DrawPreviewTexture(imageRect, texture);
            ManagerUi.LabelValue(textRect, serviceDetail.SelectedImageInfo.title, ManagerUiStyle.ImageTitleLabel);

            Rect lectRect = new Rect(imageRect.xMin - 10 - 50, (position.height - 50) * 0.5f, 50, 50);
            Rect rightRect = new Rect(imageRect.xMax + 10, (position.height - 50) * 0.5f, 50, 50);

            if (GUI.Button(lectRect, new GUIContent(ManagerUiIcon.PREV_ICON)) == true)
            {
                serviceDetail.PrevImage();
            }

            if (GUI.Button(rightRect, new GUIContent(ManagerUiIcon.NEXT_ICON)) == true)
            {
                serviceDetail.NextImage();
            }
        }

        private string GetSelectLanguageCode()
        {
            if (selectedLanguageIndex >= languages.Length)
            {
                return string.Empty;
            }

            return languages[selectedLanguageIndex];
        }

        private int GetSelectLanguageIndex(string languageCode)
        {
            for (int i = 0; i < languages.Length; i++)
            {
                if (languages[i].Equals(languageCode) == true)
                {
                    return i;
                }
            }

            return ManagerUiDefine.LANGUAGE_NOT_FOUND;
        }

        private void OnErrorCallback(ManagerError error)
        {
            ManagerUi.ErrorDialog(error);
        }

        private static void CheckInstalledPrevBrandService()
        {
            var installedInfo = GpmManager.Instance.GetPrevBrandInstallInfo();
            if (installedInfo == null)
            {
                return;
            }

            StringBuilder builder = new StringBuilder();
            foreach (var service in installedInfo.installs)
            {
                if (Directory.Exists(GpmPathUtil.Combine(ManagerPaths.PROJECT_PREV_ASSETS_FULL_PATH, service.name)) == true)
                {
                    builder.AppendFormat("\n- {0} v{1}", service.name, service.version);
                }
            }

            if (builder.Length == 0)
            {
                GpmFileUtil.DeleteDirectory(ManagerPaths.PROJECT_PREV_DOWNLOAD_PATH);
                return;
            }

            if (ManagerUi.Dialog(ManagerStrings.WARNING_TITLE_PREV_BRAND_SERVICE,
                string.Format(ManagerInfos.GetString(ManagerStrings.WARNING_MESSAGE_PREV_BRAND_SERVICE), builder.ToString()), false) == true)
            {
                EditorUtility.FocusProjectWindow();
                UnityEngine.Object selectObj = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(GpmPathUtil.Combine(ManagerPaths.PROJECT_PREV_ASSETS_SHORT_PATH, "Common"));
                if (selectObj != null)
                {
                    Selection.activeObject = selectObj;
                    EditorGUIUtility.PingObject(selectObj);
                }
            }

        }
    }
}