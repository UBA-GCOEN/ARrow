#import "GPMCommunicator.h"
#import "GPMCommunicatorReceiver.h"
#import "GPMCommunicatorMessage.h"

#define GPM_COMMUNICATOR_DELIMITER @"${gpm_communicator}"

@implementation GPMCommunicator

@synthesize gameObjectName = _gameObjectName;
@synthesize methodName = _methodName;
@synthesize receiverDictionary = _receiverDictionary;

+ (instancetype)sharedGPMCommunicator {
    static dispatch_once_t onceToken;
    static GPMCommunicator* instance = nil;
    dispatch_once(&onceToken, ^{
        instance = [[GPMCommunicator alloc] init];
        instance.receiverDictionary = [NSMutableDictionary dictionary];
    });
    return instance;
}

- (void)setClassName:(NSString*)className {
    Class newClass = NSClassFromString(className);
    if(newClass != nil)
    {
        id newInstance = [[newClass alloc] init];
        if(newInstance == nil)
        {
            return;
        }
    }
}

- (void)setGameObjectName:(NSString*)gameObjectName methodName:(NSString*)methodName {
    _gameObjectName = gameObjectName;
    _methodName = methodName;
}

- (void)addReceiverWithDomain:(NSString*)domain receiver:(GPMCommunicatorReceiver*)receiver {
    if([_receiverDictionary objectForKey:domain] != nil) {
        NSLog(@"%@ : %@", @"The receiver is already registered", domain);
        return;
    }
    
    [_receiverDictionary setObject:receiver forKey:domain];
}

- (GPMCommunicatorReceiver*)getReceiverWithDomain:(NSString*)domain {
    GPMCommunicatorReceiver* receiver = [_receiverDictionary objectForKey:domain];
    if(receiver == nil) {
        NSLog(@"%@ : %@", @"There is no registered receiver", domain);
    }
    return receiver;
}

- (void)sendResponseWithMessage:(GPMCommunicatorMessage*)message {
    if (_gameObjectName == nil || _methodName == nil || message == nil){
    }
    else {
        NSString* sendMessage = [NSString stringWithFormat:@"%@%@%@%@%@", message.domain, GPM_COMMUNICATOR_DELIMITER, message.data, GPM_COMMUNICATOR_DELIMITER, message.extra];
        
        UnitySendMessage([_gameObjectName UTF8String], [_methodName UTF8String], [sendMessage UTF8String]);
    }
}
@end
