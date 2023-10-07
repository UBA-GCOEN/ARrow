#if CSHARP_7_3_OR_NEWER

using System.Collections.Generic;
using System.IO;
using Gpm.Common.ThirdParty.SharpCompress.Common.Rar;
using Gpm.Common.ThirdParty.SharpCompress.Common.Rar.Headers;
using Gpm.Common.ThirdParty.SharpCompress.IO;

namespace Gpm.Common.ThirdParty.SharpCompress.Readers.Rar
{
    public class RarReaderVolume : RarVolume
    {
        internal RarReaderVolume(Stream stream, ReaderOptions options)
            : base(StreamingMode.Streaming, stream, options)
        {
        }

        internal override RarFilePart CreateFilePart(MarkHeader markHeader, FileHeader fileHeader)
        {
            return new NonSeekableStreamFilePart(markHeader, fileHeader);
        }

        internal override IEnumerable<RarFilePart> ReadFileParts()
        {
            return GetVolumeFileParts();
        }
    }
}

#endif