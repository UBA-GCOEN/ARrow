#import <Foundation/Foundation.h>

@interface GPMCommunicatorMessage : NSObject

@property (nonatomic, strong) NSString* domain;
@property (nonatomic, strong) NSString* data;
@property (nonatomic, strong) NSString* extra;

- (instancetype)initWithDomain:(NSString*)domain data:(NSString*)data extra:(NSString*)extra;

@end
