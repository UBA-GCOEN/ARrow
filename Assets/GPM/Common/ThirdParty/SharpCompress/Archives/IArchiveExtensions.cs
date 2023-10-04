#if CSHARP_7_3_OR_NEWER

#if !NO_FILE
using System.Linq;
using Gpm.Common.ThirdParty.SharpCompress.Common;

#endif

namespace Gpm.Common.ThirdParty.SharpCompress.Archives
{
    public static class IArchiveExtensions
    {
#if !NO_FILE

/// <summary>
/// Extract to specific directory, retaining filename
/// </summary>
        public static void WriteToDirectory(this IArchive archive, string destinationDirectory,
                                            ExtractionOptions options = null)
        {
            foreach (IArchiveEntry entry in archive.Entries.Where(x => !x.IsDirectory))
            {
                entry.WriteToDirectory(destinationDirectory, options);
            }
        }
#endif
    }
}

#endif