using System;
using System.Xml;
using UnityEngine;

namespace UnityEditor.XR.ARKit
{
    /// <summary>
    /// Represents metadata associated with a <c>.arobject</c> archive. These archives
    /// are the source data for <see cref="UnityEngine.XR.ARKit.ARKitReferenceObjectEntry"/>.
    /// </summary>
    /// <seealso cref="ARObject"/>
    /// <seealso cref="ARObjectImporter"/>
    /// <seealso cref="UnityEngine.XR.ARKit.ARKitReferenceObjectEntry"/>
    public struct ARObjectInfo : IEquatable<ARObjectInfo>
    {
        /// <summary>
        /// Parses the <paramref name="plist"/> and constructs a new <see cref="ARObjectInfo"/>.
        /// </summary>
        /// <param name="plist">A valid <c>PlistDocument</c> to parse.</param>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="plist"/> is <c>null</c>.</exception>
        public ARObjectInfo(XmlDocument plist)
        {
            if (plist == null)
                throw new ArgumentNullException(nameof(plist));

            // Parse the plist
            var root = new Plist(plist).root;
            version = root["Version"].AsInt32() ?? 0;
            trackingDataReference = root["TrackingDataReference"].AsString() ?? "";
            imageReference = root["ImageReference"].AsString() ?? "";
            var dict = root["ReferenceOrigin"];
            if (dict != null)
            {
                var rotation = FlipHandedness(dict["rotation"].AsQuaternion() ?? Quaternion.identity);
                var translation = FlipHandedness(dict["translation"].AsVector3() ?? Vector3.zero);
                referenceOrigin = new Pose(translation, rotation);
            }
            else
            {
                referenceOrigin = Pose.identity;
            }
        }

        static Vector3 FlipHandedness(Vector3 value) => new Vector3(value.x, value.y, -value.z);

        static Quaternion FlipHandedness(Quaternion value) => new Quaternion(value.x, value.y, -value.z, -value.w);

        /// <summary>
        /// The filename of an image contained within the archive that can be used as a preview for the scanned object.
        /// </summary>
        public string imageReference { get; }

        /// <summary>
        /// The reference origin for the scanned object.
        /// </summary>
        public Pose referenceOrigin { get; }

        /// <summary>
        /// The filename of the source data representing the scanned object.
        /// </summary>
        public string trackingDataReference { get; }

        /// <summary>
        /// The version of the metadata format.
        /// </summary>
        public int version { get; }

        /// <summary>
        /// <c>true</c> if the <see cref="ARObjectInfo"/> represents valid data, otherwise <c>false</c>.
        /// </summary>
        public bool valid => version > 0;

        /// <summary>
        /// Tests for equality.
        /// </summary>
        /// <param name="other">The other <see cref="ARObjectInfo"/> to compare against.</param>
        /// <returns>`True` if every field in <paramref name="other"/> is equal to this <see cref="ARObjectInfo"/>, otherwise false.</returns>
        public bool Equals(ARObjectInfo other)
        {
            return
                string.Equals(imageReference, other.imageReference) &&
                referenceOrigin.Equals(other.referenceOrigin) &&
                string.Equals(trackingDataReference, other.trackingDataReference) &&
                (version == other.version);
        }

        /// <summary>
        /// Tests for equality.
        /// </summary>
        /// <param name="obj">The `object` to compare against.</param>
        /// <returns>`True` if <paramref name="obj"/> is of type <see cref="ARObjectInfo"/> and
        /// <see cref="Equals(ARObjectInfo)"/> also returns `true`; otherwise `false`.</returns>
        public override bool Equals(object obj) => obj is ARObjectInfo info && Equals(info);

        /// <summary>
        /// Generates a hash suitable for use with containers like `HashSet` and `Dictionary`.
        /// </summary>
        /// <returns>A hash code generated from this object's fields.</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                var hash = (imageReference == null ? 0 : imageReference.GetHashCode());
                hash = hash * 486187739 + referenceOrigin.GetHashCode();
                hash = hash * 486187739 + (trackingDataReference == null ? 0 : trackingDataReference.GetHashCode());
                hash = hash * 486187739 + version.GetHashCode();
                return hash;
            }
        }

        /// <summary>
        /// Tests for equality. Same as <see cref="Equals(ARObjectInfo)"/>.
        /// </summary>
        /// <param name="lhs">The left-hand side of the comparison.</param>
        /// <param name="rhs">The right-hand side of the comparison.</param>
        /// <returns>`True` if <paramref name="lhs"/> is equal to <paramref name="rhs"/>, otherwise `false`.</returns>
        public static bool operator ==(ARObjectInfo lhs, ARObjectInfo rhs) => lhs.Equals(rhs);

        /// <summary>
        /// Tests for inequality. Same as `!`<see cref="Equals(ARObjectInfo)"/>.
        /// </summary>
        /// <param name="lhs">The left-hand side of the comparison.</param>
        /// <param name="rhs">The right-hand side of the comparison.</param>
        /// <returns>`True` if <paramref name="lhs"/> is not equal to <paramref name="rhs"/>, otherwise `false`.</returns>
        public static bool operator !=(ARObjectInfo lhs, ARObjectInfo rhs) => !lhs.Equals(rhs);
    }
}
