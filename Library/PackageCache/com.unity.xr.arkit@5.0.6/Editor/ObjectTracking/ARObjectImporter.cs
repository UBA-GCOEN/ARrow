using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Xml;
using UnityEditor.AssetImporters;
using UnityEngine;
using UnityEngine.XR.ARKit;
using Object = UnityEngine.Object;

namespace UnityEditor.XR.ARKit
{
    /// <summary>
    /// Importer for `.arobject` files. See
    /// <a href="https://developer.apple.com/documentation/arkit/scanning_and_detecting_3d_objects">Scanning and Detecting 3D Objects</a>
    /// for instructions on how to generate these files.
    /// </summary>
    /// <seealso cref="ARKitReferenceObjectEntry"/>
    [ScriptedImporter(1, "arobject")]
    public class ARObjectImporter : ScriptedImporter
    {
        /// <summary>
        /// Invoked automatically when an `.arobject` file is imported.
        /// </summary>
        /// <param name="ctx">The context associated with the asset import.</param>
        public override void OnImportAsset(AssetImportContext ctx)
        {
            var entry = ScriptableObject.CreateInstance<ARKitReferenceObjectEntry>();
            var arobject = ReadARObject(ctx.assetPath);
            if (arobject.HasValue)
            {
                entry.m_ReferenceOrigin = arobject.Value.info.referenceOrigin;
                entry.SetSourceAsset(ctx.assetPath);
            }

            ctx.AddObjectToAsset("arobject", entry, arobject.HasValue ? arobject.Value.preview : null);
            ctx.SetMainObject(entry);
        }

        /// <summary>
        /// Attempts to read the contents of the <c>.arobject</c> archive.
        /// </summary>
        /// <param name="path">The path to an <c>.arobject</c> archive.</param>
        /// <returns>If successful, an <see cref="ARObject"/> describing the archive. Otherwise, <c>null</c>.</returns>
        public static ARObject? ReadARObject(string path)
        {
            using var fileStream = new FileStream(path, FileMode.Open);
            using var archive = new ZipArchive(fileStream, ZipArchiveMode.Read);

            // Find the plist
            var plistEntry = archive.Entries
                .FirstOrDefault(entry => string.Equals(entry.Name, "Info.plist", StringComparison.OrdinalIgnoreCase));
            if (plistEntry == null)
                return null;

            var info = ReadPlist(plistEntry);

            // Find the preview image (or null)
            var imageReference = GetImageReference(archive, info);

            return new ARObject(info, imageReference);
        }

        static Texture2D GetImageReference(ZipArchive archive, ARObjectInfo info)
        {
            var imageReferenceEntry = archive.Entries
                .FirstOrDefault(entry => string.Equals(entry.Name, info.imageReference, StringComparison.OrdinalIgnoreCase));
            if (imageReferenceEntry == null)
                return null;

            var imageReference = new Texture2D(1, 1);
            using var memoryStream = new MemoryStream();
            imageReferenceEntry.Open().CopyTo(memoryStream);
            if (imageReference.LoadImage(memoryStream.ToArray()))
            {
                return RotateTextureClockwise(imageReference);
            }

            Destroy(imageReference);
            return null;
        }

        static ARObjectInfo ReadPlist(ZipArchiveEntry plistEntry)
        {
            using var stream = plistEntry.Open();
            using var reader = new StreamReader(stream);
            var plist = new XmlDocument();
            plist.Load(reader);
            return new ARObjectInfo(plist);
        }

        /// <summary>
        /// Attempts to read metadata from the <c>.arobject</c> archive.
        /// </summary>
        /// <param name="path">The path to an <c>.arobject</c> archive.</param>
        /// <returns>If successful, an <see cref="ARObjectInfo"/> containing metadata describing the archive. Otherwise, <c>null</c>.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="path"/> is null or the empty string.</exception>
        public static ARObjectInfo? ReadInfo(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException(nameof(path));

            using var fileStream = new FileStream(path, FileMode.Open);
            using var archive = new ZipArchive(fileStream, ZipArchiveMode.Read);
            foreach (var entry in archive.Entries)
            {
                if (string.Equals(entry.Name, "Info.plist", StringComparison.OrdinalIgnoreCase))
                {
                    using var stream = entry.Open();
                    using var reader = new StreamReader(stream);
                    var plist = new XmlDocument();
                    plist.Load(reader);
                    return new ARObjectInfo(plist);
                }
            }

            return null;
        }

        static Texture2D RotateTextureClockwise(Texture2D texture)
        {
            var pixels = texture.GetPixels32();
            var w = texture.width;
            var h = texture.height;
            var n = pixels.Length;
            var rotatedPixels = new Color32[n];

            for (var y = 0; y < h; ++y)
            {
                for (var x = 0; x < w; ++x)
                {
                    rotatedPixels[(x + 1) * h - y - 1] = pixels[n - 1 - (y * w + x)];
                }
            }

            var rotatedTexture = new Texture2D(h, w);
            rotatedTexture.SetPixels32(rotatedPixels);
            rotatedTexture.Apply();
            return rotatedTexture;
        }
    }
}
