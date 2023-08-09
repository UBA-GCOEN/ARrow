using System.Xml;
using NUnit.Framework;
using UnityEngine;

namespace UnityEditor.XR.ARKit.Tests
{
    [TestFixture]
    class ARObjectInfoTests
    {
        [Test]
        public void CanParseXmlPlist()
        {
            var xml = new XmlDocument();
            xml.LoadXml(@"<?xml version=""1.0"" encoding=""utf-16""?>
<!DOCTYPE plist PUBLIC ""-//Apple//DTD PLIST 1.0//EN"" ""http://www.apple.com/DTDs/PropertyList-1.0.dtd""[]>
<plist version=""1.0"">
  <dict>
    <key>ImageReference</key>
    <string>preview.jpg</string>
    <key>ReferenceOrigin</key>
    <dict>
      <key>rotation</key>
      <array>
        <real>0.0</real>
        <real>1</real>
        <real>0.0</real>
        <real>-1.6684859991073608</real>
      </array>
      <key>translation</key>
      <array>
        <real>0.0029369667172431946</real>
        <real>0.031287968158721924</real>
        <real>-0.010643705725669861</real>
      </array>
    </dict>
    <key>TrackingDataReference</key>
    <string>trackingData.cv3dmap</string>
    <key>Version</key>
    <integer>1</integer>
  </dict>
</plist>
");

          var info = new ARObjectInfo(xml);
          Assert.AreEqual("preview.jpg", info.imageReference);
          Assert.AreEqual(new Pose(
            new Vector3(0.0029369667172431946f, 0.031287968158721924f, 0.010643705725669861f),
            new Quaternion(0, 1, 0, 1.6684859991073608f)), info.referenceOrigin);
          Assert.AreEqual("trackingData.cv3dmap", info.trackingDataReference);
          Assert.AreEqual(1, info.version);
        }
    }
}
