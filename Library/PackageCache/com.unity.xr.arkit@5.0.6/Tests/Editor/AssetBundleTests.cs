using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using Unity.Collections.LowLevel.Unsafe;
using UnityEditor.XR.ARSubsystems;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.ARKit;
using UnityEngine;

namespace UnityEditor.XR.ARKit.Tests
{
    [TestFixture]
    class AssetBundleTests
    {
        const string k_BasePath = "Packages/com.unity.xr.arkit/Tests/Editor";

        const string k_AssetsPath = k_BasePath + "/Assets";

        const string k_OutputPath = k_BasePath + "/GeneratedAssetBundles";

        const string k_ReferenceImageLibraryName = "TestReferenceImageLibrary";

        const string k_ReferenceImageLibraryPath = k_AssetsPath + "/" + k_ReferenceImageLibraryName + ".asset";

        const string k_ReferenceObjectLibraryName = "TestReferenceObjectLibrary";

        const string k_ReferenceObjectLibraryPath = k_AssetsPath + "/" + k_ReferenceObjectLibraryName + ".asset";

        const string k_BundleName = "arkit_test_bundle";

        AssetBundle m_AssetBundle;

        static OSVersion XcodeVersion
        {
            get
            {
#if UNITY_IOS
                var xcodeIndex = Math.Max(0, iOS.XcodeApplications.GetPreferedXcodeIndex());
                return OSVersion.Parse(iOS.XcodeApplications.GetXcodeApplicationPublicName(xcodeIndex));
#else
                return new OSVersion(0);
#endif
            }
        }

        [OneTimeSetUp]
        public void Setup()
        {
            Directory.CreateDirectory(k_OutputPath);
            AssetImporter.GetAtPath(k_ReferenceImageLibraryPath).assetBundleName = k_BundleName;
            AssetImporter.GetAtPath(k_ReferenceObjectLibraryPath).assetBundleName = k_BundleName;

            // Run the preprocessor for iOS
            ARBuildProcessor.PreprocessBuild(BuildTarget.iOS);

            if (BuildPipeline.IsBuildTargetSupported(BuildTargetGroup.iOS, BuildTarget.iOS))
            {
                // Build asset bundles
                var assetBundleManifest = BuildPipeline.BuildAssetBundles(k_OutputPath, BuildAssetBundleOptions.ForceRebuildAssetBundle, BuildTarget.iOS);

                // Our asset bundle should be among the generated asset bundles
                Assert.IsTrue(assetBundleManifest.GetAllAssetBundles().Contains(k_BundleName));

                // Load the asset bundle we just generated
                m_AssetBundle = AssetBundle.LoadFromFile($"{k_OutputPath}/{k_BundleName}");
            }
        }

        static void DeleteDirectory(string path)
        {
            if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
            }

            var metaFile = $"{path}.meta";
            if (File.Exists(metaFile))
            {
                File.Delete(metaFile);
            }
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            DeleteDirectory(k_OutputPath);
            AssetImporter.GetAtPath(k_ReferenceImageLibraryPath).assetBundleName = string.Empty;
            AssetImporter.GetAtPath(k_ReferenceObjectLibraryPath).assetBundleName = string.Empty;

            // Clear internal data stores
            foreach (var library in AssetDatabase
                .FindAssets($"t:{nameof(XRReferenceImageLibrary)}")
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(AssetDatabase.LoadAssetAtPath<XRReferenceImageLibrary>))
            {
                library.ClearDataStore();
            }
        }

        static unsafe void AssertBytesAreEqual(byte[] expectedBytes, byte[] actualBytes)
        {
            // Neither may be null
            Assert.NotNull(expectedBytes);
            Assert.NotNull(actualBytes);

            // Compare the two sets of bytes
            Assert.AreEqual(expectedBytes.Length, actualBytes.Length);
            fixed (byte* expected = expectedBytes)
            fixed (byte* actual = actualBytes)
            {
                Assert.AreEqual(0, UnsafeUtility.MemCmp(expected, actual, expectedBytes.Length));
            }
        }

        [Test]
        public void ReferenceImagesAreStoredInAssetBundles()
        {
            // This test requires Xcode 11.3 or higher to be installed.
            if (XcodeVersion < new OSVersion(11, 3) || m_AssetBundle == null)
                return;

            // The data store should now contain the ARKit reference image data
            var processedLibrary = AssetDatabase.LoadAssetAtPath<XRReferenceImageLibrary>(k_ReferenceImageLibraryPath);
            Assert.IsTrue(processedLibrary.dataStore.ContainsKey(ARKitPackageInfo.identifier));
            var expectedBytes = processedLibrary.dataStore[ARKitPackageInfo.identifier];
            Assert.Greater(expectedBytes?.Length, 0);

            // Load the asset bundle and extract the reference image library
            var referenceImageLibrary = m_AssetBundle.LoadAsset<XRReferenceImageLibrary>(k_ReferenceImageLibraryName);

            // Get the ARKit reference image library bytes
            Assert.IsTrue(referenceImageLibrary.dataStore.ContainsKey(ARKitPackageInfo.identifier));
            var actualBytes = referenceImageLibrary.dataStore[ARKitPackageInfo.identifier];

            AssertBytesAreEqual(expectedBytes, actualBytes);
        }

        [Test]
        public void ReferenceObjectsAreStoredInAssetBundles()
        {
            if (m_AssetBundle == null)
                return;

            // The data store should now contain the ARKit reference object data
            var expectedLibrary = AssetDatabase.LoadAssetAtPath<XRReferenceObjectLibrary>(k_ReferenceObjectLibraryPath);
            var expectedReferenceObjects = expectedLibrary
                .Select(obj => obj.FindEntry<ARKitReferenceObjectEntry>())
                .ToArray();

            foreach (var referenceObject in expectedReferenceObjects)
            {
                Assert.IsTrue(referenceObject != null);
                Assert.Greater(referenceObject.m_ReferenceObjectBytes?.Length, 0);
            }

            // Load the asset bundle and extract the reference image library
            var actualLibrary = m_AssetBundle.LoadAsset<XRReferenceObjectLibrary>(k_ReferenceObjectLibraryName);
            Assert.IsTrue(actualLibrary != null);

            var actualReferenceObjects = actualLibrary
                .Select(obj => obj.FindEntry<ARKitReferenceObjectEntry>())
                .ToArray();

            Assert.AreEqual(expectedReferenceObjects.Length, actualReferenceObjects.Length);

            for (var i = 0; i < actualReferenceObjects.Length; i++)
            {
                var actualReferenceObject = actualReferenceObjects[i];
                Assert.IsTrue(actualReferenceObject != null);

                AssertBytesAreEqual(
                    expectedReferenceObjects[i].m_ReferenceObjectBytes,
                    actualReferenceObject.m_ReferenceObjectBytes);
            }
        }
    }
}
