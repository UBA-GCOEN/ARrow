#if CSHARP_7_3_OR_NEWER

using Gpm.Common.ThirdParty.SharpCompress.Common;

namespace Gpm.Common.ThirdParty.SharpCompress.Archives
{
    internal interface IArchiveExtractionListener : IExtractionListener
    {
        void EnsureEntriesLoaded();
        void FireEntryExtractionBegin(IArchiveEntry entry);
        void FireEntryExtractionEnd(IArchiveEntry entry);
    }
}

#endif