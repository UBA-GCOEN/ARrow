#if CSHARP_7_3_OR_NEWER

using System;
using Gpm.Common.ThirdParty.SharpCompress.Readers;

namespace Gpm.Common.ThirdParty.SharpCompress.Common
{
    public class ReaderExtractionEventArgs<T> : EventArgs
    {
        internal ReaderExtractionEventArgs(T entry, ReaderProgress readerProgress = null)
        {
            Item = entry;
            ReaderProgress = readerProgress;
        }

        public T Item { get; }
        public ReaderProgress ReaderProgress { get; }
    }
}

#endif