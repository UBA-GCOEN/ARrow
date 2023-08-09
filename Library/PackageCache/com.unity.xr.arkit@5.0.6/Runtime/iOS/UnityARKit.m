#import <ARKit/ARKit.h>
#include "IUnityInterface.h"
#include "UnityAppController.h"

void UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API UnityARKitXRPlugin_PluginLoad(IUnityInterfaces* unityInterfaces);
extern void UnityARKit_SetRootView(UIView* view);

void UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API UnityARKit_EnsureRootViewIsSetup()
{
    UnityARKit_SetRootView(_UnityAppController.rootView);
}

@interface UnityARKit : NSObject

+ (void)loadPlugin;

@end

@implementation UnityARKit

+ (void)loadPlugin
{
    // This registers our plug-in with Unity
    UnityRegisterRenderingPluginV5(UnityARKitXRPlugin_PluginLoad, NULL);

    // This sets up some data our plug-in will need later
    UnityARKit_EnsureRootViewIsSetup();
}

@end

void* NSObject_get_description(void* self) {
    return (__bridge_retained void*)((__bridge NSObject*)self).description;
}

bool NSObject_isEqual_(void* self, void* other) {
    return [(__bridge NSObject*)self isEqual:(__bridge NSObject*)other];
}

uint64_t NSObject_get_hash(void* self) {
    return ((__bridge NSObject*)self).hash;
}

uint64_t NSString_lengthOfBytesUsingUTF16Encoding(void* self) {
    return [(__bridge NSString*)self lengthOfBytesUsingEncoding:NSUTF16StringEncoding];
}

void NSString_getBytes_maxLength_(void* self, void* buffer, uint64_t maxLength) {
    NSString* string = (__bridge NSString*)self;
    [string getBytes:buffer
           maxLength:maxLength
          usedLength:nil
            encoding:NSUTF16StringEncoding
             options:NSStringEncodingConversionAllowLossy
               range:NSMakeRange(0, string.length)
      remainingRange:nil];
}

uint64_t NSString_get_length(void* self) {
    return ((__bridge NSString*)self).length;
}

int64_t NSError_get_code(void* self) {
    return ((__bridge NSError*)self).code;
}

void* NSError_get_domain(void* self) {
    return (__bridge_retained void*)((__bridge NSError*)self).domain;
}

void* NSError_get_localizedDescription(void* self) {
    return (__bridge_retained void*)((__bridge NSError*)self).localizedDescription;
}

void* NSError_get_localizedRecoverySuggestion(void* self) {
    return (__bridge_retained void*)((__bridge NSError*)self).localizedRecoverySuggestion;
}

void* NSError_get_localizedFailureReason(void* self) {
    return (__bridge_retained void*)((__bridge NSError*)self).localizedFailureReason;
}

void* UnityARKit_GetARErrorDomain() {
    if (@available(iOS 11, *)) {
        return (__bridge void*)ARErrorDomain;
    }
    return NULL;
}

void* UnityARKit_GetCLErrorDomain() {
    return (__bridge void*)kCLErrorDomain;
}

void* ARReferenceObject_initWithBytes_length_(void* bytes, int length) {
    if (@available(iOS 12, *)) {
        NSData* data = [[NSData alloc] initWithBytesNoCopy:bytes length:length freeWhenDone:NO];

        NSURL* url = [NSURL fileURLWithPathComponents:[NSArray arrayWithObjects:
                                                       NSTemporaryDirectory(),
                                                       [[NSUUID UUID] UUIDString],
                                                       nil]];
        [data writeToFile:url.path atomically:NO];

        NSError* error = nil;
        ARReferenceObject* referenceObject = [[ARReferenceObject alloc] initWithArchiveURL:url
                                                                                     error:&error];
        if (error != nil) {
            NSLog(@"ARReferenceObject initWithArchiveURL failed: %@", error.localizedDescription);
        }

        [NSFileManager.defaultManager removeItemAtURL:url error:&error];
        if (error != nil) {
            NSLog(@"Failed to remove %@: %@", url.path, error.localizedDescription);
        }

        return (__bridge_retained void*)referenceObject;
    }

    return NULL;
}

void* ARReferenceObject_initWithArchiveURL_error_(void* url, void** errorOut) {
    if (@available(iOS 12, *)) {
        NSError* error = nil;
        ARReferenceObject* referenceObject = [[ARReferenceObject alloc] initWithArchiveURL:(__bridge NSURL*)url
                                                                                     error:&error];
        *errorOut = (__bridge_retained void*)error;
        return (__bridge_retained void*)referenceObject;
    }

    *errorOut = NULL;
    return NULL;
}

void* ARReferenceObject_get_name(void* self) {
    if (@available(iOS 12, *)) {
        return (__bridge void*)((__bridge ARReferenceObject*)self).name;
    }

    return NULL;
}

void ARReferenceObject_set_name(void* self, void* name) {
    if (@available(iOS 12, *)) {
        ((__bridge ARReferenceObject*)self).name = (__bridge NSString*)name;
    }
}

void* ARReferenceObject_referenceObjectsInGroupNamed_bundle_(void* groupName, void* bundle) {
    if (@available(iOS 12, *)) {
        return (__bridge_retained void*)[ARReferenceObject referenceObjectsInGroupNamed:(__bridge NSString*)groupName
                                                                                 bundle:(__bridge NSBundle*)bundle];
    }

    return NULL;
}

void* NSURL_URLWithString_(void* str) {
    return (__bridge_retained void*)[NSURL URLWithString:(__bridge NSString*)str];
}

void* NSURL_fileURLWithPath_(void* path) {
    return (__bridge_retained void*)[NSURL fileURLWithPath:(__bridge NSString*)path];
}

void* NSKeyedUnarchiver_unarchivedObjectOfClass_fromData_error_(Class cls, void* data, void** errorOut) {
    NSError* error = nil;
    id unarchivedObject = [NSKeyedUnarchiver unarchivedObjectOfClass:cls
                                                            fromData:(__bridge NSData*)data
                                                               error:&error];
    *errorOut = (__bridge_retained void*)error;
    return (__bridge_retained void*)unarchivedObject;
}

Class ARReferenceObject_class() {
    if (@available(iOS 12, *)) {
        return [ARReferenceObject class];
    }

    return NULL;
}

void* NSUUID_UUIDString(void* self) {
    return (__bridge_retained void*)[(__bridge NSUUID*)self UUIDString];
}

void* NSUUID_initWithUUIDBytes_(guid_t guid) {
    return (__bridge_retained void*)[[NSUUID alloc] initWithUUIDBytes:guid.g_guid];
}

void* NSUUID_UUID() {
    return (__bridge_retained void*)[NSUUID UUID];
}

void* NSString_initWithBytes_length_encoding_(const int16_t* bytes, int length, uint64_t encoding) {
    return (__bridge_retained void*)[[NSString alloc] initWithBytes:bytes
                                                             length:sizeof(int16_t) * length
                                                           encoding:(NSStringEncoding)encoding];
}

void* NSMutableString_init() {
    return (__bridge_retained void*)[NSMutableString new];
}

void* NSMutableString_initWithBytes_length_encoding_(const int16_t* bytes, int length, uint64_t encoding) {
    return (__bridge_retained void*)[[NSMutableString alloc] initWithBytes:bytes
                                                                    length:sizeof(int16_t) * length
                                                                  encoding:(NSStringEncoding)encoding];
}

void* NSEnumerator_get_nextObject(void* self) {
    return (__bridge void*)((__bridge NSEnumerator*)self).nextObject;
}

uint64_t NSSet_get_count(void* self) {
    return ((__bridge NSSet*)self).count;
}

bool NSSet_containsObject_(void* self, void* obj) {
    return [(__bridge NSSet*)self containsObject:(__bridge id)obj];
}

void* NSSet_get_objectEnumerator(void* self) {
    return (__bridge_retained void*)((__bridge NSSet*)self).objectEnumerator;
}

void* NSMutableSet_new() {
    return (__bridge_retained void*)[NSMutableSet new];
}

void* NSMutableSet_setWithSet_(void* set) {
    return (__bridge_retained void*)[NSMutableSet setWithSet:(__bridge NSSet*)set];
}

void NSMutableSet_addObject_(void* self, void* obj) {
    [(__bridge NSMutableSet*)self addObject:(__bridge id)obj];
}

void NSMutableSet_removeObject_(void* self, void* obj) {
    [(__bridge NSMutableSet*)self removeObject:(__bridge id)obj];
}

void NSMutableSet_removeAllObjects(void* self) {
    [(__bridge NSMutableSet*)self removeAllObjects];
}

void NSMutableSet_unionSet_(void* self, void* set) {
    [(__bridge NSMutableSet*)self unionSet:(__bridge NSSet*)set];
}

void NSMutableString_appendString_(void* self, void* str) {
    [(__bridge NSMutableString*)self appendString:(__bridge NSString*)str];
}

void* NSMutableString_initWithString_(void* str) {
    return (__bridge_retained void*)[NSMutableString stringWithString:(__bridge NSString*)str];
}

void* NSBundle_get_mainBundle() {
    return (__bridge void*)NSBundle.mainBundle;
}

void* NSBundle_get_bundlePath(void* self) {
    return (__bridge_retained void*)((__bridge NSBundle*)self).bundlePath;
}

void* NSBundle_get_bundleIdentifier(void* self) {
    return (__bridge_retained void*)((__bridge NSBundle*)self).bundleIdentifier;
}

void* ARReferenceImage_referenceImagesInGroupNamed_bundle_(void* groupName, void* bundle) {
    if (@available(iOS 11.3, *)) {
        return (__bridge_retained void*)[ARReferenceImage referenceImagesInGroupNamed:(__bridge NSString*)groupName
                                                                               bundle:(__bridge NSBundle*)bundle];
    }

    return NULL;
}

Class ARReferenceImage_class() {
    if (@available(iOS 11.3, *)) {
        return [ARReferenceImage class];
    }

    return NULL;
}

void* NSMutableData_new() {
    return (__bridge_retained void*)[NSMutableData new];
}

void NSMutableData_appendBytes_length_(void* self, const void* bytes, int length) {
    [(__bridge NSMutableData*)self appendBytes:bytes
                                        length:length];
}

void* NSMutableData_createWithBytes_length_(const void* bytes, int length) {
    return (__bridge_retained void*)[[NSMutableData alloc] initWithBytes:bytes
                                                                  length:length];
}

void* NSData_createWithBytes_length_(const void* bytes, int length)
{
    return (__bridge_retained void*)[[NSData alloc] initWithBytes:bytes
                                                           length:length];
}

void* NSData_createWithBytesNoCopy_length_freeWhenDone_(void* bytes, int length, bool freeWhenDone) {
    return (__bridge_retained void*)[[NSData alloc] initWithBytesNoCopy:bytes
                                                                 length:length
                                                           freeWhenDone:freeWhenDone];
}

uint64_t NSData_get_length(void* self) {
    return ((__bridge NSData*)self).length;
}

const void* NSData_get_bytes(void* self) {
    return ((__bridge NSData*)self).bytes;
}

Class NSSet_class() { return [NSSet class]; }
Class NSMutableSet_class() { return [NSMutableSet class]; }
Class NSString_class() { return [NSString class]; }
Class NSMutableString_class() { return [NSMutableString class]; }
Class NSData_class() { return [NSData class]; }
Class NSMutableData_class() { return [NSMutableData class]; }
Class NSBundle_class() { return [NSBundle class]; }
Class NSError_class() { return [NSError class]; }
Class NSUUID_class() { return [NSUUID class]; }
Class NSURL_class() { return [NSURL class]; }
Class NSEnumerator_class() { return [NSEnumerator class]; }
