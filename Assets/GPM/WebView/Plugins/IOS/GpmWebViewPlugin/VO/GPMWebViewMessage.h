#import <Foundation/Foundation.h>

@interface GPMWebViewMessage : NSObject {
    NSString* _scheme;
    NSString* _error;
    NSString* _data;
    NSString* _extra;
    NSInteger _callback;
    NSInteger _callbackType;
}

@property (nonatomic, strong) NSString* scheme;
@property (nonatomic, strong) NSString* data;
@property (nonatomic, strong) NSString* extra;
@property (nonatomic, strong) NSString* error;
@property (nonatomic, assign) NSInteger callback;
@property (nonatomic, assign) NSInteger callbackType;

-(id)initWithJsonString:(NSString*)jsonString;
-(NSString*)toJsonString;
@end

