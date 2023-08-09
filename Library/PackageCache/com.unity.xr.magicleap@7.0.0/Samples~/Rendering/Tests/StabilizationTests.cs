using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;
using System;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using UnityRandom = UnityEngine.Random;

namespace Unity.MagicLeap.Samples.Rendering.Tests
{
    public class StabilizationTests
    {
        [UnitySetUp]
        public void Setup()
        {
            Debug.Log("TestStarted");
        }

        [UnityTearDown]
        public void Teardown()
        {
            Debug.Log("TestEnded");
        }

        [UnityTest]
        [RequireMagicLeapDevice]
        public IEnumerator CanCreateABunchOfStabilizerObjects()
        {
            var box = new Bounds(Vector3.zero, Vector3.one * 10);
            for (int i = 0; i < 10; i++)
            {
                var go = new GameObject(string.Format("Stabilizer {0}", i));
                go.AddComponent<StabilizationComponent>();
                go.transform.position = GetRandomPointFromBoundingBox(box);
                yield return null;
            }
        }

        private Vector3 GetRandomPointFromBoundingBox(Bounds bounds)
        {
            var r = UnityRandom.insideUnitSphere;
            var max = bounds.max;
            var min = bounds.min;
            return new Vector3
            {
                x = Mathf.Lerp(min.x, max.x, r.x),
                y = Mathf.Lerp(min.y, max.y, r.y),
                z = Mathf.Lerp(min.z, max.z, r.z)
            };
        }
    }

    [AttributeUsage(System.AttributeTargets.Method, AllowMultiple = true)]
    public class RequireMagicLeapDevice : NUnitAttribute, IApplyToTest
    {
        private static string envVariable = "ML_DEVICE_CONNECTED";

        private static string m_SkippedReason =
            String.Format("{0} environment variable not set. Assuming ML device not connected. Skipping test.",
                envVariable);

        public void ApplyToTest(Test test)
        {
            if (test.RunState == RunState.NotRunnable || test.RunState == RunState.Ignored || IsMagicLeapDeviceConnected())
            {
                return;
            }
            test.RunState = RunState.Skipped;
            test.Properties.Add("_SKIPREASON", m_SkippedReason);
        }

        public static bool IsMagicLeapDeviceConnected()
        {
#if PLATFORM_LUMIN && !UNITY_EDITOR
            return true;
#else
            return !String.IsNullOrEmpty(Environment.GetEnvironmentVariable(envVariable));
#endif // PLATFORM_LUMIN && !UNITY_EDITOR
        }
    }
}
