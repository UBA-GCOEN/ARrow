#if CSHARP_7_3_OR_NEWER

namespace Gpm.Common.ThirdParty.SharpCompress.Common
{
    internal interface IExtractionListener
    {
        void FireFilePartExtractionBegin(string name, long size, long compressedSize);
        void FireCompressedBytesRead(long currentPartCompressedBytes, long compressedReadBytes);
    }
}

#endif