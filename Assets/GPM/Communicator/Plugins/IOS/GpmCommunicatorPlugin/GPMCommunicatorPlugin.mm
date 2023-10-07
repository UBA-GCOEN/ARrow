#import "GPMCommunicatorPlugin.h"
#import "GPMCommunicator.h"
#import "GPMCommunicatorReceiver.h"

@implementation GPMCommunicatorPlugin

+ (id)sharedGPMCommunicatorPlugin {
    static dispatch_once_t onceToken;
    static GPMCommunicatorPlugin* instance = nil;
    dispatch_once(&onceToken, ^{
        instance = [[GPMCommunicatorPlugin alloc] init];
    });
    return instance;
}

- (void)addReceiverWithDomain:(NSString*)domain receiver:(GPMCommunicatorReceiver*)receiver {
    [[GPMCommunicator sharedGPMCommunicator] addReceiverWithDomain:domain receiver:receiver];
}

- (void)sendResponseWithMessage:(GPMCommunicatorMessage*)message {
    [[GPMCommunicator sharedGPMCommunicator] sendResponseWithMessage:message];
}
@end
