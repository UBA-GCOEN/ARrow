#if CSHARP_7_3_OR_NEWER

using System.IO;
using Gpm.Common.ThirdParty.SharpCompress.Readers;

namespace Gpm.Common.ThirdParty.SharpCompress.Common.Zip
{
    public class ZipVolume : Volume
    {
        public ZipVolume(Stream stream, ReaderOptions readerOptions)
            : base(stream, readerOptions)
        {
        }

        public string Comment { get; internal set; }
    }
}

#endif