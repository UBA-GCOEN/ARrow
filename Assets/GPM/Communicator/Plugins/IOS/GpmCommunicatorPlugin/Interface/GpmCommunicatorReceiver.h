#import <Foundation/Foundation.h>

@class GPMCommunicatorMessage;

typedef void (^RequestMessageAsync)(GPMCommunicatorMessage*);
typedef GPMCommunicatorMessage* (^RequestMessageSync)(GPMCommunicatorMessage*);

@interface GPMCommunicatorReceiver : NSObject

@property (nonatomic, strong) RequestMessageAsync onRequestMessageAsync;
@property (nonatomic, strong) RequestMessageSync onRequestMessageSync;

@end
