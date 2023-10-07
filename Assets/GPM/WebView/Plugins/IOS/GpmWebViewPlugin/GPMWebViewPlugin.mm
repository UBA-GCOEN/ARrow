#import <GamePackageManagerWebView/GamePackageManagerWebView.h>
#import "GPMWebViewPlugin.h"
#import "GPMCommunicatorPlugin.h"
#import "GPMCommunicatorReceiver.h"
#import "GPMWebViewMessage.h"
#import "GPMWebViewJsonUtil.h"
#import "GPMCommunicatorMessage.h"


#define GPM_WEBVIEW_DOMAIN @"GPM_WEBVIEW"
#define GPM_WEBVIEW_API_SHOW_URL @"gpmwebview://showUrl"
#define GPM_WEBVIEW_API_SHOW_HTML_FILE @"gpmwebview://showHtmlFile"
#define GPM_WEBVIEW_API_SHOW_HTML_STRING @"gpmwebview://showHtmlString"
#define GPM_WEBVIEW_API_SHOW_SAFE_BROWSING @"gpmwebview://showSafeBrowsing"
#define GPM_WEBVIEW_API_CLOSE @"gpmwebview://close"
#define GPM_WEBVIEW_API_IS_ACTIVE @"gpmwebview://isActive"
#define GPM_WEBVIEW_API_EXECUTE_JAVASCRIPT @"gpmwebview://executeJavaScript"
#define GPM_WEBVIEW_API_SET_FILE_DOWNLOAD_PATH @"gpmwebview://setFileDownloadPath"
#define GPM_WEBVIEW_API_CAN_GO_BACK @"gpmwebview://canGoBack"
#define GPM_WEBVIEW_API_CAN_GO_FORWARD @"gpmwebview://canGoForward"
#define GPM_WEBVIEW_API_GO_BACK @"gpmwebview://goBack"
#define GPM_WEBVIEW_API_GO_FORWARD @"gpmwebview://goForward"
#define GPM_WEBVIEW_API_SET_POSITION @"gpmwebview://setPosition"
#define GPM_WEBVIEW_API_SET_SIZE @"gpmwebview://setSize"
#define GPM_WEBVIEW_API_SET_MARGINS @"gpmwebview://setMargins"
#define GPM_WEBVIEW_API_GET_X @"gpmwebview://getX"
#define GPM_WEBVIEW_API_GET_Y @"gpmwebview://getY"
#define GPM_WEBVIEW_API_GET_WIDTH @"gpmwebview://getWidth"
#define GPM_WEBVIEW_API_GET_HEIGHT @"gpmwebview://getHeight"
#define GPM_WEBVIEW_API_SHOW_WEB_BROWSER @"gpmwebview://showWebBrowser"

#define GPM_WEBVIEW_WEBVIEW_CALLBACK @"gpmwebview://webViewCallback";

@implementation GPMWebViewPlugin

- (id)init {
    if((self = [super init]) == nil) {
        return nil;
    }
    
    GPMCommunicatorReceiver* receiver = [[GPMCommunicatorReceiver alloc] init];
    
    receiver.onRequestMessageSync = ^GPMCommunicatorMessage*(GPMCommunicatorMessage *message) {
        return [self onSyncMessage:message];
    };
    
    receiver.onRequestMessageAsync = ^(GPMCommunicatorMessage *message) {
        [self onAsyncMessage:message];
    };
    
    [[GPMCommunicatorPlugin sharedGPMCommunicatorPlugin] addReceiverWithDomain:GPM_WEBVIEW_DOMAIN receiver:receiver];
    return self;
}

- (GPMCommunicatorMessage*)onSyncMessage: (GPMCommunicatorMessage*)message {
    GPMWebViewMessage* webviewMessage = [[GPMWebViewMessage alloc]initWithJsonString:message.data];
    GPMCommunicatorMessage* returnMessage = nil;
    
    if([webviewMessage.scheme isEqualToString:GPM_WEBVIEW_API_CAN_GO_BACK]) {
        returnMessage = [self getBoolMessage:[GPMWebView canGoBack]];
    } else if([webviewMessage.scheme isEqualToString:GPM_WEBVIEW_API_CAN_GO_FORWARD]) {
        returnMessage = [self getBoolMessage:[GPMWebView canGoForward]];
    } else if([webviewMessage.scheme isEqualToString:GPM_WEBVIEW_API_IS_ACTIVE]) {
        returnMessage = [self getBoolMessage:[GPMWebView isActive]];
    } else if([webviewMessage.scheme isEqualToString:GPM_WEBVIEW_API_GET_X]) {
        returnMessage = [self getIntMessage:[GPMWebView getX]];
    } else if([webviewMessage.scheme isEqualToString:GPM_WEBVIEW_API_GET_Y]) {
        returnMessage = [self getIntMessage:[GPMWebView getY]];
    } else if([webviewMessage.scheme isEqualToString:GPM_WEBVIEW_API_GET_WIDTH]) {
        returnMessage = [self getIntMessage:[GPMWebView getWidth]];
    } else if([webviewMessage.scheme isEqualToString:GPM_WEBVIEW_API_GET_HEIGHT]) {
        returnMessage = [self getIntMessage:[GPMWebView getHeight]];
    }
    
    return returnMessage;
}

- (void)onAsyncMessage: (GPMCommunicatorMessage*)message {
    GPMWebViewMessage* webviewMessage = [[GPMWebViewMessage alloc]initWithJsonString:message.data];
    
    if([webviewMessage.scheme isEqualToString:GPM_WEBVIEW_API_SHOW_URL]) {
        [self showUrl:webviewMessage];
    } else if([webviewMessage.scheme isEqualToString:GPM_WEBVIEW_API_SHOW_HTML_FILE]) {
        [self showHtmlFile:webviewMessage];
    } else if([webviewMessage.scheme isEqualToString:GPM_WEBVIEW_API_SHOW_HTML_STRING]) {
        [self showHtmlString:webviewMessage];
    } else if([webviewMessage.scheme isEqualToString:GPM_WEBVIEW_API_SHOW_SAFE_BROWSING]) {
        [self showSafeBrowsing:webviewMessage];
    } else if([webviewMessage.scheme isEqualToString:GPM_WEBVIEW_API_CLOSE]) {
        [self close];
    } else if([webviewMessage.scheme isEqualToString:GPM_WEBVIEW_API_EXECUTE_JAVASCRIPT]) {
        [self executeJavaScript:webviewMessage];
    } else if([webviewMessage.scheme isEqualToString:GPM_WEBVIEW_API_SET_FILE_DOWNLOAD_PATH]) {
        [self setFileDownloadPath:webviewMessage];
    } else if([webviewMessage.scheme isEqualToString:GPM_WEBVIEW_API_GO_BACK]) {
        [self goBack];
    } else if([webviewMessage.scheme isEqualToString:GPM_WEBVIEW_API_GO_FORWARD]) {
        [self goForward];
    } else if([webviewMessage.scheme isEqualToString:GPM_WEBVIEW_API_SET_POSITION]) {
        [self setPosition:webviewMessage];
    } else if([webviewMessage.scheme isEqualToString:GPM_WEBVIEW_API_SET_SIZE]) {
        [self setSize:webviewMessage];
    } else if([webviewMessage.scheme isEqualToString:GPM_WEBVIEW_API_SET_MARGINS]) {
        [self setMargins:webviewMessage];
    } else if([webviewMessage.scheme isEqualToString:GPM_WEBVIEW_API_SHOW_WEB_BROWSER]) {
        [self showWebBrowser:webviewMessage];
    }
}

- (void)showUrl:(GPMWebViewMessage*)webViewMessage {
    NSDictionary* dataDic = [webViewMessage.data JSONDictionary];
    
    NSString* url = dataDic[@"data"];
    NSArray* schemeArray = (NSArray*)dataDic[@"schemeList"];
    NSDictionary* configurationDic = dataDic[@"configuration"];
    GPMWebViewConfiguration* configuration = [self getConfiguration:configurationDic];
    
    [GPMWebView showWithURL:url viewController:UnityGetGLViewController() configuration:configuration callbackCompletion:^(NSInteger callbackType, NSString *data, GPMWebViewError *error) {
        [self sendWebViewMessage:webViewMessage.callback callbackType:callbackType data:data error:error];
    } schemeList:schemeArray];
}

- (void) showHtmlFile: (GPMWebViewMessage*)webViewMessage {
    NSDictionary* dataDic = [webViewMessage.data JSONDictionary];
    
    NSString* filePath = dataDic[@"data"];
    NSArray* schemeArray = (NSArray*)dataDic[@"schemeList"];
    NSDictionary* configurationDic = dataDic[@"configuration"];
    GPMWebViewConfiguration* configuration = [self getConfiguration:configurationDic];
    
    [GPMWebView showWithHTMLFile:filePath viewController:UnityGetGLViewController() configuration:configuration callbackCompletion:^(NSInteger callbackType, NSString *data, GPMWebViewError *error) {
        [self sendWebViewMessage:webViewMessage.callback callbackType:callbackType data:data error:error];
    } schemeList:schemeArray];
}

- (void) showHtmlString: (GPMWebViewMessage*)webViewMessage {
    NSDictionary* dataDic = [webViewMessage.data JSONDictionary];
    
    NSString* htmlString = dataDic[@"data"];
    NSArray* schemeArray = (NSArray*)dataDic[@"schemeList"];
    NSDictionary* configurationDic = dataDic[@"configuration"];
    GPMWebViewConfiguration* configuration = [self getConfiguration:configurationDic];
    
    [GPMWebView showWithHTMLString:htmlString viewController:UnityGetGLViewController() configuration:configuration callbackCompletion:^(NSInteger callbackType, NSString *data, GPMWebViewError *error) {
        [self sendWebViewMessage:webViewMessage.callback callbackType:callbackType data:data error:error];
    } schemeList:schemeArray];
}

- (void) showSafeBrowsing: (GPMWebViewMessage*)webViewMessage {
    NSDictionary *dataDic = [webViewMessage.data JSONDictionary];
    
    NSString *url = dataDic[@"url"];
    NSDictionary *configurationDic = dataDic[@"configuration"];
    GPMSafeBrowsingConfiguration *configuration = [self getSafeBrowsingConfiguration:configurationDic];
    
    [GPMWebView showSafeBrowsing:url viewController:UnityGetGLViewController() configuration:configuration callbackCompletion:^(NSInteger callbackType, NSString *data, GPMWebViewError *error) {
        [self sendWebViewMessage:webViewMessage.callback callbackType:callbackType data:data error:error];
    }];
}

- (void) executeJavaScript: (GPMWebViewMessage*)webViewMessage {
    NSDictionary* dataDic = [webViewMessage.data JSONDictionary];
    NSString* script = dataDic[@"script"];
    
    [GPMWebView executeJavaScriptWithScript:script];
}

- (void) close {
    [GPMWebView close];
}

- (GPMCommunicatorMessage*) isActive {
    BOOL result = [GPMWebView isActive];
    
    GPMCommunicatorMessage* message = [[GPMCommunicatorMessage alloc] init];
    message.domain = GPM_WEBVIEW_DOMAIN;
    message.data = result ? @"true" : @"false";
    
    return message;
}

- (void) setFileDownloadPath: (GPMWebViewMessage*)webViewMessage {
    
}

- (void) goBack {
    [GPMWebView goForward];
}

- (void) goForward {
    [GPMWebView goForward];
}

- (void)setPosition: (GPMWebViewMessage*)webViewMessage {
    NSDictionary* dataDic = [webViewMessage.data JSONDictionary];
    CGFloat x = (CGFloat)[dataDic[@"x"] intValue];
    CGFloat y = (CGFloat)[dataDic[@"y"] intValue];
    
    [GPMWebView setPosition:x y:y];
}

- (void)setSize: (GPMWebViewMessage*)webViewMessage {
    NSDictionary* dataDic = [webViewMessage.data JSONDictionary];
    CGFloat width = (CGFloat)[dataDic[@"width"] intValue];
    CGFloat height = (CGFloat)[dataDic[@"height"] intValue];
    
    [GPMWebView setSize:width height:height];
}

- (void)setMargins: (GPMWebViewMessage*)webViewMessage {
    NSDictionary* dataDic = [webViewMessage.data JSONDictionary];
    CGFloat left = (CGFloat)[dataDic[@"left"] intValue];
    CGFloat top = (CGFloat)[dataDic[@"top"] intValue];
    CGFloat right = (CGFloat)[dataDic[@"right"] intValue];
    CGFloat bottom = (CGFloat)[dataDic[@"bottom"] intValue];
    
    [GPMWebView setMargins:left top:top right:right bottom:bottom];
}

- (void) showWebBrowser: (GPMWebViewMessage*)webViewMessage {
    NSDictionary *dataDic = [webViewMessage.data JSONDictionary];
    NSString *url = dataDic[@"url"];
    
    [GPMWebView openWebBrowserWithURL:url];
}

- (GPMWebViewConfiguration *)getConfiguration: (NSDictionary *)configurationDic {
    if(configurationDic != nil && [configurationDic isEqual:[NSNull null]] == NO) {
        GPMWebViewConfiguration *configuration = [[GPMWebViewConfiguration alloc] init];
        configuration.style = (GPMWebViewStyle)[configurationDic[@"style"] intValue];
        configuration.orientationMask = (GPMWebViewOrientation)[configurationDic[@"orientation"] intValue];
        configuration.isClearCookie = [configurationDic[@"isClearCookie"] boolValue];
        configuration.isClearCache = [configurationDic[@"isClearCache"] boolValue];
        configuration.backgroundColor = [self colorFromHexString:configurationDic[@"backgroundColor"]];
        configuration.isNavigationBarVisible = [configurationDic[@"isNavigationBarVisible"] boolValue];
        configuration.navigationBarColor = [self colorFromHexString:configurationDic[@"navigationBarColor"]];
        configuration.navigationBarTitle = configurationDic[@"title"];
        configuration.isBackButtonVisible = (GPMWebViewContent)[configurationDic[@"isBackButtonVisible"] boolValue];
        configuration.isForwardButtonVisible = [configurationDic[@"isForwardButtonVisible"] boolValue];
        configuration.isCloseButtonVisible = [configurationDic[@"isCloseButtonVisible"] boolValue];
        configuration.supportMultipleWindows = [configurationDic[@"supportMultipleWindows"] boolValue];
        configuration.userAgentString = configurationDic[@"userAgentString"];
        configuration.addJavascript = configurationDic[@"addJavascript"];
        configuration.hasPosition = [configurationDic[@"hasPosition"] boolValue];
        configuration.positionX = [configurationDic[@"positionX"] intValue];
        configuration.positionY = [configurationDic[@"positionY"] intValue];
        configuration.hasSize = [configurationDic[@"hasSize"] boolValue];
        configuration.sizeWidth = [configurationDic[@"sizeWidth"] intValue];
        configuration.sizeHeight = [configurationDic[@"sizeHeight"] intValue];
        configuration.hasMargins = [configurationDic[@"hasMargins"] boolValue];
        configuration.marginsLeft = [configurationDic[@"marginsLeft"] intValue];
        configuration.marginsTop = [configurationDic[@"marginsTop"] intValue];
        configuration.marginsRight = [configurationDic[@"marginsRight"] intValue];
        configuration.marginsBottom = [configurationDic[@"marginsBottom"] intValue];
        configuration.contentMode = [configurationDic[@"contentMode"] intValue];
        configuration.isMaskViewVisible = [configurationDic[@"isMaskViewVisible"] boolValue];
        configuration.isAutoRotation = [configurationDic[@"isAutoRotation"] boolValue];
        configuration.schemeCommandList = (NSArray*)configurationDic[@"schemeCommandList"];
        
        return configuration;
    }
    return nil;
}

- (GPMSafeBrowsingConfiguration *)getSafeBrowsingConfiguration: (NSDictionary *)configurationDic {
    if(configurationDic != nil && [configurationDic isEqual:[NSNull null]] == NO) {
        GPMSafeBrowsingConfiguration *configuration = [[GPMSafeBrowsingConfiguration alloc] init];
        configuration.navigationBarColor = [self colorFromHexString:configurationDic[@"navigationBarColor"]];
        configuration.navigationTextColor = [self colorFromHexString:configurationDic[@"navigationTextColor"]];
        
        return configuration;
    }
    return nil;
}

- (void) sendWebViewMessage:(NSInteger)callback callbackType:(NSInteger)callbackType data:(NSString *)data error:(GPMWebViewError *)error {
    GPMWebViewMessage* requestMessage = [[GPMWebViewMessage alloc] init];
    requestMessage.scheme = GPM_WEBVIEW_WEBVIEW_CALLBACK;
    requestMessage.callback = callback;
    requestMessage.callbackType = callbackType;
    requestMessage.data = data;
    requestMessage.extra = nil;
    if (error != nil) {
        requestMessage.error = [error jsonString];
    }
    
    GPMCommunicatorMessage* message = [[GPMCommunicatorMessage alloc] init];
    message.domain = GPM_WEBVIEW_DOMAIN;
    message.data = [requestMessage toJsonString];
    
    [[GPMCommunicatorPlugin sharedGPMCommunicatorPlugin] sendResponseWithMessage:message];
}

- (GPMCommunicatorMessage*)getBoolMessage:(BOOL)result {
    GPMCommunicatorMessage* message = [[GPMCommunicatorMessage alloc] init];
    message.domain = GPM_WEBVIEW_DOMAIN;
    message.data = result ? @"true" : @"false";
    return message;
}

- (GPMCommunicatorMessage*)getIntMessage:(int)result {
    GPMCommunicatorMessage* message = [[GPMCommunicatorMessage alloc] init];
    message.domain = GPM_WEBVIEW_DOMAIN;
    message.data = [NSString stringWithFormat:@"%d", result];
    return message;
}

- (UIColor *)colorFromHexString:(NSString *)hexString {
    unsigned rgbValue = 0;
    NSScanner *scanner = [NSScanner scannerWithString:hexString];
    [scanner setScanLocation:1]; // bypass '#' character
    [scanner scanHexInt:&rgbValue];
    return [UIColor colorWithRed:((rgbValue & 0xFF0000) >> 16)/255.0 green:((rgbValue & 0xFF00) >> 8)/255.0 blue:(rgbValue & 0xFF)/255.0 alpha:1.0];
}
@end

