using Gpm.Common.Util;
using Gpm.Manager.Constant;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace Gpm.Manager.Internal
{
    internal static class GpmDependentServiceFinder
    {
        public static void Process(ServiceInfo findService, List<InstallInfo.Service> installedServices, Action<ManagerError> callback)
        {
            if (installedServices.Count <= 0)
            {
                callback(null);
                return;
            }

            EditorCoroutine.Start(RefreshDepedencyInfo(installedServices, findService.title,
                (dependecies, error) =>
                {
                    if (error != null)
                    {
                        callback(error);
                        return;
                    }

                    if (dependecies.Count == 0)
                    {
                        callback(null);
                    }
                    else
                    {
                        StringBuilder builder = new StringBuilder();
                        foreach (var service in dependecies)
                        {
                            builder.AppendFormat("\n- {0}", service);
                        }

                        callback(new ManagerError(ManagerErrorCode.UNINSTALL, string.Format(ManagerInfos.GetString(ManagerStrings.ERROR_MESSAGE_DEPENDENCY_SERVICE_REMOVE_FAILED), builder.ToString()), isFullMessage : true));
                    }
                }));
        }

        private static IEnumerator RefreshDepedencyInfo(List<InstallInfo.Service> installedServices, string findServiceName, Action<List<string>, ManagerError> callback)
        {
            List<string> dependencyServices = new List<string>();

            ManagerError occurrenceError = null;
            foreach (var service in installedServices)
            {
                IEnumerator innerCoroutineEnumerator = RefreshServiceInfo(service.name, (serviceInfo, error) => 
                {
                    occurrenceError = error;

                    if (serviceInfo != null && serviceInfo.dependencies.ContainsKey(findServiceName) == true)
                    {
                        dependencyServices.Add(serviceInfo.title);
                    }
                });

                while (innerCoroutineEnumerator.MoveNext() == true)
                {
                    yield return innerCoroutineEnumerator.Current;
                }

                if (occurrenceError != null)
                {
                    callback(null, occurrenceError);
                    yield break;
                }
            }

            callback(dependencyServices, null);
        }


        private static IEnumerator RefreshServiceInfo(string serviceName, Action<ServiceInfo, ManagerError> callback)
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
                else if (Common.UnityWebRequestHelper.IsError(request) == true)
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
                callback(service, new ManagerError(ManagerErrorCode.UNINSTALL, errorMessage, errorSubMessage));
            }
            else
            {
                callback(service, null);
            }
        }
    }
}