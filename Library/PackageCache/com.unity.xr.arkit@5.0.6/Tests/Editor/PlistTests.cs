using System.Collections.Generic;
using UnityEngine;
using NUnit.Framework;

namespace UnityEditor.XR.ARKit.Tests
{
    [TestFixture]
    class PlistTests
    {
        [Test]
        public void CanParseCarOutput()
        {
            const string input = @"<?xml version=""1.0"" encoding=""UTF-8""?>
<!DOCTYPE plist PUBLIC ""-//Apple//DTD PLIST 1.0//EN"" ""http://www.apple.com/DTDs/PropertyList-1.0.dtd"">
<plist version=""1.0"">
<dict>
    <key>com.apple.actool.compilation-results</key>
    <dict>
        <key>output-files</key>
        <array>
            <string>/var/folders/d2/6gjz1w057hzgs1fktks2r6000000gp/T/a18844708d9944bca2160809039462a3/Assets.car</string>
        </array>
    </dict>
</dict>
</plist>";
            var plist = Plist.ReadFromString(input);
            var dict = plist.root["com.apple.actool.compilation-results"];
            Assert.NotNull(dict);

            var outputFiles = dict["output-files"].AsArray();
            Assert.AreEqual(1, outputFiles.Length);
            Assert.AreEqual("/var/folders/d2/6gjz1w057hzgs1fktks2r6000000gp/T/a18844708d9944bca2160809039462a3/Assets.car", outputFiles[0].AsString());
        }

        [Test]
        public void CanParseAllTypes()
        {
            var plist = Plist.ReadFromString(@"<?xml version=""1.0"" encoding=""UTF-8""?>
<!DOCTYPE plist PUBLIC ""-//Apple//DTD PLIST 1.0//EN"" ""http://www.apple.com/DTDs/PropertyList-1.0.dtd"">
<plist version=""1.0"">
<dict>
    <key>stringEntry</key>
    <string>foo</string>
    <key>realEntry</key>
    <real>3.1415</real>
    <key>vectorEntry</key>
    <array>
        <real>1</real>
        <real>2</real>
        <real>3</real>
    </array>
    <key>quaternionEntry</key>
    <array>
        <real>0</real>
        <real>.707</real>
        <real>0</real>
        <real>-.707</real>
    </array>
    <key>nestedDict</key>
    <dict>
        <key>integerEntry</key>
        <integer>42</integer>
    </dict>
</dict>
</plist>");

            var root = plist.root.AsDictionary();
            Assert.AreEqual(5, root.Count);
            Assert.AreEqual("foo", root["stringEntry"].AsString());
            Assert.AreEqual(3.1415f, root["realEntry"].AsFloat());
            Assert.AreEqual(new Vector3(1, 2, 3), root["vectorEntry"].AsVector3());
            Assert.AreEqual(new Quaternion(0, .707f, 0, -.707f), root["quaternionEntry"].AsQuaternion());

            var nestedDict = root["nestedDict"];
            Assert.AreEqual(42, nestedDict["integerEntry"].AsInt32());
        }

        [Test]
        public void HandlesEmptyDictionary()
        {
            var plist = Plist.ReadFromString(@"<?xml version=""1.0"" encoding=""UTF-8""?>
<!DOCTYPE plist PUBLIC ""-//Apple//DTD PLIST 1.0//EN"" ""http://www.apple.com/DTDs/PropertyList-1.0.dtd"">
<plist version=""1.0"">
<dict></dict>
</plist>");

            Assert.AreEqual(0, plist.root.AsDictionary().Count);
        }

        [Test]
        public void ThrowsIfKeyDoesntExist()
        {
            var plist = Plist.ReadFromString(@"<?xml version=""1.0"" encoding=""UTF-8""?>
<!DOCTYPE plist PUBLIC ""-//Apple//DTD PLIST 1.0//EN"" ""http://www.apple.com/DTDs/PropertyList-1.0.dtd"">
<plist version=""1.0"">
<dict>
    <key>SomeKey</key>
    <integer>42</integer>
</dict>
</plist>");

            Assert.Throws<KeyNotFoundException>(() =>
            {
                var _ = plist.root["AKeyThatDoesntExist"];
            });
        }

        [Test]
        public void CanParseARObject()
        {
            var plist = Plist.ReadFromString(@"<?xml version=""1.0"" encoding=""utf-16""?>
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

            Assert.AreEqual("preview.jpg", plist.root["ImageReference"].AsString());
            Assert.AreEqual("trackingData.cv3dmap", plist.root["TrackingDataReference"].AsString());
            Assert.AreEqual(1, plist.root["Version"].AsInt32());

            var referenceOrigin = plist.root["ReferenceOrigin"];
            Assert.AreEqual(new Quaternion(0, 1, 0, -1.6684859991073608f), referenceOrigin["rotation"].AsQuaternion());
            Assert.AreEqual(new Vector3(0.0029369667172431946f, 0.031287968158721924f, -0.010643705725669861f), referenceOrigin["translation"].AsVector3());
        }
    }
}
