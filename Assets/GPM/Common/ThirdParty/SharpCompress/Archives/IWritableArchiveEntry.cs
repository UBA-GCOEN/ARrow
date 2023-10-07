#if CSHARP_7_3_OR_NEWER

using System.IO;

namespace Gpm.Common.ThirdParty.SharpCompress.Archives
{
    internal interface IWritableArchiveEntry
    {
        Stream Stream { get; }
    }
}

#endif