using System.IO;
using Gpm.Common.Log;
using Gpm.Common.Util;
using Gpm.Manager.Constant;
using Gpm.Manager.Internal;
using Gpm.Manager.Ui.Helper;
using Gpm.Manager.Util;
using UnityEditor;
using UnityEngine;

namespace Gpm.Manager.Ui
{
    internal class GpmServiceList : UiView
    {
        private ServiceList serviceList;
        private Vector2 scrollPos;

        private int selectedIndex = -1;

        public string SelectedService
        {
            get { return serviceList.list[selectedIndex].name; }
        }


        public GpmServiceList(GpmManagerWindow window) : base(window)
        {
        }

        public override void Clear()
        {
            selectedIndex = -1;
        }

        public override void OnGUI(Rect rect)
        {
            serviceList = GpmManager.Instance.Services;
            if (serviceList == null || serviceList.list.Count == 0)
            {
                ManagerUi.Label(ManagerStrings.SERVICE_LIST_NOT_FOUND, ManagerUiStyle.MiddleCenterLabel);
                return;
            }

            if (selectedIndex == -1)
            {
                if (File.Exists(ManagerPaths.TEMP_REFRESH_FILE_PATH) == true)
                {
                    var refreshInfo = JsonUtility.FromJson<UiRefreshInfo>(File.ReadAllText(ManagerPaths.TEMP_REFRESH_FILE_PATH));
                    selectedIndex = serviceList.list.FindIndex(data => data.name.Equals(refreshInfo.lastServiceName));

                    GpmFileUtil.DeleteFile(ManagerPaths.TEMP_REFRESH_FILE_PATH);
                }
                else
                {
                    selectedIndex = 0;
                }

                EditorApplication.delayCall += () =>
                {
                    window.SendEvent(EditorGUIUtility.CommandEvent(ManagerEvents.CHANGE_SERVICE));
                };
            }

            float height = 24;

            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            for (int index = 0; index < serviceList.list.Count; index++)
            {
                ServiceList.Service service = serviceList.list[index];
                Rect itemRect = new Rect(0, index * height, rect.width, height);

                bool selected = (index == selectedIndex);
                
                if (GUI.Button(itemRect, "", selected ? ManagerUiStyle.ListSelectedButton : ManagerUiStyle.ListNormalButton) == true)
                {
                    selectedIndex = index;
                    EditorApplication.delayCall += () =>
                    {
                        window.SendEvent(EditorGUIUtility.CommandEvent(ManagerEvents.CHANGE_SERVICE));
                    };
                }

                string serviceVersion = service.version;
                using (new EditorGUILayout.HorizontalScope(GUILayout.Height(height - 2f)))
                {
                    string installedVersion = GpmManager.Instance.Install.GetInstallVersion(service.name);

                    if (GpmManager.ServiceInstaller.IsProcessing == true && service.name.Equals(GpmManager.ServiceInstaller.ProcessService.title) == true)
                    {
                        ManagerUi.LabelValue(ManagerUiIcon.StatusWheel, GUILayout.Width(18f));
                    }
                    else if (string.IsNullOrEmpty(installedVersion) == false)
                    {
                        if (string.IsNullOrEmpty(service.version) == true)
                        {
                            GpmLogger.Error(string.Format("Check version of service in the service list. (Service= {0})", service.name), ManagerInfos.SERVICE_NAME, GetType());
                        }
                        else
                        {
                            serviceVersion = installedVersion;
                            if (service.version.VersionGreaterThan(installedVersion) == true)
                            {
                                ManagerUi.LabelValue(ManagerUiIcon.REFRESH_ICON, GUILayout.Width(18f), GUILayout.Height(height - 2f));
                            }
                            else
                            {
                                ManagerUi.LabelValue(ManagerUiIcon.CHECK_ICON, GUILayout.Width(18f), GUILayout.Height(height - 2f));
                            }
                        }
                    }
                    else
                    {
                        ManagerUi.LabelValue(string.Empty, GUILayout.Width(18f));
                    }

                    ManagerUi.LabelValue(service.name, ManagerUiStyle.ListLabel);
                    ManagerUi.LabelValue(serviceVersion, ManagerUiStyle.ListVersionLabel);
                }

                ManagerUi.DrawVerticalSplitter(0, itemRect.y + itemRect.height, rect.width);
            }
            EditorGUILayout.EndScrollView();
        }
    }
}