using System.Collections;
using System.Collections.Generic;
using Gpm.Common.ThirdParty.SharpCompress.Common;
using Gpm.WebView;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

public class SampleWebView : MonoBehaviour
{
    private const string DEFAULT_URL = "https://www.google.com";

    public InputField urlInput;
    public InputField titleInput;
    public InputField customSchemeInput;

    public InputField backgroundColorInput;
    public InputField navigationColorInput;

    public Toggle clearCookieToggle;
    public Toggle clearCacheToggle;

    public Toggle navigationBarVisibleToggle;
    public Toggle backButtonVisibleToggle;
    public Toggle forwardButtonVisibleToggle;
    public Toggle closeButtonVisibleToggle;

    public Toggle supportMultipleWindowToggle;
    public Toggle backButtonCloseCallbackUsedToggle;
    public Toggle maskViewVisibleToggle;
    public Toggle autoRotationToggle;

    public Dropdown styleDropdown;
    public Dropdown orientationDropdown;
    public Dropdown contentModeDropdown;

    public InputField popupXInput;
    public InputField popupYInput;
    public InputField popupWidthInput;
    public InputField popupHeightInput;
    public InputField popupMarginsLeftInput;
    public InputField popupMarginsTopInput;
    public InputField popupMarginsRightInput;
    public InputField popupMarginsBottomInput;

    public InputField userAgentStringInput;
    public InputField javascriptInput;

    public InputField safeBrowsingNavigationBarColor;
    public InputField safeBrowsingNavigationTextColor;

    public InputField output;

    private void Awake()
    {
        urlInput.text = DEFAULT_URL;

        backgroundColorInput.text = "#FFFFFF";
        navigationColorInput.text = "#4B96E6";
        safeBrowsingNavigationBarColor.text = "#4B96E6";
        safeBrowsingNavigationTextColor.text = "#FFFFFF";

#if UNITY_ANDROID
        maskViewVisibleToggle.isOn = false;
        autoRotationToggle.isOn = false;
#endif
    }

    public void OpenWebView()
    {
        if (string.IsNullOrEmpty(urlInput.text) == false)
        {
            GpmWebView.ShowUrl(urlInput.text, GetConfiguration(), OnWebViewCallback, GetCustomSchemeList());
        }
        else
        {
            Debug.LogError("[SampleWebView] Input url is empty.");
        }
    }

    public void OpenWithHTMLString()
    {
        string htmlString = @"<html><head><title>GPM WebView</title></head>
            <body><h1>GPM WebView Test</h1><h5>Hi.</h5>
            <script type='text/javascript'>
            function goGoogle() { window.open('https://www.google.com'); }
            </script>
            <button class='favorite styled' type='button' onclick='goGoogle()'>Go Google</button>
            </body></html>";
        GpmWebView.ShowHtmlString(htmlString, GetConfiguration(), OnWebViewCallback, GetCustomSchemeList());
    }

    public void OpenSafeBrowsing()
    {
        if (string.IsNullOrEmpty(urlInput.text) == false)
        {
            GpmWebViewSafeBrowsing.ShowSafeBrowsing(urlInput.text,
                new GpmWebViewRequest.ConfigurationSafeBrowsing
                {
                    navigationBarColor = safeBrowsingNavigationBarColor.text,
#if UNITY_IOS
                    navigationTextColor = safeBrowsingNavigationTextColor.text
#endif
                }, OnWebViewCallback);
        }
        else
        {
            Debug.LogError("[SampleWebView] Input url is empty.");
        }
    }

    public void OpenWebBrowser()
    {
        if (string.IsNullOrEmpty(urlInput.text) == false)
        {
            GpmWebView.ShowWebBrowser(urlInput.text);
        }
        else
        {
            Debug.LogError("[SampleWebView] Input url is empty.");
        }
    }

    private GpmWebViewRequest.Configuration GetConfiguration()
    {
        GpmWebViewRequest.CustomSchemePostCommand customSchemePostCommand = new GpmWebViewRequest.CustomSchemePostCommand();
        customSchemePostCommand.Close("CUSTOM_SCHEME_POST_CLOSE");

        return new GpmWebViewRequest.Configuration()
        {
            style = styleDropdown.value,
            orientation = (orientationDropdown.value == 0) ? GpmOrientation.UNSPECIFIED : 1 << (orientationDropdown.value - 1),
            isClearCache = clearCookieToggle.isOn,
            isClearCookie = clearCacheToggle.isOn,
            backgroundColor = backgroundColorInput.text,
            isNavigationBarVisible = navigationBarVisibleToggle.isOn,
            navigationBarColor = navigationColorInput.text,
            title = titleInput.text,
            isBackButtonVisible = backButtonVisibleToggle.isOn,
            isForwardButtonVisible = forwardButtonVisibleToggle.isOn,
            isCloseButtonVisible = closeButtonVisibleToggle.isOn,
            supportMultipleWindows = supportMultipleWindowToggle.isOn,
            userAgentString = userAgentStringInput.text,
            addJavascript = javascriptInput.text,
            customSchemePostCommand = customSchemePostCommand,

            position = GetConfigurationPosition(),
            size = GetConfigurationSize(),
            margins = GetConfigurationMargins(),

            isBackButtonCloseCallbackUsed = backButtonCloseCallbackUsedToggle.isOn,

#if UNITY_IOS
            contentMode = contentModeDropdown.value,
            isMaskViewVisible = maskViewVisibleToggle.isOn,
            isAutoRotation = autoRotationToggle.isOn
#endif
        };
    }

    private GpmWebViewRequest.Position GetConfigurationPosition()
    {
        bool hasValue = false;
        if (string.IsNullOrEmpty(popupXInput.text) == false && string.IsNullOrEmpty(popupYInput.text) == false)
        {
            hasValue = true;
        }

        int x = 0;
        int.TryParse(popupXInput.text, out x);

        int y = 0;
        int.TryParse(popupYInput.text, out y);

        return new GpmWebViewRequest.Position
        {
            hasValue = hasValue,
            x = x,
            y = y
        };
    }

    private GpmWebViewRequest.Size GetConfigurationSize()
    {
        bool hasValue = false;
        if (string.IsNullOrEmpty(popupWidthInput.text) == false && string.IsNullOrEmpty(popupHeightInput.text) == false)
        {
            hasValue = true;
        }

        int width = 0;
        int.TryParse(popupWidthInput.text, out width);

        int height = 0;
        int.TryParse(popupHeightInput.text, out height);

        return new GpmWebViewRequest.Size
        {
            hasValue = hasValue,
            width = width,
            height = height
        };
    }

    private GpmWebViewRequest.Margins GetConfigurationMargins()
    {
        bool hasValue = false;
        if (string.IsNullOrEmpty(popupMarginsLeftInput.text) == false &&
            string.IsNullOrEmpty(popupMarginsTopInput.text) == false &&
            string.IsNullOrEmpty(popupMarginsRightInput.text) == false &&
            string.IsNullOrEmpty(popupMarginsBottomInput.text) == false)
        {
            hasValue = true;
        }

        int marginLeft = 0;
        int.TryParse(popupMarginsLeftInput.text, out marginLeft);

        int marginTop = 0;
        int.TryParse(popupMarginsTopInput.text, out marginTop);

        int marginRight = 0;
        int.TryParse(popupMarginsRightInput.text, out marginRight);

        int marginBottom = 0;
        int.TryParse(popupMarginsBottomInput.text, out marginBottom);

        return new GpmWebViewRequest.Margins
        {
            hasValue = hasValue,
            left = marginLeft,
            top = marginTop,
            right = marginRight,
            bottom = marginBottom
        };
    }

    private List<string> GetCustomSchemeList()
    {
        List<string> customSchemeList = null;
        if (string.IsNullOrEmpty(customSchemeInput.text) == false)
        {
            string[] schemes = customSchemeInput.text.Split(',');
            customSchemeList = new List<string>(schemes);
        }
        return customSchemeList;
    }

    private void OnWebViewCallback(GpmWebViewCallback.CallbackType callbackType, string data, GpmWebViewError error)
    {
        Debug.Log("OnWebViewCallback: " + callbackType);
        switch (callbackType)
        {
            case GpmWebViewCallback.CallbackType.Open:
                if (error != null)
                {
                    Debug.LogFormat("Fail to open WebView. Error:{0}", error);
                }
                break;
            case GpmWebViewCallback.CallbackType.Close:
                if (error != null)
                {
                    Debug.LogFormat("Fail to close WebView. Error:{0}", error);
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
                }
                break;
            case GpmWebViewCallback.CallbackType.MultiWindowOpen:
                Debug.Log("MultiWindowOpen");
                break;
            case GpmWebViewCallback.CallbackType.MultiWindowClose:
                Debug.Log("MultiWindowClose");
                break;
            case GpmWebViewCallback.CallbackType.Scheme:
                Debug.LogFormat("Scheme:{0}", data);
                break;
            case GpmWebViewCallback.CallbackType.GoBack:
                Debug.Log("GoBack");
                break;
            case GpmWebViewCallback.CallbackType.GoForward:
                Debug.Log("GoForward");
                break;
            case GpmWebViewCallback.CallbackType.ExecuteJavascript:
                Debug.LogFormat("ExecuteJavascript data : {0}, error : {1}", data, error);
                break;
#if UNITY_ANDROID
            case GpmWebViewCallback.CallbackType.BackButtonClose:
                Debug.Log("BackButtonClose");
                break;
#endif
        }
    }

    public void CanGoBack()
    {
        bool value = GpmWebView.CanGoBack();
        output.text = value.ToString();
    }

    public void GoBack()
    {
        GpmWebView.GoBack();
    }

    public void CanGoForward()
    {
        bool value = GpmWebView.CanGoForward();
        output.text = value.ToString();
    }

    public void GoForward()
    {
        GpmWebView.GoForward();
    }

    public void GetX()
    {
        int value = GpmWebView.GetX();
        output.text = value.ToString();
    }

    public void GetY()
    {
        int value = GpmWebView.GetY();
        output.text = value.ToString();
    }

    public void GetWidth()
    {
        int value = GpmWebView.GetWidth();
        output.text = value.ToString();
    }

    public void GetHeight()
    {
        int value = GpmWebView.GetHeight();
        output.text = value.ToString();
    }

    public void ExecuteJavascript()
    {
        GpmWebView.ExecuteJavaScript(javascriptInput.text);
    }

    public void IsActive()
    {
        bool value = GpmWebView.IsActive();
        output.text = value.ToString();
    }

    public void Close()
    {
        GpmWebView.Close();
    }

    public void SetPosition()
    {
        int x = 0;
        int y = 0;
        int.TryParse(popupXInput.text, out x);
        int.TryParse(popupYInput.text, out y);
        GpmWebView.SetPosition(x, y);
    }

    public void SetSize()
    {
        int width = 0;
        int height = 0;
        int.TryParse(popupWidthInput.text, out width);
        int.TryParse(popupHeightInput.text, out height);
        GpmWebView.SetSize(width, height);
    }

    public void SetMargins()
    {
        int left = 0;
        int top = 0;
        int right = 0;
        int bottom = 0;
        int.TryParse(popupMarginsLeftInput.text, out left);
        int.TryParse(popupMarginsTopInput.text, out top);
        int.TryParse(popupMarginsRightInput.text, out right);
        int.TryParse(popupMarginsBottomInput.text, out bottom);
        GpmWebView.SetMargins(left, top, right, bottom);
    }

    public void ResetPosition()
    {
        popupXInput.text = string.Empty;
        popupYInput.text = string.Empty;
    }

    public void ResetSize()
    {
        popupWidthInput.text = string.Empty;
        popupHeightInput.text = string.Empty;
    }

    public void ResetMargins()
    {
        popupMarginsLeftInput.text = string.Empty;
        popupMarginsTopInput.text = string.Empty;
        popupMarginsRightInput.text = string.Empty;
        popupMarginsBottomInput.text = string.Empty;
    }
}
