#if CSHARP_7_3_OR_NEWER

using System.IO;
using Gpm.Common.ThirdParty.SharpCompress.Common;
using Gpm.Common.ThirdParty.SharpCompress.Common.Rar;

namespace Gpm.Common.ThirdParty.SharpCompress.Readers.Rar
{
    internal class SingleVolumeRarReader : RarReader
    {
        private readonly Stream stream;

        internal SingleVolumeRarReader(Stream stream, ReaderOptions options)
            : base(options)
        {
            this.stream = stream;
        }

        internal override void ValidateArchive(RarVolume archive)
        {
            if (archive.IsMultiVolume) {
                var msg = "Streamed archive is a Multi-volume archive.  Use different RarReader method to extract.";
                throw new MultiVolumeExtractionException(msg);
            }
        }

        protected override Stream RequestInitialStream()
        {
            return stream;
        }
    }
}

#endif