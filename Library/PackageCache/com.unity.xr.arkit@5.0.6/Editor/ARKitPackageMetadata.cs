using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARKit;
using UnityEditor.XR.Management.Metadata;

namespace UnityEditor.XR.ARKit
{
    class XRPackage : IXRPackage
    {
        class ARKitLoaderMetadata : IXRLoaderMetadata
        {
            public string loaderName { get; set; }
            public string loaderType { get; set; }
            public List<BuildTargetGroup> supportedBuildTargets { get; set; }
        }

        class ARKitPackageMetadata : IXRPackageMetadata
        {
            public string packageName { get; set; }
            public string packageId { get; set; }
            public string settingsType { get; set; }
            public List<IXRLoaderMetadata> loaderMetadata { get; set; }
        }

        static IXRPackageMetadata s_Metadata = new ARKitPackageMetadata()
        {
            packageName = ARKitPackageInfo.displayName,
            packageId = ARKitPackageInfo.identifier,
            settingsType = typeof(ARKitSettings).FullName,
            loaderMetadata = new List<IXRLoaderMetadata>()
            {
                new ARKitLoaderMetadata()
                {
                    loaderName = "Apple ARKit",
                    loaderType = typeof(ARKitLoader).FullName,
                    supportedBuildTargets = new List<BuildTargetGroup>()
                    {
                        BuildTargetGroup.iOS
                    }
                },
            }
        };

        public IXRPackageMetadata metadata => s_Metadata;

        public bool PopulateNewSettingsInstance(ScriptableObject obj)
        {
            if (obj is ARKitSettings settings)
            {
                ARKitSettings.currentSettings = settings;
                settings.requirement = ARKitSettings.Requirement.Required;
                settings.faceTracking = false;
                return true;
            }

            return false;
        }
    }
}
