#import <Foundation/Foundation.h>
#import "GPMWebViewMessage.h"
#import "GPMWebViewJsonUtil.h"

@implementation GPMWebViewMessage

@synthesize scheme = _scheme;
@synthesize data = _data;
@synthesize extra = _extra;
@synthesize error = _error;
@synthesize callback = _callback;
@synthesize callbackType = _callbackType;

-(id)initWithJsonString:(NSString*)jsonString {
    if(self = [super init]) {        
        NSDictionary* convertedDic = [jsonString JSONDictionary];
        
        self.scheme = convertedDic[@"scheme"];
        self.data = convertedDic[@"data"];
        self.extra = convertedDic[@"extra"];
        self.error = convertedDic[@"error"];
        self.callback = [convertedDic[@"callback"] intValue];
        self.callbackType = [convertedDic[@"callbackType"] intValue];
    }
    return self;
}

-(NSString*)toJsonString {
    NSMutableDictionary* jsonDic = [[GPMWebViewJsonUtil sharedGPMWebViewJsonUtil] mutableDictionaryFromObject:self];
    
    NSString* jsonString = [jsonDic JSONString];
    
    return jsonString;
}
@end
