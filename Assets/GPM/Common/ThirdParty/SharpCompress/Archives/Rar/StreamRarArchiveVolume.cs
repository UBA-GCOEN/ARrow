#if CSHARP_7_3_OR_NEWER

using System.Collections.Generic;
using System.IO;
using Gpm.Common.ThirdParty.SharpCompress.Common.Rar;
using Gpm.Common.ThirdParty.SharpCompress.Common.Rar.Headers;
using Gpm.Common.ThirdParty.SharpCompress.IO;
using Gpm.Common.ThirdParty.SharpCompress.Readers;

namespace Gpm.Common.ThirdParty.SharpCompress.Archives.Rar
{
    internal class StreamRarArchiveVolume : RarVolume
    {
        internal StreamRarArchiveVolume(Stream stream, ReaderOptions options)
            : base(StreamingMode.Seekable, stream, options)
        {
        }

        internal override IEnumerable<RarFilePart> ReadFileParts()
        {
            return GetVolumeFileParts();
        }

        internal override RarFilePart CreateFilePart(MarkHeader markHeader, FileHeader fileHeader)
        {
            return new SeekableFilePart(markHeader, fileHeader, Stream, ReaderOptions.Password);
        }
    }
}

#endif