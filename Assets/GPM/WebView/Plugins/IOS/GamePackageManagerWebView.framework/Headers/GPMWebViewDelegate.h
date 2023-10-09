//
//  GPMWebViewDelegate.h
//  GPMWebView
//
//  Created by NHN on 2020/11/24.
//  Copyright © 2020 NHN. All rights reserved.
//

#ifndef GPMWebViewDelegate_h
#define GPMWebViewDelegate_h

#import <WebKit/WebKit.h>

/** The GPMWebViewDelegate is a UIViewController delegate.
 */
@protocol GPMWebViewDelegate <NSObject>

@required

@optional
- (void)viewDidAppear:(BOOL)animated;
- (void)viewDidDisappear:(BOOL)animated;
- (void)close;
- (BOOL)isActive;
- (void)goBack;
- (void)goForward;
- (void)setPosition:(CGFloat)x y:(CGFloat)y;
- (void)setSize:(CGFloat)width height:(CGFloat)height;
- (void)setMargins:(CGFloat)left top:(CGFloat)top right:(CGFloat)right bottom:(CGFloat)bottom;
- (int)getX;
- (int)getY;
- (int)getWidth;
- (int)getHeight;

- (void)webView:(WKWebView *)webView decidePolicyForNavigationAction:(WKNavigationAction *)navigationAction decisionHandler:(void (^)(WKNavigationActionPolicy))decisionHandler;
- (void)webView:(WKWebView *)webView decidePolicyForNavigationResponse:(WKNavigationResponse *)navigationResponse decisionHandler:(void (^)(WKNavigationResponsePolicy))decisionHandler;
- (void)webView:(WKWebView *)webView didCommitNavigation:(WKNavigation *)navigation;
- (void)webView:(WKWebView *)webView didFinishNavigation:(WKNavigation *)navigation;
- (void)webView:(WKWebView *)webView didFailNavigation:(WKNavigation *)navigation withError:(NSError *)error;
- (void)evaluateJavaScript:(NSString *)script completionHandler:(void(^)(id data, NSError *error))completion;
- (void)evaluateJavaScript:(NSString *)script;

@end

#endif /* GPMWebViewDelegate_h */
