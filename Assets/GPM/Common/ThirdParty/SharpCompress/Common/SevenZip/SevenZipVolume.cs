#if CSHARP_7_3_OR_NEWER

using System.IO;
using Gpm.Common.ThirdParty.SharpCompress.Archives;
using Gpm.Common.ThirdParty.SharpCompress.Readers;

namespace Gpm.Common.ThirdParty.SharpCompress.Common.SevenZip
{
    public class SevenZipVolume : Volume
    {
        public SevenZipVolume(Stream stream, ReaderOptions readerFactoryOptions)
            : base(stream, readerFactoryOptions)
        {
        }
    }
}

#endif