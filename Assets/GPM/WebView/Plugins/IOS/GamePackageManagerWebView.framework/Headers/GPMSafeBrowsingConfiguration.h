//
//  GPMSafeBrowsingConfiguration.h
//  GamePackageManagerWebView
//
//  Created by NHN on 2022/05/23.
//  Copyright © 2022 NHN. All rights reserved.
//

#ifndef GPMSafeBrowsingConfiguration_h
#define GPMSafeBrowsingConfiguration_h

#import <UIKit/UIKit.h>

/** The GPMSafeBrowsingConfiguration configures the behavior of safari view launching.
 */
@interface GPMSafeBrowsingConfiguration : NSObject

/**---------------------------------------------------------------------------------------
 * @name Navigation
 *  ---------------------------------------------------------------------------------------
 */

/**
 Navigation Bar Color.
 
 @warning Default value is blue.
 */
@property (strong, nonatomic) UIColor *navigationBarColor;

/**
Navigation Text Color.
 
 @warning Default value is white.
 */
@property (strong, nonatomic) UIColor *navigationTextColor;

@end

#endif /* GPMSafeBrowsingConfiguration_h */
