using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.XR.ARSubsystems;

#if UNITY_IOS
using UnityEditor.iOS.Xcode;
#endif

namespace UnityEditor.XR.ARKit
{
    class XcodeAssetCatalog
    {
        public string name { get; set; }

        public IReadOnlyCollection<ARResourceGroup> resourceGroups => m_ResourceGroups;

        public int count => m_ResourceGroups.Count;

        public XcodeAssetCatalog(string name)
        {
            this.name = name;
        }

        public void AddResourceGroup(ARResourceGroup group)
        {
            if (group == null)
                throw new ArgumentNullException("group");

            if (m_ResourceGroups.Contains(group))
                throw new InvalidOperationException(string.Format("Duplicate resource group '{0}'", group.name));

            m_ResourceGroups.Add(group);
        }

#if UNITY_IOS
        public void WriteAndAddToPBXProject(PBXProject project, string pathToBuiltProject)
        {
            const string unityTargetName = "Unity-iPhone";
            var relativePathToAssetCatalog = Path.Combine(unityTargetName, name + ".xcassets");
            var fullPathToAssetCatalog = Path.Combine(pathToBuiltProject, relativePathToAssetCatalog);

            // Create the asset catalog, destroying an existing one.
            if (Directory.Exists(fullPathToAssetCatalog))
                Directory.Delete(fullPathToAssetCatalog, true);
            Directory.CreateDirectory(fullPathToAssetCatalog);

            // Add it to Xcode's build
            var folderGuid = project.AddFile(relativePathToAssetCatalog, relativePathToAssetCatalog);
            var targetGuid = project.GetUnityMainTargetGuid();
            project.AddFileToBuild(targetGuid, folderGuid);

            foreach (var resourceGroup in m_ResourceGroups)
            {
                resourceGroup.Write(fullPathToAssetCatalog);
            }
        }
#endif

        public void Write(string path)
        {
            if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
            }

            Directory.CreateDirectory(path);

            foreach (var resourceGroup in m_ResourceGroups)
            {
                resourceGroup.Write(path);
            }
        }

        public byte[] ToCar(Version minimumDeploymentTarget)
        {
            var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));

            // Doesn't matter what we call this filename
            var assetCatalogPath = Path.Combine(tempDir, "AssetCatalog.xcassets");

            try
            {
                // Write to a file
                Write(assetCatalogPath);

                // Invoke the actool
                var carPath = ACTool.Compile(assetCatalogPath, tempDir, minimumDeploymentTarget);

                return File.ReadAllBytes(carPath);
            }
            catch (ACTool.XCRunNotFoundException)
            {
                Debug.LogWarning($"xcrun (an Xcode tool) was not found. This is necessary to bake ARKit-specific data into an {nameof(XRReferenceImageLibrary)}. {nameof(XRReferenceImageLibrary)} assets will not work in asset bundles.");
                return null;
            }
            finally
            {
                if (Directory.Exists(tempDir))
                {
                    Directory.Delete(tempDir, true);
                }
            }
        }

        List<ARResourceGroup> m_ResourceGroups = new List<ARResourceGroup>();
    }
}
