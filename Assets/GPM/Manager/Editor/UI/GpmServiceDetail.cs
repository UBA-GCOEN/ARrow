using Gpm.Common.Util;
using Gpm.Manager.Constant;
using Gpm.Manager.Internal;
using Gpm.Manager.Ui.Helper;
using Gpm.Manager.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Gpm.Manager.Ui
{
    internal class GpmServiceDetail : UiView
    {
        private const float IMAGE_SCROLL_TIME = 0.01f;
        private const float IMAGE_LIST_WIDTH = 160f;

        private ServiceInfo service;
        private bool serviceNotFound;
        private bool installableUnityVersion;
        private string installedVersion;
        private bool isLoading = false;

        private List<string> dependencyList = new List<string>();
        private List<string> usingServiceNameList = new List<string>();
        

        private Vector2 infoScrollPos;

        private float imageScrollPosY;
        private readonly Dictionary<string, Texture2D> imageDic = new Dictionary<string, Texture2D>();

        private int selectedImageIndex = 0;
        public ServiceInfo.Image SelectedImageInfo { get; private set; }

        public GpmServiceDetail(GpmManagerWindow window) : base(window)
        {
        }

        public void LoadService(string name)
        {
            Clear();

            GpmManager.Instance.LoadServiceInfo(
                name,
                (info, error) =>
                {
                    service = info;

                    if (service != null)
                    {
                        installedVersion = GpmManager.Instance.Install.GetInstallVersion(service.title);

                        ServiceInfo.DependencyInfo unityDependencyInfo;
                        if (service.dependencies != null && service.dependencies.TryGetValue(ManagerInfos.DEPENDENCY_UNITY_INFO_KEY, out unityDependencyInfo) == true)
                        {
                            installableUnityVersion = StringUtil.IsInstallableUnityVersion(unityDependencyInfo.version);
                        }
                        else
                        {
                            installableUnityVersion = false;
                        }

                        dependencyList.Clear();
                        if (service.dependencies != null)
                        {
                            foreach (KeyValuePair<string, ServiceInfo.DependencyInfo> pair in service.dependencies)
                            {
                                string dependencyServiceName = pair.Key;
                                string dependencyServiceVersion = pair.Value.version;

                                if (dependencyServiceName.Equals(ManagerInfos.DEPENDENCY_UNITY_INFO_KEY) == false)
                                {
                                    dependencyList.Add(dependencyServiceName);
                                }
                            }
                        }

                        usingServiceNameList.Clear();
                        var usingServiceList = GpmManager.Instance.Install.GetUsingServiceList(service.title);
                        for(int i=0;i< usingServiceList.Count;i++)
                        {
                            usingServiceNameList.Add(usingServiceList[i].name);
                        }
                        GpmManager.Instance.DownloadServiceImage(service);
                    }

                    if (error != null)
                    {
                        serviceNotFound = true;
                    }

                    window.Repaint();
                });
        }

        public override void OnGUI(Rect rect)
        {
            EditorGUI.BeginDisabledGroup(isLoading);

            if (service != null)
            {
                if (service.imageList != null && service.imageList.Count > 0)
                {
                    Rect detailRect = new Rect(0, 0, rect.width - IMAGE_LIST_WIDTH, rect.height);
                    Rect imageRect = new Rect(detailRect.width, 0, IMAGE_LIST_WIDTH, rect.height);

                    GUILayout.BeginArea(detailRect);
                    {
                        DoServiceLayout(detailRect);
                    }
                    GUILayout.EndArea();

                    ManagerUi.DrawHorizontalSplitter(detailRect.width, 0, rect.height);

                    GUILayout.BeginArea(imageRect);
                    {
                        EditorGUILayout.BeginVertical();
                        DoImageListLayout(imageRect);
                        EditorGUILayout.EndVertical();
                    }
                    GUILayout.EndArea();
                }
                else
                {
                    DoServiceLayout(rect);
                }
            }
            else
            {
                ManagerUi.Label(
                    serviceNotFound == true ? ManagerStrings.SERVICE_INFO_NOT_FOUND : ManagerStrings.SERVICE_INFO_LOADING,
                    ManagerUiStyle.MiddleCenterLabel);
            }

            EditorGUI.EndDisabledGroup();
        }
        
        public Texture2D GetImage()
        {
            Texture2D texture = null;

            if (SelectedImageInfo != null)
            {
                string path = ManagerPaths.GetCachingPath(service.title, SelectedImageInfo.path);
                imageDic.TryGetValue(path, out texture);
            }

            return texture;
        }

        public override void Clear()
        {
            service = null;
            serviceNotFound = false;
            installableUnityVersion = false;
            installedVersion = string.Empty;
            imageScrollPosY = 0;
        }

        private void DoServiceLayout(Rect rect)
        {
            string serviceVersion = service.version;

            infoScrollPos = EditorGUILayout.BeginScrollView(infoScrollPos);
            EditorGUILayout.BeginVertical();
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    ManagerUi.LabelValue(service.title, ManagerUiStyle.TitleLabel);

                    switch (service.status)
                    {
                        case ServiceStatus.PREPARE:
                            {
                                ManagerUi.Label(ManagerStrings.SERVICE_PREPARE, ManagerUiStyle.RightAlignLabel);
                                break;
                            }
                        case ServiceStatus.PUBLISH:
                            {
                                if (installableUnityVersion == false)
                                {
                                    ManagerUi.Label(ManagerStrings.UNITY_NOT_SUPPORT_VERSION, ManagerUiStyle.WarningVersionLabel);
                                }

                                if (string.IsNullOrEmpty(installedVersion) == true)
                                {
                                    EditorGUI.BeginDisabledGroup(installableUnityVersion == false || GpmManager.ServiceInstaller.IsProcessing == true);
                                    {
                                        if (ManagerUi.Button(ManagerStrings.SERVICE_INSTALL, GUILayout.ExpandWidth(false)) == true)
                                        {
                                            GpmManager.ServiceInstaller.Install(service, OnServiceInstallCompleted);
                                        }
                                    }
                                    EditorGUI.EndDisabledGroup();
                                }
                                else
                                {
                                    serviceVersion = installedVersion;

                                    bool canUpdate = service.version.VersionGreaterThan(installedVersion);

                                    EditorGUI.BeginDisabledGroup(installableUnityVersion == false || GpmManager.ServiceInstaller.IsProcessing == true);
                                    {    
                                        EditorGUI.BeginDisabledGroup(canUpdate == false);

                                        string updateText;
                                        if (canUpdate == true)
                                        {
                                            updateText = string.Format(ManagerInfos.GetString(ManagerStrings.SERVICE_UPDATE_VERSION), service.version);
                                        }
                                        else
                                        {
                                            updateText = ManagerInfos.GetString(ManagerStrings.SERVICE_UPDATE);
                                        }
                                        
                                        if (GUILayout.Button(updateText, GUILayout.ExpandWidth(false)) == true)
                                        {
                                            GpmManager.ServiceInstaller.Install(service, OnServiceInstallCompleted);
                                        }
                                        EditorGUI.EndDisabledGroup();
                                    }
                                    EditorGUI.EndDisabledGroup();

                                    EditorGUI.BeginDisabledGroup(service.title.Equals(ManagerPaths.COMMON_SERVICE_NAME));
                                    if (ManagerUi.Button(ManagerStrings.SERVICE_UNINSTALL, GUILayout.ExpandWidth(false)) == true)
                                    {
                                        StringBuilder builder = new StringBuilder(ManagerInfos.GetString(ManagerStrings.SERVICE_REMOVE_TRY_MESSAGE));

                                        ServiceInfo.Package package = service.GetPackage(service.version);
                                        if (package != null)
                                        {
                                            foreach (var install in package.installList)
                                            {
                                                builder.AppendFormat("\n- {0}", install.path);
                                            }
                                        }

                                        GpmManager.IsLock = true;
                                        GpmDependentServiceFinder.Process(service, GpmManager.Instance.Install.installs, (error) =>
                                        {
                                            GpmManager.IsLock = false;

                                            if (error != null)
                                            {
                                                GpmManager.Instance.CheckServiceInfoVersion(error);
                                                return;
                                            }

                                            if (ManagerUi.TryDialog(ManagerStrings.SERVICE_REMOVE_TRY_TITLE, builder.ToString(), false) == true)
                                            {
                                                GpmManager.ServiceInstaller.Uninstall(service, OnServiceInstallCompleted);
                                            }
                                        });
                                    }
                                    EditorGUI.EndDisabledGroup();
                                }
                                break;
                            }
                    }
                }

                if (string.IsNullOrEmpty(service.version) == false)
                {
                    GUILayout.Space(ManagerUiDefine.DETAIL_ITEM_SPACE_INTERVAL);

                    using (new EditorGUILayout.HorizontalScope())
                    {
                        ManagerUi.Label(ManagerStrings.VERSION, GUILayout.ExpandWidth(false));
                        ManagerUi.LabelValue(serviceVersion);
                    }
                }

                if (service.dependencies != null)
                {
                    ServiceInfo.DependencyInfo unityDependencyInfo;
                    if (service.dependencies.TryGetValue(ManagerInfos.DEPENDENCY_UNITY_INFO_KEY, out unityDependencyInfo) == true && string.IsNullOrEmpty(unityDependencyInfo.version) == false)
                    {
                        GUILayout.Space(ManagerUiDefine.DETAIL_ITEM_SPACE_INTERVAL);

                        using (new EditorGUILayout.HorizontalScope())
                        {
                            ManagerUi.Label(ManagerStrings.SUPPORT_UNITY_VERSION, GUILayout.ExpandWidth(false));
                            ManagerUi.LabelValue(unityDependencyInfo.version);
                        }
                    }
                }

                if (service.linkList != null && service.linkList.Count > 0)
                {
                    GUILayout.Space(ManagerUiDefine.DETAIL_ITEM_SPACE_INTERVAL);

                    using (new EditorGUILayout.HorizontalScope())
                    {
                        for (int i = 0; i < service.linkList.Count; i++)
                        {
                            var link = service.linkList[i];

                            if (ManagerUi.LabelButton(link.name) == true)
                            {
                                Application.OpenURL(GpmPathUtil.UrlCombine(GpmManager.DocsUri, service.title, ManagerUi.GetValueString(link.path)));
                                GpmManagerIndicator.SendLink(service.title, link.name, ManagerUi.GetValueString(link.path));
                            }

                            if (i < service.linkList.Count - 1)
                            {
                                ManagerUi.LabelValue("-", GUILayout.ExpandWidth(false));
                            }
                        }
                    }
                }

                GUILayout.Space(ManagerUiDefine.DETAIL_PAGE_SPACE_INTERVAL);

                ManagerUi.DrawHorizontalSplitter();

                ManagerUi.LabelValue(service.description, ManagerUiStyle.WordWrapLabel, GUILayout.ExpandWidth(false), GUILayout.Width(rect.width - 10));

                GUILayout.Space(ManagerUiDefine.DETAIL_ITEM_SPACE_INTERVAL);

                ManagerUi.DrawHorizontalSplitter();

                if (service.dependencies != null)
                {
                    GUILayout.Space(ManagerUiDefine.DETAIL_PAGE_SPACE_INTERVAL);

                    using (new EditorGUILayout.VerticalScope())
                    {
                        using (new EditorGUILayout.HorizontalScope(EditorStyles.toolbar))
                        {
                            ManagerUi.Label(ManagerStrings.SERVICE_DEPENDENCY);
                        }

                        using (new EditorGUILayout.HorizontalScope())
                        {
                            ManagerUi.Label(ManagerStrings.SERVICE_DEPENDENCY_IS_USING, EditorStyles.boldLabel, GUILayout.Width(100));
                            using (new EditorGUILayout.VerticalScope())
                            {
                                GUILayout.Space(6);
                                if (dependencyList.Count > 0)
                                {
                                    for (int i = 0; i < dependencyList.Count; i++)
                                    {
                                        GUILayout.Label(dependencyList[i], GUILayout.ExpandWidth(false));
                                    }
                                }
                                else
                                {
                                    ManagerUi.Label(ManagerStrings.SERVICE_DEPENDENCY_IS_USING_EMPTY);
                                }
                            }
                        }

                        GUILayout.Space(ManagerUiDefine.DETAIL_ITEM_SPACE_INTERVAL);

                        using (new EditorGUILayout.HorizontalScope())
                        {
                            ManagerUi.Label(ManagerStrings.SERVICE_DEPENDENCY_USED_BY, EditorStyles.boldLabel, GUILayout.Width(100));
                            using (new EditorGUILayout.VerticalScope())
                            {
                                GUILayout.Space(6);
                                if (usingServiceNameList.Count > 0)
                                {
                                    for (int i = 0; i < usingServiceNameList.Count; i++)
                                    {
                                        GUILayout.Label(usingServiceNameList[i], GUILayout.ExpandWidth(false));
                                    }
                                }
                                else
                                {
                                    ManagerUi.Label(ManagerStrings.SERVICE_DEPENDENCY_USED_BY_EMPTY);
                                }
                            }
                        }

                        ManagerUi.LabelValue("");
                    }
                }
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndScrollView();
        }

        private void DoImageListLayout(Rect rect)
        {
            Rect imageRect = new Rect(10, 10 - imageScrollPosY, 140, 90);

            float listWidth = rect.width;
            float listHeight = rect.height - 60;
            float listItemViewCount = listHeight / ManagerUiDefine.IMAGE_LIST_ITEM_INTERVAL;
            int listCount = service.imageList.Count;
            int listStartItemPos = (int)(imageScrollPosY / ManagerUiDefine.IMAGE_LIST_ITEM_INTERVAL);

            Rect scrollUpButton = new Rect(0, 0, listWidth, ManagerUiDefine.IMAGE_LIST_SCROLL_BUTTON);
            Rect scrollDownButton = new Rect(0, listHeight, listWidth, ManagerUiDefine.IMAGE_LIST_SCROLL_BUTTON);
            
            if (imageScrollPosY > 0 && ManagerUi.Button(scrollUpButton, ManagerStrings.LIST_SCROLL_UP, ManagerUiStyle.ImageScrollButton) == true)
            {
                EditorCoroutine.Start(DoImageScroll(imageScrollPosY, imageScrollPosY - ManagerUiDefine.IMAGE_LIST_ITEM_INTERVAL, (value) =>
                {
                    imageScrollPosY = value;
                    window.Repaint();
                }));
            }

            if (listCount - listStartItemPos > listItemViewCount &&
                ManagerUi.Button(scrollDownButton, ManagerStrings.LIST_SCROLL_DOWN, ManagerUiStyle.ImageScrollButton) == true)
            {
                EditorCoroutine.Start(DoImageScroll(imageScrollPosY, imageScrollPosY + ManagerUiDefine.IMAGE_LIST_ITEM_INTERVAL, (value) =>
                {
                    imageScrollPosY = value;
                    window.Repaint();
                }));
            }

            for (int index = 0; index < service.imageList.Count; index++)
            {
                var image = service.imageList[index];

                string fullpath = ManagerPaths.GetCachingPath(service.title, image.path);

                Texture2D texture;
                if (imageDic.TryGetValue(fullpath, out texture) == false)
                {
                    if (File.Exists(fullpath) == true)
                    {
                        byte[] fileData = File.ReadAllBytes(fullpath);
                        texture = new Texture2D(ManagerUiDefine.IMAGE_LIST_ITEM_WIDTH, ManagerUiDefine.IMAGE_LIST_ITEM_HEGITH);
                        texture.LoadImage(fileData);

                        imageDic.Add(fullpath, texture);
                    }
                }

                if (texture != null)
                {
                    if (GUI.Button(imageRect, texture, ManagerUiStyle.CopyrightBox) == true)
                    {
                        selectedImageIndex = index;
                        SelectedImageInfo = image;

                        EditorApplication.delayCall += () =>
                        {
                            window.SendEvent(EditorGUIUtility.CommandEvent(ManagerEvents.IMAGE_SELECT));
                        };
                    }

                    EditorGUI.DrawPreviewTexture(imageRect, texture);

                    imageRect.y += ManagerUiDefine.IMAGE_LIST_ITEM_INTERVAL;
                }
            }
            
            if (imageScrollPosY > 0)
            {
                ManagerUi.Button(scrollUpButton, ManagerStrings.LIST_SCROLL_UP, ManagerUiStyle.ImageScrollButton);
            }

            if (listCount - listStartItemPos > listItemViewCount)
            {
                ManagerUi.Button(scrollDownButton, ManagerStrings.LIST_SCROLL_DOWN, ManagerUiStyle.ImageScrollButton);
            }
        }

        public void PrevImage()
        {
            selectedImageIndex--;
            if (selectedImageIndex < 0)
            {
                selectedImageIndex = service.imageList.Count - 1;
            }

            SelectedImageInfo = service.imageList[selectedImageIndex];
        }
        public void NextImage()
        {
            selectedImageIndex++;
            if (selectedImageIndex >= service.imageList.Count)
            {
                selectedImageIndex = 0;
            }

            SelectedImageInfo = service.imageList[selectedImageIndex];
        }

        private IEnumerator DoImageScroll(float a, float b, Action<float> callback)
        {
            float startTime = (float)EditorApplication.timeSinceStartup;

            float t = 0;
            while (t < 1f)
            {
                t = ((float)EditorApplication.timeSinceStartup - startTime) / IMAGE_SCROLL_TIME;
                float data = Mathf.SmoothStep(a, b, t);
                callback(data);

                yield return null;
            }
        }

        private void OnServiceInstallCompleted(ManagerError error)
        {
            if (error != null)
            {
                GpmManager.Instance.CheckServiceInfoVersion(error);
                return;
            }

            GpmManager.IsLock = false;
            installedVersion = GpmManager.Instance.Install.GetInstallVersion(service.title);

            if (GpmManagerWindow.window != null)
            {
                GpmManagerWindow.window.Repaint();
            }
        }
    }
}