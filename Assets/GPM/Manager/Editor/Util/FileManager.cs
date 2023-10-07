using Gpm.Common;
using Gpm.Common.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine.Networking;

namespace Gpm.Manager.Util
{
    internal static class FileManager
    {
        public enum StateCode
        {
            SUCCESS,
            FILE_NOT_FOUND_ERROR,
            WEB_REQUEST_ERROR,
            UNKNOWN_ERROR,
        }

        private static List<EditorCoroutine> coroutineList = new List<EditorCoroutine>();

        public static void DownloadFileToLocal(string remoteFilename, string localFilename, Action<StateCode, string> callback, Action<float> callbackProgress = null)
        {
            EditorCoroutine downloadFileCoroutine = null;

            downloadFileCoroutine = EditorCoroutine.Start(
                DownloadFile(
                    remoteFilename,
                    (stateCode, message, data) =>
                    {
                        if(StateCode.SUCCESS == stateCode)
                        {
                            try
                            {
                                File.WriteAllBytes(localFilename, data);
                                callback(StateCode.SUCCESS, null);
                            }                            
                            catch (Exception e)
                            {
                                callback(StateCode.UNKNOWN_ERROR, e.Message);
                            }
                        }
                        else
                        {
                            callback(stateCode, message);
                        }

                        if (downloadFileCoroutine != null)
                        {
                            coroutineList.Remove(downloadFileCoroutine);
                            downloadFileCoroutine = null;
                        }
                    },
                    callbackProgress
                    )
                );
            coroutineList.Add(downloadFileCoroutine);
        }

        public static void DownloadFileToString(string remoteFilename, Action<StateCode, string, string> callback, Action<float> callbackProgress = null)
        {
            EditorCoroutine downloadFileCoroutine = null;
            downloadFileCoroutine = EditorCoroutine.Start(
                DownloadFile(
                    remoteFilename,
                    (stateCode, message, data) =>
                    {
                        string encoding = null;
                        if(data != null)
                        {
                            encoding = System.Text.Encoding.Default.GetString(data);
                        }

                        callback(stateCode, message, encoding);

                        if (downloadFileCoroutine != null)
                        {
                            coroutineList.Remove(downloadFileCoroutine);
                            downloadFileCoroutine = null;
                        }
                    },
                    callbackProgress
                    )
                );
            coroutineList.Add(downloadFileCoroutine);
        }

        public static void StopDownloadFile()
        {
            foreach (EditorCoroutine coroutine in coroutineList)
            {
                if (coroutine != null)
                {
                    coroutine.Stop();
                }
            }

            coroutineList.Clear();
        }

        private static IEnumerator DownloadFile(string remoteFilename, Action<StateCode, string, byte[]> callback, Action<float> callbackProgress = null)
        {
            UnityWebRequest www = UnityWebRequest.Get(remoteFilename);

            yield return www.SendWebRequest();

            while (true)
            {
                if (www.isDone == true)
                {
                    if (UnityWebRequestHelper.IsNotFoundError(www) == true)
                    {
                        callback(StateCode.FILE_NOT_FOUND_ERROR, remoteFilename, null);
                        break;
                    }
                    else if (UnityWebRequestHelper.IsError(www) == true)
                    {
                        callback(StateCode.WEB_REQUEST_ERROR, www.error, null);
                        yield break;
                    }
                    else
                    {
                        try
                        {
                            callback(StateCode.SUCCESS, null, www.downloadHandler.data);
                        }
                        catch (Exception e)
                        {
                            callback(StateCode.UNKNOWN_ERROR, e.Message, null);
                        }
                        yield break;
                    }
                }
                else
                {
                    if (callbackProgress != null)
                    {
                        callbackProgress(www.downloadProgress);
                    }
                }

                yield return null;
            }
        }
    }
}