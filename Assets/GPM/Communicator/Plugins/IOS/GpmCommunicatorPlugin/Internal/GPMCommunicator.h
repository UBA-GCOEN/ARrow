#import <Foundation/Foundation.h>
#import "GPMCommunicatorReceiver.h"

@class GPMCommunicatorMessage;

@interface GPMCommunicator: NSObject

@property (nonatomic, strong)NSString* gameObjectName;
@property (nonatomic, strong)NSString* methodName;
@property (nonatomic, strong)NSMutableDictionary* receiverDictionary;

+ (instancetype)sharedGPMCommunicator;
- (void)setGameObjectName:(NSString*)gameObjectName methodName:(NSString*)methodName;
- (void)setClassName:(NSString*)className;
- (void)addReceiverWithDomain:(NSString*)domain receiver:(GPMCommunicatorReceiver*)receiver;
- (GPMCommunicatorReceiver*)getReceiverWithDomain:(NSString*)domain;
- (void)sendResponseWithMessage:(GPMCommunicatorMessage*)message;

@end
