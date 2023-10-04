#if CSHARP_7_3_OR_NEWER

using System.IO;
using System.Linq;
using Gpm.Common.ThirdParty.SharpCompress.Common;
using Gpm.Common.ThirdParty.SharpCompress.Common.Tar;

namespace Gpm.Common.ThirdParty.SharpCompress.Archives.Tar
{
    public class TarArchiveEntry : TarEntry, IArchiveEntry
    {
        internal TarArchiveEntry(TarArchive archive, TarFilePart part, CompressionType compressionType)
            : base(part, compressionType)
        {
            Archive = archive;
        }

        public virtual Stream OpenEntryStream()
        {
            return Parts.Single().GetCompressedStream();
        }

#region IArchiveEntry Members

        public IArchive Archive { get; }

        public bool IsComplete => true;

#endregion
    }
}

#endif