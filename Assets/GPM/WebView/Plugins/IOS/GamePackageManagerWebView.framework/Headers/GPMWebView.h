//
//  GPMWebView.h
//  GPMWebView
//
//  Created by NHN on 2020/11/23.
//  Copyright © 2020 NHN. All rights reserved.
//

#ifndef GPMWebView_h
#define GPMWebView_h

#import <UIKit/UIKit.h>
#import <WebKit/WebKit.h>
#import "GPMWebViewConfiguration.h"
#import "GPMSafeBrowsingConfiguration.h"

@class GPMWebViewError;

typedef void(^GPMWebViewCallbackCompletion)(NSInteger callbackType, NSString *data, GPMWebViewError *error);

typedef void(^GPMSafeBrowsingCloseCompletion)(void);

/** The GPMWebView class represents the entry point for **launching WebView**.
*/
@interface GPMWebView : NSObject
/**---------------------------------------------------------------------------------------
 * @name Properties
 *  ---------------------------------------------------------------------------------------
 */

/**
 
 This property is a global configuration for launching webview.<br/>
 When you handle the webview without any configuration, GPMWebView set its configuration with this value.
 */
@property (nonatomic, strong) GPMWebViewConfiguration *defaultWebConfiguration;


/**---------------------------------------------------------------------------------------
 * @name Initialization
 *  ---------------------------------------------------------------------------------------
 */

/**
 Creates and returns an `GPMWebView` object.
 */
+ (instancetype)sharedGPMWebView;

/**---------------------------------------------------------------------------------------
 * @name Launching WebView
 *  ---------------------------------------------------------------------------------------
 */

/**
 Show WebView that is not for local url.
 
@param urlString The string value for target url
@param viewController The presenting view controller
@warning If viewController is nil, GPMWebView set it to top most view controller automatically.
@param configuration This configuration is applied to the behavior of webview.
@warning If configuration is nil, GPMWebView set it to default value. It is described in `GPMWebViewConfiguration`.
@param callbackCompletion This completion would be called when events is invoked in the webview
@param schemeList This schemeList would be filtered every web view request and call schemeEvent
 
 */
+ (void)showWithURL:(NSString *)urlString
         viewController:(UIViewController *)viewController
          configuration:(GPMWebViewConfiguration *)configuration
     callbackCompletion:(GPMWebViewCallbackCompletion) callbackCompletion
             schemeList:(NSArray<NSString *> *)schemeList;


/**
 Show WebView for local html (or other web resources)
 
@param filePath The string value for target local path.
@param bundle where the html file is located.
@warning If bundle is nil, GPMWebView set it to main bundle automatically.
@param viewController The presenting view controller
@warning If viewController is nil, GPMWebView set it to top most view controller automatically.
@param configuration This configuration is applied to the behavior of webview.
@warning If configuration is nil, GPMWebView set it to default value. It is described in `GPMWebViewConfiguration`.
@param callbackCompletion This completion would be called when events is invoked in the webview
@param schemeList This schemeList would be filtered every web view request and call schemeEvent
 */
+ (void)showWithHTMLFile:(NSString *)filePath
                  bundle:(NSBundle *)bundle
          viewController:(UIViewController *)viewController
           configuration:(GPMWebViewConfiguration *)configuration
      callbackCompletion:(GPMWebViewCallbackCompletion) callbackCompletion
              schemeList:(NSArray<NSString *> *)schemeList;

/**
Show WebView for local html (or other web resources)

@param filePath The string value for target local path.
@param viewController The presenting view controller
@warning If viewController is nil, GPMWebView set it to top most view controller automatically.
@param configuration This configuration is applied to the behavior of webview.
@warning If configuration is nil, GPMWebView set it to default value. It is described in `GPMWebViewConfiguration`.
@param callbackCompletion This completion would be called when events is invoked in the webview
@param schemeList This schemeList would be filtered every web view request and call schemeEvent
*/
+ (void)showWithHTMLFile:(NSString *)filePath
          viewController:(UIViewController *)viewController
           configuration:(GPMWebViewConfiguration *)configuration
      callbackCompletion:(GPMWebViewCallbackCompletion) callbackCompletion
              schemeList:(NSArray<NSString *> *)schemeList;

/**
Show WebView for local html (or other web resources)

@param htmlString The string value for HTML code.
@param viewController The presenting view controller
@warning If viewController is nil, GPMWebView set it to top most view controller automatically.
@param configuration This configuration is applied to the behavior of webview.
@warning If configuration is nil, GPMWebView set it to default value. It is described in `GPMWebViewConfiguration`.
@param callbackCompletion This completion would be called when events is invoked in the webview
@param schemeList This schemeList would be filtered every web view request and call schemeEvent
*/
+ (void)showWithHTMLString:(NSString *)htmlString
         viewController:(UIViewController *)viewController
          configuration:(GPMWebViewConfiguration *)configuration
     callbackCompletion:(GPMWebViewCallbackCompletion) callbackCompletion
             schemeList:(NSArray<NSString *> *)schemeList;

/**
 Show WebView for Safari browser
 
 @param urlString The string value for target url
 @param viewController The presenting view controller
 @warning If viewController is nil, GPMWebView set it to top most view controller automatically.
 @param configuration This configuration is applied to the behavior of safari browser.
 @param callbackCompletion This completion would be called when events is invoked in the webview
*/
+ (void)showSafeBrowsing:(NSString *)urlString
          viewController:(UIViewController *)viewController
           configuration:(GPMSafeBrowsingConfiguration *)configuration
      callbackCompletion:(GPMWebViewCallbackCompletion) callbackCompletion;

/**
 Open the Browser with urlString
 
 @param urlString The URL to be loaded.
 @warning If urlString is not valid, to open browser would be failed. Please check the url before calling.
 */
+ (void)openWebBrowserWithURL:(NSString *)urlString;

/**
 Close the presenting Webview
 */
+ (void)close;

+ (BOOL)isActive;

+ (void)executeJavaScriptWithScript:(NSString *)script;

+ (BOOL)canGoBack;
+ (BOOL)canGoForward;
+ (void)goBack;
+ (void)goForward;

/**
 Set popup webView position
 */
+ (void)setPosition:(CGFloat)x y:(CGFloat)y;

/**
 Set popup webView size
 */
+ (void)setSize:(CGFloat)width height:(CGFloat)height;

/**
 Set popup webView margins
 */
+ (void)setMargins:(CGFloat)left top:(CGFloat)top right:(CGFloat)right bottom:(CGFloat)bottom;

+ (NSInteger)getX;
+ (NSInteger)getY;
+ (NSInteger)getWidth;
+ (NSInteger)getHeight;

/**
 Manually manage screen orientation.
 */
+ (void)setOrientation:(NSInteger)orientation;

@end

#endif /* GPMWebView_h */
