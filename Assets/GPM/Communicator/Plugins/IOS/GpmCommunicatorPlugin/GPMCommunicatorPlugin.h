#import <Foundation/Foundation.h>
#import "GPMCommunicatorReceiver.h"
#import "GPMCommunicatorMessage.h"

@interface GPMCommunicatorPlugin: NSObject

+ (id)sharedGPMCommunicatorPlugin;
- (void)addReceiverWithDomain:(NSString*)domain receiver:(GPMCommunicatorReceiver*)receiver;
- (void)sendResponseWithMessage:(GPMCommunicatorMessage*)message;

@end
