#if CSHARP_7_3_OR_NEWER

using Gpm.Common.ThirdParty.SharpCompress.Common;

namespace Gpm.Common.ThirdParty.SharpCompress.Readers
{
    internal interface IReaderExtractionListener : IExtractionListener
    {
        void FireEntryExtractionProgress(Entry entry, long sizeTransferred, int iterations);
    }
}

#endif