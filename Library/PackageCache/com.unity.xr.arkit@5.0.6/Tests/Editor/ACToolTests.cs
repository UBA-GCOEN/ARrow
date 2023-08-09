using System;
using NUnit.Framework;

namespace UnityEditor.XR.ARKit.Tests
{
    [TestFixture]
    class ACToolTests
    {
        [Test]
        public void ThrowsOnNonMacOSX()
        {
#if !UNITY_EDITOR_OSX
            Assert.Throws<ACTool.XCRunNotFoundException>(() =>
            {
                ACTool.Compile("somepath", "outputDir", new Version(11, 3));
            });
#endif // UNITY_EDITOR_64
        }
    }
}
