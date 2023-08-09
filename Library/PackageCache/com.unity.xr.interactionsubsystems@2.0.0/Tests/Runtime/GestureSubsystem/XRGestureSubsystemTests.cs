using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.XR.InteractionSubsystems;

namespace UnityEngine.XR.InteractionSubsystems.Tests
{
    public class XRGestureSubsystemTest : XRGestureSubsystem
    {
        public class XRGestureSubsystemTestProvider : Provider
        {
            public XRGestureSubsystemTestProvider() { }

            public override void Start() { }

            public override void Stop() { }

            public override void Update() { }

            public override void Destroy()
            {
                base.Destroy();
            }
        }
    }

    [TestFixture]
    public class XRGestureSubsystemTestFixture
    {
        [OneTimeSetUp]
        public void RegisterTestDescriptor()
        {
            XRGestureSubsystemDescriptor.RegisterDescriptor(new XRGestureSubsystemDescriptor.Cinfo
            {
                id = "Test-Gesture",
                providerType = typeof(XRGestureSubsystemTest.XRGestureSubsystemTestProvider),
                subsystemTypeOverride = typeof(XRGestureSubsystemTest)
            });
        }

        static List<XRGestureSubsystemDescriptor> s_Descs = new List<XRGestureSubsystemDescriptor>();
        static XRGestureSubsystem CreateTestGestureSubsystem()
        {
            SubsystemManager.GetSubsystemDescriptors(s_Descs);
            foreach (var desc in s_Descs)
            {
                if (desc.id == "Test-Gesture")
                    return desc.Create();
            }

            return s_Descs[0].Create();
        }

        [Test]
        public void RunningStateTests()
        {
            XRGestureSubsystem subsystem = CreateTestGestureSubsystem();
            
            // Initial state is not running
            Assert.That(subsystem.running == false);

            // After start subsystem is running
            subsystem.Start();
            Assert.That(subsystem.running == true);

            // After start subsystem is running
            subsystem.Stop();
            Assert.That(subsystem.running == false);
        }
    }
}
