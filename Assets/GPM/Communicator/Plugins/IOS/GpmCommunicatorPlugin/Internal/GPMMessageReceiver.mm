#import "GPMCommunicator.h"
#import "GPMCommunicatorMessage.h"
#import "GPMCommunicatorReceiver.h"

#define GPM_COMMUNICATOR_DELIMITER @"${gpm_communicator}"

#pragma mark - extern C
extern "C" {
    void initializeUnityObject(char* gameObjectName, char* methodName)
    {
        NSString *unityGameObjectName;
        
        if(gameObjectName != nil) {
            unityGameObjectName = [NSString stringWithUTF8String:gameObjectName];
        }
        
        NSString *unityMethodName;
        
        if(methodName != nil) {
            unityMethodName = [NSString stringWithUTF8String:methodName];
        }
        
        [[GPMCommunicator sharedGPMCommunicator] setGameObjectName:unityGameObjectName methodName:unityMethodName];
    }
    
    void initializeClass(char* className)
    {
        NSString *iosClassName;
        
        if(className != nil) {
            iosClassName = [NSString stringWithUTF8String:className];
        }
        
        [[GPMCommunicator sharedGPMCommunicator] setClassName:iosClassName];
    }
    
    char* onRequestSync(char* domain, char* data, char* extra) {
        NSString *iosDomain;
        
        if(domain != nil) {
            iosDomain = [NSString stringWithUTF8String:domain];
        }
        
        NSString *iosData;
        
        if(data != nil) {
            iosData = [NSString stringWithUTF8String:data];
        }
        
        NSString *iosExtra;
        
        if(extra != nil) {
            iosExtra = [NSString stringWithUTF8String:extra];
        }
        
        GPMCommunicatorMessage* message = [[GPMCommunicatorMessage alloc] initWithDomain:iosDomain data:iosData extra:iosExtra];
        GPMCommunicatorReceiver* receiver = [[GPMCommunicator sharedGPMCommunicator] getReceiverWithDomain:iosDomain];
        if(receiver == nil) {
            NSLog(@"%@ : %@", @"There is no registered receiver", iosDomain);
            return (char*)[@"" UTF8String];
        }
        
        GPMCommunicatorMessage* responseMessage = receiver.onRequestMessageSync(message);

        NSString* responseString = [NSString stringWithFormat:@"%@%@%@%@%@", responseMessage.domain, GPM_COMMUNICATOR_DELIMITER, responseMessage.data, GPM_COMMUNICATOR_DELIMITER, responseMessage.extra];

        return (char*)[responseString UTF8String];
    }
    
    void onRequestAsync(char* domain, char* data, char* extra) {
        NSString *iosDomain;
        
        if(domain != nil) {
            iosDomain = [NSString stringWithUTF8String:domain];
        }
        
        NSString *iosData;
        
        if(data != nil) {
            iosData = [NSString stringWithUTF8String:data];
        }
        
        NSString *iosExtra;
        
        if(extra != nil) {
            iosExtra = [NSString stringWithUTF8String:extra];
        }
        
        GPMCommunicatorMessage* message = [[GPMCommunicatorMessage alloc] initWithDomain:iosDomain data:iosData extra:iosExtra];
        GPMCommunicatorReceiver* receiver = [[GPMCommunicator sharedGPMCommunicator] getReceiverWithDomain:iosDomain];
        if(receiver == nil) {
            NSLog(@"%@ : %@", @"There is no registered receiver", iosDomain);
            return;
        }
        
        [[GPMCommunicator sharedGPMCommunicator] getReceiverWithDomain:iosDomain].onRequestMessageAsync(message);
    }
}
