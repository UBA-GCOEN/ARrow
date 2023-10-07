#if CSHARP_7_3_OR_NEWER

namespace Gpm.Common.ThirdParty.SharpCompress.Common.Zip.Headers
{
    internal enum ZipHeaderType
    {
        Ignore,
        LocalEntry,
        DirectoryEntry,
        DirectoryEnd,
        Split,
        Zip64DirectoryEnd,
        Zip64DirectoryEndLocator
    }
}

#endif