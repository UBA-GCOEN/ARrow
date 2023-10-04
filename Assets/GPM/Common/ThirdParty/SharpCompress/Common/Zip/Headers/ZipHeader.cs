#if CSHARP_7_3_OR_NEWER

using System.IO;

namespace Gpm.Common.ThirdParty.SharpCompress.Common.Zip.Headers
{
    internal abstract class ZipHeader
    {
        protected ZipHeader(ZipHeaderType type)
        {
            ZipHeaderType = type;
            HasData = true;
        }

        internal ZipHeaderType ZipHeaderType { get; }

        internal abstract void Read(BinaryReader reader);

        internal bool HasData { get; set; }
    }
}

#endif