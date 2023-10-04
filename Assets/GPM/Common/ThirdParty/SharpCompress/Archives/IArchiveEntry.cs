#if CSHARP_7_3_OR_NEWER

using System.IO;
using Gpm.Common.ThirdParty.SharpCompress.Common;

namespace Gpm.Common.ThirdParty.SharpCompress.Archives
{
    public interface IArchiveEntry : IEntry
    {
        /// <summary>
        /// Opens the current entry as a stream that will decompress as it is read.
        /// Read the entire stream or use SkipEntry on EntryStream.
        /// </summary>
        Stream OpenEntryStream();

        /// <summary>
        /// The archive can find all the parts of the archive needed to extract this entry.
        /// </summary>
        bool IsComplete { get; }

        /// <summary>
        /// The archive instance this entry belongs to
        /// </summary>
        IArchive Archive { get; }
    }
}

#endif