#import <Foundation/Foundation.h>

@interface GPMWebViewJsonUtil : NSObject {
}
+(id)sharedGPMWebViewJsonUtil;

-(NSMutableDictionary*)mutableDictionaryFromObject:(NSObject*)src;
-(NSString*)prettyJsonStringFromObject:(NSObject*)src;
-(NSString*)prettyJsonStringFromJsonString:(NSString*)src;
@end

@interface NSDictionary (JSON)
- (NSString *)JSONString;
- (NSString *)PrettyJSONString;
@end

@interface NSArray (JSON)
- (NSString *)JSONString;
- (NSString *)JSONStringFromArray;
@end

@interface NSString (JSON)
- (NSDictionary *)JSONDictionary;
@end
