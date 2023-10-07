#if CSHARP_7_3_OR_NEWER

using System;

namespace Gpm.Common.ThirdParty.SharpCompress.Common
{
    public class ArchiveExtractionEventArgs<T> : EventArgs
    {
        internal ArchiveExtractionEventArgs(T entry)
        {
            Item = entry;
        }

        public T Item { get; }
    }
}

#endif