#if CSHARP_7_3_OR_NEWER

using System.Collections.Generic;
using Gpm.Common.ThirdParty.SharpCompress.Common;
using Gpm.Common.ThirdParty.SharpCompress.Common.Rar;
using Gpm.Common.ThirdParty.SharpCompress.Common.Rar.Headers;

namespace Gpm.Common.ThirdParty.SharpCompress.Readers.Rar
{
    public class RarReaderEntry : RarEntry
    {
        internal RarReaderEntry(bool solid, RarFilePart part)
        {
            Part = part;
            IsSolid = solid;
        }

        internal RarFilePart Part { get; }

        internal override IEnumerable<FilePart> Parts => Part.AsEnumerable<FilePart>();

        internal override FileHeader FileHeader => Part.FileHeader;

        public override CompressionType CompressionType => CompressionType.Rar;

        /// <summary>
        /// The compressed file size
        /// </summary>
        public override long CompressedSize => Part.FileHeader.CompressedSize;

        /// <summary>
        /// The uncompressed file size
        /// </summary>
        public override long Size => Part.FileHeader.UncompressedSize;
    }
}

#endif