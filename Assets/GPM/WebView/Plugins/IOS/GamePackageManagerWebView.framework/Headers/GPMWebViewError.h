//
//  GPMWebViewError.h
//  GPMWebView
//
//  Created by NHN on 2020/11/24.
//  Copyright © 2020 NHN. All rights reserved.
//

#ifndef GPMWebViewError_h
#define GPMWebViewError_h

#define GPM_SUCCESS   nil

#import <Foundation/Foundation.h>

/** GPMWebViewError class represents a result of some APIs or an occurred error.
 */
@interface GPMWebViewError : NSError

/**---------------------------------------------------------------------------------------
 * @name Initialization
 *  ---------------------------------------------------------------------------------------
 */

/**
 Creates GPMWebViewError instance.
 
 @param code error code
*/
+ (GPMWebViewError *)resultWithCode:(NSInteger)code;

/**
 Creates GPMWebViewError instance.
 
 @param code error code.
 @param message error message.
 */
+ (GPMWebViewError *)resultWithCode:(NSInteger)code message:(NSString *)message;

/**
 Creates GPMWebViewError instance.
 
 @param domain error domain.
 @param code error code.
 @param userInfo a dictionary with userInfo.
 */
+ (GPMWebViewError *)resultWithDomain:(NSString *)domain code:(NSInteger)code userInfo:(NSDictionary *)userInfo;

/**
 Create GPMWebViewError instance. If the description value is nil or empty string, it will be set a value to default error message.
 
 @param domain domain error domain.
 @param code error code.
 @param description description about error. If it's value is set to nil or empty.
 @param underlyingError error object what a cause of error.
 */
+ (GPMWebViewError *)resultWithDomain:(NSString *)domain code:(NSInteger)code description:(NSString *)description underlyingError:(NSError *)underlyingError;

/**---------------------------------------------------------------------------------------
 * @name Properties
 *  ---------------------------------------------------------------------------------------
 */

/**
 Error message
 */
- (NSString *)message;

/**
 Pretty JSON string
 */
- (NSString *)prettyJsonString;

/**
 JSON string
 */
- (NSString *)jsonString;

@end

#endif /* GPMWebViewError_h */
