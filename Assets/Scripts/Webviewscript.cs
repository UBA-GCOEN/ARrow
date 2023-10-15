using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gpm.WebView;
using TMPro;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using UnityEngine.SceneManagement;

public class Webviewscript : MonoBehaviour
{

    // public TextMeshProUGUI serverMsg;

    public void ShowUrlPopupPositionSize()
    {
        GpmWebViewSafeBrowsing.ShowSafeBrowsing("https://arrowserver.vercel.app/auth/google",
                new GpmWebViewRequest.ConfigurationSafeBrowsing()
                {
                    navigationBarColor = "#000000",
                    navigationTextColor = "#FFFFFF"
                },
                OnCallback);
    }

    public void ShowUrlFullScreen()
    {
        GpmWebView.ShowUrl(
            "https://my.spline.design/arrowmascot-d7549a03c91f5f13297d0d6b531d40ca/",
            new GpmWebViewRequest.Configuration()
            {
                style = GpmWebViewStyle.POPUP,
                orientation = GpmOrientation.UNSPECIFIED,
                isClearCookie = true,
                isClearCache = true,
                isNavigationBarVisible = false,
                isCloseButtonVisible = true,
                position = new GpmWebViewRequest.Position
                {
                    hasValue = true,
                    x = (int)(Screen.width * 0.1f),
                    y = (int)(Screen.height * 0.3f)
                },
                size = new GpmWebViewRequest.Size
                {
                    hasValue = true,
                    width = (int)(Screen.width * 2.5f),
                    height = (int)(Screen.height * 2.5f)
                },
                supportMultipleWindows = true,
#if UNITY_IOS
            contentMode = GpmWebViewContentMode.MOBILE,
            isMaskViewVisible = true,
#endif
            }, null, null);
    }

    private void OnCallback(
    GpmWebViewCallback.CallbackType callbackType,
    string data,
    GpmWebViewError error)
    {
        Debug.Log("OnCallback: " + callbackType);
        // serverMsg.text += "OnCallback: " + callbackType + "\n";
        switch (callbackType)
        {
            case GpmWebViewCallback.CallbackType.Open:
                if (error != null)
                {
                    Debug.LogFormat("Fail to open WebView. Error:{0}", error);
                    // Handle the error here
                }
                break;
            case GpmWebViewCallback.CallbackType.Close:
                if (error != null)
                {
                    Debug.LogFormat("Fail to close WebView. Error:{0}", error);
                    // Handle the error here
                }
                else
                {
                    // Close the browser here
                    GpmWebView.Close();

                    // Store the data in PlayerPrefs here
                    StartCoroutine(FetchJsonData(data));
                    // PlayerPrefs.SetString("Token", response.token);
                    // PlayerPrefs.SetString("UserData", JsonUtility.ToJson(response.result));
                }
                break;
            case GpmWebViewCallback.CallbackType.PageStarted:
                if (string.IsNullOrEmpty(data) == false)
                {
                    Debug.LogFormat("PageStarted Url : {0}", data);
                }
                break;
            case GpmWebViewCallback.CallbackType.PageLoad:
                if (string.IsNullOrEmpty(data) == false)
                {
                    Debug.LogFormat("Loaded Page:{0}", data);
                    // serverMsg.text += "Loaded Page:{0}" + data + "\n";
                    if (data == "https://arrowserver.vercel.app/auth/protected")
                    {
                        StartCoroutine(FetchJsonData(data));
                        GpmWebView.Close();
                    }
                }
                break;
            case GpmWebViewCallback.CallbackType.MultiWindowOpen:
                Debug.Log("MultiWindowOpen");
                break;
            case GpmWebViewCallback.CallbackType.MultiWindowClose:
                Debug.Log("MultiWindowClose");
                break;
            case GpmWebViewCallback.CallbackType.Scheme:
                if (error == null)
                {
                    if (data.Equals("https://arrowserver.vercel.app/auth/protected") == true || data.Contains("https://arrowserver.vercel.app/auth/protected") == true)
                    {
                        // serverMsg.text += "scheme:{0}" + data + "\n";
                        Debug.Log(string.Format("scheme:{0}", data));

                        StartCoroutine(FetchJsonData(data));
                        GpmWebView.Close();
                    }
                }
                else
                {
                    Debug.Log(string.Format("Fail to custom scheme. Error:{0}", error));
                    // Handle the error here
                }
                break;
#if UNITY_ANDROID
        case GpmWebViewCallback.CallbackType.BackButtonClose:
            Debug.Log("BackButtonClose");
            break;
#endif
        }
    }

    IEnumerator FetchJsonData(string url)
    {
        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        if (request.isNetworkError || request.isHttpError)
        {
            Debug.Log("Error while fetching data");
        }
        else
        {
            SigninResponse response = JsonUtility.FromJson<SigninResponse>(request.downloadHandler.text);

            PlayerPrefs.SetString("Token", response.token);

            Debug.Log(response.result);
            // serverMsg.text += "data" + response.result + "\n";
            PlayerPrefs.SetString("UserData", JsonUtility.ToJson(response.result));

            UserData user = JsonUtility.FromJson<UserData>(PlayerPrefs.GetString("UserData"));

            Debug.Log(user.isOnboarded);
            if (user.isOnboarded == true)
            {
                SceneManager.LoadScene("Home");
            }
            else
            {
                SceneManager.LoadScene("[Ob]Start");
            }
        }
    }
}