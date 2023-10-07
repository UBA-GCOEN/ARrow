using Gpm.Common.Util;
using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

namespace Gpm.Common.Multilanguage.Internal
{
    public interface IMultilanguageLoader
    {
        void Load(string filepath, Action<MultilanguageResultCode, MultilanguageXml, string> callback);
    }


    internal class MultilanguageLoader : IMultilanguageLoader
    {
        private const string STREAMING_ASSETS_DIRECTORY_NAME = "/StreamingAssets/";
        private const string RESOURCE_DIRECTORY_NAME = "/Resources/";

        public enum LoadType
        {
            LOCAL_FILE,
            DOWNLOAD_FILE,
            STREAMING_ASSET,
            RESOURCE_ASSET
        }

        private MonoBehaviour monoObject;

        public void Load(string filepath, Action<MultilanguageResultCode, MultilanguageXml, string> callback)
        {
            LoadType loadType = LoadType.LOCAL_FILE;
            if (IsWebPath(filepath) == true)
            {
                loadType = LoadType.DOWNLOAD_FILE;
            }
            else
            {
                if (filepath.StartsWith("/", StringComparison.Ordinal) == false)
                {
                    filepath = string.Format("/{0}", filepath);
                }

                if (filepath.StartsWith(STREAMING_ASSETS_DIRECTORY_NAME, StringComparison.Ordinal) == true)
                {
                    loadType = LoadType.STREAMING_ASSET;
                    filepath = filepath.Replace(STREAMING_ASSETS_DIRECTORY_NAME, "");
                }
                else if (filepath.Contains(RESOURCE_DIRECTORY_NAME) == true)
                {
                    loadType = LoadType.RESOURCE_ASSET;

                    int startIndex = filepath.LastIndexOf(RESOURCE_DIRECTORY_NAME, StringComparison.Ordinal) + RESOURCE_DIRECTORY_NAME.Length - 1;
                    filepath = filepath.Substring(startIndex);

                    int commaLastIndex = filepath.LastIndexOf(".", StringComparison.Ordinal);
                    if (commaLastIndex > -1)
                    {
                        filepath = filepath.Substring(0, commaLastIndex);
                    }
                }

                filepath = filepath.TrimStart('/');
            }

            switch (loadType)
            {
                case LoadType.LOCAL_FILE:
                    {
                        string localPath = Path.Combine(Application.dataPath, filepath);
                        LoadLocalFile(localPath, callback);
                        break;
                    }
                case LoadType.DOWNLOAD_FILE:
                    {
                        LoadDownloadFile(filepath, false, callback);
                        break;
                    }
                case LoadType.STREAMING_ASSET:
                    {
                        string streamingAssetsPath = string.Empty;

#if UNITY_ANDROID && !UNITY_EDITOR
                        streamingAssetsPath = string.Format("jar:file://{0}!/assets/{1}", Application.dataPath, filepath);
#else
                        streamingAssetsPath = Path.Combine(Application.streamingAssetsPath, filepath);
#endif

                        if (IsWebPath(streamingAssetsPath) == true)
                        {
                            LoadDownloadFile(streamingAssetsPath, true, callback);
                        }
                        else
                        {
                            LoadLocalFile(streamingAssetsPath, callback);
                        }

                        break;
                    }
                case LoadType.RESOURCE_ASSET:
                    {
                        LoadResourceAsset(filepath, callback);
                        break;
                    }
            }
        }

#region Load Process

        private void LoadLocalFile(string localPath, Action<MultilanguageResultCode, MultilanguageXml, string> callback)
        {
            XmlHelper.LoadXmlFromFile<MultilanguageXml>(
                localPath,
                (xmlResultCode, xmlData, xmlResultMessage) =>
                {
                    callback(GetResultCode(xmlResultCode), xmlData, xmlResultMessage);
                });
        }

        private void LoadResourceAsset(string assetName, Action<MultilanguageResultCode, MultilanguageXml, string> callback)
        {
            TextAsset asset = Resources.Load<TextAsset>(assetName);
            if (asset == null)
            {
                callback(MultilanguageResultCode.FILE_NOT_FOUND, null, null);
                return;
            }

            XmlHelper.LoadXmlFromText<MultilanguageXml>(
                asset.text,
                (xmlResultCode, xmlData, xmlResultMessage) =>
                {
                    callback(GetResultCode(xmlResultCode), xmlData, xmlResultMessage);
                });
        }

        private void LoadDownloadFile(string url, bool isStreamingAsset, Action<MultilanguageResultCode, MultilanguageXml, string> callback)
        {
            Action<MultilanguageResultCode, string, string> requestCallback =
                (result, loadText, resultMessage) =>
                {
                    if (result != MultilanguageResultCode.SUCCESS)
                    {
                        callback(result, null, resultMessage);
                        return;
                    }

                    if (string.IsNullOrEmpty(loadText) == true)
                    {
                        callback(MultilanguageResultCode.FILE_PARSING_ERROR, null, null);
                        return;
                    }

                    XmlHelper.LoadXmlFromText<MultilanguageXml>(
                        loadText,
                        (xmlResultCode, xmlData, xmlResultMessage) =>
                        {
                            callback(GetResultCode(xmlResultCode), xmlData, xmlResultMessage);
                        });
                };

            IEnumerator loadEnumerator = DownloadFile(url, requestCallback);

#if UNITY_EDITOR
            if (UnityEditor.EditorApplication.isPlaying == false)
            {
                EditorCoroutine.Start(loadEnumerator);
            }
            else
#endif
            {
                if (monoObject == null)
                {
                    monoObject = GameObjectContainer.GetGameObject(GpmMultilanguage.SERVICE_NAME).GetComponent<MonoBehaviour>();
                }

                monoObject.StartCoroutine(loadEnumerator);
            }
        }

        private IEnumerator DownloadFile(string url, Action<MultilanguageResultCode, string, string> callback)
        {
            using (UnityWebRequest request = UnityWebRequest.Get(url))
            {
                yield return request.SendWebRequest();

                while (request.isDone == false)
                {
                    yield return null;
                }

                if (UnityWebRequestHelper.IsSuccess(request) == false)
                {
                    callback(MultilanguageResultCode.FILE_LOAD_FAILED, string.Empty, string.Format("{0} {1} {2}", request.error, request.responseCode, url));
                    yield break;
                }

                string dataText = string.Empty;
                MultilanguageResultCode resultCode = MultilanguageResultCode.SUCCESS;
                string resultMessage = null;

                switch (request.responseCode)
                {
                    case 200:
                        {
                            dataText = request.downloadHandler.text;
                            break;
                        }
                    case 404:
                        {
                            resultCode = MultilanguageResultCode.FILE_NOT_FOUND;
                            break;
                        }
                    default:
                        {
                            resultCode = MultilanguageResultCode.FILE_LOAD_FAILED;
                            resultMessage = string.Format("Response Code: {0}", request.responseCode);
                            break;
                        }
                }

                callback(resultCode, dataText, resultMessage);
            }
        }
#endregion

        private bool IsWebPath(string path)
        {
            return path.Contains("://") == true || path.Contains(":///") == true;
        }

        protected MultilanguageResultCode GetResultCode(XmlHelper.ResponseCode xmlResponseCode)
        {
            MultilanguageResultCode resultCode;

            switch (xmlResponseCode)
            {
                case XmlHelper.ResponseCode.SUCCESS:
                    {
                        resultCode = MultilanguageResultCode.SUCCESS;
                        break;
                    }
                case XmlHelper.ResponseCode.FILE_NOT_FOUND_ERROR:
                    {
                        resultCode = MultilanguageResultCode.FILE_NOT_FOUND;
                        break;
                    }
                default:
                    {
                        resultCode = MultilanguageResultCode.FILE_PARSING_ERROR;
                        break;
                    }
            }

            return resultCode;
        }
    }
}
