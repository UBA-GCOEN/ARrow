using Gpm.Common.Compress;
using Gpm.Common.Util;
using Gpm.Manager.Constant;
using Gpm.Manager.Util;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Gpm.Manager.Internal
{
    internal class PackageInstallInfo
    {
        public string serviceName;
        public string serviceVersion;
        public string packageName;
        public string packageIntallPath;
    }

    internal class GpmServiceInstaller
    {
        private readonly GpmServiceDownloader downloader = new GpmServiceDownloader();
        private List<string> serviceDependencies = new List<string>();

        public ServiceInfo ProcessService { get; private set; }

        public bool IsProcessing { get { return ProcessService != null; } }
        
        public void Install(ServiceInfo service, Action<ManagerError> callback)
        {
            if (IsProcessing == true)
            {
                callback(new ManagerError(ManagerErrorCode.INSTALL, ManagerStrings.ERROR_MESSAGE_ALREADY_INSTALL));
                return;
            }
            
            if (service == null)
            {
                callback(new ManagerError(ManagerErrorCode.INSTALL, ManagerStrings.ERROR_MESSAGE_PARAMETER, "Install service info null."));
                return;
            }

            string installedVersion = GpmManager.Instance.Install.GetInstallVersion(service.title);
            if (string.IsNullOrEmpty(installedVersion) == false && service.version.VersionGreaterThan(installedVersion) == false)
            {
                callback(new ManagerError(ManagerErrorCode.INSTALL, ManagerStrings.ERROR_MESSAGE_ALREADY_INSTALLED));
                return;
            }

            serviceDependencies.Clear();
            foreach (var dependencyInfo in service.dependencies)
            {
                string dependencyServiceName = dependencyInfo.Key;
                if (dependencyServiceName.Equals(ManagerInfos.DEPENDENCY_UNITY_INFO_KEY) == true)
                {
                    continue;
                }
                
                InstallInfo.Service dependencyServiceInfo = GpmManager.Instance.Install.GetInstallInfo(dependencyServiceName);
                bool installable = (dependencyServiceInfo == null || string.IsNullOrEmpty(dependencyServiceInfo.version) == true) ? true : dependencyInfo.Value.version.VersionGreaterThan(dependencyServiceInfo.version);
                if (installable == true)
                {
                    if (dependencyInfo.Value.install == ServiceInstall.MANUAL)
                    {
                        callback(new ManagerError(ManagerErrorCode.INSTALL, string.Format(ManagerInfos.GetString(ManagerStrings.ERROR_MESSAGE_MANUAL_UPDATE_SERVICE), dependencyServiceName), isFullMessage: true));
                        return;
                    }
                    else
                    {
                        serviceDependencies.Add(dependencyServiceName);
                    }
                }
            }

            GpmManager.IsLock = true;
            ProcessService = service;

            downloader.Process(ProcessService, serviceDependencies,
                (error, downloadedList) =>
                {
                    if (error != null)
                    {
                        callback(error);
                        ProcessService = null;
                        GpmManager.IsLock = false;
                        return;
                    }
                    
                    EditorApplication.LockReloadAssemblies();
                    try
                    {
                        if (Directory.Exists(ManagerPaths.BACKUP_PATH) == true)
                        {
                            GpmFileUtil.DeleteDirectory(ManagerPaths.BACKUP_PATH);
                        }

                        foreach (var packageInfo in downloadedList)
                        {
                            string installPath = GpmPathUtil.Combine(Application.dataPath, packageInfo.packageIntallPath);
                            if (Directory.Exists(installPath) == true)
                            {
                                string backupPackagePath = GpmPathUtil.Combine(ManagerPaths.BACKUP_PATH, packageInfo.packageIntallPath);
                                string backupPackagePathMeta = string.Format("{0}.meta", backupPackagePath);
                                string installPathMeta = string.Format("{0}.meta", installPath);
                                GpmFileUtil.CopyDirectory(installPath, backupPackagePath, true);
                                GpmFileUtil.CopyFile(installPathMeta, backupPackagePathMeta);
                                GpmFileUtil.DeleteDirectory(installPath);
                                GpmFileUtil.DeleteFile(installPathMeta);
                            }

                            string packagePath = ManagerPaths.GetCachingPath(packageInfo.serviceName, packageInfo.serviceVersion, packageInfo.packageName);

                            AssetDatabase.Refresh();
                            
                            var responseCode = GpmCompress.ExtractUnityPackage(packagePath, ManagerPaths.TEMP_PATH, ManagerPaths.PROJECT_ROOT_PATH);
                            if (responseCode == CompressResultCode.SUCCESS)
                            {
                                AssetDatabase.Refresh();
                            }
                            else
                            {
                                ProcessService = null;
                                throw new Exception(string.Format("Unpack error. Code= {0}", responseCode));
                            }
                            
                            List<string> dependencies = new List<string>();
                            foreach (var dependencyInfo in service.dependencies)
                            {
                                string dependencyServiceName = dependencyInfo.Key;
                                if (dependencyServiceName.Equals(ManagerInfos.DEPENDENCY_UNITY_INFO_KEY) == true)
                                {
                                    continue;
                                }

                                if (dependencyServiceName.Equals(packageInfo.serviceName) == true)
                                {
                                    continue;
                                }

                                dependencies.Add(dependencyServiceName);
                            }

                            GpmManager.Instance.Install.AddService(packageInfo.serviceName, packageInfo.serviceVersion, dependencies.ToArray());
                        }

                        ProcessService = null;

                        var refreshInfo = new UiRefreshInfo
                        {
                            lastServiceName = service.title
                        };

                        Directory.CreateDirectory(ManagerPaths.LIBRARY_PATH);
                        File.WriteAllText(ManagerPaths.TEMP_REFRESH_FILE_PATH, JsonUtility.ToJson(refreshInfo));

                        foreach (var packageInfo in downloadedList)
                        {
                            if (packageInfo.serviceName.Equals(service.title) == true)
                            {
                                if (string.IsNullOrEmpty(installedVersion) == true)
                                {
                                    GpmManagerIndicator.SendInstall(packageInfo.serviceName, packageInfo.serviceVersion, service.title);
                                }
                                else
                                {
                                    GpmManagerIndicator.SendUpdate(service.title, service.version, installedVersion);
                                }
                            }
                            else
                            {
                                GpmManagerIndicator.SendInstall(packageInfo.serviceName, packageInfo.serviceVersion, service.title);
                            }   
                        }

                        callback(null);
                    }
                    catch (Exception e)
                    {
                        ProcessService = null;
                        GpmManager.IsLock = false;
                        callback(new ManagerError(ManagerErrorCode.INSTALL, ManagerStrings.ERROR_MESSAGE_INSTALL_FAILED, e.Message));
                    }
                    EditorApplication.UnlockReloadAssemblies();
                });
        }

        public void Uninstall(ServiceInfo service, Action<ManagerError> callback)
        {
            if (IsProcessing == true)
            {
                callback(new ManagerError(ManagerErrorCode.UNINSTALL, ManagerStrings.ERROR_MESSAGE_ALREADY_INSTALL));
                return;
            }

            GpmManager.IsLock = true;
            ProcessService = service;

            EditorApplication.LockReloadAssemblies();
            try
            {
                bool deletedScriptFile = false;
                ServiceInfo.Package package = service.GetPackage(service.version);
                if (package != null)
                {
                    foreach (var install in package.installList)
                    {
                        if (string.IsNullOrEmpty(install.path) == true)
                        {
                            callback(new ManagerError(ManagerErrorCode.UNINSTALL, ManagerStrings.ERROR_MESSAGE_INSTALL_PATH_NOT_FOUND, install.path));
                            continue;
                        }

                        string path = GpmPathUtil.Combine(Application.dataPath, install.path);
                        string pathMeta = string.Format("{0}.meta", path);

                        if (Directory.Exists(path) == true)
                        {
                            if (deletedScriptFile == false)
                            {
                                deletedScriptFile = GpmFileUtil.IsScriptFile(path);
                            }

                            GpmFileUtil.DeleteDirectory(path);
                        }

                        if (File.Exists(pathMeta) == true)
                        {
                            GpmFileUtil.DeleteFile(pathMeta);
                        }
                    }
                }

                if (deletedScriptFile == false)
                {
                    GpmManager.IsLock = false;
                }

                GpmManager.Instance.Install.RemoveService(service);

                var refreshInfo = new UiRefreshInfo
                {
                    lastServiceName = service.title
                };

                Directory.CreateDirectory(ManagerPaths.LIBRARY_PATH);
                File.WriteAllText(ManagerPaths.TEMP_REFRESH_FILE_PATH, JsonUtility.ToJson(refreshInfo));
                AssetDatabase.Refresh();

                ProcessService = null;

                GpmManagerIndicator.SendRemove(service.title, service.version);

                callback(null);
            }
            catch (Exception e)
            {
                ProcessService = null;
                GpmManager.IsLock = false;
                callback(new ManagerError(ManagerErrorCode.UNINSTALL, ManagerStrings.ERROR_MESSAGE_REMOVE_FAILED, e.Message));
            }
            EditorApplication.UnlockReloadAssemblies();
        }
    }
}