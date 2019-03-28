//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//

#import <UIKit/UIKit.h>
#import "AppDelegate.h"



NSString *speechKey = @"YourSubscriptionKey";
NSString *serviceRegion = @"YourServiceRegion";
NSString *intentKey = @"YourLanguageUnderstandingSubscriptionKey";
NSString *intentRegion = @"YourLanguageUnderstandingServiceRegion";


int main(int argc, char * argv[]) {
    @autoreleasepool {
        return UIApplicationMain(argc, argv, nil, NSStringFromClass([AppDelegate class]));
    }
}
