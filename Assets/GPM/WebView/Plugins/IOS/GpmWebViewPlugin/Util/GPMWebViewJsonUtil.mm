#import <Foundation/Foundation.h>
#import "GPMWebViewJsonUtil.h"
#import <objc/runtime.h>

@implementation GPMWebViewJsonUtil

+(id)sharedGPMWebViewJsonUtil {
    static dispatch_once_t onceToken;
    static GPMWebViewJsonUtil* instance = nil;
    dispatch_once(&onceToken, ^{
        instance = [[GPMWebViewJsonUtil alloc] init];
    });
    return instance;
}

-(NSMutableDictionary*)mutableDictionaryFromObject:(NSObject*)src {
    NSMutableDictionary *dict = [NSMutableDictionary dictionary];
    
    unsigned count;
    objc_property_t *properties = class_copyPropertyList([src class], &count);
    
    for (int i = 0; i < count; i++) {
        NSString *key = [NSString stringWithUTF8String:property_getName(properties[i])];
        dict[key] = [src valueForKey:key];
    }
    
    free(properties);
    
    return dict;
}

-(NSString*)prettyJsonStringFromObject:(NSObject*)src {
    NSMutableDictionary* jsonDictionary = [[GPMWebViewJsonUtil sharedGPMWebViewJsonUtil] mutableDictionaryFromObject:src];
    NSString* jsonString = [self prettyJsonStringFromMutableDictionary:jsonDictionary];
    return jsonString;
}

-(NSString*)prettyJsonStringFromJsonString:(NSString*)src {
    NSMutableDictionary* jsonDictionary = [[src JSONDictionary] mutableCopy];
    NSString* jsonString = [self prettyJsonStringFromMutableDictionary:jsonDictionary];
    return jsonString;
}

-(NSString*)prettyJsonStringFromMutableDictionary:(NSMutableDictionary*)src {
    NSError* jsonError;
    NSData* jsonData = [NSJSONSerialization dataWithJSONObject:src options:NSJSONWritingPrettyPrinted error:&jsonError];
    
    if(jsonData) {
        return [[NSString alloc] initWithData:jsonData encoding:NSUTF8StringEncoding];
    }
    
    return @"";
}
@end

@implementation NSDictionary (JSON)

- (NSString *)JSONString {
    NSError* error;
    NSData* jsonData = [NSJSONSerialization dataWithJSONObject:self options:0 error:&error];
    if (error) {
        return nil;
    }
    NSString* jsonString = [[NSString alloc] initWithData:jsonData encoding:NSUTF8StringEncoding];
    return jsonString;
}

- (NSString *)PrettyJSONString {
    NSError* error;
    NSData* jsonData = [NSJSONSerialization dataWithJSONObject:self options:NSJSONWritingPrettyPrinted error:&error];
    if (error) {
        return nil;
    }
    NSString* jsonString = [[NSString alloc] initWithData:jsonData encoding:NSUTF8StringEncoding];
    return jsonString;
}
@end

@implementation NSArray (JSON)

- (NSString *)JSONString {
    __block NSMutableArray *jsonArray = [NSMutableArray array];
    [self enumerateObjectsUsingBlock:^(id  _Nonnull obj, NSUInteger idx, BOOL * _Nonnull stop) {
        [jsonArray addObject:[obj description]];
    }];
    
    return [NSString stringWithFormat:@"[%@]", [jsonArray componentsJoinedByString:@","]];
}


- (NSString *)JSONStringFromArray {
    NSError* error;
    NSData* jsonData = [NSJSONSerialization dataWithJSONObject:self options:0 error:&error];
    if (error) {
        return nil;
    }
    NSString* jsonString = [[NSString alloc] initWithData:jsonData encoding:NSUTF8StringEncoding];
    return jsonString;
}
/*
 - (NSString *)PrettyJSONString {
 NSError* error;
 NSData* jsonData = [NSJSONSerialization dataWithJSONObject:self options:NSJSONWritingPrettyPrinted error:&error];
 if (error) {
 return nil;
 }
 NSString* jsonString = [[NSString alloc] initWithData:jsonData encoding:NSUTF8StringEncoding];
 return jsonString;
 }
 */
@end

@implementation NSString (JSON)

- (id)JSONDictionary {
    NSError* error = nil;
    if (self == nil || [self length] <= 0) {
        return nil;
    }
    
    id jsonObject = [NSJSONSerialization JSONObjectWithData:[self dataUsingEncoding:NSUTF8StringEncoding] options:0 error:&error];
	
    if (error) {
        return nil;
    }
    return jsonObject;
}

@end
