#import "GPMCommunicatorMessage.h"

@implementation GPMCommunicatorMessage

@synthesize domain = _domain;
@synthesize data = _data;
@synthesize extra = _extra;

- (instancetype)initWithDomain:(NSString*)domain data:(NSString*)data extra:(NSString*)extra {
    if(self = [super init]){
        _domain = domain;
        _data = data;
        _extra = extra;
    }
    
    return self;
}
@end
