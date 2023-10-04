namespace Gpm.WebView.Internal
{
    using System;
    using System.Collections.Generic;
    using Gpm.Common.ThirdParty.LitJson;
    using Gpm.Communicator;
    using UnityEngine;

#pragma warning disable CS0618
    public class NativeWebView : IWebView
    {
        protected static class ApiScheme
        {
            public const string SHOW_URL = "gpmwebview://showUrl";
            public const string SHOW_HTML_FILE = "gpmwebview://showHtmlFile";
            public const string SHOW_HTML_STRING = "gpmwebview://showHtmlString";
            public const string SHOW_SAFE_BROWSING = "gpmwebview://showSafeBrowsing";
            public const string CLOSE = "gpmwebview://close";
            public const string IS_ACTIVE = "gpmwebview://isActive";
            public const string EXECUTE_JAVASCRIPT = "gpmwebview://executeJavaScript";
            public const string SET_FILE_DOWNLOAD_PATH = "gpmwebview://setFileDownloadPath";
            public const string CAN_GO_BACK = "gpmwebview://canGoBack";
            public const string CAN_GO_FORWARD = "gpmwebview://canGoForward";
            public const string GO_BACK = "gpmwebview://goBack";
            public const string GO_FORWARD = "gpmwebview://goForward";
            public const string SET_POSITION = "gpmwebview://setPosition";
            public const string SET_SIZE = "gpmwebview://setSize";
            public const string SET_MARGINS = "gpmwebview://setMargins";
            public const string GET_X = "gpmwebview://getX";
            public const string GET_Y = "gpmwebview://getY";
            public const string GET_WIDTH = "gpmwebview://getWidth";
            public const string GET_HEIGHT = "gpmwebview://getHeight";
            public const string SHOW_WEB_BROWSER = "gpmwebview://showWebBrowser";
        }

        protected static class CallbackScheme
        {
            public const string SCHEME_EVENT_CALLBACK = "gpmwebview://schemeEvent";
            public const string CLOSE_CALLBACK = "gpmwebview://closeCallback";
            public const string CALLBACK = "gpmwebview://callback";
            public const string PAGE_LOAD_CALLBACK = "gpmwebview://pageLoadCallback";

            public const string WEBVIEW_CALLBACK = "gpmwebview://webViewCallback";
        }

        private const string DOMAIN = "GPM_WEBVIEW";
        private const string DEFAULT_NAVIGATION_BAR_COLOR = "#4B96E6";
        private const string DEFAULT_NAVIGATION_TEXT_COLOR = "#FFFFFF";

        protected string CLASS_NAME = string.Empty;

        private const int AUTO_ROTATION_MIN_COUNT = 2;
        private bool isAutorotateToPortrait = false;
        private bool isAutorotateToPortraitUpsideDown = false;
        private bool isAutorotateToLandscapeLeft = false;
        private bool isAutorotateToLandscapeRight = false;
        private ScreenOrientation defaultOrientation = ScreenOrientation.Unknown;

        public bool CanGoBack
        {
            get
            {
                NativeMessage message = new NativeMessage()
                {
                    scheme = ApiScheme.CAN_GO_BACK
                };

                var resultMessage = CallSync(JsonMapper.ToJson(message), string.Empty);

                return Convert.ToBoolean(resultMessage.data);
            }
        }

        public bool CanGoForward
        {
            get
            {
                NativeMessage message = new NativeMessage()
                {
                    scheme = ApiScheme.CAN_GO_FORWARD
                };

                var resultMessage = CallSync(JsonMapper.ToJson(message), string.Empty);

                return Convert.ToBoolean(resultMessage.data);
            }
        }

        public NativeWebView()
        {
            Initialize();
        }

        virtual protected void Initialize()
        {
            GpmCommunicatorVO.Configuration configuration = new GpmCommunicatorVO.Configuration()
            {
                className = CLASS_NAME
            };

            GpmCommunicator.InitializeClass(configuration);
            GpmCommunicator.AddReceiver(DOMAIN, OnAsyncEvent);
        }

        public void ShowUrl(
            string url,
            GpmWebViewRequest.Configuration configuration,
            GpmWebViewCallback.GpmWebViewDelegate callback,
            List<string> schemeList)
        {
            NativeMessage nativeMessage = new NativeMessage
            {
                scheme = ApiScheme.SHOW_URL,
                callback = NativeCallbackHandler.RegisterCallback(callback)
            };

            NativeRequest.ShowWebView showWebView = MakeShowWebView(url, configuration, schemeList);

            nativeMessage.data = JsonMapper.ToJson(showWebView);
            CallAsync(JsonMapper.ToJson(nativeMessage), null);
        }

        public void ShowHtmlFile(
            string filePath,
            GpmWebViewRequest.Configuration configuration,
            GpmWebViewCallback.GpmWebViewDelegate callback,
            List<string> schemeList)
        {
            NativeMessage nativeMessage = new NativeMessage
            {
                scheme = ApiScheme.SHOW_HTML_FILE,
                callback = NativeCallbackHandler.RegisterCallback(callback)
            };

            NativeRequest.ShowWebView showWebView = MakeShowWebView(filePath, configuration, schemeList);

            nativeMessage.data = JsonMapper.ToJson(showWebView);

            CallAsync(JsonMapper.ToJson(nativeMessage), null);
        }

        public void ShowHtmlString(
            string htmlString,
            GpmWebViewRequest.Configuration configuration,
            GpmWebViewCallback.GpmWebViewDelegate callback,
            List<string> schemeList)
        {
            NativeMessage nativeMessage = new NativeMessage
            {
                scheme = ApiScheme.SHOW_HTML_STRING,
                callback = NativeCallbackHandler.RegisterCallback(callback)
            };

            NativeRequest.ShowWebView showWebView = MakeShowWebView(htmlString, configuration, schemeList);

            nativeMessage.data = JsonMapper.ToJson(showWebView);

            CallAsync(JsonMapper.ToJson(nativeMessage), null);
        }

        public void ShowSafeBrowsing(
            string url,
            GpmWebViewRequest.ConfigurationSafeBrowsing configuration = null,
            GpmWebViewCallback.GpmWebViewDelegate callback = null)
        {
            NativeMessage nativeMessage = new NativeMessage
            {
                scheme = ApiScheme.SHOW_SAFE_BROWSING,
                callback = NativeCallbackHandler.RegisterCallback(callback)
            };

            NativeRequest.ShowSafeBrowsing showSafeBrowsing = new NativeRequest.ShowSafeBrowsing
            {
                url = url,
                configuration = new NativeRequest.ConfigurationSafeBrowsing()
                {
                    navigationBarColor = (configuration == null) ? DEFAULT_NAVIGATION_BAR_COLOR : configuration.navigationBarColor,
                    navigationTextColor = (configuration == null) ? DEFAULT_NAVIGATION_TEXT_COLOR : configuration.navigationTextColor
                }
            };

            nativeMessage.data = JsonMapper.ToJson(showSafeBrowsing);

            CallAsync(JsonMapper.ToJson(nativeMessage), null);
        }

        public void Close()
        {
            NativeMessage nativeMessage = new NativeMessage
            {
                scheme = ApiScheme.CLOSE
            };

            string jsonData = JsonMapper.ToJson(nativeMessage);

            CallAsync(jsonData, null);
        }

        public bool IsActive()
        {
            NativeMessage message = new NativeMessage()
            {
                scheme = ApiScheme.IS_ACTIVE
            };

            var resultMessage = CallSync(JsonMapper.ToJson(message), string.Empty);

            return Convert.ToBoolean(resultMessage.data);
        }

        public void ExecuteJavaScript(string script)
        {
            NativeMessage nativeMessage = new NativeMessage
            {
                scheme = ApiScheme.EXECUTE_JAVASCRIPT
            };

            nativeMessage.data = JsonMapper.ToJson(new NativeRequest.ExecuteJavaScript
            {
                script = script
            });

            string jsonData = JsonMapper.ToJson(nativeMessage);

            CallAsync(jsonData, null);
        }

        public void SetFileDownloadPath(string path)
        {
            NativeMessage nativeMessage = new NativeMessage
            {
                scheme = ApiScheme.SET_FILE_DOWNLOAD_PATH
            };

            string jsonData = JsonMapper.ToJson(nativeMessage);

            CallAsync(jsonData, null);
        }

        private NativeRequest.ShowWebView MakeShowWebView(
            string data,
            GpmWebViewRequest.Configuration configuration,
            List<string> schemeList)
        {
            List<string> schemeCommandList = new List<string>();
            if (configuration.customSchemePostCommand != null)
            {
                schemeCommandList = configuration.customSchemePostCommand.commandList;
            }

            NativeRequest.ShowWebView showWebView = new NativeRequest.ShowWebView
            {
                data = data,
                schemeList = schemeList,
                configuration = new NativeRequest.Configuration()
                {
                    style = configuration.style,
                    orientation = configuration.orientation,
                    isClearCookie = configuration.isClearCookie,
                    isClearCache = configuration.isClearCache,
                    backgroundColor = configuration.backgroundColor,
                    isNavigationBarVisible = configuration.isNavigationBarVisible,
                    navigationBarColor = configuration.navigationBarColor,
                    title = configuration.title,
                    isBackButtonVisible = configuration.isBackButtonVisible,
                    isForwardButtonVisible = configuration.isForwardButtonVisible,
                    isCloseButtonVisible = configuration.isCloseButtonVisible,
                    supportMultipleWindows = configuration.supportMultipleWindows,
                    userAgentString = configuration.userAgentString,
                    addJavascript = configuration.addJavascript,

                    hasPosition = configuration.position.hasValue,
                    positionX = configuration.position.x,
                    positionY = configuration.position.y,
                    hasSize = configuration.size.hasValue,
                    sizeWidth = configuration.size.width,
                    sizeHeight = configuration.size.height,
                    hasMargins = configuration.margins.hasValue,
                    marginsLeft = configuration.margins.left,
                    marginsTop = configuration.margins.top,
                    marginsRight = configuration.margins.right,
                    marginsBottom = configuration.margins.bottom,

                    isBackButtonCloseCallbackUsed = configuration.isBackButtonCloseCallbackUsed,

                    contentMode = configuration.contentMode,
                    isMaskViewVisible = configuration.isMaskViewVisible,
                    isAutoRotation = configuration.isAutoRotation,

                    schemeCommandList = schemeCommandList
                }
            };

            CheckAutoRotation();
        #if UNITY_ANDROID
            UpdateOrientation(configuration.orientation);
        #endif

            return showWebView;
        }

        private void CallAsync(string data, string extra)
        {
            GpmCommunicatorVO.Message message = new GpmCommunicatorVO.Message()
            {
                domain = DOMAIN,
                data = data,
                extra = extra
            };

            GpmCommunicator.CallAsync(message);
        }

        private GpmCommunicatorVO.Message CallSync(string data, string extra)
        {
            GpmCommunicatorVO.Message message = new GpmCommunicatorVO.Message()
            {
                domain = DOMAIN,
                data = data,
                extra = extra
            };

            return GpmCommunicator.CallSync(message);
        }

        private void OnAsyncEvent(GpmCommunicatorVO.Message message)
        {
            Debug.Log("OnAsyncEvent : " + message.data);
            NativeMessage nativeMessage = JsonMapper.ToObject<NativeMessage>(message.data);

            if (nativeMessage == null)
            {
                return;
            }

            if (nativeMessage.scheme == CallbackScheme.WEBVIEW_CALLBACK)
            {
                OnWebViewCallback(nativeMessage);
            }
        }

        private void OnWebViewCallback(NativeMessage nativeMessage)
        {
            var callback = NativeCallbackHandler.GetCallback<GpmWebViewCallback.GpmWebViewDelegate>(nativeMessage.callback);

            if (callback != null)
            {
                GpmWebViewError error = null;

                if (string.IsNullOrEmpty(nativeMessage.error) == false)
                {
                    error = JsonMapper.ToObject<GpmWebViewError>(nativeMessage.error);
                }

                GpmWebViewCallback.CallbackType callbackType = (GpmWebViewCallback.CallbackType)nativeMessage.callbackType;
                if (callbackType == GpmWebViewCallback.CallbackType.Close)
                {
                    NativeCallbackHandler.UnregisterCallback(nativeMessage.callback);
                    RestoreOrientation();
                }
                callback(callbackType, nativeMessage.data, error);
            }
        }

        public void GoBack()
        {
            NativeMessage nativeMessage = new NativeMessage
            {
                scheme = ApiScheme.GO_BACK
            };

            string jsonData = JsonMapper.ToJson(nativeMessage);

            CallAsync(jsonData, null);
        }

        public void GoForward()
        {
            NativeMessage nativeMessage = new NativeMessage
            {
                scheme = ApiScheme.GO_FORWARD
            };

            string jsonData = JsonMapper.ToJson(nativeMessage);

            CallAsync(jsonData, null);
        }

        public void SetPosition(int x, int y)
        {
            NativeMessage nativeMessage = new NativeMessage
            {
                scheme = ApiScheme.SET_POSITION
            };

            nativeMessage.data = JsonMapper.ToJson(new NativeRequest.Position
            {
                x = x,
                y = y
            });

            string jsonData = JsonMapper.ToJson(nativeMessage);

            CallAsync(jsonData, null);
        }

        public void SetSize(int width, int height)
        {
            NativeMessage nativeMessage = new NativeMessage
            {
                scheme = ApiScheme.SET_SIZE
            };

            nativeMessage.data = JsonMapper.ToJson(new NativeRequest.Size
            {
                width = width,
                height = height
            });

            string jsonData = JsonMapper.ToJson(nativeMessage);

            CallAsync(jsonData, null);
        }

        public void SetMargins(int left, int top, int right, int bottom)
        {
            NativeMessage nativeMessage = new NativeMessage
            {
                scheme = ApiScheme.SET_MARGINS
            };

            nativeMessage.data = JsonMapper.ToJson(new NativeRequest.Margins
            {
                left = left,
                top = top,
                right = right,
                bottom = bottom
            });

            string jsonData = JsonMapper.ToJson(nativeMessage);

            CallAsync(jsonData, null);
        }

        public int GetX()
        {
            NativeMessage message = new NativeMessage()
            {
                scheme = ApiScheme.GET_X
            };

            var resultMessage = CallSync(JsonMapper.ToJson(message), string.Empty);

            return Convert.ToInt32(resultMessage.data);
        }

        public int GetY()
        {
            NativeMessage message = new NativeMessage()
            {
                scheme = ApiScheme.GET_Y
            };

            var resultMessage = CallSync(JsonMapper.ToJson(message), string.Empty);

            return Convert.ToInt32(resultMessage.data);
        }

        public int GetWidth()
        {
            NativeMessage message = new NativeMessage()
            {
                scheme = ApiScheme.GET_WIDTH
            };

            var resultMessage = CallSync(JsonMapper.ToJson(message), string.Empty);

            return Convert.ToInt32(resultMessage.data);
        }

        public int GetHeight()
        {
            NativeMessage message = new NativeMessage()
            {
                scheme = ApiScheme.GET_HEIGHT
            };

            var resultMessage = CallSync(JsonMapper.ToJson(message), string.Empty);

            return Convert.ToInt32(resultMessage.data);
        }

        private void CheckAutoRotation()
        {
            isAutorotateToPortrait = Screen.autorotateToPortrait;
            isAutorotateToPortraitUpsideDown = Screen.autorotateToPortraitUpsideDown;
            isAutorotateToLandscapeLeft = Screen.autorotateToLandscapeLeft;
            isAutorotateToLandscapeRight =Screen.autorotateToLandscapeRight;
            defaultOrientation = Screen.orientation;
        }

        private void RestoreOrientation()
        {
            Screen.autorotateToPortrait = isAutorotateToPortrait;
            Screen.autorotateToPortraitUpsideDown = isAutorotateToPortraitUpsideDown;
            Screen.autorotateToLandscapeLeft = isAutorotateToLandscapeLeft;
            Screen.autorotateToLandscapeRight = isAutorotateToLandscapeRight;

            int orientationCount = 0;
            if (Screen.autorotateToPortrait == true)
            {
                orientationCount++;
            }
            if (Screen.autorotateToPortraitUpsideDown == true)
            {
                orientationCount++;
            }
            if (Screen.autorotateToLandscapeLeft == true)
            {
                orientationCount++;
            }
            if (Screen.autorotateToLandscapeRight == true)
            {
                orientationCount++;
            }
            if (orientationCount >= AUTO_ROTATION_MIN_COUNT)
            {
                Screen.orientation = ScreenOrientation.AutoRotation;
            }
            else
            {
                Screen.orientation = defaultOrientation;
            }
        }

        private void UpdateOrientation(int orientation)
        {
            if (orientation == GpmOrientation.PORTRAIT)
            {
                Screen.orientation = ScreenOrientation.Portrait;
            }
            else if (orientation == GpmOrientation.PORTRAIT_REVERSE)
            {
                Screen.orientation = ScreenOrientation.PortraitUpsideDown;
            }
            else if (orientation == GpmOrientation.LANDSCAPE_LEFT)
            {
                Screen.orientation = ScreenOrientation.LandscapeLeft;
            }
            else if (orientation == GpmOrientation.LANDSCAPE_REVERSE)
            {
                Screen.orientation = ScreenOrientation.LandscapeRight;
            }
            else if (orientation != GpmOrientation.UNSPECIFIED)
            {
                Screen.autorotateToPortrait = (orientation & GpmOrientation.PORTRAIT) == GpmOrientation.PORTRAIT;
                Screen.autorotateToPortraitUpsideDown = (orientation & GpmOrientation.PORTRAIT_REVERSE) == GpmOrientation.PORTRAIT_REVERSE;
                Screen.autorotateToLandscapeLeft = (orientation & GpmOrientation.LANDSCAPE_LEFT) == GpmOrientation.LANDSCAPE_LEFT;
                Screen.autorotateToLandscapeRight = (orientation & GpmOrientation.LANDSCAPE_REVERSE) == GpmOrientation.LANDSCAPE_REVERSE;
                Screen.orientation = ScreenOrientation.AutoRotation;
            }
        }

        public void ShowWebBrowser(string url)
        {
            NativeMessage nativeMessage = new NativeMessage
            {
                scheme = ApiScheme.SHOW_WEB_BROWSER
            };

            NativeRequest.ShowWebBrowser showWebBrowser = new NativeRequest.ShowWebBrowser
            {
                url = url
            };

            nativeMessage.data = JsonMapper.ToJson(showWebBrowser);

            CallAsync(JsonMapper.ToJson(nativeMessage), null);
        }
    }
}
