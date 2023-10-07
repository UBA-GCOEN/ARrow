using Gpm.Common.Util;
using Gpm.Manager.Constant;
using Gpm.Manager.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using UnityEngine.Networking;

namespace Gpm.Manager.Internal
{
    internal class GpmServiceDownloader
    {
        private const int DOWNLOAD_BUFFER = 32 * 1024;

        private readonly Queue<PackageInstallInfo> downloadQueue = new Queue<PackageInstallInfo>();

        public void Process(ServiceInfo service, List<string> dependencyServices, Action<ManagerError, List<PackageInstallInfo>> callback)
        {
            Action downloadAction = () =>
            {
                ServiceInfo.Package package = service.GetPackage(service.version);
                if(package == null)
                {
                    callback(new ManagerError(ManagerErrorCode.INSTALL,
                                    string.Format(ManagerInfos.GetString(ManagerStrings.ERROR_MESSAGE_DEPENDENCY_SERVICE_INSTALL_FAILED), service.title),
                                    ManagerInfos.GetString(ManagerStrings.UNITY_NOT_SUPPORT_VERSION)), null);
                    return;
                }

                foreach (var install in package.installList)
                {
                    downloadQueue.Enqueue(new PackageInstallInfo
                    {
                        serviceName = service.title,
                        serviceVersion = service.version,
                        packageName = install.name,
                        packageIntallPath = install.path
                    });
                }

                EditorCoroutine.Start(DownloadPackages(callback));
            };

            if (dependencyServices.Count <= 0)
            {
                downloadAction();
                return;
            }

            EditorCoroutine.Start(RefreshDepedencyInfo(dependencyServices,
                (error) =>
                {
                    if (error == null)
                    {
                        downloadAction();
                    }
                    else
                    {
                        callback(error, null);
                    }
                }));
        }

        private IEnumerator RefreshDepedencyInfo(List<string> dependencyServices, Action<ManagerError> callback)
        {
            ManagerError occurrenceError = null;
            foreach (string serviceName in dependencyServices)
            {
                bool isExsist = false;
                foreach(PackageInstallInfo downloadInfo in downloadQueue)
                {
                    if (downloadInfo.serviceName.Equals(serviceName))
                    {
                        isExsist = true;
                        break;
                    }
                }
                if (isExsist == true)
                {
                    continue;
                }

                IEnumerator innerCoroutineEnumerator = RefreshServiceInfo(serviceName, (error) => { occurrenceError = error; });
                while (innerCoroutineEnumerator.MoveNext() == true)
                {
                    yield return innerCoroutineEnumerator.Current;
                }

                if (occurrenceError != null)
                {
                    callback(occurrenceError);
                    yield break;
                }
            }

            callback(null);
        }

        private IEnumerator RefreshServiceInfo(string serviceName, Action<ManagerError> callback)
        {
            string path = GpmPathUtil.Combine(serviceName, ManagerPaths.SERVICE_FILE_NAME);
            ServiceInfo service = null;
            bool isError = false;

            string errorMessage = string.Empty;
            string errorSubMessage = string.Empty;

            yield return GpmManager.SendRequest(path, (request) =>
            {
                if (Common.UnityWebRequestHelper.IsConnectionError(request) == true)
                {
                    errorMessage = ManagerStrings.ERROR_MESSAGE_NETWORK;
                    errorSubMessage = string.Format("Service= {0}, Code= {1}, Message= {2}", serviceName, request.responseCode, request.error);
                    isError = true;
                    return;
                }
                else if(Common.UnityWebRequestHelper.IsError(request) == true)
                {
                    errorMessage = ManagerStrings.ERROR_MESSAGE_DOWNLOAD_FAILED;
                    errorSubMessage = string.Format("Service= {0}, Code= {1}, Message= {2}", serviceName, request.responseCode, request.error);
                    isError = true;
                    return;
                }
                else if (Common.UnityWebRequestHelper.IsSuccess(request) == true)
                {
                    XmlHelper.LoadXmlFromText<ServiceInfo>(
                        request.downloadHandler.text,
                        (responseCode, xmlData, message) =>
                        {
                            if (responseCode != XmlHelper.ResponseCode.SUCCESS)
                            {
                                errorMessage = ManagerStrings.ERROR_MESSAGE_SERVICE_INFO_LOAD_FAILED;
                                errorSubMessage = string.Format("Service= {0}, Code= {1}, Message= {2}", serviceName, responseCode, message);
                                isError = true;
                                return;
                            }

                            service = xmlData;
                        });
                }
            });
            
            if (isError == true)
            {
                callback(new ManagerError(ManagerErrorCode.INSTALL, errorMessage, errorSubMessage));
            }
            else
            {
                List<string> serviceDependencies = new List<string>();
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
                        if (dependencyInfo.Value.install == ServiceInstall.AUTO)
                        {
                            serviceDependencies.Add(dependencyServiceName);
                        }
                    }

                    ManagerError returnError = null;
                    yield return RefreshDepedencyInfo(serviceDependencies, (error)=>
                    {
                        returnError = error;
                    });

                    if (returnError != null)
                    {
                        callback(returnError);
                        yield break;
                    }
                }

                bool installableUnityVersion = StringUtil.IsInstallableUnityVersion(service.dependencies[ManagerInfos.DEPENDENCY_UNITY_INFO_KEY].version);
                if (installableUnityVersion == true)
                {
                    ServiceInfo.Package package = service.GetPackage(service.version);
                    if (package != null)
                    {
                        foreach (var install in package.installList)
                        {
                            downloadQueue.Enqueue(new PackageInstallInfo
                            {
                                serviceName = service.title,
                                serviceVersion = service.version,
                                packageName = install.name,
                                packageIntallPath = install.path
                            });
                        }
                        callback(null);
                    }
                    else
                    {
                        callback(new ManagerError(ManagerErrorCode.INSTALL,
                                    string.Format(ManagerInfos.GetString(ManagerStrings.ERROR_MESSAGE_DEPENDENCY_SERVICE_INSTALL_FAILED), service.title),
                                    ManagerInfos.GetString(ManagerStrings.UNITY_NOT_SUPPORT_VERSION)));
                    }
                }
                else
                {
                    callback(new ManagerError(ManagerErrorCode.INSTALL,
                        string.Format(ManagerInfos.GetString(ManagerStrings.ERROR_MESSAGE_DEPENDENCY_SERVICE_INSTALL_FAILED), service.title),
                        ManagerInfos.GetString(ManagerStrings.UNITY_NOT_SUPPORT_VERSION)));
                }
            }
        }

        private IEnumerator DownloadPackages(Action<ManagerError, List<PackageInstallInfo>> callback)
        {
            int downloadCount = downloadQueue.Count;
            List<PackageInstallInfo> downloadedList = new List<PackageInstallInfo>();

            ManagerError error = null;

            while (downloadQueue.Count > 0)
            {
                var downloadInfo = downloadQueue.Dequeue();

                string downloadUrl = GpmPathUtil.UrlCombine(GpmManager.CdnUri, downloadInfo.serviceName, downloadInfo.packageName);
                string localFilePath = ManagerPaths.GetCachingPath(downloadInfo.serviceName, downloadInfo.serviceVersion, downloadInfo.packageName);

                if (File.Exists(localFilePath) == true)
                {
                    downloadCount--;
                    downloadedList.Add(downloadInfo);
                }
                else
                {
                    string localPath = Path.GetDirectoryName(localFilePath);
                    if (Directory.Exists(localPath) == false)
                    {
                        Directory.CreateDirectory(localPath);
                    }

                    DownloadHandlerFile handler = new DownloadHandlerFile(localFilePath);
                    handler.removeFileOnAbort = true;

                    UnityWebRequest request = UnityWebRequest.Get(downloadUrl);
                    request.downloadHandler = handler;

                    UnityWebRequestAsyncOperation op = request.SendWebRequest();

                    while (op.isDone == false)
                    {
                        yield return null;
                    }

                    if (Common.UnityWebRequestHelper.IsConnectionError(request) == true)
                    {
                        if (error == null)
                        {
                            error = new ManagerError(ManagerErrorCode.NETWORK, ManagerStrings.ERROR_MESSAGE_NETWORK, request.error);
                        }
                            downloadCount--;
                    }
                    else if (Common.UnityWebRequestHelper.IsError(request) == true)
                    {
                        if (error == null)
                        {
                            error = new ManagerError(ManagerErrorCode.INSTALL, ManagerStrings.ERROR_MESSAGE_DOWNLOAD_FAILED, request.error);
                        }
                        downloadCount--;
                    }
                    else if(Common.UnityWebRequestHelper.IsSuccess(request) == true)
                    {
                        downloadCount--;
                        downloadedList.Add(downloadInfo);
                    }
                }
            }

            while (downloadCount > 0)
            {
                yield return null;
            }
            
            callback(error, downloadedList);
        }

        private bool IsNetworkError(Exception exception)
        {
            WebException webException = exception as WebException;
            if (webException == null)
            {
                return false;
            }

            return webException.Status == WebExceptionStatus.ReceiveFailure ||
                webException.Status == WebExceptionStatus.ConnectFailure ||
                webException.Status == WebExceptionStatus.KeepAliveFailure;
        }
    }
}